using UnityEngine;

public class ScannableItem : MonoBehaviour
{
    public ProductData data;

    private void OnMouseDown()
    {
        // 이미 스캔된 아이템이면 무시
        Debug.Log(gameObject.name + " 클릭됨");

        CheckoutManager.Instance.Scan(this);
    }
}
