using BookSearchSystem.Domain.Entities;
using BookSearchSystem.Domain.Interfaces;
using BookSearchSystem.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookSearchSystem.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de historial de búsquedas
/// </summary>
public class SearchHistoryRepository : ISearchHistoryRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchHistoryRepository> _logger;
    
    public SearchHistoryRepository(ApplicationDbContext context, ILogger<SearchHistoryRepository> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Inserta un nuevo registro de búsqueda usando stored procedure
    /// </summary>
    public async Task<bool> InsertSearchHistoryAsync(string authorSearched)
    {
        try
        {
            _logger.LogInformation("Iniciando inserción de historial para autor: {Author}", authorSearched);

            // Ejecutar stored procedure directamente sin usar FromSql
            var authorParam = new SqlParameter("@AuthorSearched", authorSearched);
            
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC BookSearch.sp_InsertSearchHistory @AuthorSearched", 
                authorParam);

            _logger.LogInformation("Stored procedure ejecutado para autor: {Author}. Filas afectadas: {Result}", authorSearched, result);
            
            return result >= 0; // El stored procedure siempre devuelve éxito
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar historial de búsqueda para autor: {Author}", authorSearched);
            return false;
        }
    }
    
    /// <summary>
    /// Obtiene todo el historial de búsquedas usando LINQ
    /// </summary>
    public async Task<IEnumerable<SearchHistory>> GetSearchHistoryAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo todo el historial de búsquedas");
            
            var histories = await _context.SearchHistories
                .OrderByDescending(h => h.SearchDate)
                .ToListAsync();
            
            _logger.LogInformation("Se obtuvieron {Count} registros del historial", histories.Count);
            
            return histories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de búsquedas");
            return Enumerable.Empty<SearchHistory>();
        }
    }
    
    /// <summary>
    /// Obtiene el total de registros en el historial
    /// </summary>
    public async Task<int> GetTotalRecordsAsync()
    {
        try
        {
            var count = await _context.SearchHistories.CountAsync();
            _logger.LogInformation("Total de registros en historial: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener total de registros del historial");
            return 0;
        }
    }
    
    /// <summary>
    /// Obtiene un registro específico por ID
    /// </summary>
    public async Task<SearchHistory?> GetByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Buscando registro de historial con ID: {Id}", id);
            
            var history = await _context.SearchHistories
                .FirstOrDefaultAsync(h => h.Id == id);
            
            if (history != null)
            {
                _logger.LogInformation("Registro encontrado: {Author} - {Date}", history.AuthorSearched, history.SearchDate);
            }
            else
            {
                _logger.LogWarning("No se encontró registro con ID: {Id}", id);
            }
            
            return history;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar registro por ID: {Id}", id);
            return null;
        }
    }
}
