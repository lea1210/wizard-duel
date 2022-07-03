using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTargetCircleForTarget : MonoBehaviour
{
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(0, 0.3f, 0));
    }
}
