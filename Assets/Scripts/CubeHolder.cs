using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeHolder : MonoBehaviour
{
    public static CubeHolder instance;
    [SerializeField] Animator animator;
    [SerializeField] Transform wallChecker;

    [SerializeField]private List<Cube> cube_tower = new List<Cube>();
    [SerializeField] GameObject playerObj;

    
    private List<Cube> removed_cubes = new List<Cube>();
    [SerializeField] LayerMask wallAndPlatform;
    [SerializeField] LayerMask onlyWall;

    //WRAP
    [SerializeField] GameObject wrap;
    int cubesToWrap = 4;


    //camera shake
    [SerializeField] CameraShaker cameraShaker;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddCube(Cube newCube)
    {
        StartCoroutine(Jumper(newCube));
    }

    private IEnumerator Jumper(Cube newCube)
    {
        //turn off upper cubes ability to collect cubes 
        foreach (Cube cube in cube_tower)
        {
            cube.SetAsCollector(false);
        }

        //jump at cube
        float jumpHeigh = newCube.cubeHeight;
        Vector3 newHeight = transform.localPosition + new Vector3(0, jumpHeigh, 0);
        float jumpSpeed = 10.0f;
        animator.SetTrigger("Jump");
        while (transform.localPosition != newHeight)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, newHeight, jumpSpeed * Time.deltaTime);
            yield return null;
        }
        EffectPool.instance.SpawnEffect(animator.gameObject.transform.position);

        //add new cube to children
        cube_tower.Add(newCube);
        newCube.JoinPlayer(this.transform);

        //Wrap
        if (cube_tower.Count >= cubesToWrap) wrap.SetActive(true);

        //point-to-score effect
        EffectPool.instance.SpawnPoint(transform.position);
    }

    public void RemoveCube(Cube removedCube)
    {
        cube_tower.Remove(removedCube);
        removedCube.LeavePlayer();
        removed_cubes.Add(removedCube);
        //Wrap
        if (cube_tower.Count < cubesToWrap) wrap.SetActive(false);

        if (checkWall==null)
        {
            checkWall = StartCoroutine(WallCheck());
        }
    }

    private Coroutine checkWall;
    private IEnumerator WallCheck()
    {
        cameraShaker.SmallShake();
        //wait for wall checker would detect wall under player`s back
        float rayDistance = (cube_tower.Count + removed_cubes.Count) * 5.5f;

        bool isWaitingForWall = true;
        
        while (isWaitingForWall)
        {
            Ray ray = new Ray(wallChecker.position, Vector3.down);

            if (Physics.Raycast(ray, rayDistance, onlyWall))
            {
                isWaitingForWall = false;
            }
            else yield return null;

        }

        //drop all removed cubes
        foreach (Cube cube in removed_cubes) cube.GravityAble(true);
        removed_cubes.Clear();

        //wait for player would cross the wall
        bool wallCrossed = false;
        while (!wallCrossed)
        {
            Debug.DrawRay(wallChecker.position, Vector3.down, Color.green, rayDistance);

            Ray ray = new Ray(wallChecker.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance, wallAndPlatform))
            {
                if (hit.collider.gameObject.layer != 8) yield return null;
                else wallCrossed = true;
            }
            else yield return null;

        }

        //check distance for lift down each other cube
        List<float> distanceList = new List<float>();
        float dis = 0;
        for (int i = cube_tower.Count-1; i>=0; i--)
        {
            dis += cube_tower[i].nearestDistance(rayDistance);
            distanceList.Add(dis);
            Debug.Log(cube_tower[i].gameObject.name + " - " + dis);
        }

        //lift down other cubes and player
        float fallSpeed = 7.5f;
        int distanceIndex = 0;
        for (int i = cube_tower.Count - 1; i > 0; i--)
        {
            Vector3 newLocalPos = new Vector3(0, -distanceList[distanceIndex], 0);
            cube_tower[i].LiftDown(newLocalPos, fallSpeed);
            Debug.Log(cube_tower[i].gameObject.name + " - " + newLocalPos);
            distanceIndex++;

        }
        Vector3 playerOffset = new Vector3(0, -distanceList[distanceIndex], 0);
        Vector3 newPlayerLocal = playerObj.transform.localPosition + playerOffset;
        while (playerObj.transform.localPosition != newPlayerLocal)
        {
            playerObj.transform.localPosition = Vector3.MoveTowards(playerObj.transform.localPosition, newPlayerLocal, fallSpeed * Time.deltaTime);
            yield return null;
        }



        //activate lower cube as collector
        cube_tower[cube_tower.Count - 1].SetAsCollector(true);



        checkWall = null;
    }

}
