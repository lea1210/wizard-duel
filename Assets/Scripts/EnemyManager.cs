using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;


public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    Enemy currentEnemy;

    [SerializeField]
    Player player;

    [SerializeField]
    SpellBook spellBook;

    //Timer
    int spellTimer = 0;
    int movementTimer = 0;
    int spellTimerOffset = 0;
    int movementTimerOffset = 0;
    [SerializeField]
    int spellTimerMax = 200;
    [SerializeField]
    int spellTimerOffsetMax = 60;
    [SerializeField]
    int movementTimerMax = 60;
    [SerializeField]
    int movementTimerOffsetMax = 30;

    List<SpellTypes.SpellType> spellList = new List<SpellTypes.SpellType> ();

    [SerializeField]
    Animator animator;

    [SerializeField]
    float movementSpeed;
    float baseMovementSpeed;
    Vector3 movingDirection = Vector3.zero;

    public bool moving = true;

    public float fleeingDistance = 15f;
    public float targetingDistance = 40f;
    public float avoidDistance = 5f;
    private bool movementChanged = false;
    public int castDelay = 30;
    public int castCooldown = 30;

    SpellTypes.SpellType currentSpell;

    [SerializeField]
    AudioClip speedUp;

    [SerializeField]
    int spellLevel = 0;

    [SerializeField]
    float missingAngle = 0f;
    [SerializeField]
    float chanceToMiss = 0f;

    private float startY;


    // Start is called before the first frame update
    void Start()
    {
        spellList.Add(SpellTypes.SpellType.circle);
        spellList.Add(SpellTypes.SpellType.speed);
        spellList.Add(SpellTypes.SpellType.fire);
        baseMovementSpeed = movementSpeed;
        startY = gameObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (!spellBook.GetTimeStopped())
        {
            if (currentEnemy)
            {
                if (Math.Abs(gameObject.transform.position.y - startY) < 20)
                {
                    gameObject.transform.position = new Vector3(gameObject.transform.position.x, startY, gameObject.transform.position.z);
                }

                if (spellLevel > 0)
                    spellTimer++;
                    movementTimer++;


                if (spellTimer == castCooldown)
                    moving = true;

                if (spellTimer == spellTimerMax + spellTimerOffset - castDelay)
                {
                    if (spellLevel > spellList.Count)
                        spellLevel = spellList.Count;
                    currentSpell = spellList[UnityEngine.Random.Range(0, spellLevel)];
                    if (spellBook.TestForTargetableSpell(currentSpell.ToString(), this))
                    {
                        spellBook.createEnemyTargetField(currentEnemy);
                    }
                    moving = false;
                }

                if (spellTimer == spellTimerMax + spellTimerOffset)
                {
                    animator.Play("stop");
                    spellTimer = 0;
                    spellTimerOffset = UnityEngine.Random.Range(-spellTimerOffsetMax, spellTimerOffsetMax);
                    if (currentSpell == SpellTypes.SpellType.speed)
                    {
                        currentEnemy.PlaySound(speedUp);
                    }
                    spellBook.CastSpell(currentSpell.ToString(), this);

                }

                if (moving)
                {
                    animator.Play("walk");
                    if (movementTimer >= movementTimerMax + movementTimerOffset)
                    {
                        movementTimer = 0;
                        movementTimerOffset = UnityEngine.Random.Range(-movementTimerOffsetMax, movementTimerOffsetMax);
                        CreateNewMovementDirection();
                    }
                    TestPlayerDistance();
                    AvoidCollision();
                    currentEnemy.transform.Translate(movingDirection * movementSpeed);
                    if (movementChanged)
                    {
                        movementTimer = 0;
                        movementTimerOffset = UnityEngine.Random.Range(-movementTimerOffsetMax, movementTimerOffsetMax);
                        CreateNewMovementDirection();
                    }
                }
            }
        }


    }

    public void SetSpells(List<SpellTypes.SpellType> list)
    {
        spellList = list;
    }

    public void setCurrentEnemy(Enemy enemy)
    {
        currentEnemy = enemy;
    }

    public Vector3 getSpellCenter()
    {
        return currentEnemy.transform.position;
    }

    public Vector3 getSpellCastPoint()
    {
        float distance = Vector3.Distance(currentEnemy.transform.position, player.transform.position);
        if (currentSpell == SpellTypes.SpellType.circle)
        {
            return new Vector3(0, distance / 0.085f, 0);
        }
        else
        {
            Vector3 vec =  gameObject.transform.forward;
            return vec;
        }

    }

    public Vector3 GetAlteredCastVector(Vector3 castVector)
    {
        if (UnityEngine.Random.Range(0, 100) < chanceToMiss)
        {
            if (missingAngle > 90)
                missingAngle = 90;
            float direction = UnityEngine.Random.Range(-100, 100) > 0 ? 1 : -1;
            float rotation = UnityEngine.Random.Range(0, missingAngle) * direction;
            return Quaternion.AngleAxis(rotation, Vector3.up) * castVector;
        }
        else
        {
            return castVector;
        }
    }


    private void CreateNewMovementDirection()
    {
        int RandomDirection = UnityEngine.Random.Range(0, 361);
        float radiant = RandomDirection * Mathf.PI / 180;
        movingDirection = new Vector3(Mathf.Cos(radiant),0,Mathf.Sin(radiant));

    }

    private void TestPlayerDistance()
    {
        if(Vector3.Distance(currentEnemy.transform.position, player.transform.position) > targetingDistance)
        {
            currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - player.transform.position.x, 0, currentEnemy.transform.position.z - player.transform.position.z) * -movementSpeed*0.25f, Space.World);
            movingDirection = Vector3.zero;
            movementChanged = true;
        }
        else if (Vector3.Distance(currentEnemy.transform.position, player.transform.position) < fleeingDistance)
        {
            currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - player.transform.position.x,0, currentEnemy.transform.position.z - player.transform.position.z) * movementSpeed * 0.25f, Space.World);
            movingDirection = Vector3.zero;
            movementChanged = true;
        }
        else
        {
            movementChanged = false;
        }
    }

    private void AvoidCollision()
    {
        GameObject[] physicalObjects = GameObject.FindGameObjectsWithTag("PhysicalObject");
        foreach (GameObject obj in physicalObjects)
        {
            if (Vector3.Distance(obj.transform.position, currentEnemy.transform.position) < avoidDistance)
            {
                currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - obj.transform.position.x, 0, currentEnemy.transform.position.z - obj.transform.position.z) * movementSpeed*0.25f, Space.World);
                movingDirection = Vector3.zero;
                movementChanged = true;
            }
        }
    }

    public float getMovementSpeed()
    {
        return movementSpeed;
    }

    public Enemy GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public void SetMovementSpeed(float speed)
    {
        movementSpeed = speed;
    }

    public void ResetMovementSpeed()
    {
        movementSpeed = baseMovementSpeed;
    }


}
