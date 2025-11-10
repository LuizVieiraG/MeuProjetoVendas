using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class ItemVenda
    {
        public int Id { get; set; }
        public int IdVenda { get; set; }
        public int IdProduto { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Desconto { get; set; } = 0;
        public decimal Subtotal { get; set; }
        public virtual Venda? Venda { get; set; }
        public virtual Produto? Produto { get; set; }
    }
}

