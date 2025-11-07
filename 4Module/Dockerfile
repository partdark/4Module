FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY ["4Module/4Module.csproj", "4Module/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["AnalyticsWorker/AnalyticsWorker.csproj", "AnalyticsWorker/"]
COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["NotificationService/NotificationService.csproj", "NotificationService/"]
COPY ["OrderContracts/OrderContracts.csproj", "OrderContracts/"]
COPY ["OrderWorkerService/OrderWorkerService.csproj", "OrderWorkerService/"]

RUN dotnet restore "4Module/4Module.csproj"


COPY . .

WORKDIR /src/4Module
RUN dotnet publish "4Module.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "4Module.dll"]
