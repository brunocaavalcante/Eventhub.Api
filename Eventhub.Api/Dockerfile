FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5189

ENV ASPNETCORE_URLS=http://+:5189

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Eventhub.Api/Eventhub.Api.csproj", "Eventhub.Api/"]
RUN dotnet restore "Eventhub.Api/Eventhub.Api.csproj"
COPY . .
WORKDIR "/src/Eventhub.Api"
RUN dotnet build "Eventhub.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "Eventhub.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eventhub.Api.dll"]
