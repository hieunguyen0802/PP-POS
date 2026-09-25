using MediatR;

namespace POS.Application.Features.Products.Commands.ToggleProductStatus;

public record ToggleProductStatusCommand(Guid ProductId) : IRequest<Unit>;