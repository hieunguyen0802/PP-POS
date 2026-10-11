using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Domain.Exceptions;
using Inventory.Domain.Interfaces;
using MediatR;

namespace Inventory.Application.Features.Suppliers.Queries.GetSuppliers;

public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, IReadOnlyList<SupplierDto>>
{
    private readonly ISupplierRepository _supplierRepository;

    private readonly IMapper _mapper;

    public GetSuppliersQueryHandler(
        ISupplierRepository supplierRepository,
        IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SupplierDto>> Handle(
        GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SupplierDto>>(suppliers);
    }
}