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
        public string Nome { get; set; } = String.Empty;
        public string Descricao { get; set; } = String.Empty;
        public string Marca { get; set; } = String.Empty;
        public string Modelo { get; set; } = String.Empty;
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string Especificacoes { get; set; } = String.Empty;
        public string ImagemUrl { get; set; } = String.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
        public int IdCategoria { get; set; }
        public string NomeCategoria { get; set; } = String.Empty;

        [JsonIgnore]
        public virtual CategoriaDto? Categoria { get; set; }
    }
}
