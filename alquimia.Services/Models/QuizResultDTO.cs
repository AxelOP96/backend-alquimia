namespace alquimia.Services.Models
{
    public class QuizResultDTO
    {
        public string Family { get; set; } = string.Empty;
        public List<string> CompatibleNotes { get; set; } = new();
        public ExampleFormulaDTO Formula { get; set; } = new();

    }



    public class ExampleFormulaDTO
    {
        public string TopNote { get; set; } = string.Empty;
        public string HeartNote { get; set; } = string.Empty;
        public string BaseNote { get; set; } = string.Empty;
    }
}
