using UnityEngine;

public class SwipController : MonoBehaviour
{
    [SerializeField] int maxPage;  // Maximale Anzahl an Seiten
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;  // Abstand zwischen den Seiten
    [SerializeField] RectTransform levelPageRect;

    [SerializeField] float tweenTime;  // Animationsdauer
    [SerializeField] LeanTweenType tweenType;  // Animations-Typ (Ease-In, Out usw.)

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPageRect.localPosition;
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;  // Nächste Seite (vorwärts)
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;  // Vorherige Seite (rückwärts)
            MovePage();
        }
    }

    void MovePage()
    {
        levelPageRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    }
}
