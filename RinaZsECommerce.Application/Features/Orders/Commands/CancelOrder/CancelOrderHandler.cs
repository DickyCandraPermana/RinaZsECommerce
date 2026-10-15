using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (user == null) throw new KeyNotFoundException("User tidak ditemukan.");

        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
        if (order == null || (order.UserId != user.ProfileId && user.Role != "Admin"))
            throw new KeyNotFoundException("Pesanan tidak ditemukan.");

        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.AwaitingPayment)
            throw new InvalidOperationException("Hanya pesanan berstatus Pending atau Menunggu Pembayaran yang dapat dibatalkan.");

        // Kembalikan stok untuk semua item
        foreach (var detail in order.Details)
        {
            await _unitOfWork.Products.UpdateStockAsync(
                detail.ProductId,
                detail.Amount,
                user.ProfileId,
                StockTransactionType.Return);
        }

        order.Status = OrderStatus.Cancelled;
        await _unitOfWork.CompleteAsync();

        return true;
    }
}
