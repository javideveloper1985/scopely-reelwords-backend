using Microsoft.Extensions.Configuration;
using Moq;
using ReelWords.Commands;
using ReelWords.Constants;
using ReelWords.Domain.Entities;
using ReelWords.Domain.Services;
using ReelWords.Domain.ValueObjects;
using ReelWords.UseCases;
using Scopely.Core.Result;
using Scopely.Core.Structures;
using System.Collections.Generic;

namespace ReelWordsTests.TestFixture
{
    public class ReelWordsFixture
    {
        public static Dictionary<char, int> DefaultScores = new()
        {
            ['c'] = 3,
            ['a'] = 1,
            ['t'] = 2
        };

        public static List<string> DefaultWordList = new()
        {
            "pet",
            "hat",
            "can",
            "cat"
        };

        public readonly Trie Trie;

        public static char[] Reel1 = new char[] { 'x', 'y', 'z' };
        public static char[] Reel2 = new char[] { 't', 'c', 'a' };

        public const int DefaultWordSize = 3;
        public const int DefaultPenaltyScore = 2;
        public const string DefaultUserId = "Peter";

        public IConfiguration Configuration { get; }

        public ReelWordsFixture()
        {
            //Configuration
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(x => x[ConfigKeys.WordSize]).Returns(DefaultWordSize.ToString());
            configMock.Setup(x => x[ConfigKeys.ShufflePenalty]).Returns(DefaultPenaltyScore.ToString());
            Configuration = configMock.Object;

            Trie = Trie.CreateFromListOfWords(DefaultWordList);
        }

        public Mock<IGetDictionaryService> CreateDictionaryMock(int? wordSize = null, List<string> words = null)
        {
            var size = wordSize ?? DefaultWordSize;
            var mock = new Mock<IGetDictionaryService>();
            mock
                .Setup(serv => serv.GetByWordSize(
                    It.Is<int>(x => x == size)))
                .ReturnsAsync(words ?? DefaultWordList);
            return mock;
        }

        public Mock<IGetLetterScoresService> CreateScoresMock(Dictionary<char, int> scores = null)
        {
            var mock = new Mock<IGetLetterScoresService>();
            mock
                .Setup(serv => serv.Get())
                .ReturnsAsync(scores ?? DefaultScores);
            return mock;
        }

        public Mock<IGetUserUseCase> CreateGetUserMock(string userId = null)
        {
            var user = userId ?? DefaultUserId.ToLower();
            var mock = new Mock<IGetUserUseCase>();
            mock
                .Setup(uc => uc.Execute())
                .Returns(new UserNameCommand(user));
            return mock;
        }

        public Mock<ILoadGameUseCase> CreateLoadNullGameMock(string userId = null)
        {
            var user = userId ?? DefaultUserId.ToLower();
            var mock = new Mock<ILoadGameUseCase>();
            mock
                .Setup(uc => uc.Execute(
                    It.Is<string>(x => x == user)))
                .ReturnsAsync(Result<Game>.Ok(null));
            return mock;
        }

        public Mock<ICreateGameUseCase> CreateGameUseCaseMock(Game game, int? wordSize = null, string userId = null)
        {
            var size = wordSize ?? DefaultWordSize;
            var user = userId ?? DefaultUserId;
            var mock = new Mock<ICreateGameUseCase>();
            mock
                .Setup(uc => uc.Execute(
                    It.Is<int>(x => x == size),
                    It.Is<string>(x => x == user.ToLower())))
                .ReturnsAsync(Result<Game>.Ok(game));
            return mock;
        }

        public Mock<ISaveGameUseCase> CreateSaveOkMock(Game game)
        {
            var mock = new Mock<ISaveGameUseCase>();
            mock
                .Setup(uc => uc.Execute(
                    It.Is<Game>(x => x == game)))
                .ReturnsAsync(Result<string>.Ok(game.Id));
            return mock;
        }

        public Game CreateDefaultGame(string userId = null)
        {
            return Game.CreateNew(userId ?? DefaultUserId, CreateDefaultReelPanel());
        }

        public ReelPanel CreateDefaultReelPanel()
        {
            var panel = ReelPanel.CreateEmpty(2, 3);
            panel.AddReel(0, Reel1);
            panel.AddReel(1, Reel2);
            return panel;
        }
    }
}