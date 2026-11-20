# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["MediCitas.csproj", "./"]
RUN dotnet restore "MediCitas.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Etapa final / ejecución
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MediCitas.dll"]