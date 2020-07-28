using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    bool switched = false;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("h"))
        {
            switched = !switched;
            Debug.Log("Switched");
        }

        if (switched)
        {
            // Switch between colored and monochrome.
            mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("MonochromeGround");
            mainCamera.cullingMask ^= 1 << LayerMask.NameToLayer("ColoredGround");
        }
    }
}
