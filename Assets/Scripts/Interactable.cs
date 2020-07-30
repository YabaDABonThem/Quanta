using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        gameObject.tag = "Interactable";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
