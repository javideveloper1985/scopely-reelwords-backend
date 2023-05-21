using ReelWords.Domain.Entities;
using Scopely.Core.Result;
using System.Threading.Tasks;

namespace ReelWords.UseCases
{
    public interface ICreateGameUseCase
    {
        Task<Result<Game>> Execute(int wordSize, string userId);
    }
}