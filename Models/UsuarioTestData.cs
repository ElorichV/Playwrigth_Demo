namespace Playwrigt_Demo.Models;

public class UsuarioTestData
{
    public string? CasoId { get; set; }
    public string? Usuario { get; set; }
    public string? Password { get; set; }
    public bool DebeSerExitoso { get; set; }
    public string? MensajeEsperado { get; set; }
    public string? AgenteEsperado { get; set; } 
    
    public override string ToString() => CasoId ?? "Test";
}