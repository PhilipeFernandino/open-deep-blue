using Coimbra;
using Coimbra.Services;
using Core.Util;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core.Map
{

    public class MapSystem : Actor, IMapService
    {
        private IMapLevelGeneratorService _mapLevelGeneratorService;

        protected override void OnSpawn()
        {
            _mapLevelGeneratorService = ServiceLocatorUtilities.GetServiceAssert<IMapLevelGeneratorService>();
            InitializeGame().Forget();
        }

        private async UniTask InitializeGame()
        {
            var mapMetadata = await _mapLevelGeneratorService.GenerateMapLevel();
            Debug.Log(mapMetadata.ToString());
            new MapMetadataGeneratedEvent(mapMetadata).Invoke(this);
        }
    }

    public interface IMapService : IService
    {
    }
}