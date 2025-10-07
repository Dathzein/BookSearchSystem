using BookSearchSystem.Application.DTOs;

namespace BookSearchSystem.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de aplicación de historial de búsquedas
/// </summary>
public interface ISearchHistoryApplicationService
{
    /// <summary>
    /// Obtiene todo el historial de búsquedas
    /// </summary>
    /// <returns>Lista de historiales de búsqueda</returns>
    Task<List<SearchHistoryDto>> GetSearchHistoryAsync();
    
    /// <summary>
    /// Obtiene un registro específico del historial por ID
    /// </summary>
    /// <param name="id">ID del registro</param>
    /// <returns>Registro del historial o null si no existe</returns>
    Task<SearchHistoryDto?> GetSearchHistoryByIdAsync(int id);
}
