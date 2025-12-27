using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject statUI;
    public GameObject costumeUI;
    public GameObject cashUI;

    [Header("Buttons")]
    public Button statButton;
    public Button costumeButton;
    public Button cashButton;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        statButton.onClick.AddListener(() => ShowUI(statUI));
        costumeButton.onClick.AddListener(() => ShowUI(costumeUI));
        cashButton.onClick.AddListener(() => ShowUI(cashUI));
    }

    void ShowUI(GameObject uiToShow)
    {
        // 모든 UI 비활성화
        statUI.SetActive(false);
        costumeUI.SetActive(false);
        cashUI.SetActive(false);

        // 선택한 UI 활성화
        uiToShow.SetActive(true);
    }
}
