
// 클릭 되었을 때 / 클릭을 뗏을 때
public interface IClickable
{
    void OnClickDown(UnityEngine.Vector2 worldPos);
    void OnClickUp(UnityEngine.Vector2 worldPos);
}

public interface IDraggable
{
    void OnDragStart(UnityEngine.Vector2 worldPos);
    void OnDrag(UnityEngine.Vector2 worldPos, UnityEngine.Vector2 delta);
    void OnDragEnd(UnityEngine.Vector2 worldPos);
}
