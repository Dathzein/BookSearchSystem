using BookSearchSystem.Application.DTOs;
using BookSearchSystem.Application.Interfaces;
using BookSearchSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookSearchSystem.Web.Controllers;

/// <summary>
/// Controlador principal para búsqueda de libros
/// </summary>
public class BookSearchController : Controller
{
    private readonly IBookSearchApplicationService _bookSearchService;
    private readonly ISearchHistoryApplicationService _searchHistoryService;
    private readonly ILogger<BookSearchController> _logger;
    
    public BookSearchController(
        IBookSearchApplicationService bookSearchService,
        ISearchHistoryApplicationService searchHistoryService,
        ILogger<BookSearchController> logger)
    {
        _bookSearchService = bookSearchService;
        _searchHistoryService = searchHistoryService;
        _logger = logger;
    }
    
    /// <summary>
    /// Página principal con formulario de búsqueda
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        _logger.LogInformation("Accediendo a la página principal de búsqueda");
        return View(new BookSearchViewModel());
    }
    
    /// <summary>
    /// Procesa la búsqueda de libros
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(BookSearchViewModel model)
    {
        _logger.LogInformation("Iniciando búsqueda para autor: {Author}", model.Author);
        
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Modelo inválido para búsqueda: {Author}", model.Author);
            model.ShowResults = false;
            return View("Index", model);
        }
        
        try
        {
            var request = new BookSearchRequestDto(model.Author);
            var response = await _bookSearchService.SearchBooksByAuthorAsync(request);
            
            // Mapear respuesta a ViewModel
            model.ShowResults = true;
            model.Success = response.Success;
            model.Message = response.Message;
            model.TotalResults = response.TotalResults;
            model.Books = response.Books.Select(b => new BookResultViewModel(
                b.Title, b.Authors, b.FirstPublishYear, b.Publishers)).ToList();
            
            _logger.LogInformation("Búsqueda completada para autor: {Author}. Resultados: {Count}", 
                model.Author, model.TotalResults);
            
            return View("Index", model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la búsqueda para autor: {Author}", model.Author);
            
            model.ShowResults = true;
            model.Success = false;
            model.Message = "Ocurrió un error durante la búsqueda. Por favor, intente nuevamente.";
            model.TotalResults = 0;
            model.Books = new List<BookResultViewModel>();
            
            return View("Index", model);
        }
    }
    
    /// <summary>
    /// Muestra el historial de búsquedas
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> History()
    {
        _logger.LogInformation("Accediendo al historial de búsquedas");
        
        try
        {
            var searchHistories = await _searchHistoryService.GetSearchHistoryAsync();
            
            var viewModel = new SearchHistoryViewModel
            {
                SearchHistories = searchHistories.Select(h => new SearchHistoryItemViewModel(
                    h.Id, h.AuthorSearched, h.SearchDate)).ToList(),
                TotalRecords = searchHistories.Count,
                Message = searchHistories.Any() 
                    ? $"Se encontraron {searchHistories.Count} búsquedas en el historial"
                    : "No hay búsquedas en el historial"
            };
            
            _logger.LogInformation("Historial cargado con {Count} registros", searchHistories.Count);
            
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar el historial de búsquedas");
            
            var errorViewModel = new SearchHistoryViewModel
            {
                SearchHistories = new List<SearchHistoryItemViewModel>(),
                TotalRecords = 0,
                Message = "Error al cargar el historial de búsquedas"
            };
            
            return View(errorViewModel);
        }
    }
    
    /// <summary>
    /// Realiza una nueva búsqueda desde el historial (abre en nueva ventana)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> SearchFromHistory(string author)
    {
        _logger.LogInformation("Búsqueda desde historial para autor: {Author}", author);
        
        if (string.IsNullOrWhiteSpace(author))
        {
            _logger.LogWarning("Intento de búsqueda desde historial con autor vacío");
            return RedirectToAction("Index");
        }
        
        // Ejecutar la búsqueda directamente
        var model = new BookSearchViewModel
        {
            Author = author,
            ShowResults = true
        };
        
        var requestDto = new BookSearchRequestDto { Author = author };
        var responseDto = await _bookSearchService.SearchBooksByAuthorAsync(requestDto);

        model.Books = responseDto.Books.Select(b => new BookResultViewModel
        {
            Title = b.Title,
            Authors = b.Authors,
            FirstPublishYear = b.FirstPublishYear,
            Publishers = b.Publishers
        }).ToList();
        model.Success = responseDto.Success;
        model.Message = responseDto.Message;
        model.TotalResults = model.Books.Count;
        
        _logger.LogInformation("Búsqueda desde historial completada para autor: {Author}. Resultados: {Count}", 
            author, model.Books.Count);
        
        return View("Index", model);
    }
    
    /// <summary>
    /// API endpoint para búsqueda AJAX (opcional)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SearchApi([FromBody] BookSearchRequestDto request)
    {
        _logger.LogInformation("API: Búsqueda para autor: {Author}", request.Author);
        
        try
        {
            var response = await _bookSearchService.SearchBooksByAuthorAsync(request);
            return Json(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en API de búsqueda para autor: {Author}", request.Author);
            
            var errorResponse = new BookSearchResponseDto(
                "Error interno del servidor", request.Author);
            
            return Json(errorResponse);
        }
    }
}
