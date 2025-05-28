using Blazored.Toast.Services;
using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;

namespace RegistroTecnicos.Services;

public class TecnicosService(IDbContextFactory<Contexto> DbFactory)
{
    private async Task<bool> Existe(int TecnicoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos
            .AnyAsync(t => t.TecnicoId == TecnicoId);
    }

    public async Task<bool> ExisteNombre(string nombre, int? idExcluir = null)
    {

        await using var context = await DbFactory.CreateDbContextAsync();
        var query = context.Tecnicos.AsQueryable();

        if (idExcluir.HasValue)
        {

            query = query.Where(t => t.TecnicoId != idExcluir.Value);
        }

        return await query.AnyAsync(t => t.Nombre == nombre);
    }

    private async Task<bool> Insertar(Tecnicos tecnico)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Tecnicos.Add(tecnico);
        return await contexto.SaveChangesAsync() > 0;
    }

    private async Task<bool> Modificar(Tecnicos tecnico)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        contexto.Update(tecnico);
        return await contexto.SaveChangesAsync() > 0;
    }

    public async Task<bool> Guardar(Tecnicos tecnico)
    {
        Console.WriteLine($"Guardando tecnico con Id: {tecnico.TecnicoId} ");
        if (tecnico.TecnicoId == 0)
        {
            if (await ExisteNombre(tecnico.Nombre))
                return false;

            return await Insertar(tecnico);
        }
        else
        {
            if (!await Existe(tecnico.TecnicoId)) 
                return false;

            if (await ExisteNombre(tecnico.Nombre, tecnico.TecnicoId))
                return false;

            return await Modificar(tecnico);
        }
    }


    public async Task<Tecnicos?> Buscar(int TecnicoId)
    {
        await using var context = await DbFactory.CreateDbContextAsync();
        return await context.Tecnicos
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TecnicoId == TecnicoId);
    }

    public async Task<bool> Eliminar(int tecnicoId)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        try
        {
            var rowsAffected = await contexto.Tecnicos
                .Where(t => t.TecnicoId == tecnicoId)
                .ExecuteDeleteAsync();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar técnico en el servicio: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Tecnicos>> Listar(Expression<Func<Tecnicos, bool>> criterio)
    {
        await using var contexto = await DbFactory.CreateDbContextAsync();
        return await contexto.Tecnicos
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }
}
