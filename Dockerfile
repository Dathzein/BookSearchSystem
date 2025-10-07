# ===== DOCKERFILE PARA SISTEMA DE BÚSQUEDA DE LIBROS =====
# Imagen base para el runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagen para el build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["BookSearchSystem.Web/BookSearchSystem.Web.csproj", "BookSearchSystem.Web/"]
COPY ["BookSearchSystem.Application/BookSearchSystem.Application.csproj", "BookSearchSystem.Application/"]
COPY ["BookSearchSystem.Infrastructure/BookSearchSystem.Infrastructure.csproj", "BookSearchSystem.Infrastructure/"]
COPY ["BookSearchSystem.Domain/BookSearchSystem.Domain.csproj", "BookSearchSystem.Domain/"]

RUN dotnet restore "BookSearchSystem.Web/BookSearchSystem.Web.csproj"

# Copiar todo el código fuente
COPY . .

# Build del proyecto
WORKDIR "/src/BookSearchSystem.Web"
RUN dotnet build "BookSearchSystem.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "BookSearchSystem.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app

# Copiar la aplicación publicada
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Punto de entrada
ENTRYPOINT ["dotnet", "BookSearchSystem.Web.dll"]
