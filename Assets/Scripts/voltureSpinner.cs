using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voltureSpinner : MonoBehaviour
{

    [SerializeField]
    SpellBook spellBook;
    void Update()
    {
        if(spellBook.GetTimeStopped() == false)
            gameObject.transform.Rotate(Vector3.up, -0.5f);
    }
}
