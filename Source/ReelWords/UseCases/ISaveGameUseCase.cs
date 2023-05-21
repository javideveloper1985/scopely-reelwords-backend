using ReelWords.Domain.Entities;
using Scopely.Core.Result;
using System.Threading.Tasks;

namespace ReelWords.UseCases
{
    public interface ISaveGameUseCase
    {
        Task<Result<string>> Execute(Game game);
    }
}