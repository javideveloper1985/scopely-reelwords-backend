using ReelWords.Commands.Implementations;

namespace ReelWords.UseCases;

public interface IGetUserUseCase
{
    IUserGameCommand Execute();
}
