
using backendAlquimia.Data.Entities;

namespace backendAlquimia.Services
{
    public interface ICompatibilityService
    {
        IEnumerable<Nota> GetCompatibleNotes(IEnumerable<Nota> seleccionadas, IEnumerable<Nota> todasNotas, int minCompatibilidad = 60);
    }

    public class CompatibilityService : ICompatibilityService
    {
        private readonly Dictionary<(string, string), int> _matrizCompatibilidad = new()
        {
            { ("Gourmand", "Gourmand"), 100 },
            { ("Gourmand", "Frutal"), 85 },
            { ("Gourmand", "Amaderada"), 75 },
            { ("Gourmand", "Floral"), 60 },
            { ("Gourmand", "Cítrica"), 45 },
            { ("Gourmand", "Verde"), 30 },
            { ("Gourmand", "Acuática"), 20 },

            // Agregá también la simetría (Frutal, Gourmand) = 85, etc.
            { ("Frutal", "Gourmand"), 85 },
            { ("Amaderada", "Gourmand"), 75 },
            { ("Floral", "Gourmand"), 60 },
            { ("Cítrica", "Gourmand"), 45 },
            { ("Verde", "Gourmand"), 30 },
            { ("Acuática", "Gourmand"), 20 },

            // Podés completar más pares según necesites...
        };

        public IEnumerable<Nota> GetCompatibleNotes(IEnumerable<Nota> seleccionadas, IEnumerable<Nota> todasNotas, int minCompatibilidad = 60)
        {
            // Si no hay seleccionadas, devolvemos todas las que tengan al menos compatibilidad consigo mismas >= min
            if (!seleccionadas.Any())
            {
                return todasNotas.Where(n =>
                {
                    var familia = n.FamiliaOlfativa?.Nombre;
                    return familia != null &&
                           _matrizCompatibilidad.TryGetValue((familia, familia), out var score) &&
                           score >= minCompatibilidad;
                });
            }

            var compatibles = new List<(Nota nota, int score)>();

            foreach (var nota in todasNotas)
            {
                var familiaNota = nota.FamiliaOlfativa?.Nombre;
                if (familiaNota == null) continue;

                // Obtenemos compatibilidades promedio con todas las seleccionadas
                int totalScore = 0;
                int count = 0;

                foreach (var sel in seleccionadas)
                {
                    var familiaSel = sel.FamiliaOlfativa?.Nombre;
                    if (familiaSel == null) continue;

                    if (_matrizCompatibilidad.TryGetValue((familiaSel, familiaNota), out var score1))
                    {
                        totalScore += score1;
                        count++;
                    }
                    else if (_matrizCompatibilidad.TryGetValue((familiaNota, familiaSel), out var score2))
                    {
                        totalScore += score2;
                        count++;
                    }
                }

                if (count > 0)
                {
                    int promedio = totalScore / count;
                    if (promedio >= minCompatibilidad)
                    {
                        compatibles.Add((nota, promedio));
                    }
                }
            }

            // Devolver ordenado por compatibilidad descendente
            return compatibles
                .OrderByDescending(c => c.score)
                .Select(c => c.nota)
                .Distinct()
                .ToList();
        }
    }
}
