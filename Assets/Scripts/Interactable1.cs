using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable1 : MonoBehaviour
{
    public Animator animator;

    public int dualConfirm = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (dualConfirm == 2)
        {
            run();
            dualConfirm -= 2;
        }
    }

    public void run()
    {
        animator.SetBool("Interact", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>() != null) 
        {
            dualConfirm++;
        }
    }

}
