using Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfraEstrutura.Data
{
    public class EmpresaContexto:DbContext
    {

        public EmpresaContexto(
            DbContextOptions<EmpresaContexto> opcoes)
            :base(opcoes) { 
        }


        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(builder => {
                builder.Property(p => p.Nome)
                .IsRequired().HasMaxLength(100);
                builder.Property(p => p.Descricao)
                .HasMaxLength(500);
                builder.ToTable("Categoria");
                builder.HasKey(p => p.Id);
            });


            modelBuilder.Entity<Produto>(builder => {
                builder.ToTable("Produto");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Nome)
                .IsRequired().HasMaxLength(200);
                builder.Property(p => p.Descricao)
                .HasMaxLength(1000);
                builder.Property(p => p.Marca)
                .HasMaxLength(100);
                builder.Property(p => p.Modelo)
                .HasMaxLength(100);
                builder.Property(p => p.Preco)
                .HasPrecision(10,2).IsRequired();
                builder.Property(p => p.QuantidadeEstoque)
                .IsRequired();
                builder.Property(p => p.Especificacoes)
                .HasMaxLength(2000);
                builder.Property(p => p.ImagemUrl)
                .HasMaxLength(500);
                
                //relacionamento de produto com categoria
                builder.HasOne(p=>p.Categoria)
                .WithMany(p=>p.Produtos)
                .HasForeignKey(p=>p.IdCategoria)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade Cliente
            modelBuilder.Entity<Cliente>(builder => {
                builder.ToTable("Cliente");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Nome)
                .IsRequired().HasMaxLength(200);
                builder.Property(p => p.Email)
                .IsRequired().HasMaxLength(100);
                builder.Property(p => p.Telefone)
                .HasMaxLength(20);
                builder.Property(p => p.Cpf)
                .HasMaxLength(14);
                builder.Property(p => p.Endereco)
                .HasMaxLength(300);
                builder.Property(p => p.Cidade)
                .HasMaxLength(100);
                builder.Property(p => p.Estado)
                .HasMaxLength(2);
                builder.Property(p => p.Cep)
                .HasMaxLength(10);
            });

            // Configuração da entidade Venda
            modelBuilder.Entity<Venda>(builder => {
                builder.ToTable("Venda");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.ValorTotal)
                .HasPrecision(10,2).IsRequired();
                builder.Property(p => p.Desconto)
                .HasPrecision(10,2);
                builder.Property(p => p.Status)
                .HasMaxLength(50);
                builder.Property(p => p.FormaPagamento)
                .HasMaxLength(50);
                builder.Property(p => p.Observacoes)
                .HasMaxLength(1000);
                
                //relacionamento de venda com cliente
                builder.HasOne(p=>p.Cliente)
                .WithMany(p=>p.Vendas)
                .HasForeignKey(p=>p.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade ItemVenda
            modelBuilder.Entity<ItemVenda>(builder => {
                builder.ToTable("ItemVenda");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Quantidade)
                .IsRequired();
                builder.Property(p => p.PrecoUnitario)
                .HasPrecision(10,2).IsRequired();
                builder.Property(p => p.Desconto)
                .HasPrecision(10,2);
                builder.Property(p => p.Subtotal)
                .HasPrecision(10,2).IsRequired();
                
                //relacionamento de itemvenda com venda
                builder.HasOne(p=>p.Venda)
                .WithMany(p=>p.ItensVenda)
                .HasForeignKey(p=>p.IdVenda)
                .OnDelete(DeleteBehavior.Cascade);
                
                //relacionamento de itemvenda com produto
                builder.HasOne(p=>p.Produto)
                .WithMany(p=>p.ItensVenda)
                .HasForeignKey(p=>p.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade Usuario
            modelBuilder.Entity<Usuario>(builder => {
                builder.ToTable("Usuario");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Nome)
                .IsRequired().HasMaxLength(200);
                builder.Property(p => p.Email)
                .IsRequired().HasMaxLength(100);
                builder.Property(p => p.UserName)
                .IsRequired().HasMaxLength(50);
                builder.Property(p => p.SenhaHash)
                .IsRequired().HasMaxLength(255);
                builder.Property(p => p.Role)
                .HasMaxLength(50).HasDefaultValue("User");
                builder.Property(p => p.RefreshToken)
                .HasMaxLength(500);
                
                // Índices únicos
                builder.HasIndex(p => p.Email).IsUnique();
                builder.HasIndex(p => p.UserName).IsUnique();
            });

        }
    }
}
