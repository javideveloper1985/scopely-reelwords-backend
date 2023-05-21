using ReelWords.Infrastructure.Services;
using ReelWords.Infrastructure.Tests.TestFixture;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GetWordsFileServiceTests : IClassFixture<InfrastructureFixture>
    {
        private readonly InfrastructureFixture _fixture;

        public GetWordsFileServiceTests(InfrastructureFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetAll_WhenPathExists_ShouldThrowException()
        {
            var maxWordSize = 5;
            var expectedWords = 16;

            var wordsService = new GetDictionaryFileService(_fixture.Configuration);

            var words = await wordsService.GetByWordSize(maxWordSize);

            Assert.Equal(expectedWords, words.Count);
        }
    }
}