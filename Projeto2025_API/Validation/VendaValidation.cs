using Dominio.Dtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projeto2025_API.Validation
{
    public class VendaValidation : AbstractValidator<VendaDto>
    {
        public VendaValidation()
        {
            RuleFor(x => x.IdCliente)
                .GreaterThan(0).WithMessage("Cliente é obrigatório");

            RuleFor(x => x.ValorTotal)
                .GreaterThan(0).WithMessage("Valor total deve ser maior que zero")
                .LessThan(999999.99m).WithMessage("Valor total deve ser menor que 999.999,99");

            RuleFor(x => x.Desconto)
                .GreaterThanOrEqualTo(0).WithMessage("Desconto não pode ser negativo")
                .LessThan(999999.99m).WithMessage("Desconto deve ser menor que 999.999,99");

            RuleFor(x => x.Status)
                .Must(ValidarStatus).WithMessage("Status deve ser: Pendente, Confirmada, Cancelada ou Finalizada");

            RuleFor(x => x.FormaPagamento)
                .NotEmpty().WithMessage("Forma de pagamento é obrigatória")
                .MaximumLength(50).WithMessage("Forma de pagamento deve ter no máximo 50 caracteres");

            RuleFor(x => x.Observacoes)
                .MaximumLength(1000).WithMessage("Observações devem ter no máximo 1000 caracteres");

            RuleFor(x => x.ItensVenda)
                .NotEmpty().WithMessage("Venda deve ter pelo menos um item")
                .Must(ValidarItensVenda).WithMessage("Itens da venda são inválidos");
        }

        private bool ValidarStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
                return false;

            var statusValidos = new[] { "Pendente", "Confirmada", "Cancelada", "Finalizada" };
            return statusValidos.Contains(status);
        }

        private bool ValidarItensVenda(List<ItemVendaDto> itens)
        {
            if (itens == null || !itens.Any())
                return false;

            foreach (var item in itens)
            {
                if (item.IdProduto <= 0)
                    return false;

                if (item.Quantidade <= 0)
                    return false;

                if (item.PrecoUnitario <= 0)
                    return false;

                if (item.Desconto < 0)
                    return false;

                if (item.Subtotal != (item.Quantidade * item.PrecoUnitario - item.Desconto))
                    return false;
            }

            return true;
        }
    }
}

