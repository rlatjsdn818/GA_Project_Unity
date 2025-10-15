using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class TurnBasedCombat : MonoBehaviour
{

    SimplePriorityQueue<string> queue = new SimplePriorityQueue<string>();

    public float speed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        //float maxPriority = 20f;
        //var queue = new SimplePriorityQueue<string>();
        
        

    }

    // Update is called once per frame
    void Update()
    {
        queue.Enqueue("Warrior", speed - 5);
        queue.Enqueue("Magician", speed - 7);
        queue.Enqueue("Archer", speed - 10);
        queue.Enqueue("Thief", speed - 12);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            while (queue.Count > 0)
            {
                Debug.Log("스페이스 눌림");
                Debug.Log(queue.Dequeue());
                if (queue.Count > 4)
                {
                    
                }
            }
        }
    }

    public void test()
    {
        Debug.Log("스페이스 눌림");
        Debug.Log(queue.Dequeue());
    }
}
