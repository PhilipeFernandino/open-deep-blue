using Coimbra.Services;
using System.Collections;

namespace Core.Map
{
    public interface IMapLevelGeneratorService : IService
    {
        Tile[,] GenerateMapLevel();
    }
}