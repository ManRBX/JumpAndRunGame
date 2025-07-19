using UnityEngine;
using System.Collections.Generic;

public class ForceCursorVisible : MonoBehaviour
{
    [Header("🔘 Maus immer sichtbar?")]
    public bool forceCursorVisible = true;

    [Header("🎛 Sichtbar bei diesen aktiven Panels")]
    public List<GameObject> visibleWhenThesePanelsAreActive;

    void Start()
    {
        ApplyCursorSettings();
    }

    void LateUpdate()
    {
        if (ShouldCursorBeVisible())
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private bool ShouldCursorBeVisible()
    {
        if (forceCursorVisible)
            return true;

        foreach (var panel in visibleWhenThesePanelsAreActive)
        {
            if (panel != null && panel.activeInHierarchy)
                return true;
        }

        return false;
    }

    private void ApplyCursorSettings()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
