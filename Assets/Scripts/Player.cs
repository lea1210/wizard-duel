using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    float currentHealth;
    [SerializeField]
    float maxHealth = 100f;
    [SerializeField]
    RectTransform healthbar;

    Animator animator;

    AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth >= 0)
            UpdateHealth();
    }

    public void OnCollison(Collision collision)
    {
        if (collision.gameObject.tag != "OwnSpell")
        {   
            switch (collision.gameObject.name)
            {
                //case SpellTypes.SpellType.circle.ToString():
                case "circle":
                    currentHealth -= (int)SpellTypes.SpellType.circle;
                    animator.Play("DamageShake");
                    break;
                case "fire":
                    currentHealth -= (int)SpellTypes.SpellType.fire;
                    animator.Play("DamageShake");
                    break;
            }

            
            if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString())
            {
                collision.gameObject.GetComponent<MeshRenderer>().enabled = false;
                Destroy(collision.gameObject, 1f);
                
            }
            else if (String.Compare(collision.gameObject.tag, "Spell") == 0)
            {
                collision.gameObject.name = "detected";
            }
        }
    }


    void UpdateHealth()
    {
        float healthRatio = currentHealth / maxHealth;
        float value = 3 - 3 * healthRatio;
        healthbar.offsetMin = new Vector2(3 - 3 * healthRatio, healthbar.offsetMin.y);
        
    }

    public void playSound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }


}
