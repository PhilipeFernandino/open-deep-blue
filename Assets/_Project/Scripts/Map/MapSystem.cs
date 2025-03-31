using Core.Util;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core.Map
{

    public class MapSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _player;

        private ITilemapService _tilemapService;

        public event Action<Map> MapLoaded;

        private void Start()
        {
            _tilemapService = ServiceLocatorUtilities.GetServiceAssert<ITilemapService>();
            InitializeGame().Forget();
        }

        private async UniTask InitializeGame()
        {
            var sw = new Stopwatch();
            sw.Start();
            var map = await UniTask.RunOnThreadPool(() =>
            {
                return _tilemapService.GenerateTilemap();
            });
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete first map gen");

            var pt = map.Metadata.PointsOfInterest[1];
            Debug.Log($"Setting player at: {pt}");
            _player.transform.position = new Vector2(pt.X, pt.Y);

            MapLoaded.Invoke(map);
        }
    }
}