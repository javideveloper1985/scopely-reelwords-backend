using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ReelWords.Domain.Entities;
using ReelWords.Infrastructure.Mappers;
using ReelWords.Infrastructure.Repositories;

namespace ReelWords.Infrastructure.Tests.Services
{
    public class GameLocalFileRepositoryMock : GameLocalFileRepository
    {
        private readonly Game _game;

        public string CompletePath = "";
        public string Content = "";

        public GameLocalFileRepositoryMock(
            IConfiguration configuration, 
            Game game) : base(configuration) 
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        protected override bool WriteFile(string mainPath, string fileName, string content)
        {
            CompletePath = Path.Combine(mainPath, fileName);
            var dto = GameMapper.ToDto(_game);
            Content = JsonConvert.SerializeObject(dto);
            return true;
        }
    }
}