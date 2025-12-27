using UnityEngine;

public class MoveAreaManager : Singleton<MoveAreaManager>
{    
    [SerializeField] private Collider2D moveArea;

    public Collider2D GetMoveArea()
    {
        return moveArea;
    }
}

