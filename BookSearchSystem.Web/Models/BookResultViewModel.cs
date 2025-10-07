namespace BookSearchSystem.Web.Models;

/// <summary>
/// ViewModel para mostrar un libro en los resultados
/// </summary>
public class BookResultViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Authors { get; set; } = string.Empty;
    public string FirstPublishYear { get; set; } = string.Empty;
    public string Publishers { get; set; } = string.Empty;
    
    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public BookResultViewModel() { }
    
    /// <summary>
    /// Constructor con parámetros
    /// </summary>
    public BookResultViewModel(string title, string authors, string firstPublishYear, string publishers)
    {
        Title = title;
        Authors = authors;
        FirstPublishYear = firstPublishYear;
        Publishers = publishers;
    }
}
