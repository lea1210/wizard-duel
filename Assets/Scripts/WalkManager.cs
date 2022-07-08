using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkManager : MonoBehaviour
{
    private Vector3 rightStartPoint;
    private Vector3 leftStartPoint;

    private float lastRightPercentage;
    private float lastLeftPercentage;  

    [SerializeField]
    private float fullStepTreshhold = 0.25f;

    private bool useHalfTreshhold = false;

    [SerializeField]
    GameObject playerObj;
    Rigidbody playerBody;
    [SerializeField]
    Player player;

    AudioSource audioSource;
    public List<AudioClip> audios = new List<AudioClip>();

    float summDistance = 0;

    int side = -1;

    [SerializeField]
    BookManager bookManager;

    bool leftHandUp = false;
    bool rightHandUp = false;


    public bool canWalk;

    private void Start()
    {
        playerBody = playerObj.gameObject.GetComponent<Rigidbody>();
        canWalk = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audios[UnityEngine.Random.Range(0, audios.Count)];
    }

    public void setRightStartPoint(Vector3 point)
    {
        rightStartPoint = point;
        useHalfTreshhold=true;
    }

    public void setLeftStartPoint(Vector3 point)
    {
        leftStartPoint = point;
        useHalfTreshhold = true;

    }

   

    public void testHandsForWalking(Vector3 leftHandsPos, Vector3 rightHandPos)
    {
        float rightPercentage = calculateDistance(rightStartPoint, rightHandPos,false);
        float leftPercentage = calculateDistance(leftStartPoint, leftHandsPos, true);


            if (rightPercentage > 0.5)
            {
                movePlayer(rightPercentage);
                side = 1;
            }
        

   
            if (leftPercentage > 0.5)
            {
                movePlayer(leftPercentage);
                side = 2;
            }
       

        if (useHalfTreshhold)
        {
            detectNewStartPoint(leftPercentage,rightPercentage, leftHandsPos, rightHandPos);
        }
    }

    private float calculateDistance(Vector3 startPos, Vector3 currentPos, bool leftSide)
    {

        float percentage = 0;
        Debug.Log("Distance: " + Mathf.Sqrt((startPos.y - currentPos.y) * 2));
        if (useHalfTreshhold)
        {

            percentage = (startPos.y - currentPos.y) / (fullStepTreshhold / 2);
            return Math.Abs(percentage) > 1 ? 1 : Math.Abs(percentage);
        }
        else
        {
            percentage = (startPos.y - currentPos.y) / fullStepTreshhold;
            return percentage > 1 ? 1 : percentage;
        }
    }

    private void detectNewStartPoint(float leftPercentage , float rightPercentage, Vector3 leftHandsPos, Vector3 rightHandPos)
    {
        if(leftPercentage < lastLeftPercentage)
        {
            useHalfTreshhold = false;
            leftStartPoint = leftHandsPos;
        }
        if(rightPercentage < lastRightPercentage)
        {
            useHalfTreshhold=false;
            rightStartPoint = rightHandPos;
        }

    }

    public void movePlayer(float movePercentage)
    {
        if (canWalk)
        {
            float multipler = 1f;
            if (!bookManager.getOpen())
                multipler = 1.5f;
            float distance = player.GetMovementSpeed() * movePercentage * multipler;
            playerBody.transform.Translate(new Vector3(Camera.main.transform.forward.x * distance, 0, Camera.main.transform.forward.z * distance),Space.World);
            Camera.main.fieldOfView = 70;
            if(summDistance > player.GetMovementSpeed()*10)
            {
                playAudio();
                summDistance = 0;
            }
            else
            {
                summDistance += distance;
            }
            
        }
    }


    void playAudio()
    {
        audioSource.Play();
        audioSource.clip = audios[UnityEngine.Random.Range(0, audios.Count)];
    }



}
