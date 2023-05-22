using Microsoft.Extensions.Logging;
using Moq;
using ReelWords.Commands;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Main;
using ReelWords.Services;
using ReelWords.UseCases;
using ReelWords.UseCases.Requests;
using ReelWordsTests.TestFixture;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ReelWordsTests.Main
{
    public class ReelWordsGameManagerTests : IClassFixture<ReelWordsFixture>
    {
        private readonly ReelWordsFixture _fixture;

        public ReelWordsGameManagerTests(ReelWordsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task Start_WhenCompleteGamePlayingOneRound_ShouldChangeScore()
        {
            var expectedPoints = 6;
            var expecteUserId = "martha";

            var game = _fixture.CreateDefaultGame(expecteUserId);

            var playRoundMock = new Mock<IPlayRoundUseCase>();
            playRoundMock
                .SetupSequence(serv => serv.Execute(It.IsAny<PlayRoundGameContext>()))
                .ReturnsAsync(new InvalidWordCommand(Messages.WrongWordLanguage))
                .ReturnsAsync(new InvalidWordCommand(Messages.WrongWordReel))
                .ReturnsAsync(new InvalidWordCommand(Messages.WrongWordDictionary))
                .ReturnsAsync(new WordSubmittedCommand("cat", expectedPoints))
                .ReturnsAsync(new ExitGameCommand(true)); //Exit with save
            var gameUiMock = new Mock<IReelWordsUserInterfaceService>();
            gameUiMock
                .Setup(serv => serv.InputUserName())
                .Returns(expecteUserId);
            var logMock = new Mock<ILogger<ReelWordsGameManager>>();
            var saveMock = _fixture.CreateSaveOkMock(game);

            var reelWordsGameManager = new ReelWordsGameManager(
                _fixture.CreateGetUserMock(expecteUserId).Object,
                _fixture.CreateLoadNullGameMock(expecteUserId).Object,
                _fixture.CreateGameUseCaseMock(game, null, expecteUserId).Object,
                playRoundMock.Object,
                saveMock.Object,
                _fixture.CreateScoresMock().Object,
                _fixture.CreateDictionaryMock().Object,
                gameUiMock.Object,
                _fixture.Configuration,
                logMock.Object);

            await reelWordsGameManager.Start();

            //Asserts
            gameUiMock
                .Verify(serv => serv.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordLanguage)),
                    Times.Once);
            gameUiMock
                .Verify(uc => uc.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordReel)),
                    Times.Once);
            gameUiMock
                .Verify(uc => uc.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordDictionary)),
                    Times.Once);
            saveMock
                .Verify(uc => uc.Execute(It.Is<Game>(x => x == game && x.UserId == expecteUserId)),
                Times.Once);

            Assert.Equal(expectedPoints, game.Score);
        }

        [Fact]
        public async Task Start_WhenUserDecidesExitsAtTheBegining_ShouldExitApplicationWithoutAskToSave()
        {
            var getUserMock = new Mock<IGetUserUseCase>();
            getUserMock
                .Setup(uc => uc.Execute())
                .Returns(new ExitGameCommand(false));

            var gameUiMock = new Mock<IReelWordsUserInterfaceService>();
            var loadGameMock = new Mock<ILoadGameUseCase>();
            var createGameMock = new Mock<ICreateGameUseCase>();
            var saveGameMock = new Mock<ISaveGameUseCase>();
            var playRoundMock = new Mock<IPlayRoundUseCase>();
            var scoresMock = _fixture.CreateScoresMock();
            var dictionaryMock = _fixture.CreateDictionaryMock();
            var logMock = new Mock<ILogger<ReelWordsGameManager>>();

            var reelWordsGameManager = new ReelWordsGameManager(
                getUserMock.Object,
                loadGameMock.Object,
                createGameMock.Object,
                playRoundMock.Object,
                saveGameMock.Object,
                scoresMock.Object,
                dictionaryMock.Object,
                gameUiMock.Object,
                _fixture.Configuration,
                logMock.Object);

            await reelWordsGameManager.Start();

            //Asserts
            scoresMock
                .Verify(serv => serv.Get(),
                Times.Once);
            dictionaryMock
                .Verify(serv => serv.GetByWordSize(
                    It.Is<int>(x => x == ReelWordsFixture.DefaultWordSize)),
                Times.Once);
            loadGameMock.VerifyNoOtherCalls();
            createGameMock.VerifyNoOtherCalls();
            saveGameMock.VerifyNoOtherCalls();
            gameUiMock
                .Verify(uc => uc.ShowGoodbye(),
                    Times.Once);
            gameUiMock
                .Verify(uc => uc.CheckIfUserWantsSaveGame(),
                    Times.Never);
            saveGameMock
                .Verify(uc => uc.Execute(It.IsAny<Game>()),
                Times.Never);
        }
    }
}
