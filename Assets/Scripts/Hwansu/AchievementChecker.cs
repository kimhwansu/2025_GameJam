using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AchievementChecker : Singleton<AchievementChecker>
{
    [Header("StatBar References")]
    [SerializeField] private List<StatBar> statBars = new List<StatBar>(); // 여러 StatBar를 추가 가능

    [Header("Ending UI References")]
    [SerializeField] private GameObject endingCanvas; // 엔딩 전용 Canvas (있다면)
    [SerializeField] private GameObject successEndingImage; // 성공 엔딩 이미지 (18개 달성)
    [SerializeField] private GameObject failEndingImage;    // 실패 엔딩 이미지 (18개 미달)

    [Header("Ending Buttons - Fail")]
    [SerializeField] private Button failReturnToTitleButton; // 실패: 타이틀로 돌아가기 버튼
    [SerializeField] private Button failQuitGameButton;       // 실패: 게임 종료 버튼

    [Header("Ending Buttons - Success")]
    [SerializeField] private Button successQuitGameButton;       // 성공: 게임 종료 버튼

    [Header("Scene Settings")]
    [SerializeField] private string titleSceneName = "Title"; // 타이틀 씬 이름

    [Header("Settings")]
    [SerializeField] private int targetCount = 18; // 목표 카운트

    private bool isChecked = false; // 한 번만 체크하도록

    void Start()
    {
        // 실패 엔딩 버튼 이벤트 등록
        if (failReturnToTitleButton != null)
        {
            failReturnToTitleButton.onClick.AddListener(OnReturnToTitleClicked);
            Debug.Log("실패: 타이틀로 돌아가기 버튼 이벤트 등록");
        }

        if (failQuitGameButton != null)
        {
            failQuitGameButton.onClick.AddListener(OnQuitGameClicked);
            Debug.Log("실패: 게임 종료 버튼 이벤트 등록");
        }

        if (successQuitGameButton != null)
        {
            successQuitGameButton.onClick.AddListener(OnQuitGameClicked);
            Debug.Log("성공: 게임 종료 버튼 이벤트 등록");
        }

        // UI 초기 상태 설정
        if (endingCanvas != null)
        {
            endingCanvas.SetActive(false);
            Debug.Log("AchievementChecker: 엔딩 Canvas 초기 비활성화");
        }

        if (successEndingImage != null)
        {
            successEndingImage.SetActive(false);
            Debug.Log("AchievementChecker: 성공 엔딩 이미지 초기 비활성화");
        }

        if (failEndingImage != null)
        {
            failEndingImage.SetActive(false);
            Debug.Log("AchievementChecker: 실패 엔딩 이미지 초기 비활성화");
        }
    }

    void OnDestroy()
    {
        // 실패 엔딩 버튼 이벤트 해제
        if (failReturnToTitleButton != null)
        {
            failReturnToTitleButton.onClick.RemoveListener(OnReturnToTitleClicked);
        }

        if (failQuitGameButton != null)
        {
            failQuitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        }
        if (successQuitGameButton != null)
        {
            successQuitGameButton.onClick.RemoveListener(OnQuitGameClicked);
        }
    }

    // 타이틀로 돌아가기 버튼 클릭
    private void OnReturnToTitleClicked()
    {
        Debug.Log("타이틀로 돌아가기 버튼 클릭!");

        // 필요한 경우 데이터 초기화
        ResetChecker();

        // 타이틀 씬 로드
        SceneManager.LoadScene(titleSceneName);
    }

    // 게임 종료 버튼 클릭
    private void OnQuitGameClicked()
    {
        Debug.Log("게임 종료 버튼 클릭!");

#if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 애플리케이션 종료
            Application.Quit();
#endif
    }

    // 7일차 종료 시 엔딩 체크 (DateManager에서 호출)
    public void CheckEnding()
    {
        if (isChecked)
        {
            Debug.Log("이미 엔딩 판정 완료");
            return;
        }

        int totalCount = GetTotalCount();
        Debug.Log($"[Day End Check] total={totalCount}, target={targetCount}");

        if (totalCount >= targetCount)
        {
            ShowSuccessEnding();
        }
        else
        {
            ShowFailEnding();
        }

        isChecked = true; // ✅ 여기서만 잠금
    }



    private int GetTotalCount()
    {
        int statBarCount = 0;

        // 모든 StatBar의 maxCount 합산
        foreach (var statBar in statBars)
        {
            if (statBar != null)
            {
                int count = statBar.GetMaxCount();
                statBarCount += count;
                Debug.Log($"StatBar MAX 카운트: {count}");
            }
            else
            {
                Debug.LogWarning("StatBar가 null입니다!");
            }
        }

        // PurchaseButton의 구매 횟수
        int purchaseCount = PurchaseButton.GetTotalPurchaseCount();

        Debug.Log($"=== 카운트 집계 ===");
        Debug.Log($"StatBar 총 MAX 카운트: {statBarCount}");
        Debug.Log($"PurchaseButton 총 구매 카운트: {purchaseCount}");
        Debug.Log($"합계: {statBarCount + purchaseCount}");

        return statBarCount + purchaseCount;
    }

    private void ShowSuccessEnding()
    {
        Debug.Log("★★★ 성공 엔딩! 18개 달성! ★★★");

        if (successEndingImage != null)
        {
            // 부모 오브젝트들도 모두 활성화 (혹시 모를 경우 대비)
            Transform current = successEndingImage.transform;
            while (current != null)
            {
                current.gameObject.SetActive(true);
                current = current.parent;
                if (current != null && current.GetComponent<Canvas>() != null)
                    break; // Canvas까지만 활성화
            }

            successEndingImage.SetActive(true);
            Debug.Log("성공 엔딩 이미지 활성화 완료!");
        }
        else
        {
            Debug.LogError("성공 엔딩 이미지가 null입니다! Inspector에서 설정해주세요.");
        }

        if (failEndingImage != null)
        {
            failEndingImage.SetActive(false);
        }
    }

    private void ShowFailEnding()
    {
        Debug.Log("실패 엔딩... 18개 미달성");

        if (successEndingImage != null)
        {
            successEndingImage.SetActive(false);
        }

        if (failEndingImage != null)
        {
            // 부모 오브젝트들도 모두 활성화 (혹시 모를 경우 대비)
            Transform current = failEndingImage.transform;
            while (current != null)
            {
                current.gameObject.SetActive(true);
                current = current.parent;
                if (current != null && current.GetComponent<Canvas>() != null)
                    break; // Canvas까지만 활성화
            }

            failEndingImage.SetActive(true);
            Debug.Log("실패 엔딩 이미지 활성화 완료!");
        }
        else
        {
            Debug.LogError("실패 엔딩 이미지가 null입니다! Inspector에서 설정해주세요.");
        }
    }

    // 현재 총 카운트 확인용 (디버그/UI 표시용)
    public int GetCurrentTotalCount()
    {
        return GetTotalCount();
    }

    // 목표까지 남은 카운트
    public int GetRemainingCount()
    {
        return Mathf.Max(0, targetCount - GetTotalCount());
    }

    // 리셋 (재도전 시 사용)
    public void ResetChecker()
    {
        isChecked = false;
        PurchaseButton.ResetTotalPurchaseCount();

        if (endingCanvas != null)
            endingCanvas.SetActive(false);

        if (successEndingImage != null)
            successEndingImage.SetActive(false);

        if (failEndingImage != null)
            failEndingImage.SetActive(false);

        Debug.Log("AchievementChecker 리셋 완료");
    }
}