using Coimbra.Services;
using Cysharp.Threading.Tasks;

namespace Core.Map
{
    [DynamicService]
    public interface IMapLevelGeneratorService : IService
    {
        UniTask<MapMetadata> GenerateMapLevel();
    }
}