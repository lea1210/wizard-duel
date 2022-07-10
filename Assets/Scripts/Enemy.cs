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
    GameObject enemyModel;

    [SerializeField]
    bool trainMode = false;
    [SerializeField]
    bool poisonMode = false;

    int damageTimer = 150; 


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
        if (poisonMode)
        {
            currentHealth -= 1f;
        }
        if(currentHealth >= 0)
            UpdateHealth();

        if (trainMode)
        {
            damageTimer++;
            if(damageTimer >= 450)
            {
                if(currentHealth < maxHealth)
                    currentHealth += 1f;
            }
            else if(currentHealth < 30)
            {
                if (currentHealth < maxHealth)
                    currentHealth += 10f;
            }
        }
        
    }

    public void OnCollison(Collision collision)
    {
        if (collision.gameObject.tag == "OwnSpell")
        {
            switch (collision.gameObject.name)
            {
                case "stone":
                    currentHealth -= (int)SpellTypes.SpellType.stone;
                    break;
                case "fire":
                    currentHealth -= (int)SpellTypes.SpellType.fire;
                    break;
            }
        }
        if (collision.gameObject.name != "detected")
        {
            damageTimer = 0;
            if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString() && collision.gameObject.tag != "Spell")
            {
                GameObject child = collision.gameObject.transform.GetChild(0).gameObject;
                child.SetActive(false);
                Destroy(collision.gameObject, 1.5f);
            }
            else if (String.Compare(collision.gameObject.tag, "OwnSpell") == 0)
            {
                collision.gameObject.name = "detected";
            }
        }
    }



    void UpdateHealth()
    {
        float healthRatio = currentHealth / maxHealth;
        float value = 3 - 3 * healthRatio;
        healthbar.offsetMin = new Vector2 (9 - 9* healthRatio, healthbar.offsetMin.y);

    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }


}
