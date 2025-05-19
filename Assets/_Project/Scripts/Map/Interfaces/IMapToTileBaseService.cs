using Coimbra.Services;
using Cysharp.Threading.Tasks;
using System;

namespace Core.Map
{
    [DynamicService]
    public interface IMapToTileBaseService : IService
    {
        public async UniTask<Map> GenerateTilemap(MapMetadata map) { throw new NotImplementedException(); }
    }
}
