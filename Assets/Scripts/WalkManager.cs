using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkManager : MonoBehaviour
{
    private Vector3 rightStartPoint;
    private Vector3 leftStartPoint;
    [SerializeField]
    private float stepThreshhold = 0.2f;
    private hands activeHand = hands.rightHand;
    private enum hands { leftHand, rightHand,both };
    [SerializeField]
    private float stepLength = 0.5f;
    [SerializeField]
    private float stepPerSecond = 1f;

    int walkResetTimer = 0;
    int stepTimer = 0;

    GameObject player;
 
    void Update()
    {

        if (stepTimer > 0)
        {
            walkResetTimer = 90;
            movePlayer();
            stepTimer--;
            
        }
        else
        {
            Camera.main.fieldOfView = 60;
            if (walkResetTimer >= 0)
            {
                walkResetTimer--;
            }
            else
            {
                activeHand = hands.both;
            }

        }

    }

    public void setRightStartPoint(Vector3 point)
    {
        rightStartPoint = point;
    }

    public void setLeftStartPoint(Vector3 point)
    {
        leftStartPoint = point;
    }

    public void testHandsForWalking(Vector3 leftHandsPos, Vector3 rightHandPos)
    {
        switch (activeHand)
        {
            case hands.leftHand:
                if(calculateDistance(leftStartPoint, leftHandsPos))
                {
                    setStepTimer();
                    activeHand = hands.rightHand;
                }
                break;
            case hands.rightHand:
                if (calculateDistance(rightStartPoint, rightHandPos))
                {
                    setStepTimer();
                    activeHand = hands.rightHand;
                }
                break;
            case hands.both:
                if (calculateDistance(leftStartPoint, leftHandsPos))
                {
                    setStepTimer();
                    activeHand = hands.rightHand;
                }
                else if (calculateDistance(rightStartPoint, rightHandPos))
                {
                    setStepTimer();
                    activeHand = hands.rightHand;
                }
                break;
        }
    }

    private bool calculateDistance(Vector3 startPos,Vector3 currentPos)
    {
        Debug.Log("Distance: " + Mathf.Sqrt((startPos.y - currentPos.y) * 2));
        if (Mathf.Sqrt((startPos.y - currentPos.y) * 2) >= stepThreshhold){
            return true;
        }

        return false;
    }

    private void setStepTimer()
    {
        stepTimer = Application.targetFrameRate;
    }

    private void movePlayer()
    {
      float distance = stepLength * stepPerSecond * Time.deltaTime;
      player.transform.position = player.transform.position + new Vector3(Camera.main.transform.forward.x * distance, 0, Camera.main.transform.forward.z * distance);
      Camera.main.fieldOfView = 70;
    }


}
