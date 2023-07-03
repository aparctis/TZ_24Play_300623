using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [Header("All walls prefabs")]
    [SerializeField] List<GameObject> wall_list = new List<GameObject>();

    [Header("Positions for cube spawn")]
    [SerializeField] List<Transform> line_1 = new List<Transform>();
    [SerializeField] List<Transform> line_2 = new List<Transform>();
    [SerializeField] List<Transform> line_3 = new List<Transform>();
    [SerializeField] List<Transform> line_4 = new List<Transform>();
    [SerializeField] List<Transform> line_5 = new List<Transform>();
    List<List<Transform>> all_lines = new List<List<Transform>>();

    // distance from player for back to pool at level update
    float backToPoolDistance = 50.0f;

    private List<GameObject> spawnedCubes = new List<GameObject>();
    //for first cubes only
    [SerializeField] private LevelSpawner levelSpawner;
    private void Awake()
    {
        all_lines.Add(line_1);
        all_lines.Add(line_2);
        all_lines.Add(line_3);
        all_lines.Add(line_4);
        all_lines.Add(line_5);
    }

    private void OnEnable()
    {
        Suscribe();
    }

    private void OnDisable()
    {
        UnSuscribe();

    }

    private void Suscribe()
    {
        if (levelSpawner != null) levelSpawner.onLevelUpdatte.AddListener(BackToPool);
        else
        {
            levelSpawner = LevelSpawner.instance;
            levelSpawner.onLevelUpdatte.AddListener(BackToPool);
        }
    }

    private void UnSuscribe()
    {
        if (levelSpawner != null) levelSpawner.onLevelUpdatte.RemoveListener(BackToPool);
        else
        {
            levelSpawner = LevelSpawner.instance;
            levelSpawner.onLevelUpdatte.RemoveListener(BackToPool);
        }
    }

    public void InitiateOnSpawn()
    {
        //pick one wall
        foreach (GameObject wall in wall_list) wall.SetActive(false);
        int wall_id = Random.Range(0, wall_list.Count);
        wall_list[wall_id].SetActive(true);

        //pick count and positions of cubes on platform
        spawnedCubes.Clear();
        int cubeCount = Random.Range(3, 6);
        for(int i = 0; i<cubeCount; i++)
        {
            //pick random position for cube
            int lineIndex = Random.Range(0, all_lines.Count);
            List<Transform> list = all_lines[lineIndex];
            //Vector3 cubePos = list[i].localPosition;
            
            //take cube from pool 
            GameObject newCube = CubePool.instance.newCube(list[i]);
            //set cube as child object while platform isn`t at upPosition
            spawnedCubes.Add(newCube);
            //newCube.transform.parent = this.transform;
            //newCube.transform.localPosition = cubePos;
            newCube.transform.localPosition = Vector3.zero;
            //activate cube
            newCube.SetActive(true);

        }

        StartCoroutine(Orphanage());
    }

    private IEnumerator Orphanage()
    {
        while (transform.position.y < 0) yield return null;
        foreach (GameObject cube in spawnedCubes)
        {
            //Vector3 parentPos = cube.transform.parent.position;
            cube.transform.parent = null;
            //cube.transform.position = parentPos;
            cube.GetComponent<Cube>().GravityAble(true);
        }
        Debug.Log("Orphanage done");
    }


    private void BackToPool()
    {
        if (CubeHolder.instance.transform.position.z > transform.position.z + backToPoolDistance)
        {
            gameObject.SetActive(false);
            LevelSpawner.instance.BackToPool(this.gameObject);
        }
    }
}
