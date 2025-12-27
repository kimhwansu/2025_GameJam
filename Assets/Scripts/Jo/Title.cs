using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup titleGroup;   // 버튼+이미지 묶인 루트에 붙은 CanvasGroup
    [SerializeField] private Button startButton;

    [Header("Scene")]
    [SerializeField] private string targetSceneName = "Main"; // 이동할 씬 이름

    [Header("Fade")]
    [SerializeField] private float fadeDuration = 1.0f;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (startButton != null)
            startButton.onClick.AddListener(OnClickStart);

        // 초기 상태 보장
        if (titleGroup != null)
        {
            titleGroup.alpha = 1f;
            titleGroup.interactable = true;
            titleGroup.blocksRaycasts = true;
        }
    }

    public void OnClickStart()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        StartCoroutine(FadeAndLoad());
    }

    private IEnumerator FadeAndLoad()
    {
        if (titleGroup != null)
        {
            // 클릭 방지
            titleGroup.interactable = false;
            titleGroup.blocksRaycasts = false;

            float t = 0f;
            float startAlpha = titleGroup.alpha;

            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime; // 타이틀에서 timeScale 바뀌어도 안정적
                float a = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
                titleGroup.alpha = a;
                yield return null;
            }

            titleGroup.alpha = 0f;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
