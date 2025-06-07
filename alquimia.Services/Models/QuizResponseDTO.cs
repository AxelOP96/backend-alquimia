namespace alquimia.Services.Models
{
    public class QuizResponseDTO
    {
        public string SuperFamily { get; set; } = string.Empty;
        public List<string> AllSubFamilies { get; set; } = new();
        public List<SubFamilyDTO> TopMatchedSubFamilies { get; set; } = new();
        public List<ExampleFormulaDTO> Formulas { get; set; } = new();
        public string ConcentrationType { get; set; } = string.Empty;
    }
}
