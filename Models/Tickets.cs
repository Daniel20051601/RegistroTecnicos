using System.ComponentModel.DataAnnotations;

namespace RegistroTecnicos.Models;

public class Tickets
{
    [Key]
    public int TicketId { get; set; }
    
    public DateTime Fecha { get; set; }

    [Required(ErrorMessage = "Seleccione una Prioridad para el Ticket")]
    public string Prioridad { get; set; } = null!;

    [Required(ErrorMessage = "Debe seleccionar un Cliente")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "Debe ingresar el asunto del Ticket")]
    [StringLength(150, ErrorMessage = "La dirección debe tener un maximo de 200 caracteres")]
    public string Asunto { get; set; } = null!;

    [Required(ErrorMessage = "Ingrese una descripción para el Ticket")]
    [StringLength(500, ErrorMessage = "La dirección debe tener un maximo de 200 caracteres")]
    public string Descripcion { get; set; } = null!;

    [Required(ErrorMessage ="Ingrese el tiempo invertido")]
    [Range(1.00, 2000.00, ErrorMessage = "El tiempo invertido debe ser un valor positivo entre {1} y {2}.")]
    public double TiempoInvertido { get; set; }

    [Required(ErrorMessage = "Selecione un Tecnico Encargado")]
    public int TecnicoId { get; set; }

    public Tecnicos Tecnico { get; set; } = null!;
    public Clientes Cliente { get; set; } = null!;
}
