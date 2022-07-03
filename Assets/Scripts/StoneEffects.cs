using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneEffects : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource audioSource;
    [SerializeField]
    AudioClip throwStone;
    [SerializeField]
    AudioClip impactStone;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = throwStone;  
        audioSource.Play();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.name != "detected")
        {
            audioSource.clip = impactStone;
            audioSource.Play();
        }
    }
}
