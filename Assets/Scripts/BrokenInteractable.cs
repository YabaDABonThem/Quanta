using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenInteractable : MonoBehaviour
{
    // Hopefully you need an animator but honestly I'm not sure what it does
    public Animator animator;

    // What's dualConfirm? Well since I have no clue I'm just gonna not include it rn

    // You need a function to tell if something is intereacting
    public void run()
    {
        animator.SetBool("Interact", true);
    }





    // I don't know if I even need this stuff below
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
