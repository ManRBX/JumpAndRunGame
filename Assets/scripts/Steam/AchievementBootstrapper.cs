using UnityEngine;

public class AchievementBootstrapper : MonoBehaviour
{
    public GameObject trackerPrefab;

    private void Awake()
    {
        if (AchievementProgressTracker.Instance == null)
        {
            GameObject obj = Instantiate(trackerPrefab);
            DontDestroyOnLoad(obj);
        }
    }
}
