using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;

namespace RegistroTecnicos.Services;

public class TicketService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int ticketId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tickets.AnyAsync(t => t.TicketId == ticketId);
    }

    public async Task<bool> Insertar(Tickets ticket)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Tickets.Add(ticket);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Tickets ticket)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Tickets.Update(ticket);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Tickets ticket)
    {
        if (!await Existe(ticket.TicketId))
        {
            return await Insertar(ticket);
        }
        else
        {
            return await Modificar(ticket);
        }
    }
    public async Task<Tickets?> Buscar(int ticketId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tickets
            .Include(t => t.Cliente)
            .Include(t => t.Tecnico)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
    }

    public async Task<bool> Eliminar(int ticketId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tickets
            .AsNoTracking()
            .Where(t => t.TicketId == ticketId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<Tickets>> Listar (Expression<Func<Tickets, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tickets
            .Include(t => t.Cliente)
            .Include(t => t.Tecnico)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Paginacion<Tickets>> ListarTicketsFiltrados(
    string filtroTipo,
    string? valorFiltro,
    DateTime? fechaDesde,
    DateTime? fechaHasta,
    int pageIndex = 1, 
    int pageSize = 10) 
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        IQueryable<Tickets> query = contexto.Tickets
            .Include(t => t.Cliente)
            .Include(t => t.Tecnico)
            .AsNoTracking(); 

        if (!string.IsNullOrWhiteSpace(valorFiltro))
        {
            string lowerValorFiltro = valorFiltro.ToLower();
            switch (filtroTipo)
            {
                case "Prioridad":
                    query = query.Where(c => c.Prioridad != null && c.Prioridad.ToLower().Contains(lowerValorFiltro));
                    break;
                case "NombreCliente":
                    query = query.Where(c => c.Cliente != null && c.Cliente.Nombres.ToLower().Contains(lowerValorFiltro));
                    break;
                case "Asunto":
                    query = query.Where(c => c.Asunto != null && c.Asunto.ToLower().Contains(lowerValorFiltro));
                    break;
                case "Descripcion":
                    query = query.Where(c => c.Descripcion != null && c.Descripcion.ToLower().Contains(lowerValorFiltro));
                    break;
                case "NombreTecnico":
                    query = query.Where(c => c.Tecnico != null && c.Tecnico.Nombre.ToLower().Contains(lowerValorFiltro));
                    break;
                case "TicketId":
                    if (int.TryParse(valorFiltro, out int id))
                    {
                        query = query.Where(c => c.TicketId == id);
                    }
                    break;
            }
        }

        DateTime? fechaDesdeUtc = fechaDesde?.ToUniversalTime().Date;
        DateTime? fechaHastaUtc = fechaHasta?.ToUniversalTime().Date;

        if (fechaDesdeUtc.HasValue && fechaHastaUtc.HasValue)
        {
            query = query.Where(c => c.Fecha.Date >= fechaDesdeUtc.Value && c.Fecha.Date <= fechaHastaUtc.Value);
        }
        else if (fechaDesdeUtc.HasValue)
        {
            query = query.Where(c => c.Fecha.Date >= fechaDesdeUtc.Value);
        }
        else if (fechaHastaUtc.HasValue)
        {
            query = query.Where(c => c.Fecha.Date <= fechaHastaUtc.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query.OrderBy(c => c.TicketId) 
                             .Skip((pageIndex - 1) * pageSize) 
                             .Take(pageSize)                   
                             .ToListAsync();

        return new Paginacion<Tickets>(items, totalCount, pageIndex, pageSize);
    }
}
