using ReelWords.Domain.ValueObjects;

namespace ReelWords.Domain.Services;

public interface ICreateReelPanelService
{
    Task<ReelPanel> Create(int wordSize);
}