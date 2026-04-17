FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj
COPY ["src/Aruba.Document.Api/Aruba.Document.Api.csproj", "src/Aruba.Document.Api/"]
COPY ["src/Aruba.Document.Application/Aruba.Document.Application.csproj", "src/Aruba.Document.Application/"]
COPY ["src/Aruba.Document.Domain/Aruba.Document.Domain.csproj", "src/Aruba.Document.Domain/"]
COPY ["src/Aruba.Document.Infrastructure/Aruba.Document.Infrastructure.csproj", "src/Aruba.Document.Infrastructure/"]

# Restore
RUN dotnet restore "src/Aruba.Document.Api/Aruba.Document.Api.csproj"

# Copy everything
COPY . .

# Publish
RUN dotnet publish "src/Aruba.Document.Api/Aruba.Document.Api.csproj" -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Aruba.Document.Api.dll"]
