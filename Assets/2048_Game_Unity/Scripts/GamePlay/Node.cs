using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 Pos => transform.position;
    private Block _occupiedBlock;
    public Block GetBlock
    {
        get => _occupiedBlock;
        set => _occupiedBlock = value;
    }
}
