using Microsoft.Extensions.Options;
using Moq;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;
using ReelWords.Infrastructure.Tests.TestFixture;
using ReelWords.Main;
using ReelWords.Services;
using ReelWords.UseCases;
using Scopely.Core.Result;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReelWordsTests.Main
{
    public class ReelWordsGameManagerTests : IClassFixture<ReelWordsFixture>
    {
        private readonly ReelWordsFixture _fixture;

        public ReelWordsGameManagerTests(ReelWordsFixture fixture)
        {
            _fixture = fixture ?? throw new System.ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task Start_WhenCompleteGameWithRounds_ShouldChangeScore()
        {
            var expectedPoints = 6;
            var userId = "peter";
            var wordSize = int.Parse(_fixture.Configuration[ConfigKeys.WordSize]);
            var penalty = int.Parse(_fixture.Configuration[ConfigKeys.ShufflePenalty]);
            var wordList = new List<string>() { "cat", "hat", "can" };
            var scores = new Dictionary<char, int>() { ['c'] = 3, ['a'] = 1, ['t'] = 2 };
            var reelPanel = ReelPanel.CreateEmpty(2, 3);
            reelPanel.AddReel(0, new char[] { 'x', 'y', 'z' });
            reelPanel.AddReel(1, new char[] { 't', 'c', 'a' });
            var game = Game.CreateNew(userId, reelPanel);

            var getLetterScoresMock = new Mock<IGetLetterScoresService>();
            getLetterScoresMock
                .Setup(serv => serv.Get())
                .ReturnsAsync(scores);

            var getDictionaryMock = new Mock<IGetDictionaryService>();
            getDictionaryMock
                .Setup(serv => serv.GetByWordSize(
                    It.Is<int>(x => x == wordSize)))
                .ReturnsAsync(wordList);

            var uiMock = new Mock<IConsoleUserInterfaceService>();
            uiMock
                .Setup(serv => serv.GetInput(
                    It.Is<string>(x => x == Messages.EnterUser)))
                .Returns(userId);
            uiMock
                .SetupSequence(serv => serv.GetInput(
                    It.Is<string>(x => x == Messages.EnterWord)))
                .Returns("123") //Invalid chars
                .Returns("ppp") //Invalid word
                .Returns("ca") //Invalid word
                .Returns("cat") //Valid word
                .Returns(UserKeyWords.Exit); //Exit
            uiMock
                .Setup(serv => serv.AskForInputOption(
                    It.Is<string>(x => x == Messages.AskSave),
                    It.IsAny<string[]>()))
                .Returns("y");

            var loadGameMock = new Mock<ILoadGameUseCase>();
            loadGameMock
                .Setup(uc => uc.Execute(
                    It.Is<string>(x => x == userId)))
                .ReturnsAsync(Result<Game>.Ok(null));

            var createGameMock = new Mock<ICreateGameUseCase>();
            createGameMock
                .Setup(uc => uc.Execute(
                    It.Is<int>(x => x == wordSize),
                    It.Is<string>(x => x == userId)))
                .ReturnsAsync(Result<Game>.Ok(game));

            var saveGameMock = new Mock<ISaveGameUseCase>();
            saveGameMock
                .Setup(uc => uc.Execute(game))
                .ReturnsAsync(Result<string>.Ok(game.Id));

            var reelWordsGameManager = new ReelWordsGameManager(
                loadGameMock.Object,
                createGameMock.Object,
                saveGameMock.Object,
                getLetterScoresMock.Object,
                getDictionaryMock.Object,
                uiMock.Object,
                _fixture.Configuration);

            await reelWordsGameManager.Start();

            uiMock
                .Verify(serv => serv.ShowError(
                    It.Is<string>(x => x == Messages.WrongWordReel)),
                    Times.Once);

            uiMock
                .Verify(uc => uc.ShowError(
                    It.Is<string>(x => x == Messages.WrongWordLanguage)),
                    Times.Once);

            uiMock
                .Verify(uc => uc.ShowError(
                    It.Is<string>(x => x == Messages.WrongWordDictionary)),
                    Times.Once);

            uiMock
                .Verify(uc => uc.AskForInputOption(
                    It.Is<string>(x => x == Messages.AskSave),
                    It.IsAny<string[]>()),
                    Times.Once);

            saveGameMock
                .Verify(uc => uc.Execute(It.Is<Game>(x => x == game)),
                Times.Once);

            Assert.Equal(expectedPoints, game.Score);
        }
    }
}
