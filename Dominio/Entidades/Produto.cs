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
        public string Descricao { get; set; } = String.Empty;
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public int IdCategoria { get; set; }
        public virtual Categoria? Categoria { get; set; }
    }
}
