using BookSearchSystem.Domain.Interfaces;
using BookSearchSystem.Domain.ValueObjects;
using BookSearchSystem.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BookSearchSystem.Infrastructure.ExternalServices;

/// <summary>
/// Servicio para consultar la API de OpenLibrary
/// </summary>
public class OpenLibraryService : IBookSearchService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenLibraryService> _logger;
    private readonly OpenLibraryApiSettings _apiSettings;
    
    public OpenLibraryService(
        HttpClient httpClient, 
        ILogger<OpenLibraryService> logger,
        IOptions<OpenLibraryApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiSettings = apiSettings.Value;
        
        // Configurar timeout
        _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutSeconds);
    }
    
    /// <summary>
    /// Busca libros por autor en la API de OpenLibrary
    /// </summary>
    public async Task<IEnumerable<Book>> SearchBooksByAuthorAsync(string author)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                _logger.LogWarning("Se intentó buscar con un autor vacío o nulo");
                return Enumerable.Empty<Book>();
            }
            
            var encodedAuthor = Uri.EscapeDataString(author.Trim());
            var url = $"{_apiSettings.BaseUrl}{encodedAuthor}";
            
            _logger.LogInformation("Buscando libros para autor: {Author} - URL: {Url}", author, url);
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var jsonContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Respuesta de API recibida. Tamaño: {Size} caracteres", jsonContent.Length);
            
            var searchResult = JsonSerializer.Deserialize<OpenLibraryResponse>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNameCaseInsensitive = true
            });
            
            if (searchResult?.Docs == null || !searchResult.Docs.Any())
            {
                _logger.LogInformation("No se encontraron libros para el autor: {Author}", author);
                return Enumerable.Empty<Book>();
            }
            
            var books = searchResult.Docs
                .Take(_apiSettings.MaxResults)
                .Select(MapToBook)
                .Where(book => book.IsValid())
                .ToList();
            
            _logger.LogInformation("Se encontraron {Count} libros válidos para el autor: {Author}", books.Count, author);
            
            return books;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de red al buscar libros para autor: {Author}", author);
            throw new InvalidOperationException($"Error al conectar con la API de OpenLibrary: {ex.Message}", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout al buscar libros para autor: {Author}", author);
            throw new InvalidOperationException("La búsqueda tardó demasiado tiempo. Intente nuevamente.", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error al deserializar respuesta JSON para autor: {Author}", author);
            throw new InvalidOperationException("Error al procesar la respuesta de la API.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al buscar libros para autor: {Author}", author);
            throw new InvalidOperationException("Error inesperado durante la búsqueda.", ex);
        }
    }
    
    /// <summary>
    /// Mapea un documento de OpenLibrary a un objeto Book
    /// </summary>
    private static Book MapToBook(OpenLibraryDoc doc)
    {
        return new Book(
            title: doc.Title ?? string.Empty,
            authors: doc.AuthorName ?? new List<string>(),
            firstPublishYear: doc.FirstPublishYear,
            publishers: doc.Publisher ?? new List<string>()
        );
    }
}

/// <summary>
/// Respuesta de la API de OpenLibrary
/// </summary>
public class OpenLibraryResponse
{
    public int NumFound { get; set; }
    public int Start { get; set; }
    public bool NumFoundExact { get; set; }
    public List<OpenLibraryDoc>? Docs { get; set; }
}

/// <summary>
/// Documento individual de la respuesta de OpenLibrary
/// </summary>
public class OpenLibraryDoc
{
    public List<string>? AuthorName { get; set; }
    public string? Title { get; set; }
    public int? FirstPublishYear { get; set; }
    public List<string>? Publisher { get; set; }
}
