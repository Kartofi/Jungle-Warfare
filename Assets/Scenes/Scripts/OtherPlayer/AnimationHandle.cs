using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandle : MonoBehaviour
{
    public Animator animator;
    private OtherPlayer data;

    private void Awake()
    {
        data = gameObject.GetComponent<OtherPlayer>();
    }
    private void Update()
    {
        UpdateAnimation();
    }
    int lastState = (int)PlayerStates.idle;
    private void UpdateAnimation()
    {
        if (lastState == data.state)
        {
            return;
        }
        if (data.state == (int)PlayerStates.falling)
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsFalling", true);
        }else if (data.state == (int)PlayerStates.walking)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsFalling", false);
        }else
        {
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsFalling", false);
        }
        lastState = data.state;
    }

}
