using UnityEngine;

// 아이템 드래그 기능을 제공하는 컴포넌트
// InputRouter2D를 통해 드래그 이벤트를 받아 처리
[DisallowMultipleComponent]
public class DraggableItem : MonoBehaviour, IDraggable, IClickable
{
    [Header("Drag")]
    [SerializeField] private bool useZLock = true;          // 2D 탑다운이면 보통 true (Z 좌표 고정)
    [SerializeField] private float lockedZ = 0f;            // 고정할 Z 좌표 값
    [SerializeField] private bool snapToCursor = false;     // true면 클릭 지점 무시하고 커서 중앙으로 붙음

    [Header("Move Area")]
    [SerializeField] private bool clampByItemBounds = true; // 아이템 크기까지 고려해 안쪽으로 제한 (경계 밖으로 나가지 않음)

    private Camera cam;
    private Vector3 grabOffsetWorld; // 클릭 지점과 오브젝트 중심의 위치 차이 (오프셋)
    private bool isDragging; // 현재 드래그 중인지 여부

    private SpriteRenderer[] renderers; // 모든 SpriteRenderer (sortingOrder 변경용)
    private Collider2D myCol; // 아이템의 콜라이더 (크기 계산용)

    // 초기화: 카메라, SpriteRenderer, Collider 참조 저장
    void Awake()
    {
        cam = Camera.main;
        renderers = GetComponentsInChildren<SpriteRenderer>(true); // 모든 자식의 SpriteRenderer
        myCol = GetComponent<Collider2D>(); // 본체 콜라이더
    }

    // 클릭 다운 이벤트 (현재 미사용, 필요 시 하이라이트 등 추가 가능)
    public void OnClickDown(Vector2 worldPos) { /* 필요 시 하이라이트 */ }
    
    // 클릭 업 이벤트 (현재 미사용, 필요 시 하이라이트 해제 등 추가 가능)
    public void OnClickUp(Vector2 worldPos) { /* 필요 시 하이라이트 해제 */ }

    // 드래그 시작 이벤트
    // InputRouter2D에서 호출됨
    public void OnDragStart(Vector2 worldPos)
    {
        if (cam == null) cam = Camera.main;

        isDragging = true;

        // 드래그 시작 시 가장 높은 sortingOrder로 설정 (복원 없음)
        // 드래그한 아이템이 항상 맨 위에 표시됨
        BringToFront();

        Vector3 world = new Vector3(worldPos.x, worldPos.y, transform.position.z);

        // 오프셋 계산
        if (snapToCursor)
        {
            grabOffsetWorld = Vector3.zero; // 커서 중앙에 붙음
        }
        else
        {
            // 클릭한 지점 기준으로 들고 가는 느낌 (오프셋 유지)
            grabOffsetWorld = transform.position - world;
        }
    }

    // 드래그 중 이벤트 (매 프레임 호출)
    // InputRouter2D에서 호출됨
    public void OnDrag(Vector2 worldPos, Vector2 delta)
    {
        if (!isDragging) return;

        // 마우스 위치 + 오프셋으로 다음 위치 계산
        Vector3 world = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        Vector3 next = world + grabOffsetWorld;

        // Z 좌표 고정 (2D 게임에서 깊이 고정)
        if (useZLock)
            next.z = lockedZ;

        // MoveAreaManager의 moveArea로 범위 제한
        next = ClampToArea(next);

        transform.position = next;
    }

    // 드래그 종료 이벤트
    // InputRouter2D에서 호출됨
    public void OnDragEnd(Vector2 worldPos)
    {
        if (!isDragging) return;

        isDragging = false;
        // sortingOrder는 복원하지 않음 - 드래그한 물품이 맨 위에 유지됨
    }

    // CheckoutManager에서 새로운 최대 sortingOrder를 받아와서 적용
    // 드래그 종료 후에도 복원되지 않아 맨 위에 유지됨
    private void BringToFront()
    {
        if (renderers == null || renderers.Length == 0) return;
        
        int newOrder = 0;
        if (ItemManager.Instance != null)
        {
            newOrder = ItemManager.Instance.GetNextSortingOrder();
        }

        foreach (var sr in renderers)
        {
            if (sr != null)
                sr.sortingOrder = newOrder;
        }
    }

    // MoveAreaManager의 moveArea 범위 내로 위치 제한
    // clampByItemBounds가 true면 아이템 크기를 고려해 경계 밖으로 나가지 않도록 제한
    private Vector3 ClampToArea(Vector3 pos)
    {
        // MoveAreaManager가 없으면 제한 없이 반환
        if (MoveAreaManager.Instance == null) return pos;
        
        Collider2D moveArea = MoveAreaManager.Instance.GetMoveArea();
        if (moveArea == null) return pos;

        Bounds area = moveArea.bounds;

        // 아이템 크기 고려: 아이템이 경계를 넘지 않게 안쪽으로 제한
        float padX = 0f, padY = 0f;
        if (clampByItemBounds && myCol != null)
        {
            Bounds b = myCol.bounds;
            padX = b.extents.x; // 아이템 크기의 절반 (X)
            padY = b.extents.y; // 아이템 크기의 절반 (Y)
        }

        // 패딩을 고려한 범위로 Clamp
        float x = Mathf.Clamp(pos.x, area.min.x + padX, area.max.x - padX);
        float y = Mathf.Clamp(pos.y, area.min.y + padY, area.max.y - padY);

        return new Vector3(x, y, pos.z);
    }
}
