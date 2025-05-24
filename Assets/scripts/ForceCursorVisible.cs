using UnityEngine;

public class ForceCursorVisible : MonoBehaviour
{
    [Header("Immer sichtbar und nicht gelockt")]
    public bool forceCursorVisible = true;

    void Start()
    {
        ApplyCursorSettings();
    }

    void Update()
    {
        if (forceCursorVisible && (Cursor.lockState != CursorLockMode.None || !Cursor.visible))
        {
            ApplyCursorSettings();
        }
    }

    void ApplyCursorSettings()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
