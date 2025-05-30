using UnityEngine;

public class AchievementTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        LevelComplete,
        LevelUnlock,
        SecretFound
    }

    public TriggerType triggerType;
    public string targetID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                break;

            case TriggerType.LevelUnlock:
                AchievementProgressTracker.Instance?.OnLevelUnlocked(targetID);
                break;

            case TriggerType.SecretFound:
                AchievementProgressTracker.Instance?.OnSecretFound(targetID);
                break;
        }
    }
}
