using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;

namespace RegistroTecnicos.Services;

public class VentasService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> Existe(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas.AnyAsync(v => v.VentaId == ventaId);
    }

    public async Task<bool> Modificar(Ventas venta)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Entry(venta).State = EntityState.Modified;
        var detallesExistentes = await contexto.VentasDetalle
                                               .Where(d => d.VentaId == venta.VentaId)
                                               .ToListAsync();

        foreach (var detalleExistente in detallesExistentes)
        {
            if (!venta.VentasDetalles.Any(d => d.VentasDetalleId == detalleExistente.VentasDetalleId))
            {
                contexto.VentasDetalle.Remove(detalleExistente);
            }
        }

        foreach (var detalleNuevoOModificado in venta.VentasDetalles)
        {
            if (detalleNuevoOModificado.VentasDetalleId == 0) 
            {
                detalleNuevoOModificado.VentaId = venta.VentaId;
                contexto.VentasDetalle.Add(detalleNuevoOModificado);

                if (detalleNuevoOModificado.Sistema != null && detalleNuevoOModificado.Sistema.SistemaId > 0)
                {
                    contexto.Entry(detalleNuevoOModificado.Sistema).State = EntityState.Unchanged;
                }
            }
            else 
            {
                contexto.Entry(detalleNuevoOModificado).State = EntityState.Modified;

                if (detalleNuevoOModificado.Sistema != null && detalleNuevoOModificado.Sistema.SistemaId > 0)
                {
                    contexto.Entry(detalleNuevoOModificado.Sistema).State = EntityState.Unchanged;
                }
            }
        }

        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Insertar(Ventas venta)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        foreach (var detalle in venta.VentasDetalles)
        {
            if (detalle.Sistema != null && detalle.Sistema.SistemaId > 0)
            {
                contexto.Entry(detalle.Sistema).State = EntityState.Unchanged;
            }
        }

        contexto.Ventas.Add(venta); 
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Ventas venta)
    {
        if (venta.VentaId == 0 || !await Existe(venta.VentaId))
        {
            return await Insertar(venta);
        }
        else
        {
            return await Modificar(venta);
        }
    }

    public async Task<Ventas?> Buscar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(c => c.Cliente)
            .Include(v => v.VentasDetalles)
                .ThenInclude(s => s.Sistema)
            .FirstOrDefaultAsync(v => v.VentaId == ventaId);
    }

    public async Task<bool> Eliminar(int ventaId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Where(v => v.VentaId == ventaId)
            .ExecuteDeleteAsync() > 0;
    }

    public async Task<List<Ventas>> Listar(Expression<Func<Ventas, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(c => c.Cliente)
            .Include(v => v.VentasDetalles)
                .ThenInclude(s => s.Sistema)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Paginacion<Ventas>> ListarVentasFiltradas(
        string filtroTipo,
        string? valorFiltro,
        DateTime? fechaDesde,
        DateTime? fechaHasta,
        int pageIndex = 1,
        int pageSize = 10)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        IQueryable<Ventas> query = contexto.Ventas
            .Include(c => c.Cliente)
            .Include(v => v.VentasDetalles)
                .ThenInclude(s => s.Sistema)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(valorFiltro))
        {
            string lowerValorFiltro = valorFiltro.ToLower();
            switch (filtroTipo)
            {
                case "Cliente":
                    query = query.Where(c => c.Cliente.Nombres != null && c.Cliente.Nombres.ToLower().Contains(lowerValorFiltro));
                    break;
                case "DescripcionSistema":
                    query = query.Where(s => s.VentasDetalles.Any() &&
                                             s.VentasDetalles.FirstOrDefault().Sistema.DescripcionSistema != null
                                             && s.VentasDetalles.FirstOrDefault().Sistema.DescripcionSistema.ToLower().Contains(lowerValorFiltro));
                    break;
                case "VentaId":
                    if (int.TryParse(valorFiltro, out int id))
                    {
                        query = query.Where(v => v.VentaId == id);
                    }
                    break;
            }
        }

        DateTime? fechaDesdeUtc = fechaDesde?.ToUniversalTime().Date;
        DateTime? fechaHastaUtc = fechaHasta?.ToUniversalTime().Date;

        if (fechaDesdeUtc.HasValue && fechaHastaUtc.HasValue)
        {
            query = query.Where(v => v.Fecha.Date >= fechaDesdeUtc.Value && v.Fecha.Date <= fechaHastaUtc.Value);
        }
        else if (fechaDesdeUtc.HasValue)
        {
            query = query.Where(v => v.Fecha.Date >= fechaDesdeUtc.Value);
        }
        else if (fechaHastaUtc.HasValue)
        {
            query = query.Where(v => v.Fecha.Date <= fechaHastaUtc.Value);
        }

        int totalCount = await query.CountAsync();

        var items = await query.OrderBy(v => v.VentaId)
                               .Skip((pageIndex - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

        return new Paginacion<Ventas>(items, totalCount, pageIndex, pageSize);
    }
}