using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using alquimia.Services.QuizLogic;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Services
{

    public class QuizService : IQuizService
    {
        private readonly AlquimiaDbContext _context;
        private readonly INoteService _noteService;

        public QuizService(AlquimiaDbContext context, INoteService noteService)
        {
            _context = context;
            _noteService = noteService;
        }

        public async Task<List<QuestionDTO>> GetQuestionsAsync()
        {
            var preguntas = await _context.Questions.Include(q => q.IdOpcionesNavigation).ToListAsync();

            return preguntas.Select(q => new QuestionDTO
            {
                Id = q.Id,
                Pregunta = q.Pregunta,
                Opciones = new List<OptionDTO>
                {
                    new OptionDTO { Letra = "A", Texto = q.IdOpcionesNavigation?.Option1 ?? "", ImagenUrl = q.IdOpcionesNavigation?.Image1 },
                    new OptionDTO { Letra = "B", Texto = q.IdOpcionesNavigation?.Option2 ?? "", ImagenUrl = q.IdOpcionesNavigation?.Image2 },
                    new OptionDTO { Letra = "C", Texto = q.IdOpcionesNavigation?.Option3 ?? "", ImagenUrl = q.IdOpcionesNavigation?.Image3 },
                    new OptionDTO { Letra = "D", Texto = q.IdOpcionesNavigation?.Option4 ?? "", ImagenUrl = q.IdOpcionesNavigation?.Image4 },
                }
            }).ToList();
        }

        public Task SaveAnswersAsync(List<AnswerDTO> respuestas)
        {
            return Task.CompletedTask;
        }

        public async Task<QuizResponseDTO?> GetResultAsync(List<AnswerDTO> answers)
        {
            var subFamilyScores = new Dictionary<string, int>();

            foreach (var answer in answers.Where(a => a.QuestionId != 10))
            {
                var mapping = FamilyMappingData.Mappings.FirstOrDefault(m => m.QuestionId == answer.QuestionId && m.OptionIndex.ToString() == answer.SelectedOption);
                if (mapping != null)
                {
                    foreach (var subFam in mapping.Families)
                    {
                        subFamilyScores[subFam] = subFamilyScores.GetValueOrDefault(subFam, 0) + 1;
                    }
                }
            }

            var superFamilyScores = SuperFamilyMapping.Groups.ToDictionary(g => g.Key, g => g.Value.Sum(sf => subFamilyScores.GetValueOrDefault(sf, 0)));
            var dominantSuperFamily = superFamilyScores.OrderByDescending(s => s.Value).FirstOrDefault().Key;
            var allSubFamilies = SuperFamilyMapping.Groups[dominantSuperFamily];

            var orderedSubFamilies = allSubFamilies.Where(sf => subFamilyScores.ContainsKey(sf)).OrderByDescending(sf => subFamilyScores[sf]).ToList();
            var topMatchedSubFamilies = orderedSubFamilies.Take(2).ToList();
            var fallbackSubFamilies = orderedSubFamilies.Skip(2).ToList();

            var result = new QuizResponseDTO
            {
                SuperFamily = dominantSuperFamily,
                AllSubFamilies = allSubFamilies,
                TopMatchedSubFamilies = new List<SubFamilyDTO>(),
                Formulas = new List<ExampleFormulaDTO>()
            };

            foreach (var subFamily in topMatchedSubFamilies)
            {
                var family = await _context.OlfactoryFamilies.Include(f => f.Notes).FirstOrDefaultAsync(f => f.Nombre.ToLower() == subFamily.ToLower());

                if (family != null)
                {
                    result.TopMatchedSubFamilies.Add(new SubFamilyDTO
                    {
                        Name = subFamily,
                        CompatibleNotes = family.Notes.Select(n => n.Name).ToList()
                    });
                }
            }

            var usedNoteIds = new HashSet<int>();
            string? top = await GetNoteWithFallback(1, topMatchedSubFamilies, fallbackSubFamilies, usedNoteIds);
            string? heart = await GetNoteWithFallback(2, topMatchedSubFamilies, fallbackSubFamilies, usedNoteIds);
            string? baseNote = await GetNoteWithFallback(3, topMatchedSubFamilies, fallbackSubFamilies, usedNoteIds);

            result.Formulas.Add(new ExampleFormulaDTO
            {
                TopNote = top ?? "Nota compatible",
                HeartNote = heart ?? "Nota compatible",
                BaseNote = baseNote ?? "Nota compatible"
            });

            var intensity = answers.FirstOrDefault(a => a.QuestionId == 10)?.SelectedOption;
            result.ConcentrationType = intensity switch
            {
                "1" => "Body Splash",
                "2" => "Eau de Toilette",
                "3" => "Eau de Parfum",
                _ => "Desconocido"
            };

            return result;
        }

        private async Task<string?> GetNoteWithFallback(int pyramidLevel, List<string> topFamilies, List<string> fallbackFamilies, HashSet<int> usedNoteIds)
        {
            foreach (var fam in topFamilies.Concat(fallbackFamilies))
            {
                var family = await _context.OlfactoryFamilies.Include(f => f.Notes).FirstOrDefaultAsync(f => f.Nombre.ToLower() == fam.ToLower());
                if (family == null) continue;

                foreach (var note in family.Notes)
                {
                    if (note.OlfactoryPyramidId != pyramidLevel || usedNoteIds.Contains(note.Id))
                        continue;

                    var isIncompatible = false;
                    foreach (var usedId in usedNoteIds)
                    {
                        var incompatibles = await GetIncompatibleNoteIdsAsync(usedId);
                        if (incompatibles.Contains(note.Id))
                        {
                            isIncompatible = true;
                            break;
                        }
                    }

                    if (!isIncompatible)
                    {
                        usedNoteIds.Add(note.Id);
                        return note.Name;
                    }
                }
            }

            return null;
        }

        private async Task<List<int>> GetIncompatibleNoteIdsAsync(int noteId)
        {
            return await _context.IncompatibleNotes
                .Where(i => i.NotaId == noteId)
                .Select(i => i.NotaIncompatibleId)
                .ToListAsync();
        }
    }

}