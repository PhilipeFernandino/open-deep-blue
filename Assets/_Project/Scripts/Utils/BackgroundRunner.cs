using UnityEngine;

public class BackgroundRunner : MonoBehaviour
{
    private void Start()
    {
        Application.runInBackground = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
}