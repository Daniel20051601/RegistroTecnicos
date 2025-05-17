using Microsoft.EntityFrameworkCore;
using RegistroTecnicos.DAL;
using RegistroTecnicos.Models;
using System.Linq.Expressions;


namespace RegistroTecnicos.Services
{
    public class TecnicosService(IDbContextFactory<Contexto> DbFactory)
    {
        private async Task<bool> Existe(int TecnicoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Tecnicos
                .AnyAsync(t => t.TecnicoId == TecnicoId);
        }

        private async Task<bool> Insertar(Tecnicos tecnico)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync ();
            contexto.Tecnicos.Add(tecnico);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<bool> Modificar(Tecnicos tecnico)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            contexto.Update(tecnico);
            return await contexto.SaveChangesAsync() > 0;
        }

        private async Task<Boolean> Guardar(Tecnicos tecnico)
        {
            if(!await Existe(tecnico.TecnicoId))
            {
                return await Insertar(tecnico);
            }
            else
            {
                return await Modificar(tecnico);
            }
        }

        public async Task<bool> Buscar(int tecnicoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Tecnicos
                ///.Include(t=>t.TecnicoId)
                .FirstOrDefaultAsync(t => t.TecnicoId == tecnicoId) != null;
        }

        public async Task<bool> Eliminar(int tecnicoId)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Tecnicos.AsNoTracking()
                .Where(t => t.TecnicoId == tecnicoId)
                .ExecuteDeleteAsync() > 0;
        }

        public async Task<List<Tecnicos>> Listar(Expression<Func<Tecnicos,bool>> criterio)
        {
            await using var contexto = await DbFactory.CreateDbContextAsync();
            return await contexto.Tecnicos
                ///.Include(t => t.TecnicoId)
                .Where(criterio)
                .AsNoTracking()
                .ToListAsync();

        }
    }
}
