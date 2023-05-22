using Moq;
using ReelWords.Commands;
using ReelWords.Constants;
using ReelWords.Services;
using ReelWords.UseCases.Implementations;
using Xunit;

namespace ReelWordsTests.Main
{
    public class GetUserUseCaseTests 
    {
        [Fact]
        public void Execute_WhenUserInputIsExit_ShouldReturnExitGameCommand()
        {
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputUserName())
                .Returns(UserKeyWords.Exit);

            var useCase = new GetUserUseCase(gameUiService.Object);
            var command = useCase.Execute();

            Assert.IsType<ExitGameCommand>(command);
            Assert.False((command as ExitGameCommand).SaveGame);
        }

        [Fact]
        public void Execute_WhenUserInputIsHelp_ShouldReturnHelpCommand()
        {
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputUserName())
                .Returns(UserKeyWords.Help);

            var useCase = new GetUserUseCase(gameUiService.Object);
            var command = useCase.Execute();

            Assert.IsType<HelpCommand>(command);
        }

        [Fact]
        public void Execute_WhenUserInputIsWrong_ShouldReturnWrongInputCommand()
        {
            var gameUiService = new Mock<IReelWordsUserInterfaceService>();
            gameUiService
                .Setup(serv => serv.InputUserName())
                .Returns("*hello");

            var useCase = new GetUserUseCase(gameUiService.Object);
            var command = useCase.Execute();

            Assert.IsType<WrongNameCommand>(command);
        }
    }
}
