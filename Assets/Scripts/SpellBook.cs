using System;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;

public class SpellBook : MonoBehaviour
{
    
    public Dictionary<string,int> spellbook = new Dictionary<string,int>();
    private Vector3 spellCenter;
    public GameObject spellTargetWall;
    GameObject newSpellTargetWall;

    Vector3 spellCastStartPoint;

    [SerializeField]
    private GameObject stone;

    GameObject spellObject;

    [SerializeField]
    GameObject rightHand;



    // Start is called before the first frame update
    void Start()
    {
        //Fill Spellbook
        spellbook["circle"] = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createTargetField(List<Vector3> spellPositions, Point[] spellPoints)
    {
        float xAverage = 0;
        float yAverage = 0;
        float zAverage = 0;

        foreach(Vector3 vector3 in spellPositions)
        {
            xAverage = xAverage + vector3.x;
            yAverage = yAverage + vector3.y;
            zAverage = zAverage + vector3.z;    
        }
        Vector3 playerPosition = Camera.main.transform.position;
        
        spellCenter = new Vector3(xAverage/spellPositions.Count, yAverage / spellPositions.Count, zAverage / spellPositions.Count);
       
        
        Vector3 playerWallVektor = spellCenter - playerPosition;
        spellCenter += playerWallVektor;
        newSpellTargetWall = Instantiate(spellTargetWall, spellCenter, Quaternion.identity);
    }

    public void safeStartPoint(Vector3 startPoint)
    {
        spellCastStartPoint = startPoint;

    }

    public bool checkTargetField(Vector3 currentPosition)
    {
        if (Vector3.Distance(newSpellTargetWall.transform.position, currentPosition) < 0.1)
        {
            return true;
        }
        return false;
    }

    public void castSpell(string spellName)
    {
        Destroy(newSpellTargetWall);
        newSpellTargetWall = null;
        switch (spellbook[spellName])
        {
            case 1:
                summonStone();
                break;
            default:
                break;

        }
    }



    public void summonStone()
    {
        stone = Instantiate(stone,spellCenter,Quaternion.identity);
        Rigidbody body = stone.gameObject.GetComponent<Rigidbody>();
        
        Vector3 forceVector = (spellCenter - spellCastStartPoint);
        
        body.AddForce(forceVector*1000);
        body.useGravity = true;
    }

    public void destroyTargetWall()
    {
        if (newSpellTargetWall)
        {
            Destroy(newSpellTargetWall);
            newSpellTargetWall = null;
        }
    } 
}
