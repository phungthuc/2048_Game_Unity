using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Datas;
using UnityEngine;
using DG.Tweening;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Sequence = DG.Tweening.Sequence;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private List<LevelData> _levelDatas = new List<LevelData>();
    [SerializeField] private GameObject _nodePrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private GameObject _blockPrefab;
    [SerializeField] private TextMeshProUGUI _textLevelCurrent;

    [SerializeField] private List<BlockType> _types;

    private int _width = 4;
    private int _height = 4;
    private bool _click = true;
    private float _travelTime = 0.2f;

    public UnityEvent Loaded;
    [FormerlySerializedAs("WinLevel")] public UnityEvent WinLevelEvent;

    [FormerlySerializedAs("LossLevelEvent")] [FormerlySerializedAs("LossLevel")]
    public UnityEvent LoseLevelEvent;

    private List<Node> _nodes;
    private List<Block> _blocks;
    private SpriteRenderer _board;
    private int _currentLevel = 1;
    private Data _levelData;
    private Sequence _sequence;

    private bool _isWon = false;
    private BlockType GetBlocktypeByValue(int value) => _types.First(type => type.Value == value);

    private System.Random Random = new System.Random();

    public void Start()
    {
        ChangeState(GameState.LoadLevelData);
        Loaded.Invoke();
    }

    public void LoadGame()
    {
        ChangeState(GameState.GenerateLevel);
    }

    public void LoadData(string status)
    {
        _click = true;
        _isWon = false;
        if (status.Equals("nextLevel") && _currentLevel < 3)
        {
            _currentLevel++;
        }

        ChangeState(GameState.LoadLevelData);
        ChangeState(GameState.GenerateLevel);
    }

    public void ChangeState(GameState newState)
    {
        switch (newState)
        {
            case GameState.LoadLevelData:
                LoadLevelData(_currentLevel);
                break;
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(2);
                break;
            case GameState.Win:
                WinLevelCurrent();
                break;
            case GameState.Lose:
                LossLevelCurrent();
                break;
            default:
                break;
        }
    }
    void LoadLevelData(int level)
    {
        _levelData = _levelDatas[0].GetDatas(level);
        Debug.Log("Load Level Data Completed!");
    }

    void GenerateGrid()
    {
        _nodes = new List<Node>();      
        _blocks = new List<Block>();
        _nodes.Clear();
        _blocks.Clear();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x, y), Quaternion.identity);
                var component = node.GetComponent<Node>();
                _nodes.Add(component);
            }
        }

        var center = new Vector2((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f);

        _board = Instantiate(_boardPrefab, center, Quaternion.identity);
        _board.size = new Vector2(_width, _height);

        Camera.main.transform.position = new Vector3(center.x, center.y, -10);
        _textLevelCurrent.text = "Level " + _currentLevel;

        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {
        if (_isWon)
        {
            return;
        }

        var freeNodes = _nodes.Where(node => node.GetBlock == null)
            .OrderBy(b => (float)Random.NextDouble()).ToList();
        foreach (var node in freeNodes.Take(amount))
        {
            SpawnBlock(node, (float)Random.NextDouble() > 0.8f ? 4 : 2);
        }

        if (freeNodes.Count() == 1)
        {
            ChangeState(GameState.Lose);
        }
    }

    void SpawnBlock(Node node, int value)
    {
        if (value == _levelData.Max)
        {
            ChangeState(GameState.Win);
        }

        if (_isWon)
        {
            return;
        }

        var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity, transform);
        var component = block.GetComponent<Block>();
        component.Init(GetBlocktypeByValue(value));
        component.SetBlock(node);
        _blocks.Add(component);
    }

    public void getInput(String input)
    {
        if (_click == true && !_isWon)
        {
            switch (input)
            {
                case "right":
                    MoveBlocks(Vector2.right);
                    _click = false;
                    break;
                case "left":
                    MoveBlocks(Vector2.left);
                    _click = false;
                    break;
                case "up":
                    MoveBlocks(Vector2.up);
                    _click = false;
                    break;
                case "down":
                    MoveBlocks(Vector2.down);
                    _click = false;
                    break;
                default:
                    break;
            }
        }
    }

    void MoveBlocks(Vector2 direction)
    {
        if (_blocks == null)
        {
            return;
        }

        var oderByBlocks = _blocks.OrderBy(block => block.Pos.x)
            .ThenBy(block => block.Pos.y).ToList();
        if (direction == Vector2.right || direction == Vector2.up) oderByBlocks.Reverse();

        foreach (var oderByBlock in oderByBlocks)
        {
            var next = oderByBlock.Node;
            do
            {
                oderByBlock.SetBlock(next);

                var posibleNode = GetNodeAtPosition(next.Pos + direction);

                if (posibleNode != null)
                {
                    if (posibleNode.GetBlock != null && posibleNode.GetBlock.CanMerge(oderByBlock.Value))
                    {
                        oderByBlock.MergeBlock(posibleNode.GetBlock);
                    }
                    else if (posibleNode.GetBlock == null) next = posibleNode;
                }
            } while (next != oderByBlock.Node);
        }

        _sequence = DOTween.Sequence();

        foreach (var oderByBlock in oderByBlocks)
        {
            var movePoint = oderByBlock.GetBlock != null ? oderByBlock.GetBlock.Node.Pos : oderByBlock.Node.Pos;
            _sequence.Insert(0, oderByBlock.transform.DOMove(movePoint, _travelTime));
        }

        _sequence.OnComplete(() =>
        {
            foreach (var oderByBlock in oderByBlocks.Where(block => block.GetBlock != null))
            {
                MergeBlocks(oderByBlock.GetBlock, oderByBlock);
            }

            SpawnBlocks(1);
            _click = true;
        });
    }

    void MergeBlocks(Block baseBlock, Block mergingBlock)
    {
        SpawnBlock(baseBlock.Node, baseBlock.Value * 2);
        RemoveBlock(baseBlock);
        RemoveBlock(mergingBlock);
    }

    void RemoveBlock(Block block)
    {
        _blocks.Remove(block);
        Destroy(block.gameObject);
    }

    void EndGame()
    {
        _isWon = true;
        foreach (var node in _nodes)
        {
            Destroy(node.gameObject);
        }

        for (var i = _blocks.Count - 1; i >= 0; i--)
        {
            Destroy(_blocks[i].gameObject);
        }

        Destroy(_board.gameObject);
    }

    void WinLevelCurrent()
    {
        WinLevelEvent.Invoke();
        EndGame();
    }

    void LossLevelCurrent()
    {
        LoseLevelEvent.Invoke();
        EndGame();
    }

    Node GetNodeAtPosition(Vector2 pos)
    {
        return _nodes.FirstOrDefault(node => node.Pos == pos);
    }
}

[Serializable]
public struct BlockType
{
    public int Value;
    public Color Color;
}

public enum GameState
{
    LoadLevelData,
    GenerateLevel,
    SpawningBlocks,
    Win,
    Lose
}