using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCameraCollision : MonoBehaviour
{
    [SerializeField]
    Player player;

    private void OnCollisionEnter(Collision collision)
    {

        player.OnCollison(collision);
    }

    private void Update()
    {
        
    }
}
