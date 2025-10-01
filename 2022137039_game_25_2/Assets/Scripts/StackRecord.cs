using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackRecord : MonoBehaviour
{
    public float speed = 5f;

    private Queue<Vector3> moveQueue;

    private bool isMoving = false;

    private Vector3 targetPos;

    private Stack<Vector3> moveHistory;

    // Start is called before the first frame update
    void Start()
    {
        moveHistory = new Stack<Vector3>();
        moveQueue = new Queue<Vector3>();
        targetPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (!isMoving)
        {
            if (x != 0 || y != 0)
            {
                Vector3 move = new Vector3(x, y, 0).normalized * speed * Time.deltaTime;
                moveHistory.Push(transform.position);
                
                targetPos += move;
                moveQueue.Enqueue(targetPos);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!isMoving && moveQueue.Count > 0)
                {
                    isMoving = true;
                    moveHistory.Push(transform.position);
                }
            }
        }
        else
        {
            if (moveQueue.Count > 0)
            {
                transform.position = moveQueue.Dequeue();
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        if (Input.GetKey(KeyCode.R))
        {
            if (moveHistory.Count > 0)
            {
                transform.position = moveHistory.Pop();
            }
        }
    }
}
