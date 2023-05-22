using Moq;
using Newtonsoft.Json;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;
using ReelWords.Infrastructure.Mappers;
using ReelWords.Infrastructure.Repositories;
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
        public async Task Create_WhenGameSaved_ShouldGenerateJsonAndPathProperly()
        {
            var game = _fixture.CreateDefaultGame();
            var expectedContent = JsonConvert.SerializeObject(GameMapper.ToDto(game));
            var savePath = _fixture.Configuration[GameLocalFileRepository.FolderKey];
            var userDocsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var expectedFolderPath = Path.Combine($"{userDocsFolder}", savePath);
            var expectedComplete = Path.Combine(expectedFolderPath, $"{ReelWordsFixture.DefaultUserId}.txt");

            var filePathParam = string.Empty;
            var contentParam = string.Empty;
            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(serv => serv.WriteFile(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .Callback((string filePath, string content) =>
                {
                    filePathParam = filePath;
                    contentParam = content;
                });

            var repoMock = new GameLocalFileRepository(_fixture.Configuration, fileServiceMock.Object);

            await repoMock.Create(game);

            Assert.Equal(expectedComplete, filePathParam);
            Assert.Equal(expectedContent, contentParam);
        }

        [Fact]
        public async Task GetGameByUserId_WhenUserFileExists_ShouldLoadTheGameProperly()
        {
            var userId = "marthaGame";

            var testFilePath = $".\\TestData\\SavedGames\\{userId}.txt";
            var expectedContent = File.ReadAllText(testFilePath);
            var expectedFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var savedGamesFolder = _fixture.Configuration[GameLocalFileRepository.FolderKey];
            var expectedFilePath = Path.Combine(expectedFolder, $"{savedGamesFolder}\\{userId}.txt");

            var fileServiceMock = new Mock<IFileService>();
            fileServiceMock
                .Setup(serv => serv.ReadFile(
                    It.Is<string>(x => x == expectedFilePath)))
                .Returns(expectedContent);

            var gameRepo = new GameLocalFileRepository(_fixture.Configuration, fileServiceMock.Object);
            var loadedGame = await gameRepo.GetGameByUserId(userId);

            var expectedId = "e5eee5aa-f464-4c04-9bc7-4b53bca4f7d7";
            var expectedScore = 24;
            var expectedReel1 = new char[] { 'e', 'j', 'l' };
            var expectedReel2 = new char[] { 'v', 'n', 'y' };
            var playedWords = new List<Word>()
            {
                Word.Create("x", 4),
                Word.Create("y", 4)
            };

            Assert.NotNull(loadedGame);
            Assert.Equal(expectedId, loadedGame.Id);
            Assert.Equal(userId, loadedGame.UserId);
            Assert.Equal(expectedReel1, loadedGame.ReelPanel.GetReelByRow(0));
            Assert.Equal(expectedReel2, loadedGame.ReelPanel.GetReelByRow(1));
            Assert.Equal(playedWords, loadedGame.PlayedWords);
            Assert.Equal(expectedScore, loadedGame.Score);
        }
    }
}