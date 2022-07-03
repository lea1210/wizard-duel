using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    public Enemy currentEnemy;

    [SerializeField]
    Player player;

    [SerializeField]
    public SpellBook spellBook;

    //Timer
    int randomTimer = 0;
    int spellTimer = 0;
    int movementTimer = 0;
    int spellTimerOffset = 0;
    int movementTimerOffset = 0;
    [SerializeField]
    public int spellTimerMax = 200;
    [SerializeField]
    public int spellTimerOffsetMax = 60;
    [SerializeField]
    public int movementTimerMax = 60;
    [SerializeField]
    public int movementTimerOffsetMax = 30;

    List<SpellTypes.SpellType> spellList = new List<SpellTypes.SpellType> ();

    UnityEvent playerCastEvent;
    [SerializeField]
    Animator animator;

    [SerializeField]
    public float movementSpeed = 0.1f;
    Vector3 movingDirection = Vector3.zero;
    public bool doDodge = false;

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


    // Start is called before the first frame update
    void Start()
    {
        playerCastEvent = spellBook.playerCastEvent;
        playerCastEvent.AddListener(tryDodge);
        spellList.Add(SpellTypes.SpellType.fire);
        spellList.Add(SpellTypes.SpellType.circle);
        //spellList.Add(SpellTypes.SpellType.speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentEnemy)
        {
            randomTimer++;
            spellTimer++;
            movementTimer++;

            if (randomTimer > 100)
                randomTimer = 0;

            if (spellTimer == castCooldown)
                moving = true;

            if (spellTimer == spellTimerMax + spellTimerOffset - castDelay)
            {
                currentSpell = spellList[Random.Range(0, spellList.Count)];
                if (spellBook.TestForTargetableSpell(currentSpell.ToString(), this))
                {
                    spellBook.createEnemyTargetField(currentEnemy);
                    Debug.Log("Created Wall");
                }
                moving = false;
            }

            if (spellTimer == spellTimerMax + spellTimerOffset)
            {
                animator.Play("stop");
                spellTimer = 0;
                spellTimerOffset = Random.Range(-spellTimerOffsetMax, spellTimerOffsetMax);
                if (currentSpell == SpellTypes.SpellType.speed) {
                    currentEnemy.playSound(speedUp);
                }
                spellBook.CastSpell(currentSpell.ToString(), this);

            }

            if (moving)
            {
                animator.Play("walk");
                if (movementTimer >= movementTimerMax + movementTimerOffset)
                {
                    movementTimer = 0;
                    movementTimerOffset = Random.Range(-movementTimerOffsetMax, movementTimerOffsetMax);
                    CreateNewMovementDirection();
                }
                TestPlayerDistance();
                AvoidCollision();
                currentEnemy.transform.Translate(movingDirection * movementSpeed);
                if (movementChanged)
                {
                    movementTimer = 0;
                    movementTimerOffset = Random.Range(-movementTimerOffsetMax, movementTimerOffsetMax);
                    CreateNewMovementDirection();
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
            return new Vector3(0, distance, 0);
        }
    }

    private void tryDodge()
    {
        if (doDodge)
        {
            if (randomTimer > 50)
            {

            }
            Debug.Log("Enemy: Dodging");
        }
        Debug.Log("Enemy: Not Dodging");
    }

    private void CreateNewMovementDirection()
    {
        int randomDirection = Random.Range(0, 361);
        float radiant = randomDirection * Mathf.PI / 180;
        movingDirection = new Vector3(Mathf.Cos(radiant),0,Mathf.Sin(radiant));

    }

    private void TestPlayerDistance()
    {
        if(Vector3.Distance(currentEnemy.transform.position, player.transform.position) > targetingDistance)
        {
            currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - player.transform.position.x, 0, currentEnemy.transform.position.z - player.transform.position.z) * -movementSpeed, Space.World);
            movingDirection = Vector3.zero;
            movementChanged = true;
           // Debug.Log("Targeting");
        }
        else if (Vector3.Distance(currentEnemy.transform.position, player.transform.position) < fleeingDistance)
        {
            currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - player.transform.position.x,0, currentEnemy.transform.position.z - player.transform.position.z) * movementSpeed,Space.World);
            movingDirection = Vector3.zero;
            movementChanged = true;
            //Debug.Log("Fleeing");
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
                currentEnemy.transform.Translate(new Vector3(currentEnemy.transform.position.x - obj.transform.position.x, 0, currentEnemy.transform.position.z - obj.transform.position.z) * movementSpeed, Space.World);
                movingDirection = Vector3.zero;
                movementChanged = true;
                //Debug.Log("avoiding");
            }
        }
    }


}
