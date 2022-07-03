using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDisabler : MonoBehaviour
{



    private void OnCollisionEnter(Collision collision)
    {
        if (String.Compare(collision.gameObject.tag, "Spell") == 0)
        {
            Debug.Log(collision.gameObject.name);
            if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString())
            {
                collision.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Destroy(collision.gameObject, 1f);
            }
            else
            {
                collision.gameObject.name = "detected";
            }

        }
    }
}