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

    [SerializeField]
    private float movementSpeed = 0.005f;
    float baseMovementSpeed;
    Animator animator;

    [SerializeField]
    Animator fadeAnimator;

    AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        baseMovementSpeed = movementSpeed;
        playFadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 0)
            currentHealth = 0;
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

            
            if (collision.gameObject.name == SpellTypes.SpellType.fire.ToString() && collision.gameObject.tag != "OwnSpell")
            {
                GameObject child = collision.gameObject.transform.GetChild(0).gameObject;
                child.SetActive(false);
                Destroy(collision.gameObject, 1.5f);
                
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

    public float GetMovementSpeed()
    {
        return movementSpeed;
    }

    public void modifyMovementSpeed(float modifyer)
    {
        movementSpeed = movementSpeed * modifyer;   
    }
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public void playFadeOut()
    {
        fadeAnimator.Play("FadeOut");
    }


    public void playFadeIn()
    {
        fadeAnimator.Play("FadeIn");
    }

    public void ResetMovementSpeed()
    {
        movementSpeed = baseMovementSpeed;
    }


}
