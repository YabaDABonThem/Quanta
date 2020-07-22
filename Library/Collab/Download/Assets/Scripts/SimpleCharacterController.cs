using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    public float speed = 5.0f;
    public ParticleSystem rippleFX;

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    Vector2 move;

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        //vertical = Input.GetAxis("Vetical");



        move = new Vector2(horizontal, 0);
        if(!Mathf.Approximately(move.x, 0f) || !Mathf.Approximately(move.y, 0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        else
        {
            lookDirection.Set(move.x, move.y);
        }

        Check();
        WorldToggle();
    }

    private void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + horizontal * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
        animator.SetFloat("MoveX", lookDirection.x);

    }

    private void WorldToggle()
    {
        if(Input.GetKeyDown(KeyCode.G))
        {
            GameObject rfx = Instantiate(rippleFX.gameObject, rigidbody2d.position, Quaternion.identity);
        }
    }

    private void Check()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(lookDirection.x);
        }
    }
}
