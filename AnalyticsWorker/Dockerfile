FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AnalyticsWorker/AnalyticsWorker.csproj", "AnalyticsWorker/"]
RUN dotnet restore "AnalyticsWorker/AnalyticsWorker.csproj"
COPY . .
WORKDIR "/src/AnalyticsWorker"
RUN dotnet build "AnalyticsWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnalyticsWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AnalyticsWorker.dll"]