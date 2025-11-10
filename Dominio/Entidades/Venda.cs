using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Venda
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public DateTime DataVenda { get; set; } = DateTime.Now;
        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; } = 0;
        public string Status { get; set; } = "Pendente"; // Pendente, Confirmada, Cancelada, Finalizada
        public string FormaPagamento { get; set; } = String.Empty; // Dinheiro, Cartão, PIX, etc.
        public string Observacoes { get; set; } = String.Empty;
        public virtual Cliente? Cliente { get; set; }
        public virtual ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
    }
}

