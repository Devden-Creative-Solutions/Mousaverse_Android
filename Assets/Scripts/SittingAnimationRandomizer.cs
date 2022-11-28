using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SittingAnimationRandomizer : MonoBehaviour
{
    Coroutine cr;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        Invoke("DelayedStartCR", Random.Range(0, 25));
    }

    void DelayedStartCR()
    {
        if (gameObject.activeInHierarchy)
            cr = StartCoroutine(nameof(StartSittingAnimation));
    }

    IEnumerator StartSittingAnimation()
    {
        while (true)
        {
            var delay = Random.Range(5, 50);
            yield return new WaitForSeconds(delay);

            anim.SetInteger("SittingType", Random.Range(0, 5));
            anim.SetTrigger("Sit");
        }
    }

    private void OnDisable()
    {
        if (cr != null)
            StopCoroutine(cr);
        cr = null;
    }
}
