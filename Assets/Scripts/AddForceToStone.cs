using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceToStone : MonoBehaviour
{

    public void emitForce(Vector3 vector)
    {
        Rigidbody body = this.gameObject.GetComponent<Rigidbody>();
        body.AddForce(vector*100);
        body.useGravity = true;
    }
    
}
