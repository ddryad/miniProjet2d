using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField, Min(0.1f)] private float attackRange = 0.6f;

    private Animator anim;
    private SpriteRenderer sprite;
    private float attackPointOffsetX;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        attackPointOffsetX = attackPoint.localPosition.x;
    }

    private void Update()
    {
        FlipAttackPoint();

        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("playerAttack");
        }
    }

    private void FlipAttackPoint()
    {
        Vector3 localPos = attackPoint.localPosition;
        localPos.x = sprite.flipX ? -attackPointOffsetX : attackPointOffsetX;
        attackPoint.localPosition = localPos;
    }

    public void OnAttackHit()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Tree"))
            {
                hit.GetComponent<Tree>()?.Hit();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}