using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkDetecter : MonoBehaviour
{
    [SerializeField]
    WalkManager manager;

    [SerializeField]
    MovementRecognizer recognizer;

    [SerializeField]
    Player player;

    [SerializeField]
    SpellBook SpellBook;
    private void OnCollisionEnter(Collision collision)
    {

        Debug.Log("Collission!");
        if(collision.gameObject.tag=="Spell")
        {
            player.OnCollison(collision);
        }
        if (collision.gameObject.CompareTag("PhysicalObject"))
        {
            manager.canWalk = false;

        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("PhysicalObject"))
            manager.canWalk = true;
        
    }

    public void ignoreSpell(GameObject spell)
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), spell.GetComponent<Collider>());
    }
}
