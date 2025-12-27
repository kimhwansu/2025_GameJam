using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 오프닝 연출: 패널들이 순차적으로 페이드 아웃 후 비활성화
/// </summary>
public class OpeningManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private Image firstPanel;       // 첫 번째 패널
    [SerializeField] private CanvasGroup secondPanel; // 두 번째 패널 (CanvasGroup - 자식 포함 페이드)
    
    [Header("Duration")]
    [SerializeField] private float firstPanelDelay = 1f;     // 첫 번째 패널 페이드 시작 전 대기 시간
    [SerializeField] private float firstPanelFade = 1f;      // 첫 번째 패널 페이드 아웃 시간
    [SerializeField] private float secondPanelDelay = 3f;    // 두 번째 패널 페이드 시작 전 대기 시간
    [SerializeField] private float secondPanelFade = 1f;     // 두 번째 패널 페이드 아웃 시간
    
    private void Start()
    {
        PlayOpening();
    }
    
    public void PlayOpening()
    {
        // 초기 상태: 패널 활성화, 알파값 1 (완전히 보임)
        if (firstPanel != null)
        {
            firstPanel.gameObject.SetActive(true);
            Color c1 = firstPanel.color;
            c1.a = 1f;
            firstPanel.color = c1;
        }
        
        if (secondPanel != null)
        {
            secondPanel.gameObject.SetActive(true);
            secondPanel.alpha = 1f;
        }
        
        // 순차 실행
        Sequence seq = DOTween.Sequence();
        
        // 1초 대기 후 첫 번째 패널 페이드 아웃
        seq.AppendInterval(firstPanelDelay);
        if (firstPanel != null)
        {
            seq.Append(firstPanel.DOFade(0f, firstPanelFade));
        }
        
        // 3초 대기 후 두 번째 패널 페이드 아웃 (자식 포함)
        seq.AppendInterval(secondPanelDelay);
        if (secondPanel != null)
        {
            seq.Append(secondPanel.DOFade(0f, secondPanelFade));
        }
        
        // 완료 후 패널 둘 다 비활성화
        seq.OnComplete(() =>
        {
            if (firstPanel != null)
                firstPanel.gameObject.SetActive(false);
            
            if (secondPanel != null)
                secondPanel.gameObject.SetActive(false);
        });
    }
}

