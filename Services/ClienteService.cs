using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;

namespace RegistroTecnicos.Services;

public class ClienteService(IDbContextFactory<Contexto> DbFactory)
{
    public async Task<bool> ExisteNombreORnc(Expression<Func<Clientes, bool>> criterio, int? idExcluir = null)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();

        var query = contexto.Clientes.AsQueryable();

        if (idExcluir.HasValue)
        {
            query = query.Where(c => c.ClienteId !=idExcluir.Value);
        }
        return await query.AnyAsync(criterio);
    }

    public async Task<bool> Insertar(Clientes cliente)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Clientes.Add(cliente);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Modificar(Clientes cliente)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Clientes.Update(cliente);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Clientes cliente)
    {
        if (cliente.ClienteId == 0)
        {
            if (await ExisteNombreORnc(c => c.Nombres == cliente.Nombres))
            {
                return false;
            }
            if (await ExisteNombreORnc(c => c.Rnc == cliente.Rnc))
            {
                return false;
            }
            return await Insertar(cliente);
        }
        else
        {
            if (await ExisteNombreORnc(c => c.Nombres == cliente.Nombres, cliente.ClienteId))
            {
                return false;
            }
            if (await ExisteNombreORnc(c => c.Rnc == cliente.Rnc, cliente.ClienteId))
            {
                return false;
            }
            return await Modificar(cliente);
        }
    }

    public async Task<Clientes?> Buscar(int clienteId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Clientes
            .Include(c => c.Tecnico)  
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
    }

    public async Task<bool> Eliminar(int clienteId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        try
        {
            var filaAfectada = await contexto.Clientes
                .Where(c => c.ClienteId == clienteId)  
                .ExecuteDeleteAsync();
            return filaAfectada > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar cliente en el servicio: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Clientes>> Listar(Expression<Func<Clientes, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Clientes
            .Include(c => c.Tecnico)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}