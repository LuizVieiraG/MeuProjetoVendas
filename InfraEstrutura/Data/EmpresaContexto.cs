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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Categoria>(builder => {
                builder.Property(p => p.Descricao)
                .IsRequired().HasMaxLength(150);//Descricao é obrigatorio com max 150
                builder.ToTable("Categoria");//nome da tabela
                builder.HasKey(p => p.Id);//chave primaria
                
            });


            modelBuilder.Entity<Produto>(builder => {
                builder.ToTable("Produto");
                builder.HasKey(p => p.Id);
                builder.Property(p => p.Descricao)
                .IsRequired().HasMaxLength(151);
                builder.Property(p => p.Valor)
                .HasPrecision(8,2).IsRequired();
                builder.Property(p => p.Quantidade)
                .IsRequired();
                //relacionamento de produto com categoria
                builder.HasOne(p=>p.Categoria) //lado um
                .WithMany(p=>p.produtos)//lado muitos
                .HasForeignKey(p=>p.IdCategoria) //chave estrangeira
                .OnDelete(DeleteBehavior.Restrict);
            });


        }
    }
}
