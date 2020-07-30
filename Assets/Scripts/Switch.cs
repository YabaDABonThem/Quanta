using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * NOTE: Make sure to uncheck one of the layers from the culling mask, or else this script won't work. 
 */

public class Switch : MonoBehaviour
{
    Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        // Remember to attach this to the camera or ur a dum dum
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Mono")) // TODO: change to GetButtonDown later
        {
            // Debug.Log("Switched");
            // Switch between colored and monochrome.
            mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("MonochromeGround");
            mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("ColoredGround");
            // Change these names later, when we actually have assets. Smh we'd probably have some if a few art people weren't slow af
        }
    }
}
