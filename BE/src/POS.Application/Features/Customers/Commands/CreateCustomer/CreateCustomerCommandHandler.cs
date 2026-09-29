using MediatR;
using POS.Domain.Entities;
using POS.Domain.Exceptions;
using POS.Domain.Interfaces;

namespace POS.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, ICodeGenerator codeGenerator, IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _codeGenerator = codeGenerator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var phoneAlreadyUsed = await _customerRepository.ExistsByPhoneAsync(request.CustomerPhone, cancellationToken);
        if (phoneAlreadyUsed)
            throw new DomainExceptions.EntityAlreadyExists("Customer", request.CustomerPhone);

        var customerCode = await _codeGenerator.GenerateCodeAsync("CUST", cancellationToken);

        var customer = new Customer(request.CustomerName, request.CustomerPhone, request.CustomerEmail, customerCode, request.CustomerAddress);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}
