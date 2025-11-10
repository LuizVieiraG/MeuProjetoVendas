using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repositorio
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario> addAsync(Usuario usuario);
        Task<Usuario?> getAsync(int id);
        Task<Usuario?> getByUserNameAsync(string userName);
        Task<Usuario?> getByEmailAsync(string email);
        Task<IEnumerable<Usuario>> getAllAsync();
        Task removeAsync(int id);
        Task<Usuario> updateAsync(Usuario usuario);
        Task<Usuario?> getByRefreshTokenAsync(string refreshToken);
    }
}
