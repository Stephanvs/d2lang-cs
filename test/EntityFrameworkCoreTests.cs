using System.ComponentModel;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Tests;

[TestClass]
public class EntityFrameworkCoreTests
{
  [TestMethod]
  public void ToD2Diagram_MapsRelationalTablesColumnsConstraintsAndRelationships()
  {
    using var context = CreateContext();

    var actual = context.ToD2Diagram().ToString();

    var expected = string.Join(
      Environment.NewLine,
      "catalog.posts: {",
      "  id: int NOT NULL { constraint: primary_key }",
      "  title: \"nvarchar(max) NOT NULL\"",
      "  user_id: int NOT NULL { constraint: foreign_key }",
      "  shape: sql_table",
      "}",
      "catalog.users: {",
      "  id: int NOT NULL { constraint: primary_key }",
      "  email: \"nvarchar(450) NOT NULL\" { constraint: unique }",
      "  shape: sql_table",
      "}",
      "catalog.posts.user_id -> catalog.users.id");

    Assert.AreEqual(expected, actual);
  }

  [TestMethod]
  public void ToD2Diagram_CanOmitSchemaNullabilityAndConnections()
  {
    using var context = CreateContext();

    var actual = context.ToD2Diagram(options =>
    {
      options.IncludeDatabaseSchemas = false;
      options.IncludeNullability = false;
      options.IncludeForeignKeyConnections = false;
    }).ToString();

    StringAssert.Contains(actual, "posts: {");
    StringAssert.Contains(actual, "title: \"nvarchar(max)\"");
    Assert.IsFalse(actual.Contains("catalog.", StringComparison.Ordinal));
    Assert.IsFalse(actual.Contains(" -> ", StringComparison.Ordinal));
  }

  [TestMethod]
  public async Task AddD2Schema_GeneratesFileAtHostStartupAndSkipsUnchangedContent()
  {
    var contentRoot = Path.Combine(Path.GetTempPath(), $"d2-efcore-{Guid.NewGuid():N}");
    Directory.CreateDirectory(contentRoot);

    try
    {
      var outputPath = Path.Combine(contentRoot, "docs", "schema.d2");
      using (var host = CreateHost(contentRoot))
      {
        await host.StartAsync();
        Assert.IsTrue(File.Exists(outputPath));
        var source = await File.ReadAllTextAsync(outputPath);
        StringAssert.Contains(source, "catalog.users");
        await host.StopAsync();
      }

      var unchangedTimestamp = DateTime.UtcNow.AddMinutes(-5);
      File.SetLastWriteTimeUtc(outputPath, unchangedTimestamp);

      using (var host = CreateHost(contentRoot))
      {
        await host.StartAsync();
        Assert.AreEqual(unchangedTimestamp, File.GetLastWriteTimeUtc(outputPath));
        await host.StopAsync();
      }
    }
    finally
    {
      Directory.Delete(contentRoot, recursive: true);
    }
  }

  [TestMethod]
  public void GeneratedEfSchema_PassesD2ValidationWhenCliIsAvailable()
  {
    using var context = CreateContext();
    var source = context.ToD2Diagram().ToString();
    var path = Path.Combine(Path.GetTempPath(), $"d2lang-cs-efcore-{Guid.NewGuid():N}.d2");
    File.WriteAllText(path, source);

    try
    {
      using var process = new Process
      {
        StartInfo = new ProcessStartInfo
        {
          FileName = "d2",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardError = true,
        }
      };
      process.StartInfo.ArgumentList.Add("validate");
      process.StartInfo.ArgumentList.Add(path);

      try
      {
        process.Start();
      }
      catch (Win32Exception)
      {
        Assert.Inconclusive("The D2 CLI is not installed; parser-backed validation was skipped.");
        return;
      }

      var output = process.StandardOutput.ReadToEnd();
      var error = process.StandardError.ReadToEnd();
      process.WaitForExit();
      Assert.AreEqual(
        0,
        process.ExitCode,
        $"d2 validate failed.{Environment.NewLine}{output}{error}{Environment.NewLine}{source}");
    }
    finally
    {
      File.Delete(path);
    }
  }

  private static IHost CreateHost(string contentRoot)
  {
    var settings = new HostApplicationBuilderSettings { ContentRootPath = contentRoot };
    var builder = Host.CreateApplicationBuilder(settings);
    builder.Services
      .AddDbContext<SchemaDbContext>(options => options.UseSqlServer(TestConnectionString))
      .AddD2Schema<SchemaDbContext>("docs/schema.d2");
    return builder.Build();
  }

  private static SchemaDbContext CreateContext()
  {
    var options = new DbContextOptionsBuilder<SchemaDbContext>()
      .UseSqlServer(TestConnectionString)
      .Options;
    return new SchemaDbContext(options);
  }

  private const string TestConnectionString =
    "Server=localhost;Database=d2_schema_tests;Integrated Security=true;TrustServerCertificate=true";

  private sealed class SchemaDbContext : DbContext
  {
    public SchemaDbContext(DbContextOptions<SchemaDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<User>(entity =>
      {
        entity.ToTable("users", "catalog");
        entity.HasKey(user => user.Id);
        entity.Property(user => user.Id).HasColumnName("id");
        entity.Property(user => user.Email).HasColumnName("email").IsRequired();
        entity.HasIndex(user => user.Email).IsUnique();
      });

      modelBuilder.Entity<Post>(entity =>
      {
        entity.ToTable("posts", "catalog");
        entity.HasKey(post => post.Id);
        entity.Property(post => post.Id).HasColumnName("id");
        entity.Property(post => post.Title).HasColumnName("title").IsRequired();
        entity.Property(post => post.UserId).HasColumnName("user_id");
        entity.HasOne(post => post.User)
          .WithMany(user => user.Posts)
          .HasForeignKey(post => post.UserId);
      });
    }
  }

  private sealed class User
  {
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public List<Post> Posts { get; set; } = new();
  }

  private sealed class Post
  {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
  }
}
