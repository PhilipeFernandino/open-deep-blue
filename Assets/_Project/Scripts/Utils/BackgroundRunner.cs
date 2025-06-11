using UnityEngine;

public class BackgroundRunner : MonoBehaviour
{
    [Range(1f, 20f)][SerializeField] private float _timeScale;

    private void Start()
    {
        Application.runInBackground = true;
        Time.timeScale = _timeScale;
    }
}