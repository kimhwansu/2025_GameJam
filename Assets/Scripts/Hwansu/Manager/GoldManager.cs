using UnityEngine;
using System;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    public int Gold { get; private set; } = 10000;  //초기골드

    public event Action OnGoldChanged;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public bool UseGold(int amount)
    {
        if (Gold < amount)
            return false;

        Gold -= amount;
        OnGoldChanged?.Invoke();   // UI에 알림
        return true;
    }

    public void AddGold(int amount)
    {
        Gold += amount;
        OnGoldChanged?.Invoke();
    }
}
