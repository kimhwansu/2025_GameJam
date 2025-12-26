using UnityEngine;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackInterval = 1.5f;
    [SerializeField] private LayerMask monsterLayer;

    private float attackTimer = 0f;
    private Monster currentTarget;

    private void Update()
    {
        FindTargetInRange();

        if (currentTarget != null)
        {
            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);

            if (distance <= attackRange)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackInterval)
                {
                    Attack();
                    attackTimer = 0f;
                }
            }
            else
            {
                currentTarget = null;
                attackTimer = 0f;
            }
        }
    }

    private void FindTargetInRange()
    {
        if (currentTarget != null) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackRange, monsterLayer);

        if (hitColliders.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            Monster closestMonster = null;

            foreach (Collider2D collider in hitColliders)
            {
                Monster monster = collider.GetComponent<Monster>();
                if (monster != null)
                {
                    float distance = Vector2.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestMonster = monster;
                    }
                }
            }

            if (closestMonster != null)
            {
                SetTarget(closestMonster);
            }
        }
    }

    private void Attack()
    {
        if (currentTarget != null)
            currentTarget.TakeDamage();
    }

    public void SetTarget(Monster target)
    {
        currentTarget = target;
        attackTimer = 0f;
    }
}
