using Newtonsoft.Json;
using ReelWords.Domain.Entities;
using ReelWords.Domain.ValueObjects;
using ReelWords.Infrastructure.Mappers;
using ReelWords.Infrastructure.Repositories;
using ReelWords.Infrastructure.Services;
using ReelWords.Infrastructure.Tests.TestFixture;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GameLocalFileRepositoryTests : IClassFixture<ReelWordsFixture>
    {
        private readonly ReelWordsFixture _fixture;

        public GameLocalFileRepositoryTests(ReelWordsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetAll_WhenPathExists_ShouldThrowException()
        {
            var userId = "peter";
            var reelPanel = ReelPanel.CreateEmpty(2, 2);
            reelPanel.AddReel(0, new char[] { 'a', 'b' });
            reelPanel.AddReel(1, new char[] { 'c', 'd' });
            var game = Game.CreateNew(userId, reelPanel);

            var repoMock = new GameLocalFileRepositoryMock(_fixture.Configuration, game);
            await repoMock.Create(game);

            var expectedContent = JsonConvert.SerializeObject(GameMapper.ToDto(game));
            var savePath = _fixture.Configuration[GameLocalFileRepository.FileKey];
            var expectedPath = Path.Combine(
                $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}",
                savePath,
                $"{userId}.txt");

            Assert.Equal(expectedPath, repoMock.CompletePath);
            Assert.Equal(expectedContent, repoMock.Content);

        }
    }
}