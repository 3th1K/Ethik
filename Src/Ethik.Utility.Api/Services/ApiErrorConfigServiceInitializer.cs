using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ethik.Utility.Api.Services;

/// <summary>
/// Initializes and manages the lifecycle of the <see cref="ApiErrorConfigService"/> as an IHostedService.
/// This class ensures that the <see cref="ApiErrorConfigService"/> is properly initialized at application start and disposed of at application shutdown.
/// </summary>
internal class ApiErrorConfigServiceInitializer : IHostedService
{
    private readonly ApiErrorConfigService _apiErrorConfigService;
    private readonly ILogger<ApiErrorConfigServiceInitializer> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiErrorConfigServiceInitializer"/> class.
    /// </summary>
    /// <param name="apiErrorConfigService">The instance of <see cref="ApiErrorConfigService"/> to be initialized.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public ApiErrorConfigServiceInitializer(ApiErrorConfigService apiErrorConfigService, ILogger<ApiErrorConfigServiceInitializer> logger)
    {
        _apiErrorConfigService = apiErrorConfigService ?? throw new ArgumentNullException(nameof(apiErrorConfigService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Starts the <see cref="ApiErrorConfigService"/> initialization process.
    /// This method is called when the application starts.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to monitor for stopping requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Explicitly initialize the service
            _apiErrorConfigService.Initialize();
            _logger.LogInformation("ApiErrorConfigService has been initialized.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to Initialilse ApiErrorConfigService");
            throw;
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Disposes of the <see cref="ApiErrorConfigService"/> and performs any necessary cleanup.
    /// This method is called when the application is stopping.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to monitor for stopping requests.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _apiErrorConfigService.DisposeObject();
        return Task.CompletedTask;
    }
}