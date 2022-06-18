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
    public String currentSpell = "";
    int spellCooldown = 0;
    //#################################################################
    //Walk Handling
    public bool isWalking = false;
    [SerializeField]
    WalkManager walkManager;


    // Start is called before the first frame update
    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach(var item in gestureFiles)
        {
            gestureList.Add(GestureIO.ReadGestureFromFile(item));
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(rightHand), rightTriggerButton, out bool rightTriggerIsPressed, inputThreshold);
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftTriggerButton, out bool leftTriggerIsPressed, inputThreshold);

        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(leftHand), leftGrabButton, out bool restart, inputThreshold);

        if (restart)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                walkManager.testHandsForWalking(leftMovementSource.position,rightMovementSource.position);
            }

        }
        else
        {
            isWalking = false;
            if (spellCooldown == 0)
            {
                //spellBook.destroyTargetWall();
                //Move Start
                if (!isMoving && rightTriggerIsPressed)
                {
                    StartMovement();
                }

                //End Move
                if (isMoving && !rightTriggerIsPressed)
                {
                    EndMovement();
                }

                // in Move
                if (isMoving && rightTriggerIsPressed)
                {
                    inMove();

                }
            }
            else
            {
                spellCooldown--;
            }
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
                spellBook.safeStartPoint(rightMovementSource.position);
            }
        }catch(Exception e)
        {
            Debug.Log("Error 1:" + e.Message + " " + e.StackTrace); Debug.Log("Error:" + e.Message);
        }
    }

    void inMove()
    {
        try
        {
            if (String.Equals(currentSpell, ""))
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
                if (spellBook.checkTargetField(rightMovementSource.position))
                {
                    spellBook.castSpell(currentSpell);
                    currentSpell = "";
                    spellCooldown = 60;
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
                    creationMode = false;
                }
                else
                {

                    Debug.Log("Mode:Recognize Mode");
                    Result result = PointCloudRecognizer.Classify(newGesture, gestureList.ToArray());

                    if (result.Score > recognitionThreshold)
                    {
                        // onRecognized.Invoke(result.GestureClass);
                        currentSpell = result.GestureClass;
                        Debug.Log("Spell:" + currentSpell);
                        spellBook.createTargetField(positionList, pointArray);

                    }

                }
            }
        }catch(Exception e)
        {
            Debug.Log("Error 2:" + e.Message+" "+e.StackTrace);
        }
    }

 
}
