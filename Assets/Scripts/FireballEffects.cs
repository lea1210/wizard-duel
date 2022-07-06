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
        audioSource.clip = fireballExplosion;
        audioSource.Play();
    }

}
