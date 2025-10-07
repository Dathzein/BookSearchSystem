# 📚 Sistema de Búsqueda de Libros

## 🐳 Ejecutar con Docker

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
```bash
# Esperar a que SQL Server esté completamente listo
echo "Esperando a que SQL Server inicie..."
sleep 15

# Verificar que SQL Server esté respondiendo
docker exec booksearch-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "BookSearch123!" -C -Q "SELECT 1" || echo "SQL Server aún no está listo, esperando más..."
sleep 5

# Copiar script de base de datos
docker cp Database_Setup.sql booksearch-sqlserver:/tmp/Database_Setup.sql

# Ejecutar script de inicialización
echo "Ejecutando script de base de datos..."
docker exec -it booksearch-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "BookSearch123!" -C -i /tmp/Database_Setup.sql

# Verificar que la base de datos se creó correctamente
echo "Verificando base de datos..."
docker exec booksearch-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "BookSearch123!" -C -Q "SELECT name FROM sys.databases WHERE name = 'BookSearchDB'"
```

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
