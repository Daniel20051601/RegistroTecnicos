using System.ComponentModel.DataAnnotations;

namespace RegistroTecnicos.Models;

public class Tecnicos
{
    [Key]
    public int TecnicoId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre debe tener un maximo de 100 caracteres")]
    [RegularExpression(@"^(?=.*[a-zA-Z\u00C0-\u017F])[a-zA-Z\u00C0-\u017F\s.'-]+$",
                       ErrorMessage = "El nombre debe contener al menos una letra y solo puede incluir letras, espacios, apóstrofes, puntos y guiones (incluyendo acentos).")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar un sueldo por hora")]
    [Range(1.00, 1000000.00, ErrorMessage = "El sueldo por hora debe estar entre 1 y 1000000 ")]
    [RegularExpression(@"^\d{1,5}(\.\d{1,2})?$", ErrorMessage = "Máximo 2 decimales permitidos")]
    public double SueldoPorHora { get; set; }

    public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();


}

