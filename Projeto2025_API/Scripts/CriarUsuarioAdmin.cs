using Dominio.Entidades;
using InfraEstrutura.Data;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Projeto2025_API.Scripts
{
    public class CriarUsuarioAdmin
    {
        public static async Task CriarAdminAsync()
        {
            var optionsBuilder = new DbContextOptionsBuilder<EmpresaContexto>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-TIU724C;DataBase=dbEmpresa2025;integrated security=true;TrustServerCertificate=True;");
            
            using var context = new EmpresaContexto(optionsBuilder.Options);
            
            // Verificar se já existe um admin
            var adminExistente = await context.Usuarios
                .FirstOrDefaultAsync(u => u.UserName == "ana" || u.Role == "Admin");
            
            if (adminExistente == null)
            {
                // Criar usuário administrador
                var admin = new Usuario
                {
                    Nome = "Administrador",
                    Email = "admin@sistema.com",
                    UserName = "ana",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456", BCrypt.Net.BCrypt.GenerateSalt(12)),
                    Role = "Admin",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                };
                
                context.Usuarios.Add(admin);
                await context.SaveChangesAsync();
                
                Console.WriteLine("✅ Usuário administrador criado com sucesso!");
                Console.WriteLine("Username: ana");
                Console.WriteLine("Senha: 123456");
                Console.WriteLine("Role: Admin");
            }
            else
            {
                Console.WriteLine("ℹ️ Usuário administrador já existe!");
            }
        }
    }
}
