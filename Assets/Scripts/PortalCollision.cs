using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollision : MonoBehaviour
{
    [SerializeField]
    EventManager eventManager;

    [SerializeField]
    ArenaScenes.ArenaScene scene;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name =="VR Camera" || collision.gameObject.name == "WalkDetecter")
        {
            eventManager.invokePoratelCollision(scene);
        }
    }
}
