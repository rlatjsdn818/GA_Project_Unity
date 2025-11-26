using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5.0f;

    // 높이 고정값
    private const float Y_POSITION = 1.2f;

    public void MoveAlongPath(List<Vector2Int> path)
    {
        StopAllCoroutines();
        StartCoroutine(MoveRoutine(path));
    }

    IEnumerator MoveRoutine(List<Vector2Int> path)
    {
        if (path == null || path.Count == 0)
        {
            yield break;
        }

        foreach (var node in path)
        {
            Vector3 targetPosition = new Vector3(node.x, Y_POSITION, node.y);
            Vector3 currentPosition = transform.position;

            float distance = Vector3.Distance(currentPosition, targetPosition);

            float duration = distance / moveSpeed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;

                transform.position = Vector3.Lerp(currentPosition, targetPosition, t);

                elapsed += Time.deltaTime;

                yield return null;
            }

            transform.position = targetPosition;
        }
    }
}