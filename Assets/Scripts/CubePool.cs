using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePool : MonoBehaviour
{
    public static CubePool instance;

    private List<GameObject> pool = new List<GameObject>();
    [SerializeField] GameObject cubePfb;

    private int startPoolCount = 0;

    private void Awake()
    {
        if (instance == null) instance = this;

        for(int i = 0; i<startPoolCount; i++)
        {
            GameObject obj;
            obj = Instantiate(cubePfb) as GameObject;
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    public void BackToPool(GameObject cubeObj)
    {
        pool.Add(cubeObj);
    }

    public GameObject newCube()
    {
        GameObject obj;
        if(pool.Count>0)
        {
            obj = pool[0];
            pool.RemoveAt(0);
        }

        else
        {
            obj = Instantiate(cubePfb) as GameObject;
            obj.SetActive(false);
        }

        return obj;
    }

    public GameObject newCube(Transform cubeParent)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool[0];
            pool.RemoveAt(0);
        }

        else
        {
            obj = Instantiate(cubePfb) as GameObject;
            obj.SetActive(false);
        }
        obj.transform.parent = cubeParent;
        return obj;
    }

}
