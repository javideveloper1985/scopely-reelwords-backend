using ReelWords.Domain.Entities;
using Scopely.Core.Result;
using System.Threading.Tasks;

namespace ReelWords.UseCases;

public interface ILoadGameUseCase
{
    Task<Result<Game>> Execute(string userId);
}
