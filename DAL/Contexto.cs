using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.Models;

namespace RegistroTecnicos.DAL;

public class Contexto : DbContext
{
    public DbSet<Tecnicos> Tecnicos { get; set; }

    public DbSet<Clientes> Clientes { get; set; }

    public DbSet<Tickets> Tickets { get; set; }

    public DbSet<Sistemas> Sistemas { get; set; }

    public DbSet<Ventas> Ventas { get; set; }

    public DbSet<VentasDetalle> VentasDetalle { get; set; }

    public Contexto(DbContextOptions<Contexto> options) : base(options) { }

    public Contexto() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            
            optionsBuilder.UseNpgsql("Host=aws-0-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.jczkyrhoxopsyvirssqf;Password=eiGWHWqdluUFDeTD;SSL Mode=Require;CommandTimeout=60");
        }
    }
}
