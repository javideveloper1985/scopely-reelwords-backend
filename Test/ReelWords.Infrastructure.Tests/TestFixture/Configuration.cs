using Microsoft.Extensions.Configuration;
using Moq;
using ReelWords.Infrastructure.Services;

namespace ReelWords.Infrastructure.Tests.TestFixture
{
    public class InfrastructureFixture
    {
        public IConfiguration Configuration { get; init; }

        public InfrastructureFixture()
        {
            var configMock = new Mock<IConfiguration>();

            configMock.Setup(x => x[GetLetterScoresFileService.FileKey]).Returns("TestData/scores.txt");
            configMock.Setup(x => x[CreateReelPanelFileService.FileKey]).Returns("TestData/reels.txt");
            configMock.Setup(x => x[GetDictionaryFileService.FileKey]).Returns("TestData/words.txt");

            Configuration = configMock.Object;
        }
    }
}