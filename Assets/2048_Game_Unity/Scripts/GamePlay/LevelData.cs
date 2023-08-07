using System;
using System.Collections;
using Datas;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObject/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<Data> _levelDatas = new List<Data>();

    public Data GetDatas(int level)
    {
        return _levelDatas[level - 1];
    }
}
