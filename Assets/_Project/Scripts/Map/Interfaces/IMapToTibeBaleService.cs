using Coimbra.Services;
using Cysharp.Threading.Tasks;
using System;

namespace Core.Map
{
    [DynamicService]
    public interface IMapToTibeBaleService : IService
    {
        public async UniTask<Map> GenerateTilemap() { throw new NotImplementedException(); }
    }
}
