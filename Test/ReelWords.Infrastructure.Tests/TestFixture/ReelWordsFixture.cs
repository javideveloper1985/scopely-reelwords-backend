using Microsoft.Extensions.Configuration;
using Moq;
using ReelWords.Domain.Entities;
using ReelWords.Infrastructure.Repositories;
using ReelWords.Infrastructure.Services.Implementations;

namespace ReelWords.Infrastructure.Tests.TestFixture
{
    public class ReelWordsFixture
    {
        public IConfiguration Configuration { get; init; }

        public static char[] Reel1 = new char[] { 'x', 'y', 'z' };
        public static char[] Reel2 = new char[] { 't', 'c', 'a' };

        public const int DefaultWordSize = 3;
        public const int DefaultPenaltyScore = 2;
        public const string DefaultUserId = "Peter";

        public ReelWordsFixture()
        {
            var configMock = new Mock<IConfiguration>();

            configMock.Setup(x => x[GameLocalFileRepository.FolderKey]).Returns("TestData\\SavedGames");
            configMock.Setup(x => x[GetLetterScoresFileService.FileKey]).Returns("TestData\\scores.txt");
            configMock.Setup(x => x[CreateReelPanelFileService.FileKey]).Returns("TestData\\reels.txt");
            configMock.Setup(x => x[GetDictionaryFileService.FileKey]).Returns("TestData\\words.txt");

            Configuration = configMock.Object;
        }

        public Game CreateDefaultGame(string userId = null)
        {
            return Game.CreateNew(userId ?? DefaultUserId, CreateDefaultReelPanel());
        }

        public ReelPanel CreateDefaultReelPanel()
        {
            var panel = ReelPanel.CreateEmpty(2, 3);
            panel.AddReel(0, Reel1);
            panel.AddReel(1, Reel2);
            return panel;
        }
    }
}