using Coimbra.Services;
using Cysharp.Threading.Tasks;
using System.Collections;

namespace Core.Map
{
    [DynamicService]
    public interface IMapLevelGeneratorService : IService
    {
        UniTask<Map> GenerateMapLevel();
    }
}