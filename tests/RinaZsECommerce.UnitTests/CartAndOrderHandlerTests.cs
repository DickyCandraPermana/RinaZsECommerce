using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RinaZsECommerce.Application.Features.Carts.Commands.UpdateQuantity;
using RinaZsECommerce.Application.Features.Orders.Commands.CancelOrder;
using RinaZsECommerce.Application.Features.Products.Queries.GetProductById;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Enums;
using RinaZsECommerce.Domain.Interfaces;
using Xunit;

namespace RinaZsECommerce.UnitTests;

public class CartAndOrderHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<ICartItemRepository> _cartRepoMock;
    private readonly Mock<IOrderRepository> _orderRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;

    public CartAndOrderHandlerTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _cartRepoMock = new Mock<ICartItemRepository>();
        _orderRepoMock = new Mock<IOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();

        _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _uowMock.Setup(u => u.Carts).Returns(_cartRepoMock.Object);
        _uowMock.Setup(u => u.Orders).Returns(_orderRepoMock.Object);
        _uowMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
    }

    [Fact]
    public async Task UpdateCartItemQuantity_ValidQuantity_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var mockUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "Customer",
            ProfileId = profileId
        };

        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Price = 10000m,
            Stock = 10,
            SupplierId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };

        var cartItem = new CartItem
        {
            Id = cartItemId,
            UserId = profileId,
            ProductId = productId,
            Amount = 1,
            Product = product
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(mockUser);
        _cartRepoMock.Setup(r => r.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);
        _uowMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var handler = new UpdateCartItemQuantityHandler(_uowMock.Object);
        var command = new UpdateCartItemQuantityCommand(userId, cartItemId, 3);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        cartItem.Amount.Should().Be(3);
        _uowMock.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCartItemQuantity_QuantityExceedsStock_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var mockUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "Customer",
            ProfileId = profileId
        };

        var product = new Product
        {
            Id = productId,
            Name = "Low Stock Item",
            Price = 10000m,
            Stock = 2,
            SupplierId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };

        var cartItem = new CartItem
        {
            Id = cartItemId,
            UserId = profileId,
            ProductId = productId,
            Amount = 1,
            Product = product
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(mockUser);
        _cartRepoMock.Setup(r => r.GetByIdAsync(cartItemId)).ReturnsAsync(cartItem);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var handler = new UpdateCartItemQuantityHandler(_uowMock.Object);
        var command = new UpdateCartItemQuantityCommand(userId, cartItemId, 5);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*tidak mencukupi*");
    }

    [Fact]
    public async Task CancelOrder_PendingOrder_SetsCancelledAndRestocks()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var mockUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "Customer",
            ProfileId = profileId
        };

        var mockProduct = new Product
        {
            Id = productId,
            Name = "Kaos Combed",
            Price = 25000m,
            Stock = 10,
            SupplierId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid()
        };

        var mockProfile = new UserProfile
        {
            Id = profileId,
            UserId = userId,
            Address = "Surakarta"
        };

        var order = new Order
        {
            Id = orderId,
            UserId = profileId,
            Status = OrderStatus.Pending,
            TotalAmount = 50000m,
            Address = "Test Address",
            User = mockProfile,
            Details = new List<OrderDetail>
            {
                new OrderDetail
                {
                    OrderId = orderId,
                    ProductId = productId,
                    Amount = 2,
                    PriceAtPurchase = 25000m,
                    Product = mockProduct
                }
            }
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(mockUser);
        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _productRepoMock.Setup(r => r.UpdateStockAsync(productId, 2, profileId, StockTransactionType.Return)).ReturnsAsync(true);
        _uowMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);

        var handler = new CancelOrderHandler(_uowMock.Object);
        var command = new CancelOrderCommand(userId, orderId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
        _productRepoMock.Verify(r => r.UpdateStockAsync(productId, 2, profileId, StockTransactionType.Return), Times.Once);
        _uowMock.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelOrder_NonPendingOrder_ThrowsInvalidOperationException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var profileId = Guid.NewGuid();
        var orderId = Guid.NewGuid();

        var mockUser = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = "Customer",
            ProfileId = profileId
        };

        var mockProfile = new UserProfile
        {
            Id = profileId,
            UserId = userId,
            Address = "Surakarta"
        };

        var order = new Order
        {
            Id = orderId,
            UserId = profileId,
            Status = OrderStatus.Shipped,
            TotalAmount = 50000m,
            Address = "Test Address",
            User = mockProfile,
            Details = new List<OrderDetail>()
        };

        _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(mockUser);
        _orderRepoMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        var handler = new CancelOrderHandler(_uowMock.Object);
        var command = new CancelOrderCommand(userId, orderId);

        // Act
        var act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Hanya pesanan berstatus Pending*");
    }

    [Fact]
    public async Task GetProductById_ExistingProduct_ReturnsMappedResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var catId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Mechanical Keyboard",
            Description = "Solid tactile switch",
            Price = 450000m,
            Stock = 15,
            Rating = 4.8m,
            CategoryId = catId,
            SupplierId = Guid.NewGuid(),
            Category = new Category { Id = catId, Name = "Electronics" },
            Images = new List<ProductImage>
            {
                new ProductImage { ProductId = productId, ImageUrl = "https://example.com/kbd.jpg" }
            }
        };

        _productRepoMock.Setup(r => r.GetByIdAsync(productId)).ReturnsAsync(product);

        var handler = new GetProductByIdHandler(_uowMock.Object);
        var query = new GetProductByIdQuery(productId);

        // Act
        var response = await handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(productId);
        response.Name.Should().Be("Mechanical Keyboard");
        response.CategoryName.Should().Be("Electronics");
        response.ImageUrls.Should().ContainSingle().Which.Should().Be("https://example.com/kbd.jpg");
    }
}
