using Microsoft.Extensions.Configuration;
using Moq;
using ReelWords.Constants;

namespace ReelWords.Infrastructure.Tests.TestFixture
{
    public class ReelWordsFixture
    {
        public IConfiguration Configuration { get; init; }

        public ReelWordsFixture()
        {
            var configMock = new Mock<IConfiguration>();

            configMock.Setup(x => x[ConfigKeys.WordSize]).Returns("4");
            configMock.Setup(x => x[ConfigKeys.ShufflePenalty]).Returns("2");

            Configuration = configMock.Object;
        }
    }
}