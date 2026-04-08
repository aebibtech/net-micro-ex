#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Wpm.Management.Api/Wpm.Management.Api.csproj", "Wpm.Management.Api/"]
RUN dotnet restore "Wpm.Management.Api/Wpm.Management.Api.csproj"
COPY . .
WORKDIR "/src/Wpm.Management.Api"
RUN dotnet build "Wpm.Management.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wpm.Management.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wpm.Management.Api.dll"]
