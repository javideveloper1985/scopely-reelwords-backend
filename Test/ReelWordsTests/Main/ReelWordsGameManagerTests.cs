using Moq;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Infrastructure.Tests.TestFixture;
using ReelWords.Main;
using ReelWords.Services;
using ReelWords.UseCases;
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
        public async Task Start_WhenCompleteGameWithRounds_ShouldChangeScore()
        {
            var expectedPoints = 6;
            var expecteUserId = "martha";

            var gameUiMock = new Mock<IReelWordsUserInterfaceService>();
            gameUiMock
                .Setup(serv => serv.InputUserName())
                .Returns(expecteUserId);
            gameUiMock
                .SetupSequence(serv => serv.InputWord())
                .Returns("123") //Invalid chars
                .Returns("ppp") //Invalid word
                .Returns("ca") //Invalid word
                .Returns("cat") //Valid word
                .Returns(UserKeyWords.Exit); //Exit
            gameUiMock
                .Setup(serv => serv.CheckIfUserWantsSaveGame())
                .Returns(true);

            var game = _fixture.CreateDefaultGame(expecteUserId);
            var saveMock = _fixture.CreateSaveOkMock(game);
            var reelWordsGameManager = new ReelWordsGameManager(
                _fixture.CreateLoadNullGameMock(expecteUserId).Object,
                _fixture.CreateGameUseCaseMock(game, null, expecteUserId).Object,
                saveMock.Object,
                _fixture.CreateScoresMock().Object,
                _fixture.CreateDictionaryMock().Object,
                gameUiMock.Object,
                _fixture.Configuration);

            await reelWordsGameManager.Start();

            gameUiMock
                .Verify(serv => serv.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordReel)),
                    Times.Once);

            gameUiMock
                .Verify(uc => uc.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordLanguage)),
                    Times.Once);

            gameUiMock
                .Verify(uc => uc.ShowWrongInput(
                    It.Is<string>(x => x == Messages.WrongWordDictionary)),
                    Times.Once);

            gameUiMock
                .Verify(uc => uc.CheckIfUserWantsSaveGame(),
                    Times.Once);

            saveMock
                .Verify(uc => uc.Execute(It.Is<Game>(x => x == game && x.UserId == expecteUserId)),
                Times.Once);

            Assert.Equal(expectedPoints, game.Score);
        }

        [Fact]
        public async Task Start_WhenUserDecidesExitsAtTheBegining_ShouldExitApplicationWithoutAskToSave()
        {
            var gameUiMock = new Mock<IReelWordsUserInterfaceService>();
            var uiMock = new Mock<IConsoleUserInterfaceService>();
            uiMock
                .Setup(serv => serv.GetInput(
                    It.Is<string>(x => x == Messages.EnterUserName)))
                .Returns(UserKeyWords.Exit);

            var loadGameMock = new Mock<ILoadGameUseCase>();
            var createGameMock = new Mock<ICreateGameUseCase>();
            var saveGameMock = new Mock<ISaveGameUseCase>();
            var scoresMock = _fixture.CreateScoresMock();
            var dictionaryMock = _fixture.CreateDictionaryMock();

            var reelWordsGameManager = new ReelWordsGameManager(
                loadGameMock.Object,
                createGameMock.Object,
                saveGameMock.Object,
                scoresMock.Object,
                dictionaryMock.Object,
                gameUiMock.Object,
                _fixture.Configuration);

            await reelWordsGameManager.Start();

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

            uiMock
                .Verify(uc => uc.ShowMessage(
                    It.Is<string>(x => x.Contains(Messages.Thanks)),
                    It.IsAny<ConsoleColor>()),
                    Times.Once);

            uiMock
                .Verify(uc => uc.AskForInputOption(
                    It.Is<string>(x => x == Messages.AskSave),
                    It.IsAny<string[]>()),
                    Times.Never);

            saveGameMock
                .Verify(uc => uc.Execute(It.IsAny<Game>()),
                Times.Never);
        }
    }
}
