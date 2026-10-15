using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Admin.Orders.Commands.UpdateStatus;

public class UpdateOrderStatusHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderStatusHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null) return false;

        order.Status = request.NewStatus;
        await _unitOfWork.CompleteAsync();
        return true;
    }
}
