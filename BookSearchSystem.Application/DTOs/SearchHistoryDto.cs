namespace BookSearchSystem.Application.DTOs;

/// <summary>
/// DTO para representar el historial de búsquedas
/// </summary>
public class SearchHistoryDto
{
    public int Id { get; set; }
    public string AuthorSearched { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public string FormattedSearchDate { get; set; } = string.Empty;
    
    // Constructor por defecto
    public SearchHistoryDto() { }
    
    // Constructor con parámetros
    public SearchHistoryDto(int id, string authorSearched, DateTime searchDate)
    {
        Id = id;
        AuthorSearched = authorSearched;
        SearchDate = searchDate;
        FormattedSearchDate = searchDate.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
