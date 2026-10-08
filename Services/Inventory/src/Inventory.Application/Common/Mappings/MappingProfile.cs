using AutoMapper;
using Inventory.Application.DTOs.Inventory;
using Inventory.Domain.Entities;

namespace Inventory.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<InventoryItem, StockLevelDto>()
        .ForCtorParam("Quantity", opt => opt.MapFrom(src => src.QuantityOnHand))
        .ForCtorParam("ReorderLevel", opt => opt.MapFrom(src => src.ReorderThreshold))
        .ForCtorParam("IsBelowReorderLevel", opt => opt.MapFrom(src => src.IsBelowReorderThreshold()));
    }
}