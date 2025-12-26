using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ResultTextUIManager : Singleton<ResultTextUIManager>
{
    [Header("UI")]
    [SerializeField] private Text resultText; // 결과 텍스트
    
    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 100f; // 위로 이동할 거리
    [SerializeField] private float animationDuration = 2f; // 애니메이션 지속 시간
    
    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green; // 성공 색상
    [SerializeField] private Color failColor = Color.red; // 실패 색상
    
    private Vector2 startPosition; // 시작 위치
    private RectTransform rectTransform;
    private Coroutine currentAnimation;
    
    private void Start()
    {
        if (resultText != null)
        {
            rectTransform = resultText.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                startPosition = rectTransform.anchoredPosition; // 시작 위치 저장
            }
            
            // 초기화 시 비활성화
            resultText.gameObject.SetActive(false);
        }
    }
    
    // 결과 표시 (성공/실패)
    public void ShowResult(bool isSuccess)
    {
        if (resultText == null || rectTransform == null) return;
        
        // 기존 애니메이션 중지
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        
        // 텍스트 활성화
        resultText.gameObject.SetActive(true);
        
        // 텍스트 설정
        resultText.text = isSuccess ? "성공!" : "실패(패널티 -1000)";
        
        // 색상 설정
        resultText.color = isSuccess ? successColor : failColor;
        
        // 위치 초기화 (시작 위치로 복귀)
        rectTransform.anchoredPosition = startPosition;
        
        // 알파값 초기화
        Color color = resultText.color;
        color.a = 1f;
        resultText.color = color;
        
        // 애니메이션 시작
        currentAnimation = StartCoroutine(AnimateResult());
    }
    
    // 결과 애니메이션 코루틴
    private IEnumerator AnimateResult()
    {
        float elapsed = 0f;
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = startPos + Vector2.up * moveDistance;
        
        Color startColor = resultText.color;
        Color endColor = startColor;
        endColor.a = 0f; // 알파값 0으로
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            // 위치 보간 (위로 이동)
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            
            // 알파값 보간 (서서히 흐려지기)
            Color currentColor = Color.Lerp(startColor, endColor, t);
            resultText.color = currentColor;
            
            yield return null;
        }
        
        // 애니메이션 완료 후 초기화
        rectTransform.anchoredPosition = startPosition;
        Color finalColor = resultText.color;
        finalColor.a = 0f;
        resultText.color = finalColor;
        
        // 텍스트 비활성화
        if (resultText != null)
        {
            resultText.gameObject.SetActive(false);
        }
        
        currentAnimation = null;
    }
}

