using ReelWords.Infrastructure.Services;
using ReelWords.Infrastructure.Tests.TestFixture;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GetLetterScoresFileServiceTests : IClassFixture<ReelWordsFixture>
    {
        private readonly ReelWordsFixture _fixture;

        public GetLetterScoresFileServiceTests(ReelWordsFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetAll_WhenPathExists_ShouldThrowException()
        {
            var letterScoresService = new GetLetterScoresFileService(_fixture.Configuration);

            var scores = await letterScoresService.Get();

            Assert.True(scores.ContainsKey('a'));
            Assert.Equal(1, scores['a']);
            Assert.True(scores.ContainsKey('c'));
            Assert.Equal(3, scores['c']);
        }
    }
}