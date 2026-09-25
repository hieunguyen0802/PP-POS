using AutoMapper;
using POS.Application.DTOs.Customer;
using POS.Application.DTOs.Order;
using POS.Application.DTOs.Payment;
using POS.Application.DTOs.Product;
using POS.Application.DTOs.User;
using POS.Domain.Entities;

namespace POS.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForCtorParam("Status", opt => opt.MapFrom(src => src.OrderStatus.ToString()))
            .ForCtorParam("Items", opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<OrderItem, OrderItemDto>()
            .ForCtorParam("ProductName", opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));
        CreateMap<Product, ProductDto>();
        CreateMap<Customer, CustomerDto>();
        CreateMap<User, UserDto>()
            .ForCtorParam("Role", opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<Payment, PaymentDto>()
            .ForCtorParam("PaymentMethod", opt => opt.MapFrom(src => src.PaymentMethod.ToString()));

    }
}