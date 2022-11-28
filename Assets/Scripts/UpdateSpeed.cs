using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateSpeed : MonoBehaviour
{
    [SerializeField] Rigidbody PlayerRB;
    [SerializeField] Animator animator;

    private void Update()
    {
        animator.SetFloat("Speed", PlayerRB.velocity.magnitude);
    }
}
