using System.Diagnostics;
using UnityEngine;

public class BackgroundRunner : MonoBehaviour
{
    [Range(1f, 20f)][SerializeField] private float _timeScale;

    private void Start()
    {
        Application.runInBackground = true;
        SetTimeScale();
        UnityEngine.Debug.Log($"Timescale {Time.timeScale}", this);
    }

    [Conditional("UNITY_EDITOR")]
    private void SetTimeScale()
    {
        Time.timeScale = _timeScale;
        UnityEngine.Debug.Log($"Set timescale with: {_timeScale}", this);
    }
}