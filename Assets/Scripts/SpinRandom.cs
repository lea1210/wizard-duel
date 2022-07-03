using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinRandom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.Rotate(new Vector3(0, Random.Range(0, 361), 0));
        gameObject.transform.localScale = transform.localScale * Random.Range(1f,2.1f);
    }

}
