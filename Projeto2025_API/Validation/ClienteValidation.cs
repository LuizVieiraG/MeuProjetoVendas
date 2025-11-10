using Dominio.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Projeto2025_API.Validation
{
    public class ClienteValidation : AbstractValidator<ClienteDto>
    {
        public ClienteValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email deve ter um formato válido")
                .MaximumLength(100).WithMessage("Email deve ter no máximo 100 caracteres");

            RuleFor(x => x.Telefone)
                .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

            RuleFor(x => x.Cpf)
                .Must(ValidarCpf).WithMessage("CPF deve ter um formato válido")
                .MaximumLength(14).WithMessage("CPF deve ter no máximo 14 caracteres");

            RuleFor(x => x.DataNascimento)
                .LessThan(DateTime.Now).WithMessage("Data de nascimento deve ser anterior à data atual")
                .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Data de nascimento inválida")
                .When(x => x.DataNascimento != default(DateTime));

            RuleFor(x => x.Endereco)
                .MaximumLength(300).WithMessage("Endereço deve ter no máximo 300 caracteres");

            RuleFor(x => x.Cidade)
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

            RuleFor(x => x.Estado)
                .MaximumLength(2).WithMessage("Estado deve ter no máximo 2 caracteres");

            RuleFor(x => x.Cep)
                .Must(ValidarCep).WithMessage("CEP deve ter um formato válido")
                .MaximumLength(10).WithMessage("CEP deve ter no máximo 10 caracteres");
        }

        private bool ValidarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return true; // CPF é opcional

            cpf = Regex.Replace(cpf, @"[^\d]", "");
            
            if (cpf.Length != 11)
                return false;

            // Verificar se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validar dígitos verificadores
            int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicadores2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private bool ValidarCep(string cep)
        {
            if (string.IsNullOrEmpty(cep))
                return true; // CEP é opcional

            cep = Regex.Replace(cep, @"[^\d]", "");
            return cep.Length == 8;
        }
    }
}

