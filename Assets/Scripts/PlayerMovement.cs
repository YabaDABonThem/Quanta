using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;

    public Animator animator;

    //Fields for WorldToggle class
    public WorldToggle worldToggle;

    public float RunSpeed = 40f;

    float HorizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    // Update is called once per frame
    void Update()
    {

        HorizontalMove = Input.GetAxisRaw("Horizontal") * RunSpeed;

        animator.SetFloat("SpeedX", Mathf.Abs(HorizontalMove));

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            animator.SetBool("Jump", true);
        }
        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            worldToggle.Toggle();
        }
        
        
    }
    public void OnLanding()
    {
        animator.SetBool("Jump", false);
    }
    public void OnCrouching(bool isCrouching)
    {
        animator.SetBool("Crouch", isCrouching);
    }
    void FixedUpdate ()
    {
        controller.Move(HorizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Interactable")
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Interactable1 interactable = collision.GetComponent<Interactable1>();
                if(interactable == null)
                {
                    Debug.Log("null");
                }
                interactable.dualConfirm++;
            }
        }
    }

}