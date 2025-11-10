using Dominio.Entidades;
using InfraEstrutura.Data;
using Interface.Repositorio;
using Microsoft.EntityFrameworkCore;

namespace InfraEstrutura.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly EmpresaContexto _contexto;

        public UsuarioRepositorio(EmpresaContexto contexto)
        {
            _contexto = contexto;
        }

        public async Task<Usuario> addAsync(Usuario usuario)
        {
            await _contexto.Usuarios.AddAsync(usuario);
            await _contexto.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> getAsync(int id)
        {
            return await _contexto.Usuarios.FindAsync(id);
        }

        public async Task<Usuario?> getByUserNameAsync(string userName)
        {
            return await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == userName && u.Ativo);
        }

        public async Task<Usuario?> getByEmailAsync(string email)
        {
            return await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
        }

        public async Task<IEnumerable<Usuario>> getAllAsync()
        {
            return await _contexto.Usuarios
                .Where(u => u.Ativo)
                .ToListAsync();
        }

        public async Task removeAsync(int id)
        {
            var usuario = await _contexto.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Ativo = false;
                await _contexto.SaveChangesAsync();
            }
        }

        public async Task<Usuario> updateAsync(Usuario usuario)
        {
            _contexto.Usuarios.Update(usuario);
            await _contexto.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario?> getByRefreshTokenAsync(string refreshToken)
        {
            return await _contexto.Usuarios
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && 
                                    u.RefreshTokenExpiryTime > DateTime.Now && 
                                    u.Ativo);
        }
    }
}
