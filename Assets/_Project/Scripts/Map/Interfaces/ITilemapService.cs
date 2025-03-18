using Coimbra.Services;
using Cysharp.Threading.Tasks;
using System;

namespace Core.Map
{
    [DynamicService]
    public interface ITilemapService : IService
    {
        public async UniTask<Map> GenerateTilemap() { throw new NotImplementedException(); }
    }
}
