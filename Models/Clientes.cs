using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroTecnicos.Models;

[Index(nameof(Nombres), IsUnique = true)] 
[Index(nameof(Rnc), IsUnique = true)]

public class Clientes
{
    [Key]
    public int ClienteId { get; set; }

    public DateTime FechaIngreso { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre debe tener un maximo de 100 caracteres")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar la dirección")]
    [StringLength(200, ErrorMessage = "La dirección debe tener un maximo de 200 caracteres")]
    public string Direccion { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar el RNC")]
    [StringLength(11, ErrorMessage = "El RNC debe tener un maximo de 11 caracteres")]
    public string Rnc { get; set; } = null!;

    [Required(ErrorMessage = "Debe un limite de crédito")]
    [Range(1.00, 1000000.00, ErrorMessage = "El límite de crédito debe ser un valor positivo entre {1} y {2}.")]
    public double LimiteCredito { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un técnico")]
    public int TecnicoId { get; set; }

    public Tecnicos Tecnico { get; set; } = null!;


}
