using Photon.Pun;
using System.Collections;
using UnityEngine;


public class CharacterAnimator : MonoBehaviour
{
    public static readonly int HORIZONTAL_SPEED = Animator.StringToHash("HorizontalSpeed");
    public static readonly int VERTICAL_SPEED = Animator.StringToHash("VerticalSpeed");
    public static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    public static readonly int IDLE = Animator.StringToHash("Idle");
    public static readonly int IDLE_THINKING = Animator.StringToHash("IdleThinking");
    public static readonly int IDLE_REJECTED = Animator.StringToHash("IdleRejected");
    public static readonly int GREETINGS = Animator.StringToHash("Greetings");

    private Animator animator;
    private Character character;
    private bool isGreeting = false; //While this is true no animation should play
    [SerializeField] AudioClip footSteps1;
    [SerializeField] AudioClip footSteps2;

    public bool canWalk = true;

    public PhotonView photonView;


    protected virtual void Awake()
    {
        if (!photonView.IsMine)
            return;

        this.animator = this.GetComponent<Animator>();
        this.character = this.GetComponent<Character>();
    }

    protected virtual void Update()
    {
        if (!photonView.IsMine)
            return;
        if (canWalk)
        {
        this.animator.SetFloat(HORIZONTAL_SPEED, this.character.HorizontalSpeed);
        this.animator.SetFloat(VERTICAL_SPEED, this.character.VerticalSpeed);
        }
        this.animator.SetBool(IS_GROUNDED, this.character.IsGrounded);

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectAndPlayGreetings(GreetingStyle.Salute);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectAndPlayGreetings(GreetingStyle.Hi);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectAndPlayGreetings(GreetingStyle.ShakeHands);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectAndPlayGreetings(GreetingStyle.InformalBow);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectAndPlayGreetings(GreetingStyle.Taunt1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectAndPlayGreetings(GreetingStyle.SwingDance);
        }
        //if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    SelectAndPlayGreetings(GreetingStyle.Sit);
        //}
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectAndPlayGreetings(GreetingStyle.FistPump);
        }

        if (this.animator.GetFloat(HORIZONTAL_SPEED) > .1)
        {
            this.animator.SetBool("isWalking", true);
            isGreeting = false;
        }
        else
        {
            this.animator.SetBool("isWalking", false);
        }
    }

    public void SelectAndPlayGreetings(GreetingStyle greetingStyle)
    {
        if (isGreeting) return;

        this.animator.SetInteger(GREETINGS, (int)greetingStyle);
        AnimationStart(GetAnimationClipLength(greetingStyle.ToString()));
        Invoke(nameof(ReInitializeAnimationValues), .1f);
    }

    void ReInitializeAnimationValues()
    {
        this.animator.SetInteger(GREETINGS, -1);
    }

    float GetAnimationClipLength(string animationName)
    {
        float length = 0;
        foreach (AnimationClip animationClip in this.animator.runtimeAnimatorController.animationClips)
        {
            if (animationClip.name == animationName)
            {
                length = animationClip.length;
            }
        }
        return length;
    }

    void AnimationStart(float animationLength)
    {
        if (!photonView.IsMine) return;
        print("The Animation has Started!!");
        isGreeting = true;
        StartCoroutine(WaitTillAnimationEnds(animationLength));
    }

    IEnumerator WaitTillAnimationEnds(float animationLength)
    {
        yield return new WaitForSeconds(animationLength);
        AnimationEnd();
    }

    void AnimationEnd()
    {
        if (!photonView.IsMine) return;
        print("The Animation has Ended!!");
        isGreeting = false;
    }

    public void PlayFootSteps1()
    {
        if (!photonView.IsMine) return;
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = footSteps1;
        audioSource.Play();
    }

    public void PlayFootSteps2()
    {
        if (!photonView.IsMine) return;
        AudioSource audioSource = this.GetComponent<AudioSource>();
        audioSource.clip = footSteps1;
        audioSource.Play();
    }

    public enum GreetingStyle
    {
        Salute,
        Hi,
        ShakeHands,
        InformalBow,
        Taunt1,
        SwingDance,
        BreakDance,
        FistPump
    }
}
