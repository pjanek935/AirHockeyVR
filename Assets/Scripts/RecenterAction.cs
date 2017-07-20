using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecenterAction : ButtonAction
{

    public GameObject mainCamera;

    public override void Action()
    {
        mainCamera.transform.eulerAngles = new Vector3(0, 0, 0);
        UnityEngine.VR.InputTracking.Recenter();
    }
}
