using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReelWords.Main;
using ReelWords.Services;
using System;
using System.Threading.Tasks;

namespace ReelWords;

public static class Program
{
    private static IConsoleUserInterfaceService _uiService;
    private static ILogger _logger;

    static async Task Main(string[] args)
    {
        try
        {
            var serviceProvider = Startup.InitializeServices();

            InitializeLogs(serviceProvider);

            var reelWordManager = serviceProvider.GetService<IReelWordsGameManager>();
            await reelWordManager.Start();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            _uiService.ShowError($"Unexpected error using ReelWords. Please, restart application.");
        }

        _uiService.GetInput("Press Enter to exit.");
    }

    private static void InitializeLogs(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<ReelWordsGameManager>();
        _uiService = serviceProvider.GetService<IConsoleUserInterfaceService>();
    }
}