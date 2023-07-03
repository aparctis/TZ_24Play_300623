using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelSpawner : MonoBehaviour
{
    public static LevelSpawner instance;

    [SerializeField] GameObject platformPfb;
    private List<GameObject> platformPool = new List<GameObject>();
    private int startPoolCount = 3;


    [SerializeField] Transform player;

    //where should be player for level update
    float spawnAfter = 0;
    private Vector3 nextPlatformPosition = new Vector3(0, 0, 30.0f);
    private Vector3 downOffset = new Vector3(0, -80.0f, 0);   
    private float platformLength = 30.0f;
    private float liftSpeed = 40.0f;



    private void Awake()
    {
        if (instance == null) instance = this;
        for(int i = 0; i<startPoolCount; i++)
        {
            GameObject obj;
            obj = Instantiate(platformPfb) as GameObject;
            obj.SetActive(false);
            platformPool.Add(obj);
        }
    }

    private void Update()
    {
        if (player.position.z > spawnAfter)
        {
            spawnAfter += platformLength;
            StartCoroutine(PlatformSpawner());
            onLevelUpdatte.Invoke();
        }
    }

    public UnityEvent onLevelUpdatte;

    //platform pool
    public void BackToPool(GameObject platform)
    {
        platformPool.Add(platform);
    }

    private GameObject newPlatform()
    {
        GameObject obj;
        if (platformPool.Count > 0)
        {
            obj = platformPool[0];
            platformPool.RemoveAt(0);
        }

        else
        {
            obj = Instantiate(platformPfb) as GameObject;
            obj.SetActive(false);
        }

        return obj;
    }

    private IEnumerator PlatformSpawner()
    {
        Vector3 upPosition = nextPlatformPosition;
        Vector3 downPosition = upPosition + downOffset;

        GameObject nextPlatform = newPlatform();
        nextPlatform.transform.position = downPosition;
        nextPlatform.SetActive(true);

        nextPlatform.GetComponent<Platform>().InitiateOnSpawn();

        while (nextPlatform.transform.position != upPosition)
        {
            nextPlatform.transform.position = Vector3.MoveTowards(nextPlatform.transform.position, upPosition, liftSpeed * Time.deltaTime);
            yield return null;
        }

        nextPlatformPosition.z += platformLength;
    }

}
