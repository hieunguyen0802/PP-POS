FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# .csproj files first, so the restore layer is cached
COPY src/POS.Domain/POS.Domain.csproj                     POS.Domain/
COPY src/POS.Application/POS.Application.csproj           POS.Application/
COPY src/POS.Infrastructure/POS.Infrastructure.csproj     POS.Infrastructure/
COPY src/POS.Api/POS.Api.csproj                           POS.Api/
RUN dotnet restore POS.Api/POS.Api.csproj

COPY src/ .
RUN dotnet publish POS.Api/POS.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "POS.Api.dll"]