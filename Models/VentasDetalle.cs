using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegistroTecnicos.Models;

public class VentasDetalle
{
    [Key]
    public int VentasDetalleId { get; set; }

    [Required]
    public int Cantidad { get; set; }

    [Required]
    public decimal Monto { get; set; }

    public int SistemaId { get; set; }

    public int VentaId { get; set; }

    [ForeignKey("VentaId")]
    [InverseProperty("VentasDetalles")]
    public virtual Ventas Venta { get; set; } = null!;

    [ForeignKey("SistemaId")]
    public Sistemas Sistema { get; set; }

}
