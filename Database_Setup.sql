-- =============================================
-- Script de Configuración de Base de Datos
-- Sistema de Búsqueda de Libros
-- =============================================

-- Crear la base de datos
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BookSearchDB')
BEGIN
    CREATE DATABASE BookSearchDB;
    PRINT 'Base de datos BookSearchDB creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La base de datos BookSearchDB ya existe.';
END


-- Usar la base de datos
USE BookSearchDB;


-- Crear el esquema BookSearch
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'BookSearch')
BEGIN
    EXEC('CREATE SCHEMA BookSearch');
    PRINT 'Esquema BookSearch creado exitosamente.';
END
ELSE
BEGIN
    PRINT 'El esquema BookSearch ya existe.';
END


-- Crear la tabla search_history
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'BookSearch.search_history') AND type in (N'U'))
BEGIN
    CREATE TABLE BookSearch.search_history (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        AuthorSearched NVARCHAR(255) NOT NULL,
        SearchDate DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        
        -- Índices para mejorar rendimiento
        INDEX IX_search_history_AuthorSearched NONCLUSTERED (AuthorSearched),
        INDEX IX_search_history_SearchDate NONCLUSTERED (SearchDate DESC)
    );
    
    PRINT 'Tabla BookSearch.search_history creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla BookSearch.search_history ya existe.';
END


-- =============================================
-- Stored Procedure: sp_InsertSearchHistory
-- Descripción: Inserta un registro de búsqueda solo si no se ha realizado 
--              la misma búsqueda en el último minuto
-- =============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'BookSearch.sp_InsertSearchHistory') AND type in (N'P', N'PC'))
BEGIN
    DROP PROCEDURE BookSearch.sp_InsertSearchHistory;
    PRINT 'Stored Procedure anterior eliminado.';
END


CREATE PROCEDURE BookSearch.sp_InsertSearchHistory
    @AuthorSearched NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @LastSearchDate DATETIME2;
    DECLARE @CurrentDate DATETIME2 = GETDATE();
    DECLARE @OneMinuteAgo DATETIME2 = DATEADD(MINUTE, -1, @CurrentDate);
    
    BEGIN TRY
        -- Verificar si existe una búsqueda del mismo autor en el último minuto
        SELECT TOP 1 @LastSearchDate = SearchDate 
        FROM BookSearch.search_history 
        WHERE AuthorSearched = @AuthorSearched 
          AND SearchDate >= @OneMinuteAgo
        ORDER BY SearchDate DESC;
        
        -- Si no hay búsquedas recientes del mismo autor, insertar el registro
        IF @LastSearchDate IS NULL
        BEGIN
            INSERT INTO BookSearch.search_history (AuthorSearched, SearchDate)
            VALUES (@AuthorSearched, @CurrentDate);
            
            PRINT 'Búsqueda registrada exitosamente para el autor: ' + @AuthorSearched;
            SELECT 1 as Success, 'Búsqueda registrada exitosamente.' as Message;
        END
        ELSE
        BEGIN
            -- No insertar si ya existe una búsqueda reciente, pero no mostrar error al usuario
            PRINT 'Búsqueda duplicada detectada. No se insertará registro duplicado.';
            SELECT 1 as Success, 'Búsqueda procesada.' as Message;
        END
        
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Error en sp_InsertSearchHistory: ' + @ErrorMessage;
        SELECT 0 as Success, 'Error al registrar la búsqueda: ' + @ErrorMessage as Message;
        
        -- Re-lanzar el error si es crítico
        IF @ErrorSeverity > 10
        BEGIN
            RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
        END
    END CATCH
END

-- =============================================
-- Datos de prueba (opcional)
-- =============================================
PRINT '=== Insertando datos de prueba ===';

-- Ejecutar el stored procedure para insertar algunos datos de prueba
EXEC BookSearch.sp_InsertSearchHistory @AuthorSearched = 'Gabriel García Márquez';
WAITFOR DELAY '00:00:02'; -- Esperar 2 segundos
EXEC BookSearch.sp_InsertSearchHistory @AuthorSearched = 'Mario Vargas Llosa';
WAITFOR DELAY '00:00:02'; -- Esperar 2 segundos
EXEC BookSearch.sp_InsertSearchHistory @AuthorSearched = 'Isabel Allende';

-- Verificar los datos insertados
PRINT '=== Verificando datos insertados ===';
SELECT * FROM BookSearch.search_history ORDER BY SearchDate DESC;

PRINT '=== Script de configuración completado exitosamente ===';
