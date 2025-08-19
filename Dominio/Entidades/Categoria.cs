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
        public string Descricao { get; set; } = String.Empty;
        public string DescricaoDetalhada { get; set; }



        public virtual List<Produto> produtos { get; set; } 
            = new List<Produto>();
    }
}
