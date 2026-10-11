using AutoMapper;
using Inventory.Application.DTOs;
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

        CreateMap<StockAdjustment, StockAdjustmentDto>();
        CreateMap<Supplier, SupplierDto>();
        CreateMap<PurchaseOrderItem, PurchaseOrderItemDto>();
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
        .ForCtorParam("SupplierName", opt => opt.MapFrom(src => src.Supplier.Name))
        .ForCtorParam("Total", opt => opt.MapFrom(src => src.Items.Sum(i => i.Quantity * i.UnitPrice)));
    }
}