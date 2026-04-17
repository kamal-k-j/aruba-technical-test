FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj
COPY ["src/Aruba.Identity.Api/Aruba.Identity.Api.csproj", "src/Aruba.Identity.Api/"]
COPY ["src/Aruba.Identity.Application/Aruba.Identity.Application.csproj", "src/Aruba.Identity.Application/"]
COPY ["src/Aruba.Identity.Domain/Aruba.Identity.Domain.csproj", "src/Aruba.Identity.Domain/"]
COPY ["src/Aruba.Identity.Infrastructure/Aruba.Identity.Infrastructure.csproj", "src/Aruba.Identity.Infrastructure/"]

# Restore
RUN dotnet restore "src/Aruba.Identity.Api/Aruba.Identity.Api.csproj"

# Copy everything
COPY . .

# Publish
RUN dotnet publish "src/Aruba.Identity.Api/Aruba.Identity.Api.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Aruba.Identity.Api.dll"]
