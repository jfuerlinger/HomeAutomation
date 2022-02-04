#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HomeAutomation.BlazorApp/Server/HomeAutomation.BlazorApp.Server.csproj", "HomeAutomation.BlazorApp/Server/"]
COPY ["HomeAutomation.BlazorApp/Client/HomeAutomation.BlazorApp.Client.csproj", "HomeAutomation.BlazorApp/Client/"]
COPY ["HomeAutomation.BlazorApp/Shared/HomeAutomation.BlazorApp.Shared.csproj", "HomeAutomation.BlazorApp/Shared/"]
COPY ["HomeAutomation.Services/HomeAutomation.Services.csproj", "HomeAutomation.Services/"]
COPY ["HomeAutomation.Core/HomeAutomation.Core.csproj", "HomeAutomation.Core/"]
COPY ["HomeAutomation.Services.Mqtt/HomeAutomation.Services.Mqtt.csproj", "HomeAutomation.Services.Mqtt/"]
COPY ["HomeAutomation.Persistence.InMemory/HomeAutomation.Persistence.InMemory.csproj", "HomeAutomation.Persistence.InMemory/"]
RUN dotnet restore "HomeAutomation.BlazorApp/Server/HomeAutomation.BlazorApp.Server.csproj"
COPY . .
WORKDIR "/src/HomeAutomation.BlazorApp/Server"
RUN dotnet build "HomeAutomation.BlazorApp.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HomeAutomation.BlazorApp.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HomeAutomation.BlazorApp.Server.dll"]