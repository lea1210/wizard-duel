using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    
    public GameObject target;

    private void Start()
    {
        target = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        transform.LookAt(target.transform);

    }

    void removeLookAt()
    {
        target = null;
    }
}
