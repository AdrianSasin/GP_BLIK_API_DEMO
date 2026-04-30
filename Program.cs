using System;
using System.Diagnostics;

// Global Payments
using GlobalPayments.Api;
using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Entities.Enums;
using GlobalPayments.Api.PaymentMethods;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Utils;

// alias żeby uniknąć konfliktu z System.Threading.Channels
using GpChannel = GlobalPayments.Api.Entities.Channel;

namespace BlikConsoleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Podaj kwotę (PLN):");

                var input = Console.ReadLine();

                if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
                {
                    Console.WriteLine("Nieprawidłowa kwota.");
                    return;
                }

                // 🔹 konfiguracja SDK
                ServicesContainer.RemoveConfig();

                var config = new GpApiConfig
                {
                    AppId = System.Environment.GetEnvironmentVariable("GP_APP_ID")?.Trim(),
                    AppKey = System.Environment.GetEnvironmentVariable("GP_APP_KEY")?.Trim(),
                    Channel = GpChannel.CardNotPresent,
                    Country = "PL",
                    ServiceUrl = "https://apis.sandbox.eservicegateway.com/ucp"
                };

                // 🔹 walidacja
                if (string.IsNullOrEmpty(config.AppId) || string.IsNullOrEmpty(config.AppKey))
                {
                    Console.WriteLine("Brak GP_APP_ID lub GP_APP_KEY w zmiennych środowiskowych");
                    return;
                }

                // 🔹 ważne: nazwana konfiguracja
                ServicesContainer.ConfigureService(config, "blikConfig");

                // 🔹 BLIK payment
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
                    .Charge(amount)
                    .WithCurrency("PLN")
                    .WithDescription("Console BLIK payment")
                    .Execute("blikConfig");

                Console.WriteLine($"ResponseCode: {response.ResponseCode}");
                Console.WriteLine($"ResponseMessage: {response.ResponseMessage}");

                if (response.AlternativePaymentResponse?.RedirectUrl != null)
                {
                    var url = response.AlternativePaymentResponse.RedirectUrl;

                    Console.WriteLine("\nOtwieram przeglądarkę...");
                    Console.WriteLine(url);

                    // 🔹 otwarcie przeglądarki
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine("Brak RedirectUrl — coś poszło nie tak.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nBLAD:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz...");
            Console.ReadKey();
        }
    }
}