using AutoMapper;
using MediatR;
using POS.Application.DTOs.Payment;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Payments.Queries.GetPaymentById;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment == null) throw new DomainExceptions.EntityNotFound("Payment", request.Id);

        return _mapper.Map<PaymentDto>(payment);
    }
}
