using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voltureSpinner : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up, -1);
    }
}
