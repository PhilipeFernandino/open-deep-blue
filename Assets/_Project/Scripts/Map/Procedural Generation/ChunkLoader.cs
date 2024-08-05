using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkLoader : MonoBehaviour
{
    [SerializeField]
    private int _chunkLoadDistance = 1;

    [SerializeField]
    private int _chunkSize = 8;

    private Dictionary<Vector2Int, Tilemap> _tilemaps = new Dictionary<Vector2Int, Tilemap>();
    private Transform _player;

    private void Start()
    {
        _player = FindObjectOfType<Transform>();

        foreach (var tilemap in gameObject.GetComponentsInChildren<Tilemap>())
        {
            Debug.Log(tilemap);
            // Os tilemaps vão se traduzir para a matriz usando as posiçÕes no mundo
            int i = (int)tilemap.transform.position.x / _chunkSize;
            int j = (int)tilemap.transform.position.y / _chunkSize;
            var ij = new Vector2Int(i, j);
            _tilemaps.Add(ij, tilemap);
            tilemap.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        LoadChunks(_player.transform.position);
    }

    private void LoadChunks(Vector2 playerPos)
    {
        var playerChunk = new Vector2Int((int)playerPos.x / _chunkSize, (int)playerPos.y / _chunkSize);
        if (_tilemaps.TryGetValue(playerChunk, out Tilemap tilemap))
        {
            tilemap.gameObject.SetActive(true);
        }
    }
}
