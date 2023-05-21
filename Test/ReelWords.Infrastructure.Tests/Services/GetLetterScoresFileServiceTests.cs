using ReelWords.Infrastructure.Services;
using ReelWords.Infrastructure.Tests.TestFixture;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GetLetterScoresFileServiceTests : IClassFixture<InfrastructureFixture>
    {
        private readonly InfrastructureFixture _fixture;

        public GetLetterScoresFileServiceTests(InfrastructureFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        [Fact]
        public async Task GetAll_WhenPathExists_ShouldThrowException()
        {
            var letterScoresService = new GetLetterScoresFileService(_fixture.Configuration);

            var scores = await letterScoresService.GetAll();

            Assert.True(scores.ContainsKey('a'));
            Assert.Equal(1, scores['a'].Score);
            Assert.True(scores.ContainsKey('c'));
            Assert.Equal(3, scores['c'].Score);
        }
    }
}