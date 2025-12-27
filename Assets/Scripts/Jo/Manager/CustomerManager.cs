using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CustomerManager : Singleton<CustomerManager>
{
    [Header("Customer Image")]
    [SerializeField] private Image customerImage; // 손님 이미지 표시용
    [SerializeField] private CustomerAnime customerAnime; // 손님 애니메이션
    
    [Header("Rest Settings")]
    [SerializeField] private float restDuration = 30f; // 휴식 시간 (초)
    [SerializeField] private GameObject restUIObject; // 휴식 시간 UI 오브젝트
    
    private int customerCount = 0;      // 처리한 손님 수 (전체 누적)
    private int cycleCustomerCount = 0; // 현재 사이클에서 처리한 손님 수
    
    // portraitId에 맞는 손님 이미지 설정
    public void SetCustomerByPortraitId(string portraitId)
    {
        if (customerImage == null || string.IsNullOrEmpty(portraitId))
            return;
        
        // Resources에서 portraitId에 맞는 이미지 로드
        Sprite sprite = PortraitLoader.Load(portraitId);
        if (sprite != null)
        {
            customerImage.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"CustomerManager: portraitId '{portraitId}'에 해당하는 이미지를 찾을 수 없습니다.");
        }
    }
    
    // 손님 수만 증가 (이미지 변경 없이)
    public void IncrementCustomerCount()
    {
        customerCount++;
        cycleCustomerCount++;
        
        // DateManager에 손님 처리 완료 알림
        if (DateManager.Instance != null)
        {
            DateManager.Instance.OnCustomerProcessed();
        }
    }
    
    // 손님 수 반환
    public int GetCustomerCount()
    {
        return customerCount;
    }
    
    // 휴식이 필요한지 확인
    public bool NeedsRest()
    {
        // DateManager에서 하루에 오는 손님 수를 가져와서 확인
        if (DateManager.Instance != null)
        {
            int customersPerDay = DateManager.Instance.GetCustomersPerDay();
            // 현재 일차에서 처리한 손님 수가 하루 손님 수와 같으면 휴식 필요
            return DateManager.Instance.IsDayComplete();
        }
        
        // DateManager가 없으면 기본값 사용 (하위 호환성)
        Debug.Log(cycleCustomerCount);
        return cycleCustomerCount >= 6; // 기본값 6명
    }
    
    // 사이클 초기화 (휴식 시간 후 호출)
    public void ResetCycle()
    {
        cycleCustomerCount = 0; // 새 사이클 시작
    }
    
    // 휴식 시간 반환
    public float GetRestDuration()
    {
        return restDuration;
    }
    
    // 손님 이미지 활성화 (휴식 UI 비활성화)
    public void ShowCustomerImage()
    {
        // CustomerAnime가 있으면 애니메이션으로 표시
        if (customerAnime != null)
        {
            customerAnime.EnterShow();
        }
        // CustomerAnime가 없으면 기본 방식으로 표시
        else if (customerImage != null)
        {
            customerImage.gameObject.SetActive(true);
        }
        
        // 휴식 UI 비활성화
        if (restUIObject != null)
        {
            restUIObject.SetActive(false);
        }
    }
    
    // 손님 이미지 비활성화 (휴식 UI 활성화)
    public void HideCustomerImage(Action onComplete = null)
    {
        // CustomerAnime가 있으면 애니메이션으로 숨김
        if (customerAnime != null)
        {
            customerAnime.ExitShow(() =>
            {
                // 휴식 UI 활성화
                if (restUIObject != null)
                {
                    restUIObject.SetActive(true);
                }
                
                // 완료 콜백 호출
                onComplete?.Invoke();
            });
        }
        // CustomerAnime가 없으면 기본 방식으로 숨김
        else
        {
            if (customerImage != null)
            {
                customerImage.gameObject.SetActive(false);
            }
            
            // 휴식 UI 활성화
            if (restUIObject != null)
            {
                restUIObject.SetActive(true);
            }
            
            // 완료 콜백 호출
            onComplete?.Invoke();
        }
    }
}

