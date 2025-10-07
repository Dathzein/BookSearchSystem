# 📚 Sistema de Búsqueda de Libros

## 🏗️ Arquitectura del Sistema

### **Clean Architecture (Arquitectura Limpia)**

Este proyecto implementa **Clean Architecture** con las siguientes capas:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│              (BookSearchSystem.Web)                         │
│         Controllers, Views, Models, wwwroot                 │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                   Application Layer                          │
│            (BookSearchSystem.Application)                   │
│        Services, DTOs, Mappers, Interfaces                 │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                    Domain Layer                             │
│              (BookSearchSystem.Domain)                     │
│           Entities, Value Objects, Interfaces              │
└─────────────────────────────────────────────────────────────┘
                              │
┌─────────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                        │
│           (BookSearchSystem.Infrastructure)                │
│    Repositories, External Services, Data Context           │
└─────────────────────────────────────────────────────────────┘
```

### **Beneficios de esta Arquitectura:**

✅ **Separación de Responsabilidades:** Cada capa tiene una función específica  
✅ **Testabilidad:** Fácil creación de pruebas unitarias por capa  
✅ **Mantenibilidad:** Cambios en una capa no afectan las demás  
✅ **Escalabilidad:** Permite agregar nuevas funcionalidades sin romper el código existente  
✅ **Independencia de Framework:** La lógica de negocio no depende de tecnologías específicas  
✅ **Inversión de Dependencias:** Las capas superiores no dependen de las inferiores  

### **Componentes Principales:**

- **Domain:** Entidades (`SearchHistory`), Value Objects (`Book`), Interfaces
- **Application:** Servicios de aplicación, DTOs, Mappers, Validaciones
- **Infrastructure:** Repositorios (Entity Framework), Servicios externos (OpenLibrary API)
- **Presentation:** Controllers MVC, Views Razor, Models, JavaScript/CSS

### **Patrones de Diseño Implementados:**

🔹 **Repository Pattern:** Abstrae el acceso a datos (`ISearchHistoryRepository`)  
🔹 **Dependency Injection:** Inyección de dependencias en toda la aplicación  
🔹 **DTO Pattern:** Objetos de transferencia de datos entre capas  
🔹 **Mapper Pattern:** Conversión entre entidades y DTOs  
🔹 **Service Layer Pattern:** Lógica de negocio en servicios de aplicación  
🔹 **Options Pattern:** Configuración tipada desde `appsettings.json`  
🔹 **Factory Pattern:** HttpClient factory para servicios externos  

### **Tecnologías Utilizadas:**

- **Backend:** ASP.NET Core 8.0 MVC (Razor Pages)
- **Base de Datos:** SQL Server con Entity Framework Core
- **Frontend:** Bootstrap 5, jQuery, Font Awesome
- **API Externa:** OpenLibrary API para búsqueda de libros
- **Containerización:** Docker y Docker Compose
- **Arquitectura:** Clean Architecture + DDD principles

### **Flujo de Datos:**

```
Usuario → Controller → Application Service → Domain Service → Repository → Database
                                        ↓
                   External API ← Infrastructure Service
```

1. **Usuario** interactúa con la **Vista** (Razor)
2. **Controller** recibe la petición y valida datos
3. **Application Service** coordina la lógica de negocio
4. **Repository** maneja persistencia de datos
5. **External Service** consume APIs externas
6. **Mappers** convierten entre DTOs y Entidades

### **Funcionalidades del Sistema:**

📚 **Búsqueda de Libros:**
- Búsqueda por nombre de autor
- Integración con OpenLibrary API
- Manejo de errores y datos faltantes
- Validación de formularios (cliente y servidor)

📋 **Historial de Búsquedas:**
- Registro automático de búsquedas
- Prevención de duplicados (< 1 minuto)
- Visualización de historial completo
- Botón "Buscar Nuevamente" en nueva ventana

🎨 **Interfaz de Usuario:**
- Diseño responsivo con Bootstrap 5
- Tema morado personalizado
- Validaciones en tiempo real
- Experiencia de usuario optimizada

🔧 **Características Técnicas:**
- Stored Procedures para operaciones de base de datos
- Logging detallado en todas las capas
- Manejo de excepciones centralizado
- Configuración flexible via `appsettings.json`

---

## 🐳 Opción 1: Ejecutar con Docker

### **1. Crear red y volumen de Docker:**
```bash
# Crear red para comunicación entre contenedores
docker network create booksearch-network

# Crear volumen para persistir datos de SQL Server
docker volume create booksearch-sqldata
```

### **2. Ejecutar SQL Server:**
```bash
docker run -d --name booksearch-sqlserver --network booksearch-network -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=BookSearch123!" -p 1433:1433 -v booksearch-sqldata:/var/opt/mssql mcr.microsoft.com/mssql/server:2022-latest
```

### **3. Configurar base de datos:**
- Conectarse a SQL Server usando **DBeaver** o cualquier cliente SQL
- **Host:** `localhost`
- **Puerto:** `1433`
- **Usuario:** `SA`
- **Contraseña:** `BookSearch123!`
- Ejecutar el script `Database_Setup.sql` desde el cliente

### **4. Compilar imagen de la aplicación:**
```bash
docker build -t booksearch-web .
```

### **5. Ejecutar aplicación:**
```bash
docker run -d --name booksearch-web --network booksearch-network -p 8080:8080 -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=http://+:8080" -e "ConnectionStrings__DefaultConnection=Server=booksearch-sqlserver,1433;Database=BookSearchDB;User Id=SA;Password=BookSearch123!;TrustServerCertificate=true;MultipleActiveResultSets=true" -e "OpenLibraryApi__BaseUrl=https://openlibrary.org/search.json?fields=author_name,title,first_publish_year,publisher&author=" booksearch-web
```

### **6. Acceder a la aplicación:**
- **URL:** http://localhost:8080

---

## 💻 Opción 2: Ejecutar sin Docker (Desarrollo Local)

### **Requisitos:**
- **.NET 8.0 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (Local, Express, o instancia remota)
- **Visual Studio 2022** o **VS Code** (opcional)

### **1. Configurar SQL Server:**
- Instalar SQL Server localmente o usar una instancia existente
- Crear la base de datos ejecutando `Database_Setup.sql` desde:
  - **SQL Server Management Studio (SSMS)**
  - **Azure Data Studio**
  - **DBeaver**
  - **Visual Studio Code** con extensión SQL Server

### **2. Configurar cadena de conexión:**
Editar `BookSearchSystem.Web/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BookSearchDB;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "OpenLibraryApi": {
    "BaseUrl": "https://openlibrary.org/search.json?fields=author_name,title,first_publish_year,publisher&author="
  }
}
```

### **3. Restaurar paquetes NuGet:**
```bash
cd BookSearchSystem.Web
dotnet restore
```

### **4. Compilar la solución:**
```bash
dotnet build
```

### **5. Ejecutar la aplicación:**
```bash
dotnet run
```

### **6. Acceder a la aplicación:**
- **URL:** https://localhost:7001 o http://localhost:5000
- La URL exacta se mostrará en la consola al ejecutar

### **Comandos adicionales para desarrollo:**
```bash
# Ejecutar con hot reload (recarga automática)
dotnet watch run

# Ejecutar en modo Release
dotnet run --configuration Release

# Publicar para producción
dotnet publish -c Release -o ./publish
```

---

## 🎯 Justificación de la Arquitectura

### **¿Por qué Clean Architecture?**

**1. Mantenibilidad a Largo Plazo:**
- Código organizado y fácil de entender
- Cambios en UI no afectan la lógica de negocio
- Fácil agregar nuevas funcionalidades

**2. Testabilidad:**
- Cada capa puede probarse independientemente
- Mocking sencillo de dependencias
- Cobertura de pruebas más efectiva

**3. Flexibilidad Tecnológica:**
- Cambiar de Entity Framework a Dapper sin afectar el dominio
- Migrar de MVC a Web API sin tocar la lógica
- Intercambiar proveedores de base de datos fácilmente

**4. Escalabilidad:**
- Equipos pueden trabajar en paralelo en diferentes capas
- Microservicios futuros pueden reutilizar capas Domain/Application
- Fácil implementación de nuevos patrones

### **Alternativas Consideradas:**

❌ **Arquitectura en Capas Tradicional:**
- Acoplamiento fuerte entre capas
- Difícil testing y mantenimiento
- Dependencias hacia abajo problemáticas

❌ **Arquitectura Monolítica Simple:**
- Todo en un solo proyecto
- Código espagueti a medida que crece
- Difícil escalabilidad del equipo

✅ **Clean Architecture (Elegida):**
- Inversión de dependencias
- Separación clara de responsabilidades
- Preparada para crecimiento futuro

### **Decisiones de Diseño Específicas:**

🔸 **Entity Framework + Stored Procedures:** Combina la facilidad de EF con el control de SPs  
🔸 **DTOs en Application Layer:** Evita exponer entidades de dominio  
🔸 **Repository Pattern:** Abstrae el acceso a datos para mejor testing  
🔸 **Options Pattern:** Configuración tipada y validada  
🔸 **MVC con Razor:** Balance entre productividad y control  

## 🛑 Detener y limpiar:
```bash
# Detener contenedores
docker stop booksearch-web booksearch-sqlserver

# Eliminar contenedores
docker rm booksearch-web booksearch-sqlserver

# Eliminar red
docker network rm booksearch-network

# Eliminar imagen (opcional)
docker rmi booksearch-web

# IMPORTANTE: Eliminar volumen solo si quieres perder los datos
# docker volume rm booksearch-sqldata
```

## 💾 Gestión de datos:
```bash
# Ver volúmenes creados
docker volume ls

# Inspeccionar el volumen de datos
docker volume inspect booksearch-sqldata

# Hacer backup del volumen (opcional)
docker run --rm -v booksearch-sqldata:/data -v $(pwd):/backup alpine tar czf /backup/sqlserver-backup.tar.gz -C /data .

# Restaurar backup (opcional)
docker run --rm -v booksearch-sqldata:/data -v $(pwd):/backup alpine tar xzf /backup/sqlserver-backup.tar.gz -C /data
```

## 📋 Requisitos:
- Docker instalado
- Puertos 8080 y 1433 disponibles

## ✅ Beneficios del volumen:
- **Persistencia:** Los datos de la base de datos se mantienen aunque elimines el contenedor
- **Backup:** Puedes hacer copias de seguridad del volumen
- **Reutilización:** Al recrear el contenedor, los datos siguen ahí
- **Performance:** Mejor rendimiento que bind mounts en algunos sistemas
