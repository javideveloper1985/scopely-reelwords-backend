using ReelWords.Domain.Entities;
using Scopely.Core.Result;
using System.Threading.Tasks;

namespace ReelWords.UseCases;

public interface IGetCurrentUserUseCase
{
    Task<Result<User>> Execute();
}