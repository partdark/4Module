FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["OrderWorkerService/OrderWorkerService.csproj", "OrderWorkerService/"]
COPY ["OrderContracts/OrderContracts.csproj", "OrderContracts/"]

RUN dotnet restore "OrderWorkerService/OrderWorkerService.csproj"

COPY . .

WORKDIR /src/OrderWorkerService

RUN dotnet build "OrderWorkerService.csproj" -c Release -o /app/build
RUN dotnet publish "OrderWorkerService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderWorkerService.dll"]
