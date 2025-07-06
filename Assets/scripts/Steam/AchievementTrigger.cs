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

#if UNITY_EDITOR
    [SerializeField, Tooltip("Nur bei LevelComplete: Wieviele Special Coins benötigt werden.")]
    private int requiredSpecialCoins = 3;
#else
    public int requiredSpecialCoins = 3;
#endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType)
        {
            case TriggerType.LevelComplete:
                string coinKey = targetID + "-special-coins";
                int collectedCoins = PlayerPrefs.GetInt(coinKey, 0);

                if (collectedCoins >= requiredSpecialCoins)
                {
                    Debug.Log($"✅ LevelComplete Achievement ausgelöst für {targetID} mit {collectedCoins} Special Coins.");
                    AchievementProgressTracker.Instance?.OnLevelCompleted(targetID);
                }
                else
                {
                    Debug.Log($"❌ Nicht genug Special Coins ({collectedCoins}/{requiredSpecialCoins}) für {targetID}, kein Achievement.");
                }
                break;

            case TriggerType.LevelUnlock:
                AchievementProgressTracker.Instance?.OnLevelUnlocked(targetID);
                Debug.Log($"✅ LevelUnlock Achievement ausgelöst für {targetID}.");
                break;

            case TriggerType.SecretFound:
                AchievementProgressTracker.Instance?.OnSecretFound(targetID);
                Debug.Log($"✅ Secret Found Achievement ausgelöst für {targetID}.");
                break;

            case TriggerType.ObjectFound:
                AchievementProgressTracker.Instance?.OnObjectFound(targetID);
                Debug.Log($"✅ Object Found Achievement ausgelöst für {targetID}.");
                break;

            case TriggerType.BonusLevelFound:
                AchievementProgressTracker.Instance?.OnBonusLevelFound(targetID);
                Debug.Log($"✅ Bonus Level Achievement ausgelöst für {targetID}.");
                break;
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Nur bei LevelComplete soll requiredSpecialCoins sichtbar sein
        UnityEditor.SerializedObject so = new UnityEditor.SerializedObject(this);
        var prop = so.FindProperty("requiredSpecialCoins");
        prop.isExpanded = triggerType == TriggerType.LevelComplete;
    }
#endif
}
