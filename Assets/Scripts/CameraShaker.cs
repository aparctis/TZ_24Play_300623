using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public void SmallShake()
    {
        StartCoroutine(Shaker(0.05f));
    }

    public void BigShake()
    {
        StartCoroutine(Shaker(0.15f));

    }
    private IEnumerator Shaker(float shakeOfset)
    {
        Vector3 center = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        Vector3 right = new Vector3(center.x + shakeOfset, center.y, center.z);
        Vector3 left = new Vector3(center.x - shakeOfset, center.y, center.z);

        int shakeCount = 3;
        float shakespeed = 7.5f;

        Handheld.Vibrate();

        for(int i = 0; i<shakeCount; i++)
        {
            while (transform.localPosition != right)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, right, shakespeed * Time.deltaTime);
                yield return null;
            }

            while (transform.localPosition != left)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, left, shakespeed * Time.deltaTime);
                yield return null;
            }
        }

        while (transform.localPosition != center)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, center, shakespeed * Time.deltaTime);
            yield return null;
        }
    }
}
