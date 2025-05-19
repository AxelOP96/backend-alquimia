using backendAlquimia.Data;
using backendAlquimia.Data.Entities;
using backendAlquimia.Models;
using backendAlquimia.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backendAlquimia.Services
{
    public class FormulaService : IFormulaService
    {
        private readonly AlquimiaDbContext _context;

        public FormulaService(AlquimiaDbContext context)
        {
            _context = context;
        }

        /*--------------------------------------------------------------
         *  MÉTODOS PÚBLICOS
         *-------------------------------------------------------------*/
        public async Task<GETFormulaDTO> GuardarAsync(POSTFormulaDTO dto)
        {
            #region 1.  VALIDAR NOTAS
            var notasSalida = await _context.Notas.Where(n => dto.NotasSalidaIds.Contains(n.Id) && n.SectorId == 1).ToListAsync();
            var notasCorazon = await _context.Notas.Where(n => dto.NotasCorazonIds.Contains(n.Id) && n.SectorId == 2).ToListAsync();
            var notasFondo = await _context.Notas.Where(n => dto.NotasFondoIds.Contains(n.Id) && n.SectorId == 3).ToListAsync();

            if (notasSalida.Count != dto.NotasSalidaIds.Count ||
                notasCorazon.Count != dto.NotasCorazonIds.Count ||
                notasFondo.Count != dto.NotasFondoIds.Count)
            {
                throw new ArgumentException("Una o más notas no existen o no pertenecen al sector correspondiente.");
            }
            #endregion

            #region 2.  CREAR COMBINACIÓN
            var combinacion = new Combinacion
            {
                NotaSalida = notasSalida,
                NotaCorazon = notasCorazon,
                NotaFondo = notasFondo
            };

            _context.Combinaciones.Add(combinacion);
            await _context.SaveChangesAsync();
            #endregion

            #region 3.  CALCULAR CONCENTRACIONES
            var concentracionAgua = CalcularConcentracionAgua(dto.IdIntensidad);
            var concentracionAlcohol = CalcularConcentracionAlcohol(dto.IdIntensidad);
            var concentracionEsencia = CalcularConcentracionEsencia(dto.IdIntensidad);
            #endregion

            #region 4.  CREAR FORMULA
            var formula = new Formula
            {
                CombinacionId = combinacion.Id,
                IntensidadDatoCalculado = dto.IdIntensidad,
                CreadorId = dto.IdCreador,
                ConcentracionAlcohol = concentracionAlcohol,
                ConcentracionAgua = concentracionAgua,
                ConcentracionEsencia = concentracionEsencia
            };

            _context.Formulas.Add(formula);
            await _context.SaveChangesAsync();
            #endregion

            #region 5.  DTO DE RESPUESTA
            return new GETFormulaDTO
            {
                Id = formula.Id,
                NotasSalidaIds = dto.NotasSalidaIds,
                NotasCorazonIds = dto.NotasCorazonIds,
                NotasFondoIds = dto.NotasFondoIds,
                IdIntensidad = dto.IdIntensidad,
                IdCreador = dto.IdCreador,
                ConcentracionAlcohol = concentracionAlcohol,
                ConcentracionAgua = concentracionAgua,
                ConcentracionEsencia = concentracionEsencia
            };
            #endregion
        }

        public async Task<List<GETFormulaDTO>> ObtenerFormulasAsync()
        {
            return await _context.Formulas
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaSalida)
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaCorazon)
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaFondo)
                .Select(f => new GETFormulaDTO
                {
                    Id = f.Id,
                    NotasSalidaIds = f.Combinacion.NotaSalida.Select(n => n.Id).ToList(),
                    NotasCorazonIds = f.Combinacion.NotaCorazon.Select(n => n.Id).ToList(),
                    NotasFondoIds = f.Combinacion.NotaFondo.Select(n => n.Id).ToList(),
                    IdIntensidad = f.IntensidadDatoCalculado,
                    IdCreador = f.CreadorId,
                    ConcentracionAlcohol = f.ConcentracionAlcohol,
                    ConcentracionAgua = f.ConcentracionAgua,
                    ConcentracionEsencia = f.ConcentracionEsencia
                })
                .ToListAsync();
        }

        public async Task<GETFormulaDTO?> ObtenerPorIdAsync(int id)
        {
            var formula = await _context.Formulas
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaSalida)
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaCorazon)
                .Include(f => f.Combinacion)
                    .ThenInclude(c => c.NotaFondo)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (formula is null) return null;

            return new GETFormulaDTO
            {
                Id = formula.Id,
                NotasSalidaIds = formula.Combinacion.NotaSalida.Select(n => n.Id).ToList(),
                NotasCorazonIds = formula.Combinacion.NotaCorazon.Select(n => n.Id).ToList(),
                NotasFondoIds = formula.Combinacion.NotaFondo.Select(n => n.Id).ToList(),
                IdIntensidad = formula.IntensidadDatoCalculado,
                IdCreador = formula.CreadorId,
                ConcentracionAlcohol = formula.ConcentracionAlcohol,
                ConcentracionAgua = formula.ConcentracionAgua,
                ConcentracionEsencia = formula.ConcentracionEsencia
            };
        }

        /*--------------------------------------------------------------
         *  MÉTODOS PRIVADOS DE CÁLCULO
         *  (valores de ejemplo – ajustar a la lógica de negocio real)
         *-------------------------------------------------------------*/
        private static double CalcularConcentracionAgua(int intensidadId) =>
            intensidadId switch
            {
                1 => 80,   // baja intensidad
                2 => 70,   // media
                3 => 60,   // alta
                _ => 70
            };

        private static double CalcularConcentracionAlcohol(int intensidadId) =>
            intensidadId switch
            {
                1 => 10,
                2 => 15,
                3 => 20,
                _ => 15
            };

        private static double CalcularConcentracionEsencia(int intensidadId) =>
            intensidadId switch
            {
                1 => 10,
                2 => 15,
                3 => 20,
                _ => 15
            };

        public Task<GETFormulaDTO> guardar(POSTFormulaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<List<GETFormulaDTO>> ObtenerIntensidadAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GETFormulaDTO> ObtenerPorId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
