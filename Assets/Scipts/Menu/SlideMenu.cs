using UnityEngine;
using DG.Tweening;

public class SlideMenu : MonoBehaviour
{
    public static SlideMenu Instance { get; private set; }

    public RectTransform menuPanel;
    private bool isMenuOpen = false;
    private float hiddenXPosition;
    private float shownXPosition;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optional based on your game needs
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        shownXPosition = menuPanel.anchoredPosition.x;
        hiddenXPosition = shownXPosition + menuPanel.rect.width;
        menuPanel.anchoredPosition = new Vector2(hiddenXPosition, menuPanel.anchoredPosition.y);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        menuPanel.DOAnchorPosX(isMenuOpen ? shownXPosition : hiddenXPosition, 0.5f).SetEase(Ease.InOutExpo);
    }

    public bool IsMenuOpen()
    {
        return isMenuOpen;
    }
}
