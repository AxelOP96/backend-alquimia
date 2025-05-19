namespace backendAlquimia.Data.Entities;
public class Combinacion
{
    public int Id { get; set; }
    public ICollection<Nota> NotaSalida { get; set; }
    public ICollection<Nota> NotaCorazon { get; set; }
    public ICollection<Nota> NotaFondo { get; set; }
    public ICollection<Formula> Formulas { get; set; } = new List<Formula>();
}