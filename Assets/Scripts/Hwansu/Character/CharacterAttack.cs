using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Monster currentTarget;
    [SerializeField] private float attackInterval = 1.5f; // 1.5초마다 공격

    private float attackTimer = 0f;

    private void Update()
    {
        if (currentTarget != null)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackInterval)
            {
                Attack();
                attackTimer = 0f;
            }
        }
    }

    public void Attack()
    {
        if (currentTarget != null)
        {
            // 플레이어 공격력 상관없이 몬스터는 무조건 -1 데미지를 받음
            currentTarget.TakeDamage();
        }
    }

    public void SetTarget(Monster target)
    {
        currentTarget = target;
        attackTimer = 0f;
    }
}