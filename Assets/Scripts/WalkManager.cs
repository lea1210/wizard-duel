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
    public float stepLength = 0.005f;

    [SerializeField]
    private float fullStepTreshhold = 0.25f;

    private bool useHalfTreshhold = false;

    [SerializeField]
    GameObject player;
    Rigidbody playerBody;

    AudioSource audioSource;
    public List<AudioClip> audios = new List<AudioClip>();




    public bool canWalk;

    private void Start()
    {
        playerBody = player.gameObject.GetComponent<Rigidbody>();
        canWalk = true;
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audios[Random.Range(0, audios.Count)];
    }

    void Update()
    {

       // Debug.Log("CanWalk: " + canWalk);

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
   

        float rightPercentage = calculateDistance(rightStartPoint, rightHandPos);
        if(rightPercentage>0.5)
        movePlayer(rightPercentage);

        float leftPercentage = calculateDistance(leftStartPoint,leftHandsPos);
        if(leftPercentage>0.5)
        movePlayer(leftPercentage);

        if (useHalfTreshhold)
        {
            detectNewStartPoint(leftPercentage,rightPercentage, leftHandsPos, rightHandPos);
        }
    }

    private float calculateDistance(Vector3 startPos,Vector3 currentPos)
    {

        float percentage = 0;
        Debug.Log("Distance: " + Mathf.Sqrt((startPos.y - currentPos.y) * 2));
        if (useHalfTreshhold)
        {
            percentage = Mathf.Abs((startPos.y - currentPos.y)) / (fullStepTreshhold / 2);
            return percentage > 1 ? 1 : percentage;
        }
        else
        {
            percentage = Mathf.Abs((startPos.y - currentPos.y)) / fullStepTreshhold;
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
            float distance = stepLength * movePercentage;
            //player.transform.Translate(new Vector3(Camera.main.transform.forward.x * distance, 0, Camera.main.transform.forward.z * distance));
            playerBody.transform.Translate(new Vector3(Camera.main.transform.forward.x * distance, 0, Camera.main.transform.forward.z * distance),Space.World);
            Camera.main.fieldOfView = 70;
        }
    }


    void playAudio()
    {
        audioSource.Play();
        audioSource.clip = audios[UnityEngine.Random.Range(0, audios.Count)];
    }



}
