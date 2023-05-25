using ReelWords.Domain.Entities;

namespace ReelWords.Domain.Services;

public interface ICreateReelPanelService
{
    Task<ReelPanel> Create(int wordSize);
}