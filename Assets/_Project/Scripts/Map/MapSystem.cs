using Core.Util;
using Cysharp.Threading.Tasks;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Core.Map
{

    public class MapSystem : MonoBehaviour
    {
        [SerializeField] private GameObject _player;

        private Map _map;
        private TileInstance[,] _tiles;
        private ITilemapService _tilemapService;

        private void Start()
        {
            _tilemapService = ServiceLocatorUtilities.GetServiceAssert<ITilemapService>();
            InitializeGame().Forget();
        }

        private async UniTask InitializeGame()
        {
            var sw = new Stopwatch();
            sw.Start();
            var map = await _tilemapService.GenerateTilemap();
            sw.Stop();
            Debug.Log($"{sw.ElapsedMilliseconds} elapsed miliseconds to complete first map gen");

            var pt = map.Metadata.PointsOfInterest[1];
            _player.transform.position = new Vector2(pt.X, pt.Y);
        }
    }
}