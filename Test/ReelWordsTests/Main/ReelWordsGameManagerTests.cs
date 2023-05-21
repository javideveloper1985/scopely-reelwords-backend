using Moq;
using ReelWords.Domain.Services;
using ReelWords.Main;
using ReelWords.Services;
using ReelWords.UseCases;
using Xunit;

namespace ReelWordsTests.Main
{
    public class ReelWordsGameManagerTests
    {
        public ReelWordsGameManagerTests()
        {
            
        }

        [Fact]
        public void Start_WhenPlayCompleteGame_ShouldFinishWithScore()
        {
            var getUserMock = new Mock<IGetCurrentUserUseCase>();
            var loadGameMock = new Mock<ILoadGameUseCase>();
            var createGameMock = new Mock<ICreateGameUseCase>();
            var getUserWordMock = new Mock<IGetUserWordUseCase>();
            var saveGameMock = new Mock<ISaveGameUseCase>();
            var getLetterScoresMock = new Mock<IGetLetterScoresService>();
            var getDictionaryMock = new Mock<IGetDictionaryService>();
            var uiMock = new Mock<IConsoleUserInterfaceService>();

            //var reelWordsGameManager = new ReelWordsGameManager();
        }
    }
}
