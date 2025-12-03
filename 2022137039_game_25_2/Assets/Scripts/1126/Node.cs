using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public Vector2Int pos;
    public float cost;

    public Node(Vector2Int pos, float cost)
    {
        this.pos = pos;
        this.cost = cost;
    }
}