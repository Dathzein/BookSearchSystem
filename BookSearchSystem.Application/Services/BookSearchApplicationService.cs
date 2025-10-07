using BookSearchSystem.Application.DTOs;
using BookSearchSystem.Application.Interfaces;
using BookSearchSystem.Application.Mappers;
using BookSearchSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace BookSearchSystem.Application.Services;

/// <summary>
/// Servicio de aplicación para búsqueda de libros
/// </summary>
public class BookSearchApplicationService : IBookSearchApplicationService
{
    private readonly IBookSearchService _bookSearchService;
    private readonly ISearchHistoryRepository _searchHistoryRepository;
    private readonly ILogger<BookSearchApplicationService> _logger;
    
    public BookSearchApplicationService(
        IBookSearchService bookSearchService,
        ISearchHistoryRepository searchHistoryRepository,
        ILogger<BookSearchApplicationService> logger)
    {
        _bookSearchService = bookSearchService;
        _searchHistoryRepository = searchHistoryRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Busca libros por autor y registra la búsqueda en el historial
    /// </summary>
    public async Task<BookSearchResponseDto> SearchBooksByAuthorAsync(BookSearchRequestDto request)
    {
        try
        {
            // Validar la solicitud
            var validationResults = ValidateRequest(request);
            if (validationResults.Any())
            {
                var errorMessage = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
                _logger.LogWarning("Solicitud de búsqueda inválida: {Errors}", errorMessage);
                return new BookSearchResponseDto(errorMessage);
            }
            
            var author = request.Author.Trim();
            _logger.LogInformation("Iniciando búsqueda de libros para autor: {Author}", author);
            
            // Registrar la búsqueda en el historial
            try
            {
                var historyInserted = await _searchHistoryRepository.InsertSearchHistoryAsync(author);
                _logger.LogInformation("Historial de búsqueda registrado para autor: {Author}. Resultado: {Result}", author, historyInserted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar historial para autor: {Author}", author);
            }
            
            // Buscar libros
            var books = await _bookSearchService.SearchBooksByAuthorAsync(author);
            var bookDtos = BookMapper.ToDto(books);
            
            _logger.LogInformation("Búsqueda completada para autor: {Author}. Encontrados: {Count} libros", 
                author, bookDtos.Count);
            
            return new BookSearchResponseDto(bookDtos, author);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Error de operación al buscar libros para autor: {Author}", request?.Author);
            return new BookSearchResponseDto(ex.Message, request?.Author ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar libros para autor: {Author}", request?.Author);
            return new BookSearchResponseDto("Ocurrió un error inesperado durante la búsqueda. Intente nuevamente.", 
                request?.Author ?? "");
        }
    }
    
    /// <summary>
    /// Valida la solicitud de búsqueda
    /// </summary>
    private static List<ValidationResult> ValidateRequest(BookSearchRequestDto request)
    {
        var validationResults = new List<ValidationResult>();
        
        if (request == null)
        {
            validationResults.Add(new ValidationResult("La solicitud no puede ser nula"));
            return validationResults;
        }
        
        var validationContext = new ValidationContext(request);
        Validator.TryValidateObject(request, validationContext, validationResults, true);
        
        return validationResults;
    }
}
