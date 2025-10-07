namespace BookSearchSystem.Application.DTOs;

/// <summary>
/// DTO para representar un libro en las respuestas de la API
/// </summary>
public class BookDto
{
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string FirstPublishYear { get; set; } = string.Empty;
    public string Publishers { get; set; } = string.Empty;
    
    // Constructor por defecto
    public BookDto() { }
    
    // Constructor con parámetros
    public BookDto(string title, string authors, string firstPublishYear, string publishers)
    {
        Title = title;
        Authors = authors;
        FirstPublishYear = firstPublishYear;
        Publishers = publishers;
    }
}
