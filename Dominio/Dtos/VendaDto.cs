using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio.Dtos
{
    public class VendaDto
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string NomeCliente { get; set; } = String.Empty;
        public DateTime DataVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Desconto { get; set; }
        public string Status { get; set; } = String.Empty;
        public string FormaPagamento { get; set; } = String.Empty;
        public string Observacoes { get; set; } = String.Empty;

        [JsonIgnore]
        public virtual ClienteDto? Cliente { get; set; }
        public virtual List<ItemVendaDto> ItensVenda { get; set; } = new List<ItemVendaDto>();
    }
}

