namespace BookSearchSystem.Application.DTOs;

/// <summary>
/// DTO para las respuestas de búsqueda de libros
/// </summary>
public class BookSearchResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<BookDto> Books { get; set; } = new();
    public int TotalResults { get; set; }
    public string SearchedAuthor { get; set; } = string.Empty;
    
    // Constructor por defecto
    public BookSearchResponseDto() { }
    
    // Constructor para respuesta exitosa
    public BookSearchResponseDto(List<BookDto> books, string searchedAuthor)
    {
        Success = true;
        Books = books ?? new List<BookDto>();
        TotalResults = Books.Count;
        SearchedAuthor = searchedAuthor;
        Message = TotalResults > 0 
            ? $"Se encontraron {TotalResults} libros para el autor '{searchedAuthor}'"
            : $"No se encontraron libros para el autor '{searchedAuthor}'";
    }
    
    // Constructor para respuesta con error
    public BookSearchResponseDto(string errorMessage, string searchedAuthor = "")
    {
        Success = false;
        Message = errorMessage;
        Books = new List<BookDto>();
        TotalResults = 0;
        SearchedAuthor = searchedAuthor;
    }
}
