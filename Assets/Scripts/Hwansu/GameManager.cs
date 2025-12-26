using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UserData userData;
    public CharacterData characterData;

    void Start()
    {
        userData.ResetData();
        characterData.ResetData();
    }
}
