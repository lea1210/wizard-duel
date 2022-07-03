using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Enemy : MonoBehaviour
{

    float currentHealth;
    [SerializeField]
    float maxHealth = 100f;
    [SerializeField]
    RectTransform healthbar;
    [SerializeField]
    public GameObject enemyModel;


    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth >= 0)
            UpdateHealth();
        
    }

    public void OnCollison(Collision collision)
    {
        switch (collision.gameObject.name)
        {
            //case SpellTypes.SpellType.circle.ToString():
            case "circle":
                currentHealth -= (int)SpellTypes.SpellType.circle;
                break;
            case "fire":
                currentHealth -= (int)SpellTypes.SpellType.fire;
                break;
        }
        audioSource.Play();
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString())
        {
            Destroy(collision.gameObject);
        }
        else if (String.Compare(collision.gameObject.tag, "Spell") == 0 || String.Compare(collision.gameObject.tag, "OwnSpell") == 0)
        {


            collision.gameObject.name = "detected";
        }
    }



    void UpdateHealth()
    {
        float healthRatio = currentHealth / maxHealth;
        float value = 3 - 3 * healthRatio;
        healthbar.offsetMin = new Vector2 (9 - 9* healthRatio, healthbar.offsetMin.y);

    }

    public void playSound(AudioClip sound)
    {
        Debug.Log("Play Sound: " + sound.name); 
        audioSource.clip = sound;
        audioSource.Play();
    }


}
