FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY *.sln .
COPY TariffApp/*.csproj ./TariffApp/
RUN dotnet restore

COPY . .
WORKDIR /app/TariffApp
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "TariffApp.dll"]
