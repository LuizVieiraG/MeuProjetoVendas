using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public string Descricao { get; set; } = String.Empty;
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}
