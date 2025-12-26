using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerData playerData;

    void Start()
    {
        playerData.ResetData();
    }
}
