using BookSearchSystem.Domain.ValueObjects;

namespace BookSearchSystem.Domain.Interfaces;

/// <summary>
/// Interfaz para el servicio de búsqueda de libros
/// </summary>
public interface IBookSearchService
{
    /// <summary>
    /// Busca libros por autor utilizando la API externa
    /// </summary>
    /// <param name="author">Nombre del autor a buscar</param>
    /// <returns>Lista de libros encontrados</returns>
    Task<IEnumerable<Book>> SearchBooksByAuthorAsync(string author);
}
