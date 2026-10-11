# ---- build stage: full SDK, compiles the code ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj files first, so the restore step is cached
# and only reruns when dependencies change, not on every code edit.
COPY src/Inventory.Domain/Inventory.Domain.csproj          Inventory.Domain/
COPY src/Inventory.Application/Inventory.Application.csproj Inventory.Application/
COPY src/Inventory.Infrastructure/Inventory.Infrastructure.csproj Inventory.Infrastructure/
COPY src/Inventory.Api/Inventory.Api.csproj                Inventory.Api/
RUN dotnet restore Inventory.Api/Inventory.Api.csproj

# Now the rest of the source, then publish
COPY src/ .
RUN dotnet publish Inventory.Api/Inventory.Api.csproj -c Release -o /app/publish --no-restore

# ---- runtime stage: small image, only what is needed to run ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Never run as root inside the container
USER $APP_UID

# .NET 8 images listen on 8080 by default
EXPOSE 8080 8081
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]