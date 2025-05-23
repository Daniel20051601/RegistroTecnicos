using System.ComponentModel.DataAnnotations;

namespace RegistroTecnicos.Models;

public class Tecnicos
{
    [Key]
    public int TecnicoId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre debe tener un maximo de 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar un sueldo por hora")]
    [Range(1, double.MaxValue, ErrorMessage = "El sueldo por hora debe ser igual o mayor a 1")]
    public double SueldoPorHora { get; set; }


}

