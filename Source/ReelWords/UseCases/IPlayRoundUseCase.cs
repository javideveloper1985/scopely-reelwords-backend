using System.Threading.Tasks;
using ReelWords.Commands.Implementations;
using ReelWords.UseCases.Implementations;

namespace ReelWords.UseCases;

public interface IPlayRoundUseCase
{
    Task<IUserGameCommand> PlayRound(PlayRoundUseCaseRequest request);
}
