using UnityEngine;

public class AchievementTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        LevelComplete,
        LevelUnlock,
        SecretFound,
        ObjectFound,
        BonusLevelFound,
    }

    public TriggerType triggerType;

    [Tooltip("z. B. Level01")]
    public string targetID;

    [Tooltip("Nur bei LevelComplete: benötigte Special Coins")]
    public int requiredSpecialCoins = 3;

    [Tooltip("Nur einmal auslösen?")]
    public bool triggerOnce = true;

    private bool fired;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (triggerOnce && fired) return;

        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                {
                    string coinKey = targetID + "-special-coins";
                    int collectedCoins = PlayerPrefs.GetInt(coinKey, 0);
                    if (collectedCoins >= requiredSpecialCoins)
                    {
                        Debug.Log($"✅ LevelComplete für {targetID} mit {collectedCoins}/{requiredSpecialCoins} Coins.");
                        AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                        fired = true;
                    }
                    else
                    {
                        Debug.Log($"❌ Nicht genug Special Coins ({collectedCoins}/{requiredSpecialCoins}) für {targetID}.");
                    }
                }
                break;

            case TriggerType.LevelUnlock:
                AchievementProgressTracker.Instance?.OnLevelUnlocked(targetID);
                Debug.Log($"✅ LevelUnlock für {targetID}.");
                fired = true;
                break;

            case TriggerType.SecretFound:
                AchievementProgressTracker.Instance?.OnSecretFound(targetID);
                Debug.Log($"✅ Secret Found für {targetID}.");
                fired = true;
                break;

            case TriggerType.ObjectFound:
                AchievementProgressTracker.Instance?.OnObjectFound(targetID);
                Debug.Log($"✅ Object Found für {targetID}.");
                fired = true;
                break;

            case TriggerType.BonusLevelFound:
                AchievementProgressTracker.Instance?.OnBonusLevelFound(targetID);
                Debug.Log($"✅ Bonus Level Found für {targetID}.");
                fired = true;
                break;
        }
    }
}
