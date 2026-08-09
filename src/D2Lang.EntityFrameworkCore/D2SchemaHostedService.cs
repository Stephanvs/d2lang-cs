using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace d2.EntityFrameworkCore;

internal sealed class D2SchemaHostedService<TContext> : IHostedService
  where TContext : DbContext
{
  private readonly IServiceScopeFactory _scopeFactory;
  private readonly IHostEnvironment _environment;
  private readonly IOptionsMonitor<D2SchemaOptions> _options;
  private readonly ILogger<D2SchemaHostedService<TContext>> _logger;

  public D2SchemaHostedService(
    IServiceScopeFactory scopeFactory,
    IHostEnvironment environment,
    IOptionsMonitor<D2SchemaOptions> options,
    ILogger<D2SchemaHostedService<TContext>> logger)
  {
    _scopeFactory = scopeFactory;
    _environment = environment;
    _options = options;
    _logger = logger;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var options = _options.Get(D2SchemaRegistration.OptionsName<TContext>());
    options.Validate();

    using var scope = _scopeFactory.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<TContext>();
    var diagram = D2ModelExtensions.CreateD2Diagram(context.Model, options);
    var source = NormalizeSource(diagram.ToString());
    var outputPath = ResolveOutputPath(options.OutputPath);

    if (File.Exists(outputPath))
    {
      var existing = await File.ReadAllTextAsync(outputPath, cancellationToken).ConfigureAwait(false);
      if (string.Equals(existing, source, StringComparison.Ordinal))
      {
        _logger.LogDebug("D2 database schema is unchanged at {OutputPath}.", outputPath);
        return;
      }
    }

    var directory = Path.GetDirectoryName(outputPath)
      ?? throw new InvalidOperationException($"The D2 schema output path '{outputPath}' has no directory.");
    Directory.CreateDirectory(directory);

    var temporaryPath = $"{outputPath}.{Guid.NewGuid():N}.tmp";
    try
    {
      await File.WriteAllTextAsync(
        temporaryPath,
        source,
        new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        cancellationToken).ConfigureAwait(false);
      File.Move(temporaryPath, outputPath, overwrite: true);
    }
    finally
    {
      if (File.Exists(temporaryPath))
      {
        File.Delete(temporaryPath);
      }
    }

    _logger.LogInformation("Generated D2 database schema at {OutputPath}.", outputPath);
  }

  public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

  private string ResolveOutputPath(string outputPath)
    => Path.GetFullPath(
      Path.IsPathRooted(outputPath)
        ? outputPath
        : Path.Combine(_environment.ContentRootPath, outputPath));

  private static string NormalizeSource(string source)
  {
    var normalized = source.Replace("\r\n", "\n").Replace('\r', '\n');
    return normalized.Length == 0 ? normalized : $"{normalized.TrimEnd('\n')}\n";
  }
}
