using BookSearchSystem.Application.DTOs;
using BookSearchSystem.Domain.ValueObjects;

namespace BookSearchSystem.Application.Mappers;

/// <summary>
/// Mapper para convertir entre Book y BookDto
/// </summary>
public static class BookMapper
{
    /// <summary>
    /// Convierte un Book del dominio a BookDto
    /// </summary>
    public static BookDto ToDto(Book book)
    {
        if (book == null)
            throw new ArgumentNullException(nameof(book));
        
        return new BookDto(
            title: book.DisplayTitle,
            authors: book.DisplayAuthors,
            firstPublishYear: book.DisplayFirstPublishYear,
            publishers: book.DisplayPublishers
        );
    }
    
    /// <summary>
    /// Convierte una lista de Books del dominio a lista de BookDto
    /// </summary>
    public static List<BookDto> ToDto(IEnumerable<Book> books)
    {
        if (books == null)
            return new List<BookDto>();
        
        return books.Select(ToDto).ToList();
    }
    
    /// <summary>
    /// Convierte un BookDto a Book del dominio
    /// </summary>
    public static Book ToDomain(BookDto bookDto)
    {
        if (bookDto == null)
            throw new ArgumentNullException(nameof(bookDto));
        
        // Parsear los autores (separados por coma)
        var authors = string.IsNullOrWhiteSpace(bookDto.Authors) 
            ? new List<string>() 
            : bookDto.Authors.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim())
                .ToList();
        
        // Parsear los publishers (separados por coma)
        var publishers = string.IsNullOrWhiteSpace(bookDto.Publishers) 
            ? new List<string>() 
            : bookDto.Publishers.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        
        // Parsear el año de publicación
        int? firstPublishYear = null;
        if (int.TryParse(bookDto.FirstPublishYear, out var year))
        {
            firstPublishYear = year;
        }
        
        return new Book(
            title: bookDto.Title,
            authors: authors,
            firstPublishYear: firstPublishYear,
            publishers: publishers
        );
    }
}
