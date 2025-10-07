using System.ComponentModel.DataAnnotations;

namespace BookSearchSystem.Web.Models;

/// <summary>
/// ViewModel para el formulario de búsqueda de libros
/// </summary>
public class BookSearchViewModel
{
    [Required(ErrorMessage = "El nombre del autor es obligatorio")]
    [StringLength(255, MinimumLength = 1, ErrorMessage = "El nombre del autor debe tener entre 1 y 255 caracteres")]
    [Display(Name = "Nombre del Autor")]
    public string Author { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si se debe mostrar los resultados
    /// </summary>
    public bool ShowResults { get; set; } = false;
    
    /// <summary>
    /// Mensaje de resultado de la búsqueda
    /// </summary>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica si la búsqueda fue exitosa
    /// </summary>
    public bool Success { get; set; } = false;
    
    /// <summary>
    /// Lista de libros encontrados
    /// </summary>
    public List<BookResultViewModel> Books { get; set; } = new();
    
    /// <summary>
    /// Total de resultados encontrados
    /// </summary>
    public int TotalResults { get; set; } = 0;
}
