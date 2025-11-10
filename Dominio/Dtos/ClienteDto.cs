using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dominio.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Telefone { get; set; } = String.Empty;
        public string Cpf { get; set; } = String.Empty;
        public DateTime DataNascimento { get; set; }
        public string Endereco { get; set; } = String.Empty;
        public string Cidade { get; set; } = String.Empty;
        public string Estado { get; set; } = String.Empty;
        public string Cep { get; set; } = String.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }

        [JsonIgnore]
        public virtual List<VendaDto> Vendas { get; set; } = new List<VendaDto>();
    }
}

