using System.ComponentModel.DataAnnotations;

namespace BookSearchSystem.Application.DTOs;

/// <summary>
/// DTO para las solicitudes de búsqueda de libros
/// </summary>
public class BookSearchRequestDto
{
    [Required(ErrorMessage = "El nombre del autor es obligatorio")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "El nombre del autor debe tener entre 1 y 255 caracteres")]
    public string Author { get; set; } = string.Empty;
    
    // Constructor por defecto
    public BookSearchRequestDto() { }
    
    // Constructor con parámetros
    public BookSearchRequestDto(string author)
    {
        Author = author?.Trim() ?? string.Empty;
    }
}
