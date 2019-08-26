using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienMinionController : MonoBehaviour
{
    PrimaryShape creator;
    public float moveSpeed = 0.5f;
    public GameObject bulletPrefab;
    public int maxNumberOfShots;
    public float attackSpeed;

    public void SetStats(PrimaryShape _creator, int _shots, float _atkSpd)
    {
        creator = _creator;
        maxNumberOfShots = _shots;
        attackSpeed = _atkSpd;

        StartCoroutine(ShootAtCivilians());
        StartCoroutine(MoveInACircleAround());
    }

    IEnumerator ShootAtCivilians()
    {
        //Spawning first
        //play anim i guess

        for (int i = 0; i < maxNumberOfShots; i++)
        {
            yield return new WaitForSeconds(attackSpeed);
            GameObject target = null;
            while (target == null)
            {
                target = GiveClosestCiv();

                yield return null;
            }

            GameObject go = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            go.GetComponent<BulletController>().SetStats(target);
        }

        creator.numOfCurrentMinions -= 1;
        Destroy(this.gameObject);
    }

    IEnumerator MoveInACircleAround()
    {
        while (true)
        {
            transform.RotateAround(transform.parent.position, Vector3.forward, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    GameObject GiveClosestCiv()
    {
        GameObject closest = null;

        GameObject[] civs = GameObject.FindGameObjectsWithTag("Civilian");
        if (civs.Length > 0)
        {
            List<GameObject> applicables = new List<GameObject>();
            for (int i = 0; i < civs.Length; i++)
            {
                if(civs[i].transform.childCount == 0)
                {
                    applicables.Add(civs[i]);
                }
            }

            if (applicables.Count > 0)
            {
                GameObject go = applicables[0];
                float dist = Vector3.Distance(go.transform.position, transform.position);

                for (int i = 1; i < applicables.Count; i++)
                {
                    float tempDist = Vector3.Distance(applicables[i].transform.position, transform.position);
                    if (tempDist < dist)
                    {
                        dist = tempDist;
                        go = applicables[i];
                    }
                }

                closest = go;
            }
        }

        return closest;
    }
}
