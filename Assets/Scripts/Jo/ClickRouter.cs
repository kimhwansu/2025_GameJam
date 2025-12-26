using UnityEngine;

public class InputRouter2D : MonoBehaviour
{
    [SerializeField] private LayerMask clickableMask;

    private Camera cam;

    private IClickable activeClick;
    private IDraggable activeDrag;

    private Vector2 lastWorldPos;
    private bool isPointerDown;
    private bool hasDragged; // 실제로 드래그가 발생했는지 여부

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (cam == null) return;

        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Down
        if (Input.GetMouseButtonDown(0))
        {
            isPointerDown = true;
            hasDragged = false;
            lastWorldPos = worldPos;

            Collider2D bestCol = PickByParentSortingLayer(worldPos);

            if (bestCol == null)
            {
                ClearActive();
                return;
            }

            // 선택된 콜라이더 자체에서 컴포넌트 확인
            IDraggable directDrag = bestCol.GetComponent<IDraggable>();
            IClickable directClick = bestCol.GetComponent<IClickable>();
            
            // 1순위: 콜라이더 자체에 IClickable만 있으면 클릭 처리 (바코드)
            if (directClick != null)
            {
                activeClick = directClick;
                activeDrag = null;
                activeClick.OnClickDown(worldPos);
            }
            // 2순위: 콜라이더 자체에 IDraggable이 있으면 드래그 처리 (본체)
            if (directDrag != null)
            {
                activeDrag = directDrag;
                activeClick = null;
                activeDrag.OnDragStart(worldPos);
            }
            // 3순위: 콜라이더 자체에 없으면 부모에서 찾기
            else
            {
                activeDrag = bestCol.GetComponentInParent<IDraggable>();
                activeClick = bestCol.GetComponentInParent<IClickable>();
                
                if (activeDrag != null)
                {
                    activeDrag.OnDragStart(worldPos);
                    activeClick = null;
                }
                else if (activeClick != null)
                {
                    activeClick.OnClickDown(worldPos);
                }
            }
        }

        // Drag (Down 유지 중일 때만)
        if (isPointerDown && Input.GetMouseButton(0) && activeDrag != null)
        {
            Vector2 delta = worldPos - lastWorldPos;
            
            // 실제로 이동이 발생했으면 드래그로 간주
            if (delta.sqrMagnitude > 0.0001f)
            {
                hasDragged = true;
            }
            
            activeDrag.OnDrag(worldPos, delta);
            lastWorldPos = worldPos;
        }

        // Up
        if (isPointerDown && Input.GetMouseButtonUp(0))
        {
            // 드래그가 있었으면 드래그 종료만 처리
            if (activeDrag != null)
            {
                activeDrag.OnDragEnd(worldPos);
            }
            // 드래그가 없었고 클릭만 있었으면 클릭 처리
            else if (activeClick != null && !hasDragged)
            {
                activeClick.OnClickUp(worldPos);
            }
            
            ClearActive();
        }
    }

    private void ClearActive()
    {
        activeClick = null;
        activeDrag = null;
        isPointerDown = false;
        hasDragged = false;
    }

    /// 클릭 지점에서 겹친 모든 콜라이더 중,
    /// "그 콜라이더의 부모(본체) SpriteRenderer"의 sortingOrder 값이 가장 큰(앞) 것을 선택
    private Collider2D PickByParentSortingLayer(Vector2 worldPos)
    {
        var hits = Physics2D.RaycastAll(worldPos, Vector2.zero, 0f, clickableMask);
        if (hits == null || hits.Length == 0) return null;

        Collider2D best = null;
        int bestLayerValue = int.MinValue;
        int bestSortingOrder = int.MinValue;

        foreach (var h in hits)
        {
            var col = h.collider;
            if (col == null) continue;

            // ✅ "부모의 스프라이트" 기준
            var sr = col.GetComponentInParent<SpriteRenderer>(true);
            if (sr == null) continue;

            int layerValue = SortingLayer.GetLayerValueFromID(sr.sortingLayerID);
            int sortingOrder = sr.sortingOrder;

            // 1순위: Sorting Layer 비교
            // 2순위: 같은 Sorting Layer 내에서 sortingOrder 비교
            if (layerValue > bestLayerValue || 
                (layerValue == bestLayerValue && sortingOrder > bestSortingOrder))
            {
                bestLayerValue = layerValue;
                bestSortingOrder = sortingOrder;
                best = col;
            }
        }

        return best;
    }
}
