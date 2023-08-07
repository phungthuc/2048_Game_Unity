using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int Value;
    public Node Node;
    private Block _mergingBlock;
    public Block GetBlock => _mergingBlock;
    public bool Merging;
    
    public Vector2 Pos => transform.position;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private TextMesh _text;

    public void Init(BlockType type)
    {
        Value = type.Value;
        _renderer.color = type.Color;
        _text.text = type.Value.ToString();
    }

    public void SetBlock(Node node) {
        if (Node != null) Node.GetBlock = null;
        Node = node;
        Node.GetBlock = this;
    }

    public void MergeBlock(Block blockToMergeWith)
    {
        _mergingBlock = blockToMergeWith;

        Node.GetBlock = null;

        blockToMergeWith.Merging = true;

    }

    public bool CanMerge(int value) => value == Value && !Merging && _mergingBlock == null;
}
