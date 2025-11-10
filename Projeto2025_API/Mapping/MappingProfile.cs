using AutoMapper;
using Dominio.Dtos;
using Dominio.Entidades;

namespace Projeto2025_API.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile() {
            CreateMap<Categoria, CategoriaDto>()
                .ReverseMap();
            CreateMap<Produto, ProdutoDto>()
                .ReverseMap();
            CreateMap<Cliente, ClienteDto>()
                .ReverseMap();
            CreateMap<Venda, VendaDto>()
                .ForMember(dest => dest.NomeCliente, opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Nome : string.Empty))
                .ReverseMap();
            CreateMap<ItemVenda, ItemVendaDto>()
                .ReverseMap();
        }
    }
}
