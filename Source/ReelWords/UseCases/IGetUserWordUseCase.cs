using Scopely.Core.Result;
using System.Threading.Tasks;

namespace ReelWords.UseCases;

public interface IGetUserWordUseCase
{
    Task<Result<string>> Execute();
}