// ForceCursorVisible.cs
using UnityEngine;
/// <summary>
/// Macht den Mauszeiger jederzeit sichtbar und entsperrt ihn.
/// </summary>
public class ForceCursorVisible : MonoBehaviour
{
    [Header("Immer sichtbar und nicht gelockt")]
    public bool forceCursorVisible = true;

    void Start()
    {
        ApplyCursorSettings();
        DontDestroyOnLoad(gameObject); // Ensure this object persists across scene loads
    }

    void Update()
    {
        if (forceCursorVisible && (Cursor.lockState != CursorLockMode.None || !Cursor.visible))
            ApplyCursorSettings();
    }

    void ApplyCursorSettings()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
