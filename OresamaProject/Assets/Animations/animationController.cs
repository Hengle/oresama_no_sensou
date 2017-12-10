using UnityEngine;
using System.Collections;

public class animationController : MonoBehaviour {
    public int testInt;
    Animator animator;

    const int STATE_IDLE = 0;
    const int STATE_WALK = 1;
    const int STATE_POSE = 2;
    const int STATE_ATTACK = 3;
    const int STATE_HIT = 4;
    const int STATE_DEATH = 5;

    private int hitpoints;

    public int  currentAnimationState = STATE_IDLE;

    bool isWalking = false;
    public bool isAttacking = false;

    public bool isHit = false;

    void FixedUpdate() {
        if (isWalking)
        {
            changeState(STATE_WALK);
        }
        else if (hitpoints <= 0)
        {
            changeState(STATE_DEATH);
        }

        else
        {
            changeState(STATE_IDLE);
        }
    }

    void changeState(int state)
    {
        if (currentAnimationState == state)
            return;

        switch (state)
        {
            case STATE_IDLE:
                animator.SetInteger("state", STATE_IDLE);
                break;
            case STATE_WALK:
                animator.SetInteger("state", STATE_WALK);
                break;
            case STATE_POSE:
                animator.SetInteger("state", STATE_POSE);
                break;
            case STATE_ATTACK:
                animator.SetInteger("state", STATE_ATTACK);
                break;
            case STATE_HIT:
                animator.SetInteger("state", STATE_HIT);
                break;
            case STATE_DEATH:
                animator.SetInteger("state", STATE_DEATH);
                break;
        }

        currentAnimationState = state;
    
    }

   public void setHP(int hp)
    {
        hitpoints = hp;
        animator.SetInteger("hitpoints", hp);
    }

    //public void getHit()
    public void TakeDamage()
    {
        Debug.Log("Hit");
        animator.SetTrigger("hit");
    }

    public void setAttacking() {
        animator.SetTrigger("attack");
    }

    public void addInt()
    {
        testInt--;
        animator.SetInteger("count", testInt);
    }

    public void resetInt()
    {
        testInt = Random.Range(5, 15);
        animator.SetInteger("count", testInt);
    }

    public void setWalking(bool v)
    {
        isWalking = v;
        animator.SetBool("isWalking", isWalking);
    }

    void Awake()
    {
        testInt = Random.Range(5, 10);
        animator = GetComponent<Animator>();
        animator.SetInteger("count", testInt);
    }
}
