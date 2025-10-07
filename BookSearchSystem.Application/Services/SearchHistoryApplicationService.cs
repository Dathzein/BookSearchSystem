using BookSearchSystem.Application.DTOs;
using BookSearchSystem.Application.Interfaces;
using BookSearchSystem.Application.Mappers;
using BookSearchSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace BookSearchSystem.Application.Services;

/// <summary>
/// Servicio de aplicación para historial de búsquedas
/// </summary>
public class SearchHistoryApplicationService : ISearchHistoryApplicationService
{
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    private readonly ILogger<SearchHistoryApplicationService> _logger;
    
    public SearchHistoryApplicationService(
        ISearchHistoryRepository searchHistoryRepository,
        ILogger<SearchHistoryApplicationService> logger)
    {
        _searchHistoryRepository = searchHistoryRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Obtiene todo el historial de búsquedas
    /// </summary>
    public async Task<List<SearchHistoryDto>> GetSearchHistoryAsync()
    {
        try
        {
            _logger.LogInformation("Obteniendo historial completo de búsquedas");
            
            var searchHistories = await _searchHistoryRepository.GetSearchHistoryAsync();
            var searchHistoryDtos = SearchHistoryMapper.ToDto(searchHistories);
            
            _logger.LogInformation("Se obtuvieron {Count} registros del historial", searchHistoryDtos.Count);
            
            return searchHistoryDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de búsquedas");
            return new List<SearchHistoryDto>();
        }
    }
    
    /// <summary>
    /// Obtiene un registro específico del historial por ID
    /// </summary>
    public async Task<SearchHistoryDto?> GetSearchHistoryByIdAsync(int id)
    {
        try
        {
            _logger.LogInformation("Buscando registro de historial con ID: {Id}", id);
            
            if (id <= 0)
            {
                _logger.LogWarning("ID inválido proporcionado: {Id}", id);
                return null;
            }
            
            var searchHistory = await _searchHistoryRepository.GetByIdAsync(id);
            
            if (searchHistory == null)
            {
                _logger.LogInformation("No se encontró registro con ID: {Id}", id);
                return null;
            }
            
            var searchHistoryDto = SearchHistoryMapper.ToDto(searchHistory);
            
            _logger.LogInformation("Registro encontrado: {Author} - {Date}", 
                searchHistoryDto.AuthorSearched, searchHistoryDto.FormattedSearchDate);
            
            return searchHistoryDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar registro por ID: {Id}", id);
            return null;
        }
    }
}
