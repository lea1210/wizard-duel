using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class BookManager : MonoBehaviour
{

    //Hand Tracking
    public XRNode rightHand;
    public InputHelpers.Button rightGripButton;
    bool rightGrip = false;
    public InputHelpers.Button textButton;

    public XRNode leftHand;
    public InputHelpers.Button leftGripButton;
    bool leftGrip = false;
    public InputHelpers.Button leftTriggerButton;

    public List<GameObject> pages = new List<GameObject> ();
    int currentPage = 1;
    public float inputThreshold = 0.1f;

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
    public List<AudioClip> audios = new List<AudioClip>();

    int openTimer = -1;
    [SerializeField]
    MovementRecognizer movementRecognizer;

    [SerializeField]
    SpellBook spellbook;



    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audios[UnityEngine.Random.Range(0,audios.Count)];
        open = false;
        rightGrip = false;
        leftGrip = false;
        Debug.Log("Spellbook: Level " + spellbook.getSpellbookLevel());
    }


    // Update is called once per frame
    void Update()
    {
        try
        {
       
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftTriggerButton, out bool leftTriggerIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), rightGripButton, out bool rightGripIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftGripButton, out bool leftGripIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), textButton, out bool textButtonIsPressed, inputThreshold);



            if (leftTriggerIsPressed)
            {
                openTimer = 30;
                Debug.Log("Book: Left Trigger Pressed");
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
                    Debug.Log("Page: " + currentPage);
                    closedBook.SetActive(true);
                    openBook.SetActive(false);
                    openTimer = -1;
                }
            }


            if (rightGripIsPressed != rightGrip)
            {
                Debug.Log("Book: Right Grip Pressed");
                rightGrip = rightGripIsPressed;
                if (rightGrip && open)
                {
                    if (currentPage < spellbook.getSpellbookLevel()-1)
                    {
                            animator.Play("PageTurn");
                            pages[currentPage].SetActive(false);
                            pages[currentPage+1].SetActive(true);
                            currentPage++; 
                            Debug.Log("Page: " + currentPage);
                            playAudio();
                            
                    }
                 
                }
            }
            if (!rightGripIsPressed)
                rightGrip = false;

            if (leftGripIsPressed != leftGrip)
            {
                Debug.Log("Book: Left Grip Pressed");
                leftGrip = leftGripIsPressed;
                if (leftGrip && open)
                {          
                    if(currentPage > 0)
                    {
                        animator.Play("PageTurnBack");
                        pages[currentPage].SetActive(false);
                        pages[currentPage-1].SetActive(true);
                        currentPage--;
                        Debug.Log("Page: " + currentPage);
                        playAudio();
                        
                    }
        
                }
            }
            if (!leftGripIsPressed)
                leftGrip = false;

            if (speechBubble && textButtonIsPressed)
            {
                Debug.Log("Book: Text Pressed");
                if (nextText != textButtonIsPressed)
                {
                    nextText = textButtonIsPressed;
                    chatbubble.nextText();
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

    public bool getOpen()
    {
        return open;
    }

    void playAudio()
    {
        audioSource.Play();
        audioSource.clip = audios[UnityEngine.Random.Range(0, 2)];
    }


}
