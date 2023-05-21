using ReelWords.Infrastructure.Services;
using ReelWords.Infrastructure.Tests.TestFixture;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GenerateReelPanelFileServiceTests : IClassFixture<InfrastructureFixture>
    {
        private readonly InfrastructureFixture _fixture;

        public GenerateReelPanelFileServiceTests(InfrastructureFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetAll_WhenPathExists_ShouldThrowException()
        {
            var wordSize = 7;
            var expectedJoinedChars = "udxclae";

            var reelPanelService = new CreateReelPanelFileService(_fixture.Configuration);
            var reelPanel = await reelPanelService.Create(wordSize);

            Assert.Equal(2, reelPanel.RowCount);
            for (int i = 0; i < expectedJoinedChars.Length; i++)
                Assert.Equal(expectedJoinedChars[i], reelPanel.Matrix[0, i]);
        }
    }
}