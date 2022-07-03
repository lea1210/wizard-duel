using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackEnemyCollision : MonoBehaviour
{
    [SerializeField]
    Enemy enemy;

 

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enemy: Collision with " + collision.gameObject.name);
        enemy.OnCollison(collision);
    }

    private void Update()
    {
        
    }
}
