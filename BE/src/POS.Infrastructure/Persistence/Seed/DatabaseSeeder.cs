using System.Globalization;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POS.Application.Common.Interfaces;
using POS.Domain.Entities;
using POS.Domain.Interfaces;

namespace POS.Infrastructure.Persistence.Seed;

// Dev-only convenience: populates an empty database with realistic-looking test data,
// going through the same domain constructors / ICodeGenerator / RecordPayment paths the
// real API uses, so the seeded data obeys the exact same business rules as anything
// created through Swagger. Call once (e.g. from Program.cs, gated to Development).
public static class DatabaseSeeder
{
    private const string SeedPassword = "Passw0rd!"; // login with any seeded username + this

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PosDBContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var codeGenerator = scope.ServiceProvider.GetRequiredService<ICodeGenerator>();

        // Don't double-seed: if there's already at least one user, assume this DB
        // already has data (seeded before, or real usage) and leave it alone.
        if (await context.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        Randomizer.Seed = new Random(1234); // fixed seed: same "random" dataset every reseed

        var users = SeedUsers(passwordHasher);
        await context.Users.AddRangeAsync(users, cancellationToken);

        var customers = await SeedCustomersAsync(context, codeGenerator, count: 15, cancellationToken);
        var products = await SeedProductsAsync(context, codeGenerator, count: 20, cancellationToken);

        // Persist users/customers/products first so orders below are built against real,
        // already-known entities rather than mixing unsaved and saved state in one batch.
        await context.SaveChangesAsync(cancellationToken);

        var orders = await SeedOrdersAsync(context, codeGenerator, customers, products, count: 12, cancellationToken);
        await context.Orders.AddRangeAsync(orders, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<User> SeedUsers(IPasswordHasher passwordHasher)
    {
        var passwordHash = passwordHasher.Hash(SeedPassword);

        return new List<User>
        {
            new("admin", passwordHash, "Alice", "Admin", "admin@pos.local", "0400000001", UserRole.Admin),
            new("manager", passwordHash, "Mia", "Manager", "manager@pos.local", "0400000002", UserRole.Manager),
            new("cashier1", passwordHash, "Cara", "Cashier", "cashier1@pos.local", "0400000003", UserRole.Cashier),
            new("cashier2", passwordHash, "Cody", "Cashier", "cashier2@pos.local", "0400000004", UserRole.Cashier),
        };
    }

    private static async Task<List<Customer>> SeedCustomersAsync(
        PosDBContext context, ICodeGenerator codeGenerator, int count, CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var customers = new List<Customer>(count);

        for (var i = 0; i < count; i++)
        {
            var code = await codeGenerator.GenerateCodeAsync("CUST", cancellationToken);

            customers.Add(new Customer(
                customerName: faker.Name.FullName(),
                customerPhone: faker.Phone.PhoneNumber("04########"),
                customerEmail: faker.Internet.Email(),
                customerCode: code,
                customerAddress: faker.Address.FullAddress()));
        }

        await context.Customers.AddRangeAsync(customers, cancellationToken);
        return customers;
    }

    private static async Task<List<Product>> SeedProductsAsync(
        PosDBContext context, ICodeGenerator codeGenerator, int count, CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var products = new List<Product>(count);

        for (var i = 0; i < count; i++)
        {
            var code = await codeGenerator.GenerateCodeAsync("PROD", cancellationToken);
            var priceText = faker.Commerce.Price(2, 200);

            products.Add(new Product(
                productCode: code,
                productName: faker.Commerce.ProductName(),
                unitPrice: decimal.Parse(priceText, CultureInfo.InvariantCulture),
                barcode: faker.Commerce.Ean13()));
        }

        await context.Products.AddRangeAsync(products, cancellationToken);
        return products;
    }

    private static async Task<List<Order>> SeedOrdersAsync(
        PosDBContext context,
        ICodeGenerator codeGenerator,
        List<Customer> customers,
        List<Product> products,
        int count,
        CancellationToken cancellationToken)
    {
        var random = new Random(1234);
        var orders = new List<Order>(count);

        for (var i = 0; i < count; i++)
        {
            var customer = customers[random.Next(customers.Count)];
            var orderNumber = await codeGenerator.GenerateCodeAsync("ORD", cancellationToken);
            var order = new Order(customer.Id, orderNumber);

            var itemCount = random.Next(1, 5);
            var chosenProducts = products.OrderBy(_ => random.Next()).Take(itemCount);

            foreach (var product in chosenProducts)
            {
                var quantity = random.Next(1, 4);
                order.AddItem(product.Id, quantity, product.UnitPrice);
            }

            // Cycle through payment scenarios so the seeded data covers every state
            // Swagger testing would otherwise need several manual requests to reach:
            //   0 = fully paid in one go      -> auto-completes (see Order.RecordPayment)
            //   1 = partially paid            -> stays Pending, RemainingBalance > 0
            //   2 = cancelled, never paid
            //   3 = left untouched, unpaid    -> stays Pending
            switch (i % 4)
            {
                case 0:
                    {
                        var refNumber = await codeGenerator.GenerateCodeAsync("PAY", cancellationToken);
                        order.RecordPayment(PaymentMethod.Cash, order.TotalAmount, refNumber);
                        break;
                    }
                case 1:
                    {
                        var partial = Math.Round(order.TotalAmount * 0.5m, 2);
                        if (partial > 0)
                        {
                            var refNumber = await codeGenerator.GenerateCodeAsync("PAY", cancellationToken);
                            order.RecordPayment(PaymentMethod.CreditCard, partial, refNumber);
                        }
                        break;
                    }
                case 2:
                    order.Cancel();
                    break;
            }

            orders.Add(order);
        }

        return orders;
    }
}
