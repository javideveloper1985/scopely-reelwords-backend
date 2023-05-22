using Moq;
using ReelWords.Commands;
using ReelWords.Constants;
using ReelWords.Domain.Services;
using ReelWords.Services;
using ReelWords.UseCases.Implementations;
using ReelWords.UseCases.Requests;
using ReelWordsTests.TestFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReelWordsTests.Main
{
    public class PlayRoundUseCaseUseCaseTests : IClassFixture<ReelWordsFixture>
    {
        private readonly ReelWordsFixture _fixture;

        public PlayRoundUseCaseUseCaseTests(ReelWordsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task PlayRound_WhenUserWantsToSaveAndExit_ShouldReturnExitGameCommand(bool saveGame)
        {
            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputWord())
                .Returns(UserKeyWords.Exit);
            gameUiService
                .Setup(serv => serv.CheckIfUserWantsSaveGame())
                .Returns(saveGame);

            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var command = await useCase.PlayRound(It.IsAny<PlayRoundUseCaseRequest>());

            Assert.IsType<ExitGameCommand>(command);
            Assert.Equal(saveGame, (command as ExitGameCommand).SaveGame);
        }

        [Theory]
        [InlineData(UserKeyWords.Shuffle, typeof(ShuffleCommand))]
        [InlineData(UserKeyWords.ShowWords, typeof(ShowWordsCommand))]
        public async Task PlayRound_WhenPlaySpecialKeys_ShouldReturnSpecialCommands(
           string specialKey,
           Type expectedType)
        {
            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputWord())
                .Returns(specialKey);

            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var command = await useCase.PlayRound(It.IsAny<PlayRoundUseCaseRequest>());

            Assert.IsType(expectedType, command);
        }

        [Fact]
        public async Task PlayRound_WhenPlayEmptyWord_ShouldReturnInvalidWordCommand()
        {
            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputWord())
                .Returns(string.Empty);

            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var command = await useCase.PlayRound(It.IsAny<PlayRoundUseCaseRequest>());

            Assert.IsType<InvalidWordCommand>(command);
            Assert.Equal(Messages.EmptyWordLanguage, (command as InvalidWordCommand).Message);
        }

        [Theory]
        [InlineData("123", Messages.WrongWordLanguage)]
        [InlineData("ca", Messages.WrongWordDictionary)]
        [InlineData("xxx", Messages.WrongWordReel)]
        public async Task PlayRound_WhenPlayWrongWord_ShouldReturnInvalidWordCommand(
            string word,
            string message)
        {
            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputWord())
                .Returns(word);

            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var game = _fixture.CreateDefaultGame();
            var request = PlayRoundUseCaseRequest
                .Create(game, _fixture.Trie, ReelWordsFixture.DefaultPenaltyScore);

            var command = await useCase.PlayRound(request);

            Assert.IsType<InvalidWordCommand>(command);
            Assert.Equal(message, (command as InvalidWordCommand).Message);
        }

        [Fact]
        public async Task PlayRound_WhenPlayCorrectWord_ShouldReturnWordSubmittedCommand()
        {
            var expectedScore = 6;
            
            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            getLetterScoresService
                .Setup(serv => serv.Get())
                .ReturnsAsync(new Dictionary<char, int>() 
                {
                    ['c'] = 3, ['a'] = 1, ['t'] = 2
                });
            
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputWord())
                .Returns("cat");

            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var game = _fixture.CreateDefaultGame();
            var request = PlayRoundUseCaseRequest
                .Create(game, _fixture.Trie, ReelWordsFixture.DefaultPenaltyScore);

            var command = await useCase.PlayRound(request);

            Assert.IsType<WordSubmittedCommand>(command);
            Assert.Equal(expectedScore, (command as WordSubmittedCommand).Score);
        }

        [Fact]
        public async Task PlayRound_WhenUnexpectedException_ShouldReturnUnexpectedErrorCommand()
        {
            var expectedExteption = 
                new InvalidOperationException(nameof(PlayRound_WhenPlayCorrectWord_ShouldReturnWordSubmittedCommand));

            var getLetterScoresService = new Mock<IGetLetterScoresService>();
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService.Setup(serv => serv.InputWord()).Throws(expectedExteption);
            var useCase = new PlayRoundUseCase(
                getLetterScoresService.Object,
                gameUiService.Object);

            var game = _fixture.CreateDefaultGame();
            var request = PlayRoundUseCaseRequest
                .Create(game, _fixture.Trie, ReelWordsFixture.DefaultPenaltyScore);

            var command = await useCase.PlayRound(request);

            Assert.IsType<UnexpectedErrorCommand>(command);
            Assert.Equal(expectedExteption.Message, (command as UnexpectedErrorCommand).Error.Message);
        }
    }
}
