using MediatR;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Payments.Commands.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeGenerator _codeGenerator;

    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork, ICodeGenerator codeGenerator)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
    }

    public async Task<Guid> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null) throw new DomainExceptions.EntityNotFound("Order", request.OrderId);

        var referenceNumber = await _codeGenerator.GenerateCodeAsync("PAY", cancellationToken);
        var payment = order.RecordPayment(request.PaymentMethod, request.AmountTendered, referenceNumber);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }
}
