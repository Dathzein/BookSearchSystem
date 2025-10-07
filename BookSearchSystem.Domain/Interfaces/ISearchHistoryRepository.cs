using BookSearchSystem.Domain.Entities;

namespace BookSearchSystem.Domain.Interfaces;

/// <summary>
/// Interfaz para el repositorio de historial de búsquedas
/// </summary>
public interface ISearchHistoryRepository
{
    /// <summary>
    /// Inserta un nuevo registro de búsqueda si no existe uno reciente (< 1 minuto)
    /// </summary>
    /// <param name="authorSearched">Autor buscado</param>
    /// <returns>True si se insertó el registro, False si ya existía uno reciente</returns>
    Task<bool> InsertSearchHistoryAsync(string authorSearched);
    
    /// <summary>
    /// Obtiene todo el historial de búsquedas ordenado por fecha descendente
    /// </summary>
    /// <returns>Lista completa de historiales de búsqueda</returns>
    Task<IEnumerable<SearchHistory>> GetSearchHistoryAsync();
    
    /// <summary>
    /// Obtiene el total de registros en el historial
    /// </summary>
    /// <returns>Número total de registros</returns>
    Task<int> GetTotalRecordsAsync();
    
    /// <summary>
    /// Obtiene un registro específico por ID
    /// </summary>
    /// <param name="id">ID del registro</param>
    /// <returns>Registro de historial o null si no existe</returns>
    Task<SearchHistory?> GetByIdAsync(int id);
}
