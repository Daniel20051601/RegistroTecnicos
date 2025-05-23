using System.ComponentModel.DataAnnotations;

namespace RegistroTecnicos.Models;

public class Tecnicos
{
    [Key]
    public int TecnicoId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre debe tener un maximo de 100 caracteres")]
    [RegularExpression(@"^[a-zA-Z\u00C0-\u017F\s]+$", ErrorMessage = "El nombre solo puede contener letras y espacios (incluyendo acentos).")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar un sueldo por hora")]
    [Range(1, 1000000, ErrorMessage = "El sueldo por hora debe estar entre 1 y 1000000 ")]
    public double SueldoPorHora { get; set; }


}

