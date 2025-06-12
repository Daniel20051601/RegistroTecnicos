using System.ComponentModel.DataAnnotations;

namespace RegistroTecnicos.Models;

public class Sistemas
{
    [Key]
    public int SistemaId { get; set; }

    [Required(ErrorMessage = "Debe agregar la descripción de este sistema")]
    [StringLength(300, ErrorMessage = "La descripción no puede tener mas de 300 caracteres")]
    [RegularExpression(@"^(?=.*[A-Za-z])[^\d]*\S[^\d]*$", 
        ErrorMessage = "La descripción debe contener al menos una letra, no puede ser solo números o espacios.")]
    public string DescripcionSistema { get; set; } = null!;

    [Required(ErrorMessage = "Debe agregar la complejidad del Sistema")]
    public string Complejidad { get; set; } = null!;


}
