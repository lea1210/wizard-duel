using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellDisabler : MonoBehaviour
{



    private void OnCollisionEnter(Collision collision)
    {
        if (String.Compare(collision.gameObject.tag, "Spell") == 0 || String.Compare(collision.gameObject.tag, "OwnSpell") == 0)
        {
            if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString())
            {
                Destroy(collision.gameObject, 0.5f);
            }
            else
            {
                collision.gameObject.name = "detected";
            }

        }
    }
}
