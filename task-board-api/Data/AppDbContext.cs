using Microsoft.EntityFrameworkCore;
using task_board_api.Models;

namespace task_board_api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ColumnItem> Columns => Set<ColumnItem>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ColumnItem>().HasData(
            new ColumnItem { Id = 1, Name = "Por Hacer", Order = 1 },
            new ColumnItem { Id = 2, Name = "En Proceso", Order = 2 },
            new ColumnItem { Id = 3, Name = "Completado", Order = 3 }
        );

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, Title = "Aprender C#", Description = "Estudiar sintaxis básica", ColumnId = 1, Order = 1 },
            new TaskItem { Id = 2, Title = "Crear API .NET", Description = "Configurar controladores y DbContext", ColumnId = 2, Order = 1 }
        );
    }
}
