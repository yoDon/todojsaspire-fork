using Microsoft.EntityFrameworkCore;

namespace TodojsAspire.ApiService;

public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodojsAspire.ApiService.Todo> Todo { get; set; } = default!;
}
