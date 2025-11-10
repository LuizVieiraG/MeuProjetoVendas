using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dominio.Entidades
{
    public class Cliente
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
        public bool Ativo { get; set; } = true;
        public DateTime DataCadastro { get; set; } = DateTime.Now;
        public virtual ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    }
}

