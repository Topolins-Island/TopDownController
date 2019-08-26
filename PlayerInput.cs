using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    Rigidbody2D rgdbdy;
    public Vector3 inputDir;
    [HideInInspector]
    public Vector3 lastInput;

    [HideInInspector]
    public bool cantMove = false;

    [Header("Controls")]
    public KeyCode attackKey = KeyCode.Z;
    public KeyCode mainAbilityKey = KeyCode.X;
    public KeyCode toggleAbilityKey = KeyCode.Space;

    [Header("General Stats")]
    public float moveSpeed;
    
    public GameObject attackPrefab;
    public float attackSpeed;
    public float attackSize;
    
    [Header("Ability Stats")]
    public int abilityLevel;
    private float castTime = 1;
    //private float duration = 1;
    //private float areaOfEffect = 1;
    //private float cooldown = 1;
    private int abilitySoulsCost = 1;
    
    private float transformationTime = 1;
    private float transformBackTime = 1;

    public virtual void Start ()
    {
        rgdbdy = GetComponent<Rigidbody2D>();
        StartCoroutine(RegularInput());
	}
	
	void FixedUpdate ()
    {
        if(inputDir != Vector3.zero && !cantMove && !LevelManager.paused)
            rgdbdy.position = transform.position + inputDir * Time.deltaTime * moveSpeed;
    }

    public IEnumerator RegularInput()
    {
        yield return null;
        cantMove = false;

        while (true)
        {
            //Check for inputs, and if we get one, stop this coroutine
            if (Input.GetKeyDown(attackKey) || Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                cantMove = true;
                StartCoroutine(Attack());
                yield break;
            }

            if (Input.GetKeyDown(mainAbilityKey) || Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                cantMove = true;
                StartCoroutine(MainAbility());
                yield break;
            }

            if (Input.GetKeyDown(toggleAbilityKey) || Input.GetAxis("Right Trigger") > 0.7f)
            {
                ToggleAbility();
                yield break;
            }

            //If nothing was inputted, get the movement direction
            inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (inputDir != Vector3.zero) lastInput = inputDir;

            yield return PlayerYield();
        }
    }

    public virtual IEnumerator Attack()
    {
        //Spawn object
        Quaternion butt = Quaternion.LookRotation(lastInput == Vector3.zero ? Vector3.up : lastInput, Vector3.forward);
        butt.x = 0;
        butt.y = 0;
        //GameObject go = 
        Instantiate(attackPrefab,
                    transform.position + (lastInput == Vector3.zero ? Vector3.up : lastInput),
                    butt);

        //Wait for attack speed
        yield return new WaitForSeconds(attackSpeed);

        AttackFinished.Invoke();

        StartCoroutine(RegularInput());
    }

    [HideInInspector]
    public UnityEvent AttackFinished;

    public virtual IEnumerator MainAbility()
    {
        if (LevelManager.instance.actualSouls >= abilitySoulsCost)
        {
            LevelManager.instance.ScoreChanged(-abilitySoulsCost);
            //Casting time
            yield return new WaitForSeconds(castTime);

            //Spawn the object here
            print("abilitied");
        }
        else
        {
            print("not enough");
        }

        StartCoroutine(RegularInput());
    }

    public virtual void ToggleAbility()
    {
        StartCoroutine(BaseToggleAbility());
    }

    IEnumerator BaseToggleAbility()
    {
        //Wait time here for transformation animation
        inputDir = Vector3.zero;
        yield return new WaitForSeconds(transformationTime);

        //Change any stats for transformation

        //Check if input is still going
        while (ToggleInputHeldDown())
        {
            //enact any different code while transformed
            inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            GetComponent<SpriteRenderer>().color = Color.yellow;

            yield return PlayerYield();
        }

        //Set everything back to regular
        inputDir = Vector3.zero;
        yield return new WaitForSeconds(transformBackTime);
        
        GetComponent<SpriteRenderer>().color = Color.blue;

        //Start input again
        StartCoroutine(RegularInput());
    }

    public bool ToggleInputHeldDown()
    {
        return Input.GetKey(toggleAbilityKey) || Input.GetAxis("Right Trigger") > 0.7f;
    }

    public IEnumerator PlayerYield()
    {
        while (LevelManager.paused)
        {
            yield return null;
        }
    }

    public virtual void LevelUp(Improvement improved)
    {
        if (improved == Improvement.ATTACK)
        {
            print("attack improved");
        }
        else if (improved == Improvement.ABILITY)
        {
            print("ability improved");
            abilityLevel++;
        }
        else if (improved == Improvement.TRANSFORMATION)
        {
            print("trans improved");
        }
    }

    public void SetLevels(int attack, int ability, int transformation)
    {
        for (int i = 0; i < attack; i++)
        {
            LevelUp(Improvement.ATTACK);
        }
        for (int i = 0; i < ability; i++)
        {
            LevelUp(Improvement.ABILITY);
        }
        for (int i = 0; i < transformation; i++)
        {
            LevelUp(Improvement.TRANSFORMATION);
        }
    }

    public virtual List<ShopContent> Stuff()
    {
        List<ShopContent> bleh = new List<ShopContent>();

        return bleh;
    }

    public int GiveLevel(Improvement of)
    {
        int bagingi = 0;
        if (of == Improvement.ATTACK)
        {
            print("attack improved");
        }
        else if (of == Improvement.ABILITY)
        {
            bagingi = abilityLevel;
        }
        else if (of == Improvement.TRANSFORMATION)
        {
            print("trans improved");
        }

        return bagingi;
    }
}

public enum Improvement
{
    ATTACK,
    ABILITY,
    TRANSFORMATION
}