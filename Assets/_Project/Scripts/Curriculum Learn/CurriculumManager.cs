using Core.Level;
using Core.Map;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CurriculumManager : MonoBehaviour
{
    [SerializeField] private List<TilemapAsset> _tilemaps;

    private IGridService _gridService;

    private int _currentLessonIndex = -1;

    private List<Vector2Int> _antSpawnPoints = new();
    private List<Vector2Int> _foodSpawnPoints = new();

    void Awake()
    {
        Academy.Instance.OnEnvironmentReset += OnEnvironmentReset;
    }

    void OnDestroy()
    {
        if (Academy.IsInitialized)
        {
            Academy.Instance.OnEnvironmentReset -= OnEnvironmentReset;
        }
    }

    private void OnEnvironmentReset()
    {
        int newLessonIndex = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("lesson_index", 0.0f);

        if (newLessonIndex == _currentLessonIndex)
        {
            return;
        }

        _currentLessonIndex = newLessonIndex;

        switch (_currentLessonIndex)
        {
            case 0:
                SetupForagingLesson();
                break;
            case 1:
                //SetupFungusLesson();
                break;
            case 2:
                //SetupQueenLesson();
                break;
            default:
                //SetupForagingLesson();
                break;
        }
    }

    private void SetupForagingLesson()
    {
        _gridService.ListPositions(Tile.GreenGrass, _foodSpawnPoints);
        _gridService.ListPositions(Tile.AntQueenSpawn, _antSpawnPoints);


    }
}