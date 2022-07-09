using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTargetCircle : MonoBehaviour
{

    [SerializeField]
    SpellBook spellBook;
    [SerializeField]
    bool isPlayer;
    void Update()
    {
        if (isPlayer)
        {
            gameObject.transform.Rotate(new Vector3(0, 0, 0.3f));
        }
        else
        {
            if (!spellBook.GetTimeStopped())
            {
                gameObject.transform.Rotate(new Vector3(0, 0, 0.3f));
            }
        }
    }
}
