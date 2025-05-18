# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["RegistroTecnicos.csproj", "./"]
COPY . .
RUN dotnet restore "RegistroTecnicos.csproj"
RUN dotnet build "RegistroTecnicos.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegistroTecnicos.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegistroTecnicos.dll"]