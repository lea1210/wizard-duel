using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballEffects : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audioSource;
    [SerializeField]
    AudioClip throwFireball;
    [SerializeField]
    AudioClip fireballExplosion;



    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = throwFireball;
        audioSource.Play();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(gameObject.tag == "Spell")
        {
            if(collision.gameObject.tag != "Enemy")
            {
                audioSource.clip = fireballExplosion;
                audioSource.Play();
            }
        }
        if (gameObject.tag == "OwnSpell")
        {
            if (collision.gameObject.tag != "MainCamera")
            {
                audioSource.clip = fireballExplosion;
                audioSource.Play();
            }
        }

    }

}
