using AutoMapper;
using MediatR;
using POS.Application.DTOs.Payment;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Payments.Queries.GetPaymentsByOrderId;

public class GetPaymentsByOrderIdQueryHandler : IRequestHandler<GetPaymentsByOrderIdQuery, List<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentsByOrderIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<List<PaymentDto>> Handle(GetPaymentsByOrderIdQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        return _mapper.Map<List<PaymentDto>>(payments);
    }
}
