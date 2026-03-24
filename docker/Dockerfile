FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Oficina.sln ./
COPY src/Oficina.Domain/Oficina.Domain.csproj src/Oficina.Domain/
COPY src/Oficina.Application/Oficina.Application.csproj src/Oficina.Application/
COPY src/Oficina.Infrastructure/Oficina.Infrastructure.csproj src/Oficina.Infrastructure/
COPY src/Oficina.Api/Oficina.Api.csproj src/Oficina.Api/
COPY tests/Oficina.Tests/Oficina.Tests.csproj tests/Oficina.Tests/

RUN dotnet restore Oficina.sln

COPY . .
RUN dotnet publish src/Oficina.Api/Oficina.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Oficina.Api.dll"]