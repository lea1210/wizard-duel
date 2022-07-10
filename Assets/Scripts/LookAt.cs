using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    
    public GameObject target;
    [SerializeField]
    private SpellBook spellbook;



    private void Start()
    {   

        if(target == null)
            target = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (spellbook != null)
        {
            if (target && !spellbook.GetTimeStopped())
                transform.LookAt(target.transform);
                if(gameObject.name == "Enemy")
                {
                    gameObject.transform.rotation = new Quaternion(0, gameObject.transform.rotation.y, 0, gameObject.transform.rotation.w);
                }
        }
        else
        {
            if(target != null)
                transform.LookAt(target.transform);
        }

    }

    void removeLookAt()
    {
        target = null;
    }
}
