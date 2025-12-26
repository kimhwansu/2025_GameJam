using UnityEngine;

// 상점에서 판매할 제품의 데이터를 저장하는 ScriptableObject 클래스
[CreateAssetMenu(menuName = "Shop/Product Data")]
public class ProductData : ScriptableObject
{
    public string productId;   // 제품의 고유 식별자
    public string displayName; // 상점에 표시될 제품 이름
    public int price;          // 제품의 가격
    public Sprite icon;        // 제품의 아이콘 이미지
}
