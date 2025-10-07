namespace BookSearchSystem.Infrastructure.Configuration;

/// <summary>
/// Configuración para la API de OpenLibrary
/// </summary>
public class OpenLibraryApiSettings
{
    public const string SectionName = "OpenLibraryApi";
    
    /// <summary>
    /// URL base de la API con parámetros de consulta
    /// Ejemplo: "https://openlibrary.org/search.json?fields=author_name,title,first_publish_year,publisher&author="
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;
    
    /// <summary>
    /// Timeout en segundos para las peticiones HTTP
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Número máximo de resultados a procesar
    /// </summary>
    public int MaxResults { get; set; } = 100;
}
