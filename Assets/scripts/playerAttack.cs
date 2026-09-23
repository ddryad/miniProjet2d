using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerAttack : MonoBehaviour
{
    private Animator anim;

    // Cache the animation trigger hash for performance
    private static readonly int AttackTriggerHash = Animator.StringToHash("playerAttack");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleAttackInput();
    }

    private void HandleAttackInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            anim.SetTrigger(AttackTriggerHash);
        }
    }
}