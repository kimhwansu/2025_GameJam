using UnityEngine;
using System;

public class Monster : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;

    [Header("Death Effects")]
    //[SerializeField] private GameObject deathEffectPrefab; // 사망 이펙트 프리팹
    //[SerializeField] private AudioClip deathSound; // 사망 사운드
    [SerializeField] private int goldReward = 50; // 드롭할 골드

    public event Action OnMonsterDeath;

    //private AudioSource audioSource;

    private void Start()
    {
        currentHealth = maxHealth;

        //audioSource = GetComponent<AudioSource>();
        //if (audioSource == null)
        //{
        //    audioSource = gameObject.AddComponent<AudioSource>();
        //}
    }

    public void TakeDamage()
    {
        // 플레이어 공격력 상관없이 무조건 -1 데미지
        currentHealth -= 1;
        currentHealth = Mathf.Max(0, currentHealth);


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 이펙트 재생
        //PlayDeathEffect();

        // 사망 사운드 재생
        //PlayDeathSound();

        // 골드 드롭
        GoldManager.Instance.AddGold(goldReward);

        OnMonsterDeath?.Invoke();

        // 몬스터 리셋 (위치 고정이므로 파괴하지 않고 체력만 회복)
        Invoke(nameof(Respawn), 0.5f);
        Destroy(gameObject);
    }

    private void Respawn()
    {
        currentHealth = maxHealth;
    }

    //private void PlayDeathEffect()
    //{
    //    if (deathEffectPrefab != null)
    //    {
    //        GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //        Destroy(effect, 2f);
    //    }
    //}

    //private void PlayDeathSound()
    //{
    //    if (deathSound != null && audioSource != null)
    //    {
    //        audioSource.PlayOneShot(deathSound);
    //    }
    //}
}