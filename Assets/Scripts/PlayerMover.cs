using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMover : MonoBehaviour
{
    [SerializeField] bool isMoving;
    [SerializeField] float speed = 1.0f;
    [SerializeField] float touchSpeed = 1.0f;

    //move limits
    float minX = -2.0f;
    float maxX = 2.0f;

    //touches
    private float screenWidth;
    float startX;

    private void Awake()
    {
        screenWidth = Screen.width;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Vector3 moveDirection = new Vector3(0, 0, 1);
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began) startX = touch.position.x;

                if (touch.phase == TouchPhase.Moved)
                {
                    float movedX = touch.position.x-startX;
                    startX = touch.position.x;
                    float directionX = (movedX/screenWidth)*(touchSpeed);

                    if (directionX < 0 && transform.position.x > minX) moveDirection.x = directionX;
                    else if(directionX>0&&transform.position.x<maxX) moveDirection.x = directionX;

                }
            }

            transform.Translate(moveDirection * speed*Time.deltaTime);
        }
    }

    public void SetMoveAble(bool canMove)
    {
        isMoving = canMove;
    }


}
