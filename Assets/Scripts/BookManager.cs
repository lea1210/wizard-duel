using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BookManager : MonoBehaviour
{

    //Hand Tracking
    [SerializeField]
    XRNode rightHand;
    [SerializeField]
    InputHelpers.Button rightGripButton;
    bool rightGrip = false;
    [SerializeField]
    InputHelpers.Button textButton;
    [SerializeField]
    XRNode leftHand;
    [SerializeField]
    InputHelpers.Button leftGripButton;
    bool leftGrip = false;
    [SerializeField]
    InputHelpers.Button leftTriggerButton;
    [SerializeField]
    List<GameObject> pages = new List<GameObject> ();
    int currentPage = 1;
    float inputThreshold = 0.1f;

    bool open = false;

    [SerializeField]
    GameObject openBook;
    [SerializeField]
    GameObject closedBook;

    [SerializeField]
    Animator animator;

    [SerializeField]
    GameObject speechBubble;
    [SerializeField]
    Chatbubble chatbubble;
    bool nextText;

    AudioSource audioSource;
    [SerializeField]
    List<AudioClip> audios = new List<AudioClip>();

    int openTimer = -1;
    [SerializeField]
    MovementRecognizer movementRecognizer;

    [SerializeField]
    SpellBook spellbook;

    //Debug
    [SerializeField]
    bool triggerDebug = false;
    [SerializeField]
    bool leftGripDebug = false;
    [SerializeField]
    bool rightGripDebug = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audios[UnityEngine.Random.Range(0,audios.Count)];
        open = false;
        rightGrip = false;
        leftGrip = false;
    }

    void Update()
    {
        try
        {
       
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftTriggerButton, out bool leftTriggerIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), rightGripButton, out bool rightGripIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftGripButton, out bool leftGripIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), textButton, out bool textButtonIsPressed, inputThreshold);

            //Debug
            if (triggerDebug)
            {
                leftTriggerIsPressed = true;
                triggerDebug = false;
            }
            if (leftGripDebug)
            {
                leftGripIsPressed = true;
                leftGripDebug = false;
            }
            if (rightGripDebug)
            {
                rightGripIsPressed = true;
                rightGripDebug = false;
            }

            if (leftTriggerIsPressed)
            {
                openTimer = 30;
            }

            if (!movementRecognizer.isWalking && openTimer > -1)
            {
                openTimer--;
            }
            else
            {
                openTimer = -1;
            }

            if(openTimer == 0)
            {
                open = !open;
                if (open)
                {
                    closedBook.SetActive(false);
                    openBook.SetActive(true);
                    pages[currentPage].SetActive(true);
                    openTimer = -1;
                }
                else
                {
                    closedBook.SetActive(true);
                    openBook.SetActive(false);
                    openTimer = -1;
                }
            }


            if (rightGripIsPressed != rightGrip)
            {
                rightGrip = rightGripIsPressed;
                if (rightGrip && open)
                {
                    if (currentPage < spellbook.getSpellbookLevel())
                    {
                            animator.Play("PageTurn");
                            pages[currentPage].SetActive(false);
                            pages[currentPage+1].SetActive(true);
                            currentPage++; 
                            PlayAudio();
                            
                    }
                 
                }
            }
            if (!rightGripIsPressed)
                rightGrip = false;

            if (leftGripIsPressed != leftGrip)
            {
                leftGrip = leftGripIsPressed;
                if (leftGrip && open)
                {          
                    if(currentPage > 0)
                    {
                        animator.Play("PageTurnBack");
                        pages[currentPage].SetActive(false);
                        pages[currentPage-1].SetActive(true);
                        currentPage--;
                        PlayAudio();
                        
                    }
        
                }
            }
            if (!leftGripIsPressed)
                leftGrip = false;

            if (speechBubble && textButtonIsPressed)
            {
                if (nextText != textButtonIsPressed)
                {
                    nextText = textButtonIsPressed;
                    chatbubble.NextText();
                }
            }
            else
            {
                nextText = false;
            }

        }catch(Exception e)
        {
            Debug.Log("Error Book:" + e.Message + " " + e.StackTrace);
        }




    }

    public bool GetOpen()
    {
        return open;
    }

    void PlayAudio()
    {
        audioSource.Play();
        audioSource.clip = audios[UnityEngine.Random.Range(0, 2)];
    }


}
