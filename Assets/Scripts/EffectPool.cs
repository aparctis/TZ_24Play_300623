using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{
    public static EffectPool instance;

    //Dust
    [SerializeField] GameObject effectPrfb;
    private List<GameObject> poolDust = new List<GameObject>();
    [SerializeField] private int startPoolCount = 10;

    //Point
    [SerializeField] GameObject pointPfb;
    private List<GameObject> poolPoint = new List<GameObject>();

    private void Awake()
    {
        if (instance == null) instance = this;
        for(int i =0; i<startPoolCount; i++)
        {
            GameObject ef = Instantiate(effectPrfb) as GameObject;
            ef.SetActive(false);
            poolDust.Add(ef);

            GameObject po = Instantiate(pointPfb) as GameObject;
            po.SetActive(false);
            poolPoint.Add(po);
        }

    }

    private GameObject newEffect()
    {
        GameObject ef;
        if (poolDust.Count > 0)
        {
            ef = poolDust[0];
            poolDust.RemoveAt(0);
        }

        else
        {
            ef = Instantiate(effectPrfb) as GameObject;
            ef.SetActive(false);
        }

        return ef;
    }

    private GameObject newPoint()
    {
        GameObject point;
        if (poolPoint.Count > 0)
        {
            point = poolPoint[0];
            poolPoint.RemoveAt(0);
        }

        else
        {
            point = Instantiate(pointPfb) as GameObject;
            point.SetActive(false);
        }

        return point;
    }


    public void SpawnEffect(Vector3 effectPosition)
    {
        StartCoroutine(DustSpawner(effectPosition));
    }


    //spawn dust effect at place and automaticly remove it back to pool after 3 seconds
    private IEnumerator DustSpawner(Vector3 position)
    {
        float effectLiveTime = 3.0f;
        GameObject effect = newEffect();
        effect.transform.position = position;
        effect.SetActive(true);
        yield return new WaitForSeconds(effectLiveTime);
        effect.SetActive(false);
        poolDust.Add(effect);
    }



    //POINT SPAWN
    public void SpawnPoint(Vector3 startPos)
    {
        StartCoroutine(PointSpawner(startPos));
    }
    private IEnumerator PointSpawner(Vector3 startPos)
    {
        float liveTime =5.0f;
        GameObject point = newPoint();
        point.transform.position = startPos;
        point.SetActive(true);

        while (liveTime > 0)
        {
            point.transform.Translate(Vector3.up * Time.deltaTime);
            liveTime -= Time.deltaTime;
            yield return null;

        }
        point.SetActive(false);
        poolPoint.Add(point);

    }
}
