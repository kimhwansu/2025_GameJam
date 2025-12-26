using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : Singleton<CustomerManager>
{
    [Header("Customer Image")]
    [SerializeField] private Image customerImage; // 손님 이미지 표시용
    [SerializeField] private List<Sprite> customerImages = new List<Sprite>(); // 손님 이미지 리스트
    
    [Header("Rest Settings")]
    [SerializeField] private int customersPerRest = 3; // 휴식 주기 (손님 수)
    [SerializeField] private float restDuration = 30f; // 휴식 시간 (초)
    [SerializeField] private GameObject restUIObject; // 휴식 시간 UI 오브젝트
    
    private int currentCustomerIndex = 0; // 현재 손님 이미지 인덱스
    private int customerCount = -1; // 처리한 손님 수
    
    // 손님 이미지를 순서대로 변경
    public void ChangeCustomer()
    {
        if (customerImage == null || customerImages == null || customerImages.Count == 0)
            return;
        
        // 현재 인덱스의 이미지 설정
        customerImage.sprite = customerImages[currentCustomerIndex];
        
        // 다음 손님을 위해 인덱스 증가 (리스트 끝에 도달하면 처음으로)
        currentCustomerIndex = (currentCustomerIndex + 1) % customerImages.Count;
        
        // 손님 수 증가
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

