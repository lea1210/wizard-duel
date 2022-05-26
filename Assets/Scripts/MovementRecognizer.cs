using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;
public class MovementRecognizer : MonoBehaviour
{

    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public Transform movementSource;

    public float newPositionThresholdDistance = 0.05f;
    public GameObject debugCubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    private List<Gesture> gestureList = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionList = new List<Vector3>();

    public float recognitionThreshold = 0.9f;

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent onRecognized;

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
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        
        //Move Start
        if(!isMoving && isPressed)
        {
            StartMovement();
        }

        //End Move
        if(isMoving && !isPressed)
        {
            EndMovement();
        }

        // in Move
        if(isMoving && isPressed)
        {
            inMove();

        }
    
    }

    void StartMovement()
    {
        Debug.Log("Start");
        isMoving = true;
        positionList.Clear();   
        positionList.Add(movementSource.position);

        if(debugCubePrefab)
            Destroy(Instantiate(debugCubePrefab,movementSource.position, Quaternion.identity),5);
    }

    void EndMovement()
    {
        Debug.Log("Ende");
        isMoving = false;

        //Create gesture 
        Point[] pointArray = new Point[positionList.Count];
        for(int i = 0; i < positionList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);

        if (creationMode)
        {
            newGesture.Name = newGestureName;
            gestureList.Add(newGesture);
            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray,newGestureName,fileName);
        }
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture,gestureList.ToArray());
            Debug.Log(result.GestureClass + result.Score);
            if(result.Score > recognitionThreshold)
            {
                onRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void inMove()
    {
        Debug.Log("Movin");
        Vector3 lastPos = positionList[positionList.Count - 1];
        if (Vector3.Distance(movementSource.position, lastPos) > newPositionThresholdDistance)
        {
            if (debugCubePrefab)
                Destroy(Instantiate(debugCubePrefab, movementSource.position, Quaternion.identity), 5);
            positionList.Add(movementSource.position);
        }
    }
}
