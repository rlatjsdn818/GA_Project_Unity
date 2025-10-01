using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueSample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Queue<string> queue = new Queue<string>();

        queue.Enqueue("First");
        queue.Enqueue("Second");
        queue.Enqueue("Third");

        Debug.Log("====== Queue 1 ======");
        foreach (string item in queue)
        {
            Debug.Log(item);
        }

        Debug.Log("Peek : " + queue.Peek());

        Debug.Log("Peek : " + queue.Dequeue());
        Debug.Log("Peek : " + queue.Dequeue());
        Debug.Log("Peek : " + queue.Dequeue());

        Debug.Log("Data Left: " + queue.Count);

        Debug.Log("====== Queue 2 ======");
        foreach (string item in queue)
        {
            Debug.Log(item);
        }
    }

}
