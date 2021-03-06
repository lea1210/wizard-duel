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

    [SerializeField]
    private int spellbookLevel = 3;
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
    TrackCameraCollision collisionCamera;

    [SerializeField]
    Player player;
    int speedTimerPlayer = -1;


    [SerializeField]
    EnemyManager enemyManager;
    int speedTimerEnemy = -1;

    [SerializeField]
    List<Material> passiveEffectMaterials = new List<Material>();
    [SerializeField]
    GameObject passiveEffectEnemy;
    [SerializeField]
    EditPassiveEffectCircle editEnemyPassiveEffect;

    [SerializeField]
    GameObject passiveEffectPlayer;
    [SerializeField]
    EditPassiveEffectCircle editPlayerPassiveEffect;

    AudioSource audioSource;
    [SerializeField]
    AudioClip speedUp;

    [SerializeField]
    AudioClip timeStop;
    [SerializeField]
    AudioClip timeResume;

    bool timeStopped = false;

    // Start is called before the first frame update
    void Start()
    {
        //Fill Spellbook
        spellbook[SpellTypes.SpellType.timeStop.ToString()] = -1;
        spellbook[SpellTypes.SpellType.stone.ToString()] = 1;
        spellbook[SpellTypes.SpellType.speed.ToString()] = 2;
        spellbook[SpellTypes.SpellType.fire.ToString()] = 3;
        

        nonTargetableSpells.Add(SpellTypes.SpellType.speed.ToString());
        nonTargetableSpells.Add(SpellTypes.SpellType.timeStop.ToString());

        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!timeStopped)
        {
            if (speedTimerPlayer > 0)
            {
                speedTimerPlayer--;
            }
            else if (speedTimerPlayer == 0)
            {
                player.modifyMovementSpeed(0.5f);
                speedTimerPlayer = -1;
                passiveEffectPlayer.SetActive(false);
            }

            if (speedTimerEnemy > 0)
            {
                speedTimerEnemy--;
            }
            else if (speedTimerEnemy == 0)
            {
                enemyManager.ResetMovementSpeed();
                speedTimerEnemy = -1;
                passiveEffectEnemy.SetActive(false);
            }
        }

    }

    public bool TestForTargetableSpell(string spell,EnemyManager caster = null)
    {
        if (caster)
        {
            if (nonTargetableSpells.Contains(spell))
            {
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

    public bool testSpellUnlocked(string spell)
    {
        Debug.Log("Spellbook: " + spell);
        if (spellbook[spell] <= spellbookLevel){
            return true;
        }
        else
        {
            return false;
        }
    }

    public void increaseSpellbookLevel()
    {
        spellbookLevel++;
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

                spellCenter += playerWallVektor*0.6f;
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
        enemyTempSpellTargetWall = Instantiate(enemySpellTargetWall,caster.transform.position+caster.transform.forward*1f, Quaternion.identity);
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
                    case 3:
                        SummonFireball(center, spellCastPoint, caster);
                        Destroy(enemyTempSpellTargetWall, 0.5f);
                        break;
                    case 2:
                        castSpeed(caster);
                        editEnemyPassiveEffect.SetMaterial(passiveEffectMaterials[0]);
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
                    case -1:
                        CastTimeStop();
                        editPlayerPassiveEffect.SetMaterial(passiveEffectMaterials[1]);
                        passiveEffectPlayer.SetActive(!passiveEffectPlayer.activeSelf);
                        break;
                    case 1:
                        SummonStone(center, spellCastPoint);
                        break;
                    case 3:
                        SummonFireball(center, spellCastPoint);
                        break;
                    case 2:
                        castSpeed();
                        passiveEffectPlayer.SetActive(true);
                        editPlayerPassiveEffect.SetMaterial(passiveEffectMaterials[0]);
                       
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
            enemyManager = caster;
            if (speedTimerEnemy == -1)
            {
                caster.SetMovementSpeed(caster.getMovementSpeed()*2);
            }
            speedTimerEnemy = (int)SpellTypes.SpellType.speed;

        }
        else
        {
            
            audioSource.clip = speedUp;
            audioSource.Play();
            if(speedTimerPlayer == -1)
                player.modifyMovementSpeed(2);
            speedTimerPlayer = (int)SpellTypes.SpellType.speed;  
        }
    }

    public void SummonStone(Vector3 center,Vector3 spellCastPoint,EnemyManager caster = null)
    {
        if (caster)
            Destroy(caster.GetCurrentEnemy().GetComponent<BoxCollider>());

        
        tempStone = Instantiate(stone,center,Quaternion.identity);
        walkDetecter.ignoreSpell(tempStone);
        tempStone.name = SpellTypes.SpellType.stone.ToString();
        
        Rigidbody body = tempStone.gameObject.GetComponent<Rigidbody>();
        if (caster)
        {
            tempStone.tag = "Spell";
            body.AddForce(caster.GetAlteredCastVector(caster.GetCurrentEnemy().transform.forward * 1000 + spellCastPoint));
           
        }
        else
        {
            collisionCamera.ignoreSpell(tempStone);
            tempStone.tag = "OwnSpell";
            Vector3 forceVector = (center - spellCastPoint);
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
            Destroy(caster.GetCurrentEnemy().GetComponent<BoxCollider>());

    
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
            body.velocity = caster.GetAlteredCastVector(spellCastPoint) * 20;
            tempFireball.transform.LookAt(Camera.main.transform.position);  

        }
        else
        {
            collisionCamera.ignoreSpell(tempFireball);
            tempFireball.tag = "OwnSpell";
            Vector3 forceVector = (center - spellCastPoint);
            tempFireball.transform.LookAt(Camera.main.transform.position);
            tempFireball.transform.Rotate(new Vector3(0, 180, 0));
            body.velocity = forceVector * 30;
            
        }
        body.useGravity = true;

        Destroy(tempFireball, 10);
        if (caster)
            waiter(caster.gameObject);
    }

    public void CastTimeStop()
    {
        timeStopped = !timeStopped;
        if (timeStopped)
        {
            Time.timeScale = 0;
            audioSource.clip = timeStop;
            audioSource.Play();
        }
        else
        {
            Time.timeScale = 1;
            audioSource.clip = timeResume;
            audioSource.Play();
        }
        
    }

        private IEnumerator waiter(GameObject caster)
    {
        yield return new WaitForSeconds(1);
        caster.AddComponent<BoxCollider>();
    }

    public int getSpellbookLevel()
    {
        return spellbookLevel;
    }

    public void setSpellbookLevel(int level)
    {
        spellbookLevel = level;
    }

    public bool GetTimeStopped()
    {
        return timeStopped;
    }
    
}
