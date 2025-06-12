using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;

namespace RegistroTecnicos.Services;

public class SistemasService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int SistemaId)
    {
        using var contexto = DbFactory.CreateDbContext();
        return await contexto.Sistemas.AnyAsync(s => s.SistemaId == SistemaId);
    }

    public async Task<bool> Insertar(Sistemas sistema)
    {
        using var contexto = DbFactory.CreateDbContext();
        contexto.Sistemas.Add(sistema);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Sistemas sistema)
    {
        using var contexto = DbFactory.CreateDbContext();
        contexto.Sistemas.Update(sistema);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Sistemas sistema)
    {
        if (!await Existe(sistema.SistemaId))
        {
            return await Insertar(sistema);
        }
        else
        {
            return await Modificar(sistema);
        }
    }

    public async Task<Sistemas?> Buscar(int SistemaId)
    {
        using var contexto = DbFactory.CreateDbContext();
        return await contexto.Sistemas
            .FirstOrDefaultAsync(s => s.SistemaId == SistemaId);
    }

    public async Task<bool> Eliminar(int SistemaId)
    {
        using var contexto = DbFactory.CreateDbContext();
        return await contexto.Sistemas
            .AsNoTracking()
            .Where(s => s.SistemaId == SistemaId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<Sistemas>> Listar(Expression<Func<Sistemas, bool>> criterio)
    {
        using var contexto = DbFactory.CreateDbContext();
        return await contexto.Sistemas
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Paginacion<Sistemas>> ListarTicketsFiltrados(
        string filtroTipo,
        string? valorFiltro,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageIndex = 1,
        int pageSize = 10)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        IQueryable<Sistemas> query = contexto.Sistemas;

        if (!string.IsNullOrWhiteSpace(valorFiltro))
        {
            string lowerValorFiltro = valorFiltro.ToLower();
            switch (filtroTipo)
            {
                case "Complejidad":
                    query = query.Where(s => s.Complejidad != null && s.Complejidad.ToLower().Contains(lowerValorFiltro));
                    break;
                case "Descripcion":
                    query = query.Where(s => s.DescripcionSistema != null && s.DescripcionSistema.ToLower().Contains(lowerValorFiltro));
                    break;
                case "SistemaId":
                    if (int.TryParse(valorFiltro, out int id))
                    {
                        query = query.Where(c => c.SistemaId == id);
                    }
                    break;
            }
        }

        DateTime? fechaDesdeUtc = fechaDesde?.ToUniversalTime().Date;
        DateTime? fechaHastaUtc = fechaHasta?.ToUniversalTime().Date;

        if (fechaDesdeUtc.HasValue && fechaHastaUtc.HasValue)
        {
            query = query.Where(s => s.Fecha.Date >= fechaDesdeUtc.Value && s.Fecha.Date <= fechaHastaUtc.Value);
        }
        else if (fechaDesdeUtc.HasValue)
        {
            query = query.Where(s => s.Fecha.Date >= fechaDesdeUtc.Value);
        }
        else if (fechaHastaUtc.HasValue)
        {
            query = query.Where(s => s.Fecha.Date <= fechaHastaUtc.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query.OrderBy(s => s.SistemaId)
                             .Skip((pageIndex - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();

        return new Paginacion<Sistemas>(items, totalCount, pageIndex, pageSize);
    }






}
