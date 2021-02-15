using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToggle : MonoBehaviour
{
    //access ripple VFX
    public ParticleSystem rippleFX;

    private Animator animator;
    private bool color = true;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    //trigger animation of mask (changes world)
    public void Toggle()
    {
        color = !color;
        animator.SetBool("Color", color);
        //instantiate the ripplefx
        GameObject rfx = Instantiate(rippleFX.gameObject, GetComponent<Transform>().position, Quaternion.identity);
    }
}
