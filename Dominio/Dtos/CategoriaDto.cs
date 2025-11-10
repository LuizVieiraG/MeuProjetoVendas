using Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public string Descricao { get; set; } = String.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        [JsonIgnore]
        public virtual List<ProdutoDto> Produtos { get; set; } = new List<ProdutoDto>();
    }
}
