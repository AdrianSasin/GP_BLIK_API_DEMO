using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using GpChannel = GlobalPayments.Api.Entities.Channel;

namespace BlikConsoleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var products = new[]
                {
                    new Product(1, "Kawa", 8.99m),
                    new Product(2, "Herbata", 6.50m),
                    new Product(3, "Kanapka", 14.99m),
                    new Product(4, "Ciasto", 12.00m),
                    new Product(5, "Sok", 9.50m)
                };

                var cart = new List<CartItem>();

                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine("Dostępne produkty:");

                    foreach (var product in products)
                    {
                        Console.WriteLine($"{product.Id}. {product.Name} - {product.Price:F2} PLN");
                    }

                    Console.WriteLine();
                    Console.WriteLine("Wpisz numer produktu (0 = zakończ i zapłać):");

                    if (!int.TryParse(Console.ReadLine(), out int productId))
                    {
                        Console.WriteLine("Nieprawidłowy wybór.");
                        continue;
                    }

                    if (productId == 0)
                    {
                        break;
                    }

                    Product? selectedProduct = null;

                    foreach (var product in products)
                    {
                        if (product.Id == productId)
                        {
                            selectedProduct = product;
                            break;
                        }
                    }

                    if (selectedProduct == null)
                    {
                        Console.WriteLine("Nie znaleziono produktu.");
                        continue;
                    }

                    Console.Write("Ilość: ");

                    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Nieprawidłowa ilość.");
                        continue;
                    }

                    cart.Add(new CartItem(selectedProduct, quantity));

                    Console.WriteLine($"Dodano: {selectedProduct.Name} x {quantity}");
                }

                if (cart.Count == 0)
                {
                    Console.WriteLine("Koszyk jest pusty.");
                    return;
                }

                decimal total = 0;

                Console.WriteLine();
                Console.WriteLine("Podsumowanie:");

                foreach (var item in cart)
                {
                    decimal itemTotal = item.Product.Price * item.Quantity;
                    total += itemTotal;

                    Console.WriteLine($"{item.Product.Name} x {item.Quantity} = {itemTotal:F2} PLN");
                }

                Console.WriteLine($"Do zapłaty: {total:F2} PLN");
                Console.WriteLine();

                ServicesContainer.RemoveConfig();

                var config = new GpApiConfig
                {
                    AppId = System.Environment.GetEnvironmentVariable("GP_APP_ID")?.Trim(),
                    AppKey = System.Environment.GetEnvironmentVariable("GP_APP_KEY")?.Trim(),
                    Channel = GpChannel.CardNotPresent,
                    Country = "PL",
                    ServiceUrl = "https://apis.sandbox.eservicegateway.com/ucp"
                };

                if (string.IsNullOrEmpty(config.AppId) || string.IsNullOrEmpty(config.AppKey))
                {
                    Console.WriteLine("Brak GP_APP_ID lub GP_APP_KEY w zmiennych środowiskowych.");
                    return;
                }

                ServicesContainer.ConfigureService(config, "blikConfig");

                var paymentMethodDetails = new AlternativePaymentMethod
                {
                    AlternativePaymentMethodType = AlternativePaymentType.BLIK,
                    ReturnUrl = "https://example.com/return",
                    StatusUpdateUrl = "https://example.com/status",
                    Descriptor = "BLIK TEST",
                    Country = "PL",
                    AccountHolderName = "Jan Kowalski"
                };

                Console.WriteLine("Tworzenie płatności BLIK...");

                var response = paymentMethodDetails
                    .Charge(total)
                    .WithCurrency("PLN")
                    .WithDescription("Koszyk produktów")
                    .Execute("blikConfig");

                Console.WriteLine($"ResponseCode: {response.ResponseCode}");
                Console.WriteLine($"ResponseMessage: {response.ResponseMessage}");

                if (response.AlternativePaymentResponse?.RedirectUrl != null)
                {
                    var url = response.AlternativePaymentResponse.RedirectUrl;

                    Console.WriteLine();
                    Console.WriteLine("RedirectUrl:");
                    Console.WriteLine(url);

                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Brak RedirectUrl w odpowiedzi.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("BLAD:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine();
            Console.WriteLine("Naciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }

    internal class Product
    {
        public int Id { get; }
        public string Name { get; }
        public decimal Price { get; }

        public Product(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }

    internal class CartItem
    {
        public Product Product { get; }
        public int Quantity { get; }

        public CartItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
        }
    }
}