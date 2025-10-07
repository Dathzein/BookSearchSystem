using BookSearchSystem.Application.DTOs;
using BookSearchSystem.Domain.Entities;

namespace BookSearchSystem.Application.Mappers;

/// <summary>
/// Mapper para convertir entre SearchHistory y SearchHistoryDto
/// </summary>
public static class SearchHistoryMapper
{
    /// <summary>
    /// Convierte un SearchHistory del dominio a SearchHistoryDto
    /// </summary>
    public static SearchHistoryDto ToDto(SearchHistory searchHistory)
    {
        if (searchHistory == null)
            throw new ArgumentNullException(nameof(searchHistory));
        
        return new SearchHistoryDto(
            id: searchHistory.Id,
            authorSearched: searchHistory.AuthorSearched,
            searchDate: searchHistory.SearchDate
        );
    }
    
    /// <summary>
    /// Convierte una lista de SearchHistory del dominio a lista de SearchHistoryDto
    /// </summary>
    public static List<SearchHistoryDto> ToDto(IEnumerable<SearchHistory> searchHistories)
    {
        if (searchHistories == null)
            return new List<SearchHistoryDto>();
        
        return searchHistories.Select(ToDto).ToList();
    }
    
    /// <summary>
    /// Convierte un SearchHistoryDto a SearchHistory del dominio
    /// </summary>
    public static SearchHistory ToDomain(SearchHistoryDto searchHistoryDto)
    {
        if (searchHistoryDto == null)
            throw new ArgumentNullException(nameof(searchHistoryDto));
        
        return new SearchHistory(searchHistoryDto.AuthorSearched)
        {
            Id = searchHistoryDto.Id,
            SearchDate = searchHistoryDto.SearchDate
        };
    }
}
