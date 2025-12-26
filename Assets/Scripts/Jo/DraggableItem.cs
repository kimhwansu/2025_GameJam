using UnityEngine;

[DisallowMultipleComponent]
public class DraggableItem : MonoBehaviour, IDraggable, IClickable
{
    [Header("Drag")]
    [SerializeField] private bool useZLock = true;          // 2D 탑다운이면 보통 true
    [SerializeField] private float lockedZ = 0f;            // 고정할 Z
    [SerializeField] private bool snapToCursor = false;     // true면 클릭 지점 무시하고 커서 중앙으로 붙음

    private Camera cam;
    private Vector3 grabOffsetWorld; // 클릭 지점과 오브젝트 위치 차이
    private bool isDragging;

    private SpriteRenderer[] renderers;

    void Awake()
    {
        cam = Camera.main;
        renderers = GetComponentsInChildren<SpriteRenderer>(true);
    }

    // (선택) 클릭 시 어떤 피드백이 필요하면 여기서 처리 가능
    public void OnClickDown(Vector2 worldPos) { /* 필요 시 하이라이트 */ }
    public void OnClickUp(Vector2 worldPos) { /* 필요 시 하이라이트 해제 */ }

    public void OnDragStart(Vector2 worldPos)
    {
        if (cam == null) cam = Camera.main;

        isDragging = true;

        // 드래그 시작 시 가장 높은 sortingOrder로 설정 (복원 없음)
        BringToFront();

        Vector3 world = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        if (snapToCursor)
        {
            grabOffsetWorld = Vector3.zero;
        }
        else
        {
            // 클릭한 지점 기준으로 들고 가는 느낌
            grabOffsetWorld = transform.position - world;
        }
    }

    public void OnDrag(Vector2 worldPos, Vector2 delta)
    {
        if (!isDragging) return;

        Vector3 world = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        Vector3 next = world + grabOffsetWorld;

        if (useZLock)
            next.z = lockedZ;

        transform.position = next;
    }

    public void OnDragEnd(Vector2 worldPos)
    {
        if (!isDragging) return;

        isDragging = false;
        // sortingOrder는 복원하지 않음 - 드래그한 물품이 맨 위에 유지됨
    }

    /// <summary>
    /// CheckoutManager에서 새로운 최대 sortingOrder를 받아와서 적용
    /// 드래그 종료 후에도 복원되지 않아 맨 위에 유지됨
    /// </summary>
    private void BringToFront()
    {
        if (renderers == null || renderers.Length == 0) return;
        
        int newOrder = 0;
        if (CheckoutManager.Instance != null)
        {
            newOrder = CheckoutManager.Instance.GetNextSortingOrder();
        }

        foreach (var sr in renderers)
        {
            if (sr != null)
                sr.sortingOrder = newOrder;
        }
    }
}
