using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio.Dtos
{
    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = String.Empty;
        public decimal Valor { get; set; }
        public int Quantidade { get; set; }
        public int IdCategoria { get; set; }

        [JsonIgnore]
        public virtual CategoriaDto? Categoria { get; set; }
    }
}
