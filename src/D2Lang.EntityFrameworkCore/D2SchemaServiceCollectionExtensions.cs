using d2.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Registration extensions for automatic EF Core schema generation.</summary>
public static class D2SchemaServiceCollectionExtensions
{
  /// <summary>
  /// Generates a D2 SQL-table schema from <typeparamref name="TContext"/> when the host starts.
  /// </summary>
  /// <typeparam name="TContext">The registered Entity Framework Core context type.</typeparam>
  /// <param name="services">The application's service collection.</param>
  /// <param name="outputPath">
  /// The generated <c>.d2</c> path. Relative paths are resolved from the host content root.
  /// </param>
  /// <param name="configure">Optional diagram-generation settings.</param>
  /// <returns>The same service collection for continued registration.</returns>
  public static IServiceCollection AddD2Schema<TContext>(
    this IServiceCollection services,
    string outputPath,
    Action<D2SchemaOptions>? configure = null)
    where TContext : DbContext
  {
    if (services is null) throw new ArgumentNullException(nameof(services));
    if (string.IsNullOrWhiteSpace(outputPath))
    {
      throw new ArgumentException("A D2 schema output path is required.", nameof(outputPath));
    }

    services
      .AddOptions<D2SchemaOptions>(D2SchemaRegistration.OptionsName<TContext>())
      .Configure(options =>
      {
        options.OutputPath = outputPath;
        configure?.Invoke(options);
      });

    services.TryAddEnumerable(
      ServiceDescriptor.Singleton<IHostedService, D2SchemaHostedService<TContext>>());
    return services;
  }
}

internal static class D2SchemaRegistration
{
  internal static string OptionsName<TContext>()
    => typeof(TContext).AssemblyQualifiedName
      ?? throw new InvalidOperationException("The Entity Framework Core context type has no assembly-qualified name.");
}
