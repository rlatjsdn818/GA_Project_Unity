using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 3f;

    public void MoveAlongPath(List<Vector2Int> path)
    {
        StartCoroutine(MoveRoutine(path));
    }

    IEnumerator MoveRoutine(List<Vector2Int> path)
    {
        foreach (var p in path)
        {
            Vector3 target = new Vector3(p.x, 1.2f, p.y);
            while (Vector3.Distance(transform.position, target) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}