using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraCollision : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "OwnSpell")
            player.OnCollison(collision);
    }

    public void ignoreSpell(GameObject spell)
    {
        Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), spell.GetComponent<Collider>());
    }
}
