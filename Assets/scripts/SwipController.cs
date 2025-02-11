using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] int maxPage;  // Maximum number of pages
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;  // Distance between pages
    [SerializeField] RectTransform levelPageRect;

    [SerializeField] float tweenTime;  // Animation duration
    [SerializeField] LeanTweenType tweenType;  // Animation type (Ease-In, Out, etc.)

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
            targetPos += pageStep;  // Move to the next page
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;  // Move to the previous page
            MovePage();
        }
    }

    void MovePage()
    {
        levelPageRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    }
}
