using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 마우스 커서 위치에 따라 스캔 가능 UI 이미지 표시/숨김
/// 스캔 가능한 영역(ScannableItem) 위에 마우스가 있으면 UI 이미지 활성화
/// </summary>
public class MouseCursorManager : Singleton<MouseCursorManager>
{
    [Header("Cursor UI")]
    [SerializeField] private GameObject scanCursorUI; // 스캔 가능할 때 표시할 UI 이미지
    [SerializeField] private RectTransform scanCursorRect; // UI RectTransform (위치 이동용)
    
    [Header("Settings")]
    [SerializeField] private LayerMask scannableMask; // 스캔 가능한 레이어
    [SerializeField] private Vector2 cursorOffset = new Vector2(20f, -20f); // 커서 오프셋
    
    [Header("Canvas")]
    [SerializeField] private Canvas canvas; // UI 캔버스 (Screen Space - Overlay 또는 Camera)
    
    private Camera cam;
    private bool isCursorVisible = false;
    
    private void Start()
    {
        cam = Camera.main;
        
        // 초기화 시 UI 비활성화
        if (scanCursorUI != null)
        {
            scanCursorUI.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (cam == null) return;
        
        // 마우스 위치를 월드 좌표로 변환
        Vector2 worldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        // 스캔 가능한 오브젝트가 마우스 아래에 있는지 확인
        bool canScan = CheckForScannableItem(worldPos);
        
        // 상태 변경 시에만 UI 업데이트
        if (canScan != isCursorVisible)
        {
            isCursorVisible = canScan;
            UpdateCursorUI(canScan);
        }
        
        // 커서 UI 위치 업데이트 (활성화 상태일 때만)
        if (isCursorVisible && scanCursorRect != null)
        {
            UpdateCursorPosition();
        }
    }
    
    // 마우스 위치에 스캔 가능한 아이템이 있는지 확인
    private bool CheckForScannableItem(Vector2 worldPos)
    {
        // 레이캐스트로 충돌 확인
        var hits = Physics2D.RaycastAll(worldPos, Vector2.zero, 0f, scannableMask);
        if (hits == null || hits.Length == 0) return false;
        
        // ScannableItem 컴포넌트가 있는지 확인
        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;
            
            // 직접 또는 부모에서 ScannableItem 찾기
            var scannable = hit.collider.GetComponent<ScannableItem>();
            if (scannable == null)
            {
                scannable = hit.collider.GetComponentInParent<ScannableItem>();
            }
            
            if (scannable != null)
            {
                return true;
            }
        }
        
        return false;
    }
    
    // 커서 UI 활성화/비활성화
    private void UpdateCursorUI(bool show)
    {
        if (scanCursorUI != null)
        {
            scanCursorUI.SetActive(show);
        }
    }
    
    // 커서 UI 위치 업데이트 (마우스 따라다니기)
    private void UpdateCursorPosition()
    {
        if (scanCursorRect == null) return;
        
        Vector2 screenPos = Input.mousePosition;
        
        // Canvas가 Screen Space - Overlay인 경우
        if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            scanCursorRect.position = screenPos + cursorOffset;
        }
        // Canvas가 Screen Space - Camera인 경우
        else if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out Vector2 localPoint
            );
            scanCursorRect.anchoredPosition = localPoint + cursorOffset;
        }
    }
    
    // 외부에서 커서 UI 강제 숨김 (필요 시 사용)
    public void HideCursor()
    {
        isCursorVisible = false;
        UpdateCursorUI(false);
    }
    
    // 외부에서 커서 UI 강제 표시 (필요 시 사용)
    public void ShowCursor()
    {
        isCursorVisible = true;
        UpdateCursorUI(true);
    }
}

