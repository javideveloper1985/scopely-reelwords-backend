using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReelWords.Domain.Repositories;
using ReelWords.Domain.Services;
using ReelWords.Infrastructure.Repositories;
using ReelWords.Main;
using ReelWords.Services.Implementations;
using ReelWords.Services;
using ReelWords.UseCases.Implementations;
using ReelWords.UseCases;
using System;
using Microsoft.Extensions.Configuration;
using ReelWords.Domain.Services.Implementations;
using ReelWords.Infrastructure.Services.Implementations;

namespace ReelWords
{
    public static class Startup
    {
        public static IServiceProvider InitializeServices()
        {
            var configuration = InitConfig();
            var serviceProvider = new ServiceCollection()
                .AddSingleton(configuration)
                .AddSingleton<ILoggerFactory, LoggerFactory>()
                .AddLogging(builder =>
                {
                    builder.SetMinimumLevel(LogLevel.Information);
                    builder.AddConsole();
                })
                .AddScoped<IGameRepository, GameLocalFileRepository>()
                .AddScoped<IGetDictionaryService, GetDictionaryFileService>()

                .AddSingleton<IConsoleUserInterfaceService, ConsoleUserInterfaceService>()
                .AddSingleton<ICreateReelPanelService, CreateReelPanelFileService>()
                .AddSingleton<IGetLetterScoresService, GetLetterScoresFileService>()
                .AddSingleton<IGetDictionaryService, GetDictionaryFileService>()
                .AddSingleton<ISaveGameService, SaveGameService>()
                .AddSingleton<IFileService, FileService>()

                .AddSingleton<ILoadGameUseCase, LoadGameUseCase>()
                .AddSingleton<ICreateGameUseCase, CreateGameUseCase>()
                .AddSingleton<ISaveGameUseCase, SaveGameUseCase>()
                
                .AddSingleton<IReelWordsGameManager, ReelWordsGameManager>()

                .BuildServiceProvider();

            return serviceProvider;
        }

        private static IConfiguration InitConfig()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true);

            return configurationBuilder.Build();
        }
    }
}
