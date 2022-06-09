using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceToStone : MonoBehaviour
{

    int timer = 0;
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        timer++;
        if(timer == 150)
        {
            Rigidbody body = this.gameObject.GetComponent<Rigidbody>();
            body.AddForce(new Vector3(0, 200, 100));
            body.useGravity = true;
        }
    }
}
