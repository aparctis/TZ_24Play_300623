using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Cube : MonoBehaviour
{
    /// <summary>
    /// 0 = on platform; 1 = follow player and collect other cubes; 2 = only follow; 3 = falling down
    /// </summary>
    [SerializeField]private int cubeState = 0;
    public int state => cubeState;

    private Rigidbody rb;
    private BoxCollider col;

    //for raycast
    /// <summary>
    /// ofset from cube center to cube bottom edge
    /// </summary>
    float ofsetY;
    public float cubeHeight => (ofsetY * 2);
    [SerializeField] LayerMask picaUpAndPlatform;

    //for first cubes only
    [SerializeField]private LevelSpawner levelSpawner;
    [SerializeField] bool mainCube;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        ofsetY = (col.size.y) / 2;
    }

    private void OnEnable()
    {
        if(!mainCube) Suscribe();
    }

    private void OnDisable()
    {
        if (!mainCube) UnSuscribe();

    }

    private void Suscribe()
    {
        if(levelSpawner!=null) levelSpawner.onLevelUpdatte.AddListener(BackToPool);
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


    private void OnCollisionEnter(Collision collision)
    {

        if (cubeState < 1||cubeState>2) return;
        if (collision.gameObject.tag == "PickUp"&&cubeState==1)
        {
            if (collision.gameObject.GetComponent<Cube>() != null)
            {
                Cube newCube = collision.gameObject.GetComponent<Cube>();
                if (newCube.state != 0) return;
                CubeHolder.instance.AddCube(newCube);
            }
        }

        if (collision.gameObject.tag == "Wall")
        {
            CubeHolder.instance.RemoveCube(this);

        }
    }


    public void JoinPlayer(Transform newParent)
    {
        cubeState = 1;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.parent = newParent;
        transform.position = new Vector3(newParent.position.x, ofsetY, newParent.position.z);
    }

    /// <summary>
    /// if true - cube can collect other cubes from platform
    /// </summary>
    /// <param name="isCollector"></param>
    public void SetAsCollector(bool isCollector)
    {
        if (!isCollector) cubeState = 2;
        else cubeState = 1;
    }

    public void LeavePlayer()
    {
        cubeState = 3;
        transform.parent = null;
    }

    public void GravityAble(bool canUseGravity)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = !canUseGravity;
            rb.useGravity = canUseGravity;
        }

        else
        {
            rb.isKinematic = !canUseGravity;
            rb.useGravity = canUseGravity;
        }

    }

    //distance to the nearest cube down or platform
    public float nearestDistance(float rayHeight)
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, rayHeight, picaUpAndPlatform);
        float dis = (hit.distance-ofsetY);
        if (dis < 0) dis = 0;
        return dis;

    }

    public void LiftDown(Vector3 newLocalPos, float liftSpeed)
    {
        StartCoroutine(Lifter(newLocalPos, liftSpeed));
    }

    private IEnumerator Lifter(Vector3 fallOfset, float liftSpeed)
    {
        Vector3 newLocalPos = (transform.localPosition + fallOfset);
        while (transform.localPosition != newLocalPos)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, newLocalPos, liftSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void BackToPool()
    {
        float destroyDistance = 15.0f;
        if (CubeHolder.instance.gameObject.transform.position.z > (transform.position.z + destroyDistance))
        {
            gameObject.SetActive(false);
            cubeState = 0;
            GravityAble(false);
            CubePool.instance.BackToPool(this.gameObject);
        }

    }
}
