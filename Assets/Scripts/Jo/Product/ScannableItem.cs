using UnityEngine;

public class ScannableItem : MonoBehaviour, IClickable
{
    [SerializeField] public ProductData data;


    [Header("옵션")]
    [SerializeField] private bool scanOnMouseDown = true; // Down에서 스캔할지 Up에서 스캔할지

    // 필요하면 부모(상품) 참조를 넘기고 싶을 수 있으니:
    // 바코드가 자식이라면 this 대신 부모의 ScannableItem(또는 ItemData)을 넘기도록 바꿀 수도 있음.
    // 지금 요구사항은 Scan(this)라서 그대로 구현.

    public void OnClickDown(Vector2 worldPos)
    {
        if (!scanOnMouseDown) return;
        TryScan();
    }

    public void OnClickUp(Vector2 worldPos)
    {
        
    }

    private void TryScan()
    {
        if (CheckoutManager.Instance == null)
        {
            Debug.LogWarning("[ScannableItem] CheckoutManager.Instance is null");
            return;
        }

        CheckoutManager.Instance.Scan(this);
    }
}
