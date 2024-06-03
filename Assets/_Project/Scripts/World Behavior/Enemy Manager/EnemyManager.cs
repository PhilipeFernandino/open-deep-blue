using Coimbra;
using Core.Player;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private int _outerRectDimension = 64;
    [SerializeField] private int _innerRectDimension = 16;

    [SerializeField] private GameObject _addedSRVisualizer;
    [SerializeField] private GameObject _skippedSRVisualizer;

    private Player _player;

    private List<GameObject> _spawnedEnemies;

    private List<GameObject> _destroy = new();

    private void Awake()
    {
        _player = FindObjectOfType<Player>();


        StartSpawnTask();
    }

    private async void StartSpawnTask()
    {
        for (int i = 0; i < _destroy.Count; i++)
        {
            _destroy[i].gameObject.Dispose(false);
        }
        _destroy.Clear();

        Spawn();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        StartSpawnTask();
    }

    private void Spawn()
    {
        Vector3Int position = Vector3Int.CeilToInt(_player.transform.position);

        int xOrigin = position.x - _outerRectDimension / 2;
        int yOrigin = position.y - _outerRectDimension / 2;

        BoundsInt bounds = new(
            xOrigin,
            yOrigin,
            0,
            _outerRectDimension,
            _outerRectDimension,
            1
            );

        TileBase[] tiles = _tilemap.GetTilesBlock(bounds);

        StringBuilder addingSb = new("Adding: \n");
        StringBuilder skippingSb = new("Ignoring: \n");

        for (int i = 0; i < _outerRectDimension; i++)
        {
            for (int j = 0; j < _outerRectDimension; j++)
            {
                int index = i * _outerRectDimension + j;

                Vector3 pos = new(
                    position.x - ((_outerRectDimension / 2) * 0.5f) + (i * 0.5f),
                    position.y - ((_outerRectDimension / 2) * 0.5f) + (j * 0.5f),
                    0
                    );

                if (
                    (i >= _innerRectDimension && j >= _innerRectDimension && i < _innerRectDimension * 2 && j < _innerRectDimension * 2)
                    || (tiles[index] != null))
                {
                    skippingSb.AppendLine($"{i}, {j}: {tiles[index]}");
                    var skippedGo = Instantiate(_skippedSRVisualizer, pos, Quaternion.identity);
                    _destroy.Add(skippedGo);
                    continue;
                }


                addingSb.AppendLine($"{i}, {j}: {tiles[index]}");
                var addedGo = Instantiate(_addedSRVisualizer, pos, Quaternion.identity);
                _destroy.Add(addedGo);
            }
        }

        Debug.Log(addingSb);
        Debug.Log(skippingSb);
    }
}
