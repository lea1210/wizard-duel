using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;

public class MovementRecognizer : MonoBehaviour
{
    //Hand Tracking
    public XRNode rightHand;
    public InputHelpers.Button rightTriggerButton;
    public Transform rightMovementSource;

    public XRNode leftHand;
    public InputHelpers.Button leftGrabButton;
    public InputHelpers.Button leftTriggerButton;
    public Transform leftMovementSource;
    public InputHelpers.Button leftPrimaryButton;
    public InputHelpers.Button leftSecondaryButton;

    public float inputThreshold = 0.1f;
    //#################################################################
    //Spell Handling
    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = false;
    public string newGestureName;
    private List<Gesture> gestureList = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionList = new List<Vector3>();
    public float recognitionThreshold = 0.9f;
    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent onRecognized;
    public SpellBook spellBook;
    public string currentSpell = "";
    int spellCooldown = 0;
    //#################################################################
    //Walk Handling
    public bool isWalking = false;
    [SerializeField]
    WalkManager walkManager;

    [SerializeField]
    WalkDetecter walkDetecter;

    [SerializeField]
    AudioClip spawnTargetCircle;
    [SerializeField]
    Player player;
    //#################################################################
    //Audio Handling
    AudioSource audioSource;

    //Debug
    [SerializeField]
    bool castSpeed;

    [SerializeField]
    bool castTime;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach(var item in gestureFiles)
        {
            gestureList.Add(GestureIO.ReadGestureFromFile(item));
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), rightTriggerButton, out bool rightTriggerIsPressed, inputThreshold);
            InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftTriggerButton, out bool leftTriggerIsPressed, inputThreshold);

            if (castSpeed)
            {
                spellBook.CastSpell("speed");
                castSpeed = false;
            }
            if (castTime)
            {
                spellBook.CastSpell("timeStop");
                castTime = false;
            }
  

            if (rightTriggerIsPressed && leftTriggerIsPressed)
            {
                if (!isWalking)
                {
                    walkManager.setLeftStartPoint(leftMovementSource.position);
                    walkManager.setRightStartPoint(rightMovementSource.position);

                    isWalking = true;

                }

                if (isWalking)
                {
                    walkManager.testHandsForWalking(leftMovementSource.position, rightMovementSource.position);
                }

            }
            else
            {
                Camera.main.fieldOfView = 60;

                isWalking = false;
                if (spellCooldown == 1)
                    isMoving = false;
                if (spellCooldown == 0)
                {
                    //Move Start
                    if (!isMoving && rightTriggerIsPressed)
                    {
                        StartMovement();
                    }

                    // in Move
                    if (isMoving && rightTriggerIsPressed)
                    {
                        inMove();

                    }

                    //End Move
                    if (isMoving && !rightTriggerIsPressed)
                    {
                        EndMovement();
                    }


                }
                else
                {
                    spellCooldown--;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error Move:" + e.Message + " " + e.StackTrace);
        }

    }

    void StartMovement()
    {
        try
        {

            isMoving = true;
            if (String.Equals(currentSpell,""))
            {

                positionList.Clear();
                positionList.Add(rightMovementSource.position);
            
                if (debugCubePrefab)
                   Destroy(Instantiate(debugCubePrefab, rightMovementSource.position, Quaternion.identity), 3);
            }
            else
            {
                spellBook.SafeStartPoint(rightMovementSource.position);
            }
        }catch(Exception e)
        {
            Debug.Log("Error 1:" + e.Message + " " + e.StackTrace); 
        }
    }

    void inMove()
    {
        try
        {
            if (String.Equals(currentSpell, "") && spellCooldown == 0)
            {
                Vector3 lastPos = positionList[positionList.Count - 1];
                if (Vector3.Distance(rightMovementSource.position, lastPos) > newPositionThresholdDistance)
                {

                    if (debugCubePrefab)
                    {
                        Destroy(Instantiate(debugCubePrefab, rightMovementSource.position, Quaternion.identity), 3);

                    }
                    positionList.Add(rightMovementSource.position);
                }
            }
            else
            {
                if (spellBook.CheckTargetField(rightMovementSource.position))
                {
                    spellBook.CastSpell(currentSpell);
                    currentSpell = "";
                    spellCooldown = 120;

                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error 3:" + e.Message + " " + e.StackTrace);
        }
    }

    void EndMovement()
    {
        try
        {
            
            isMoving = false;
            if (String.Equals(currentSpell, ""))
            {
                //Create gesture 
                Point[] pointArray = new Point[positionList.Count];
                for (int i = 0; i < positionList.Count; i++)
                {
                    Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);
                    pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
                }

                Gesture newGesture = new Gesture(pointArray);

                if (creationMode)
                {
                    Debug.Log("Mode:Creation Mode");
                    newGesture.Name = newGestureName;
                    gestureList.Add(newGesture);
                    string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
                    GestureIO.WriteGesture(pointArray, newGestureName, fileName);
                }
                else
                {

                    Result result = PointCloudRecognizer.Classify(newGesture, gestureList.ToArray());
   
                    if (result.Score > recognitionThreshold && spellCooldown == 0)
                    {

                        currentSpell = result.GestureClass;

                        if (spellBook.testSpellUnlocked(currentSpell))
                        {
                            if (spellBook.TestForTargetableSpell(currentSpell))
                            {

                                player.playSound(spawnTargetCircle);
                                spellBook.CreateTargetField(positionList, pointArray);
                                
                            }
                            else
                            {
                                

                                spellBook.CastSpell(currentSpell);
                                currentSpell = "";
                              
                            }
                            positionList.Clear();

                        }
                        else
                        {
                            positionList.Clear();
                            audioSource.Play();
                            currentSpell = "";
                        }

                    }
                    else
                    {
                        positionList.Clear();
                        audioSource.Play();
                    }

                }
            }
        }catch(Exception e)
        {
            Debug.Log("Error 2:" + e.Message+" "+e.StackTrace);
        }
    }

 
}
