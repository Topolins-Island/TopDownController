using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudyController : PlayerInput
{
    public ThunderCloud summonCould;
    public Tornado tornadoForm;

    [Header("Improvements")]
    public List<ThunderCloudImprovements> mainAbilityImprovements;
    
    public override IEnumerator MainAbility()
    {
        yield return new WaitForSeconds(summonCould.castTime);

        GameObject go = Instantiate(summonCould.cloudPrefab, transform.position, Quaternion.identity);
        go.GetComponent<MovingSpawnerController>().SetStats(lastInput, summonCould.timeBetweenSpawns, summonCould.duration, summonCould.moveSpeed);

        StartCoroutine(RegularInput());
    }

    public override void ToggleAbility()
    {
        StartCoroutine(BecomeAsTornado());
    }

    IEnumerator BecomeAsTornado()
    {
        float hitboxCheck = 0;
        while (ToggleInputHeldDown())
        {
            //Ensure we still move
            inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            inputDir *= tornadoForm.moveSpeedModifier;

            hitboxCheck += Time.deltaTime;
            if (hitboxCheck > tornadoForm.timeBetweenHitboxes)
            {
                Instantiate(attackPrefab, transform);
                hitboxCheck -= tornadoForm.timeBetweenHitboxes;
            }

            yield return PlayerYield();
        }

        StartCoroutine(RegularInput());
    }
}

[System.Serializable]
public class ThunderCloud
{
    public GameObject cloudPrefab;
    public float castTime;
    public float timeBetweenSpawns;
    public float duration;
    public float moveSpeed;
    public int soulsCost;
}

[System.Serializable] 
public class ThunderCloudImprovements
{
    public float castTimeImprovement;
    public float spawnTimeImprovement;
    public float durImprovement;
    public int soulsCostIncrease;
}

[System.Serializable]
public class Tornado
{
    public float moveSpeedModifier;
    public float timeBetweenHitboxes;
    public int costPerSecond;
}