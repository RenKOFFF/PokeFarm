﻿using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CutDownTreesState : BaseMonsterCommandState
{
    public CutDownTreesState(Monster monster, float speed, TileBase breakTile)
    {
        _monsterBehaviour = monster.MonsterBehaviour;
        _speed = speed;

        _waitTime = _startWaitTime;
        _breakTile = breakTile;
    }
}