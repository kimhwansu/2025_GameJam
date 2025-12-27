using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomerAnime : MonoBehaviour
{
    private RectTransform rect;
    private Image image;

    // 값을 추가해서 이동해도 되고, 특정 UI위치로 이동되게 해도 가능. (자유롭게 변경)
    [Header("이동할 위치 + 특정 값")]
    public Vector2 leftMove = new Vector2(0, 0);
    public Vector2 leftleftMove = new Vector2(0, 0);
    public Vector2 upMove = new Vector2(0, 0);


    Vector2 basePos;     // 처음 위치 -> 이동 전
    Vector2 middlePos = new Vector2(-300, 0);   // 중간 위치
    Vector3 middleScale = new Vector3(1.2f, 1.2f, 1);  // 중간 크기
    
    // 애니메이션 완료 콜백
    private Action onExitComplete; 

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }
    private void Start()
    {

        basePos = rect.anchoredPosition; // 처음 위치 저장

        //EnterShow();
    }

    // 손님 들어오는 연출 (이동 & 색상)
    public void EnterShow()
    {
        gameObject.SetActive(true);
        rect.anchoredPosition = basePos;
        middleScale = rect.localScale;
        image.color = new Color(0, 0, 0, 1);

        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOAnchorPos(rect.anchoredPosition + leftMove, 1.3f))
            .Join(image.DOColor(Color.white, 0.8f).SetEase(Ease.InQuad));

        seq.OnComplete(() =>
        {
            middlePos = rect.anchoredPosition;
            // ExitShow()는 계산이 끝난 후에 수동으로 호출됨
        });
    }

    // 손님 나가는 연출 (이동 & 색상 & a값) -> 완료 시 비활성화 
    public void ExitShow(Action onComplete = null)
    {
        onExitComplete = onComplete;
        
        Sequence seq = DOTween.Sequence();

        seq.Append(rect.DOAnchorPos(rect.anchoredPosition + leftleftMove, 1.3f))
            .Join(image.DOColor(Color.black, 0.7f))
            .Join(image.DOFade(0, 0.7f).SetEase(Ease.InQuad));

        seq.OnComplete(() => { 
            gameObject.SetActive(false); 
            rect.anchoredPosition = basePos;
            
            // 완료 콜백 호출
            onExitComplete?.Invoke();
            onExitComplete = null;
        });
    }
}