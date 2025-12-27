using DG.Tweening;
using UnityEngine;
using TMPro;

// 이벤트 트리거 연결

public class SpeechAnime : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
   
    // 초기화
    public void Setting()
    {
        canvasGroup.alpha = 0f;
        text.gameObject.SetActive(false);
    }

    // 말풍선을 누르면 호출 (a값 증가)
    public void OnSpeechShow()
    {
        Tween tween = canvasGroup.DOFade(1, 0.3f);

        tween.OnComplete(() =>
        {
            text.gameObject.SetActive(true);
        });
    }
}