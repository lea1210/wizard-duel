using System;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine;
using UnityEngine.Events;

public class SpellBook : MonoBehaviour
{
    
    public Dictionary<string,int> spellbook = new Dictionary<string,int>();
    public List<string> nonTargetableSpells = new List<string>();
    private Vector3 spellCenter;

    Vector3 spellCastStartPoint;
    //Objects
    [SerializeField]
    private GameObject spellTargetWall;
    private GameObject tempSpellTargetWall;

    [SerializeField]
    private GameObject enemySpellTargetWall;
    private GameObject enemyTempSpellTargetWall;

    [SerializeField]
    private GameObject stone;
    private GameObject tempStone;

    [SerializeField]
    private GameObject fireball;
    private GameObject tempFireball;

  
  public UnityEvent playerCastEvent;
    [SerializeField]
    WalkDetecter walkDetecter;

    [SerializeField]
    WalkManager playerWalk;
    int speedTimerPlayer = -1;


    [SerializeField]
    EnemyManager lastCaster;
    int speedTimerEnemy = -1;


    [SerializeField]
    GameObject passiveEffectEnemy;
    [SerializeField]
    GameObject passiveEffectPlayer;

    AudioSource audioSource;
    [SerializeField]
    AudioClip speedUp;

    // Start is called before the first frame update
    void Start()
    {
        //Fill Spellbook
        spellbook[SpellTypes.SpellType.circle.ToString()] = 1;
        spellbook[SpellTypes.SpellType.fire.ToString()] = 2;
        spellbook[SpellTypes.SpellType.speed.ToString()] = 3;

        nonTargetableSpells.Add(SpellTypes.SpellType.speed.ToString());

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(speedTimerPlayer > 0)
        {
            speedTimerPlayer--;
        }
        else if(speedTimerPlayer==0)
        {
            playerWalk.stepLength *= 0.5f;
            speedTimerPlayer = -1;
            passiveEffectPlayer.SetActive(false);
        }

        if (speedTimerEnemy > 0)
        {
            speedTimerEnemy--;
        }
        else if (speedTimerEnemy == 0)
        {
            lastCaster.movementSpeed *= 0.5f;
            speedTimerEnemy = -1;
            passiveEffectEnemy.SetActive(false);
        }

    }

    public bool TestForTargetableSpell(string spell,EnemyManager caster = null)
    {
        if (caster)
        {
            if (nonTargetableSpells.Contains(spell))
            {
                Debug.Log("Return false");
                return false;
            }
            return true;
        }
        else
        {
            if (nonTargetableSpells.Contains(spell))
            {
                return false;
            }
            return true;
        }

    }

    public void CreateTargetField(List<Vector3> spellPositions, Point[] spellPoints)
    {
        try
        {
            if (tempSpellTargetWall == null)
            {
                float xAverage = 0;
                float yAverage = 0;
                float zAverage = 0;

                foreach (Vector3 vector3 in spellPositions)
                {
                    xAverage = xAverage + vector3.x;
                    yAverage = yAverage + vector3.y;
                    zAverage = zAverage + vector3.z;
                }

                Vector3 playerPosition = Camera.main.transform.position;

                spellCenter = new Vector3(xAverage / spellPositions.Count, yAverage / spellPositions.Count, zAverage / spellPositions.Count);


                Vector3 playerWallVektor = spellCenter - playerPosition;

                spellCenter += playerWallVektor;
                tempSpellTargetWall = Instantiate(spellTargetWall, spellCenter, Quaternion.identity);
            }
        }catch(Exception ex)
        {
            Debug.Log("Error TargetWall: " + ex.Message + " " + ex.StackTrace);
        }
    }

    public void createEnemyTargetField(Enemy caster)
    {
        if(enemyTempSpellTargetWall == null)
        enemyTempSpellTargetWall = Instantiate(enemySpellTargetWall,caster.transform.position+caster.transform.forward, Quaternion.identity);
    }

    public void SafeStartPoint(Vector3 startPoint)
    {
        spellCastStartPoint = startPoint;

    }

    public bool CheckTargetField(Vector3 currentPosition)
    {
        if (tempSpellTargetWall)
        {
            if (Vector3.Distance(tempSpellTargetWall.transform.position, currentPosition) < 0.1)
            {
                return true;
            }
        }
        return false;
    }

    public void CastSpell(string spellName,EnemyManager caster = null)
    {
        try
        {
            Vector3 center;
            Vector3 spellCastPoint;

            if (caster)
            {
                if (enemyTempSpellTargetWall != null)
                    center = enemyTempSpellTargetWall.transform.position;
                else
                    center = Vector3.zero;
                spellCastPoint = caster.getSpellCastPoint();

                switch (spellbook[spellName])
                {
                    case 1:
                        SummonStone(center, spellCastPoint,caster);
                        Destroy(enemyTempSpellTargetWall,0.5f);
                        break;
                    case 2:
                        SummonFireball(center, spellCastPoint, caster);
                        Destroy(enemyTempSpellTargetWall, 0.5f);
                        break;
                    case 3:
                        castSpeed(caster);
                        passiveEffectEnemy.SetActive(true);
                        break;
                    default:
                        break;

                }
                
            }
            else
            {

                center = spellCenter;
                spellCastPoint = spellCastStartPoint;
                switch (spellbook[spellName])
                {
                    case 1:
                        SummonStone(center, spellCastPoint);
                        break;
                    case 2:
                        SummonFireball(center, spellCastPoint);
                        break;
                    case 3:
                        castSpeed();
                        passiveEffectPlayer.SetActive(true);
                        break;
                    default:
                        break;

                }
                Destroy(tempSpellTargetWall);
            }
            
        }catch(Exception e)
        {
            Debug.Log("Error CastSpell:" + e.Message + " " + e.StackTrace);
        }

    }

    public void castSpeed(EnemyManager caster = null)
    {
        if (caster)
        {
            lastCaster = caster;
            if (speedTimerEnemy == -1)
                caster.movementSpeed *= 2;
            speedTimerEnemy = (int)SpellTypes.SpellType.speed;
        }
        else
        {
            audioSource.clip = speedUp;
            audioSource.Play();
            playerWalk.stepLength *= 2;
            if(speedTimerPlayer == -1)
                speedTimerPlayer = (int)SpellTypes.SpellType.speed;
        }
    }

    public void SummonStone(Vector3 center,Vector3 spellCastPoint,EnemyManager caster = null)
    {
        if (caster)
            Destroy(caster.currentEnemy.GetComponent<BoxCollider>());

        
        tempStone = Instantiate(stone,center,Quaternion.identity);
        walkDetecter.ignoreSpell(tempStone);
        tempStone.name = SpellTypes.SpellType.circle.ToString();
        
        Rigidbody body = tempStone.gameObject.GetComponent<Rigidbody>();
        if (caster)
        {
            tempStone.tag = "Spell";
            body.AddForce(caster.currentEnemy.transform.forward * 1000 + spellCastPoint);
           
        }
        else
        {
            tempStone.tag = "OwnSpell";
            Vector3 forceVector = (center - spellCastPoint);
            playerCastEvent.Invoke();
            body.AddForce(forceVector * 3000);
        }
        body.useGravity = true;
        
        Destroy(tempStone, 10);
        if(caster)
            waiter(caster.gameObject);
    }

    public void SummonFireball(Vector3 center, Vector3 spellCastPoint, EnemyManager caster = null)
    {
        if (caster)
            Destroy(caster.currentEnemy.GetComponent<BoxCollider>());


        tempFireball = Instantiate(fireball, center, Quaternion.identity);
        walkDetecter.ignoreSpell(tempFireball);
        tempFireball.name = SpellTypes.SpellType.fire.ToString();

        Rigidbody body = tempFireball.gameObject.GetComponent<Rigidbody>();
        body.mass = 0.1f;
        body.useGravity = false;
        body.isKinematic = false;
        if (caster)
        {
            tempFireball.tag = "Spell";
            body.velocity = caster.currentEnemy.transform.forward * 20;

        }
        else
        { 
            tempFireball.tag = "OwnSpell";
            Vector3 forceVector = (center - spellCastPoint);
            playerCastEvent.Invoke();
            body.velocity = forceVector * 5;
        }
        body.useGravity = true;

        Destroy(tempFireball, 10);
        if (caster)
            waiter(caster.gameObject);
    }

        private IEnumerator waiter(GameObject caster)
    {
        yield return new WaitForSeconds(1);
        caster.AddComponent<BoxCollider>();
    }
    
}