using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio.Dtos
{
    public class ItemVendaDto
    {
        public int Id { get; set; }
        public int IdVenda { get; set; }
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = String.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Desconto { get; set; }
        public decimal Subtotal { get; set; }

        [JsonIgnore]
        public virtual VendaDto? Venda { get; set; }
        public virtual ProdutoDto? Produto { get; set; }
    }
}

