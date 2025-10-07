namespace BookSearchSystem.Domain.ValueObjects;

/// <summary>
/// Representa un libro obtenido de la API de OpenLibrary
/// </summary>
public class Book
{
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = new();
    public int? FirstPublishYear { get; set; }
    public List<string> Publishers { get; set; } = new();
    
    // Propiedades calculadas para manejo de valores por defecto
    public string DisplayTitle => !string.IsNullOrWhiteSpace(Title) ? Title : "Título sin registro";
    
    public string DisplayAuthors => Authors.Any() ? string.Join(", ", Authors) : "Autor sin registro";
    
    public string DisplayFirstPublishYear => FirstPublishYear?.ToString() ?? "Fecha sin registro";
    
    public string DisplayPublishers => Publishers.Any() ? string.Join(", ", Publishers) : "Editoriales sin registro";
    
    // Constructor por defecto
    public Book() { }
    
    // Constructor con parámetros
    public Book(string title, List<string> authors, int? firstPublishYear, List<string> publishers)
    {
        Title = title;
        Authors = authors ?? new List<string>();
        FirstPublishYear = firstPublishYear;
        Publishers = publishers ?? new List<string>();
    }
    
    // Método para validar si el libro tiene información mínima
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Title) || Authors.Any();
    }
    
    // Override ToString para debugging
    public override string ToString()
    {
        return $"{DisplayTitle} by {DisplayAuthors} ({DisplayFirstPublishYear})";
    }
}
