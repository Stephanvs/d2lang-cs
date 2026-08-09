using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
  .AddDbContext<BlogDbContext>(options => options.UseSqlite("Data Source=blog.db"))
  .AddD2Schema<BlogDbContext>("database-schema.d2");

using var host = builder.Build();
await host.StartAsync();
await host.StopAsync();

internal sealed class BlogDbContext : DbContext
{
  public BlogDbContext(DbContextOptions<BlogDbContext> options)
    : base(options)
  {
  }

  public DbSet<Blog> Blogs => Set<Blog>();
  public DbSet<Post> Posts => Set<Post>();
}

internal sealed class Blog
{
  public int Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public List<Post> Posts { get; set; } = new();
}

internal sealed class Post
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public int BlogId { get; set; }
  public Blog Blog { get; set; } = null!;
}
