using BookSearchSystem.Application.DTOs;

namespace BookSearchSystem.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de aplicación de búsqueda de libros
/// </summary>
public interface IBookSearchApplicationService
{
    /// <summary>
    /// Busca libros por autor y registra la búsqueda en el historial
    /// </summary>
    /// <param name="request">Solicitud de búsqueda</param>
    /// <returns>Respuesta con los libros encontrados</returns>
    Task<BookSearchResponseDto> SearchBooksByAuthorAsync(BookSearchRequestDto request);
}
