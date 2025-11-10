using Dominio.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto2025_API.Validation
{
    public class ProdutoValidation : AbstractValidator<ProdutoDto>
    {
        public ProdutoValidation()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(1000).WithMessage("Descrição deve ter no máximo 1000 caracteres");

            RuleFor(x => x.Marca)
                .MaximumLength(100).WithMessage("Marca deve ter no máximo 100 caracteres");

            RuleFor(x => x.Modelo)
                .MaximumLength(100).WithMessage("Modelo deve ter no máximo 100 caracteres");

            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero")
                .LessThan(999999.99m).WithMessage("Preço deve ser menor que 999.999,99");

            RuleFor(x => x.QuantidadeEstoque)
                .GreaterThanOrEqualTo(0).WithMessage("Quantidade em estoque não pode ser negativa");

            RuleFor(x => x.Especificacoes)
                .MaximumLength(2000).WithMessage("Especificações devem ter no máximo 2000 caracteres");

            RuleFor(x => x.ImagemUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(ValidarUrl).WithMessage("URL da imagem deve ter um formato válido")
                .When(x => !string.IsNullOrEmpty(x.ImagemUrl) && !string.IsNullOrWhiteSpace(x.ImagemUrl));

            RuleFor(x => x.IdCategoria)
                .GreaterThanOrEqualTo(0).WithMessage("Categoria é obrigatória");
        }

        private bool ValidarUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out Uri? result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}

