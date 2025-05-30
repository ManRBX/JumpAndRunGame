using UnityEngine;

public class UIVisibilityToggler : MonoBehaviour
{
    [Header("Alle UI-Elemente, die ein-/ausblendbar sein sollen")]
    public GameObject[] uiElements;

    [Header("Taste zum Umschalten")]
    public KeyCode toggleKey = KeyCode.F1;

    private bool uiVisible = true;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            uiVisible = !uiVisible;

            foreach (GameObject ui in uiElements)
            {
                if (ui != null)
                    ui.SetActive(uiVisible);
            }

            Debug.Log("🔁 UI " + (uiVisible ? "eingeblendet" : "ausgeblendet"));
        }
    }
}
