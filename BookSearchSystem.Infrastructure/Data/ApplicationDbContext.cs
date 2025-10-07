using BookSearchSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookSearchSystem.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos para la aplicación
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    // DbSets
    public DbSet<SearchHistory> SearchHistories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configuración de la entidad SearchHistory
        modelBuilder.Entity<SearchHistory>(entity =>
        {
            // Configuración de tabla y esquema
            entity.ToTable("search_history", "BookSearch");
            
            // Configuración de propiedades
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
            
            entity.Property(e => e.AuthorSearched)
                .HasColumnName("AuthorSearched")
                .HasMaxLength(255)
                .IsRequired();
            
            entity.Property(e => e.SearchDate)
                .HasColumnName("SearchDate")
                .HasColumnType("datetime2")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName("CreatedAt")
                .HasColumnType("datetime2")
                .IsRequired();
            
            // Índices para mejorar rendimiento
            entity.HasIndex(e => e.AuthorSearched)
                .HasDatabaseName("IX_search_history_AuthorSearched");
            
            entity.HasIndex(e => e.SearchDate)
                .HasDatabaseName("IX_search_history_SearchDate")
                .IsDescending();
        });
    }
    
    /// <summary>
    /// Configura valores por defecto antes de guardar
    /// </summary>
    public override int SaveChanges()
    {
        SetDefaultValues();
        return base.SaveChanges();
    }
    
    /// <summary>
    /// Configura valores por defecto antes de guardar (async)
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetDefaultValues();
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Establece valores por defecto para entidades nuevas
    /// </summary>
    private void SetDefaultValues()
    {
        var entries = ChangeTracker.Entries<SearchHistory>()
            .Where(e => e.State == EntityState.Added);
        
        foreach (var entry in entries)
        {
            var now = DateTime.UtcNow;
            
            if (entry.Entity.SearchDate == default)
                entry.Entity.SearchDate = now;
            
            if (entry.Entity.CreatedAt == default)
                entry.Entity.CreatedAt = now;
        }
    }
}
