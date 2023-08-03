using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;

public class LevelManager : MonoBehaviour
{
    [SerializeField] 
    private GameObject nodePrefab;

    [SerializeField] 
    private SpriteRenderer boardPrefab;

    [SerializeField] 
    private GameObject blockPrefab;

    private List<GameObject> listBlockPrefabs = new List<GameObject>();

    private int _width = 4;
    private int _height = 4;
    private bool click = true;
    private int minX = 0;
    private int maxX = 3;
    private int minY = 0;
    private int maxY = 3;
        
    
    void Start()
    {
        _InitBoards();
        _InitBlocks();
    }

    private void OnGUI()
    {
        if (click)
        {
            Event e = Event.current;
            switch (e.keyCode)
            {
                case KeyCode.W:
                    MoveBlocks("top", TODO);
                    click = false;
                    break;
                case KeyCode.A:
                    MoveBlocks("left", TODO);
                    click = false;
                    break;
                case KeyCode.S:
                    MoveBlocks("bottom", TODO);
                    click = false;
                    break;
                case KeyCode.D:
                    MoveBlocks("right", TODO);
                    click = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void TODO()
    {
        click = true;
    }

    void _InitBoards()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(nodePrefab, new Vector2(x, y), Quaternion.identity);
            }
        }
        var center = new Vector2((float) _width /2 - 0.5f,(float) _height / 2 -0.5f);

        var board = Instantiate(boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width,_height);

        Camera.main.transform.position = new Vector3(center.x,center.y,-10);
    }

    void _InitBlocks()
    {
        var block = Instantiate(blockPrefab, new Vector2(0, 0), Quaternion.identity);
        var block1 = Instantiate(blockPrefab, new Vector2(1, 1), Quaternion.identity);
        listBlockPrefabs.Add(block);
        listBlockPrefabs.Add(block1);
    }

    void MoveBlocks(string direction, [NotNull] TweenCallback onCompleteMoveBlock)
    {
        if (onCompleteMoveBlock == null) throw new ArgumentNullException(nameof(onCompleteMoveBlock));
        foreach (var block in listBlockPrefabs)
        {
            switch (direction)
            {
                case "top":
                    block.transform.DOMoveY(maxY, (float)0.25).OnComplete(onCompleteMoveBlock);
                    break;
                case "left":
                    block.transform.DOMoveX(minX, (float)0.25).OnComplete(onCompleteMoveBlock);
                    break;
                case "bottom":
                    block.transform.DOMoveY(minY, (float)0.25).OnComplete(onCompleteMoveBlock);
                    break;
                case "right":
                    block.transform.DOMoveX(maxX, (float)0.25).OnComplete(onCompleteMoveBlock);
                    break;
                default:
                    break;
            }
        }
    }
}