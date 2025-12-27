using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    [Header("Customer Image")]
    [SerializeField] private Image customerImage; // 손님 이미지 표시용
    
    [Header("Rest Settings")]
    [SerializeField] private int customersPerRest = 3; // 휴식 주기 (손님 수)
    [SerializeField] private float restDuration = 30f; // 휴식 시간 (초)
    [SerializeField] private GameObject restUIObject; // 휴식 시간 UI 오브젝트
    
    private int customerCount = -1; // 처리한 손님 수
    
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
    }
    
    // 손님 수 반환
    public int GetCustomerCount()
    {
        return customerCount;
    }
    
    // 휴식이 필요한지 확인
    public bool NeedsRest()
    {
        Debug.Log(customerCount);
        return customerCount > 0 && customerCount % customersPerRest == 0;
    }
    
    // 휴식 시간 반환
    public float GetRestDuration()
    {
        return restDuration;
    }
    
    // 손님 이미지 활성화 (휴식 UI 비활성화)
    public void ShowCustomerImage()
    {
        if (customerImage != null)
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
    public void HideCustomerImage()
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
    }
}

