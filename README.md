# GP BLIK Console Demo

Prosta aplikacja konsolowa w .NET, która integruje się z eService GP API i umożliwia utworzenie płatności BLIK.

Aplikacja:
- wyświetla listę produktów
- pozwala dodawać produkty do koszyka wielokrotnie
- oblicza łączną kwotę
- inicjuje płatność BLIK
- otwiera stronę płatności w przeglądarce

Wymagania

- .NET 8 SDK
- Visual Studio 2022 lub nowszy (lub VS Code)
- konto testowe eService GPAPI
- dane dostępowe:
  - AppId
  - AppKey

Instalacja

git clone https://github.com/AdrianSasin/GP_BLIK_API_DEMO.git
cd GP_BLIK_API_DEMO

dotnet restore

Projekt używa paczki:
GlobalPayments.Api

Konfiguracja

Aplikacja nie przechowuje danych dostępowych w kodzie.

Przed uruchomieniem ustaw zmienne środowiskowe.

Windows PowerShell:
setx GP_APP_ID "twoj_app_id"
setx GP_APP_KEY "twoj_app_key"

Windows CMD:
setx GP_APP_ID "twoj_app_id"
setx GP_APP_KEY "twoj_app_key"

Po ustawieniu zmiennych zamknij i uruchom ponownie terminal lub Visual Studio.

Uruchomienie

dotnet run

lub w Visual Studio uruchom projekt (F5).

Jak działa aplikacja

Po uruchomieniu wyświetlana jest lista produktów:

1. Kawa
2. Herbata
3. Kanapka
4. Ciasto
5. Sok

Użytkownik:
- wpisuje numer produktu
- podaje ilość
- może powtarzać ten proces wiele razy

Aby zakończyć dodawanie produktów, wpisz:

0

Po zakończeniu:
- wyświetlane jest podsumowanie koszyka
- obliczana jest łączna kwota
- tworzona jest płatność BLIK
- otwierana jest przeglądarka z płatnością

Endpoint

Aplikacja korzysta ze środowiska testowego:
https://apis.sandbox.eservicegateway.com/ucp

Bezpieczeństwo

Nie commituj:
- AppId
- AppKey

Dodaj do .gitignore:
bin/
obj/
*.user
*.suo
.env
appsettings.Development.json

Jeśli klucz został opublikowany w repozytorium, należy wygenerować nowy.

Typowe problemy

Brak GP_APP_ID lub GP_APP_KEY
- zmienne środowiskowe nie są ustawione lub terminal nie został zrestartowany

403 Forbidden
- błędne dane dostępowe
- dane z innego środowiska
- brak dostępu do BLIK
- nieprawidłowy endpoint

Brak RedirectUrl
- BLIK nie jest aktywny na koncie merchanta
- środowisko testowe nie obsługuje tej metody

Cel projektu

Projekt demonstracyjny pokazujący integrację płatności BLIK w aplikacji konsolowej .NET z prostym koszykiem produktów.
