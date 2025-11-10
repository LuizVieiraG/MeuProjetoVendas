using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public string Descricao { get; set; } = String.Empty;
        public string Marca { get; set; } = String.Empty;
        public string Modelo { get; set; } = String.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string Especificacoes { get; set; } = String.Empty;
        public string ImagemUrl { get; set; } = String.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public int IdCategoria { get; set; }
        public virtual Categoria? Categoria { get; set; }
        public virtual ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    }
}
