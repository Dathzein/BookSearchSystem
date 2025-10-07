namespace BookSearchSystem.Web.Models;

/// <summary>
/// ViewModel para mostrar el historial de búsquedas
/// </summary>
public class SearchHistoryViewModel
{
    public List<SearchHistoryItemViewModel> SearchHistories { get; set; } = new();
    public int TotalRecords { get; set; } = 0;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel para un elemento del historial de búsquedas
/// </summary>
public class SearchHistoryItemViewModel
{
    public int Id { get; set; }
    public string AuthorSearched { get; set; } = string.Empty;
    public DateTime SearchDate { get; set; }
    public string FormattedSearchDate { get; set; } = string.Empty;
    
    /// <summary>
    /// Constructor por defecto
    /// </summary>
    public SearchHistoryItemViewModel() { }
    
    /// <summary>
    /// Constructor con parámetros
    /// </summary>
    public SearchHistoryItemViewModel(int id, string authorSearched, DateTime searchDate)
    {
        Id = id;
        AuthorSearched = authorSearched;
        SearchDate = searchDate;
        FormattedSearchDate = searchDate.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
