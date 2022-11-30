using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[Serializable]
public class SpawnableTile
{
    public TileBase tile;
    public float spawnIntervalInSeconds = 10;
    public Vector3Int prefabsTilemapPosition;

    [NonSerialized] public float TimeLeftToSpawnTileInSeconds;
}

public class LandscapeController : MonoBehaviour
{
    [SerializeField] private Collider2D safeFromObjectSpawningArea;
    [SerializeField] public Tilemap prefabsTilemap;
    [SerializeField] private SpawnableTile[] availableToSpawnTiles;

    private Vector3Int _minPosition;
    private Vector3Int _maxPosition;
    private Unity.Mathematics.Random _random;
    private TileMapReadManager _tileMapReadManager;
    
    public readonly Dictionary<TileBase, List<Vector3Int>> SpawnableTilesDictionary = new();
    public static LandscapeController Instance;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var tile in availableToSpawnTiles)
            tile.TimeLeftToSpawnTileInSeconds = tile.spawnIntervalInSeconds;

        _random = new Unity.Mathematics.Random((uint) DateTime.Now.Millisecond);
        _tileMapReadManager = TileMapReadManager.Instance;

        var cellBounds = _tileMapReadManager.backgroundTilemap.cellBounds;

        _minPosition = cellBounds.min;
        _maxPosition = cellBounds.max;
    }

    private void Update()
    {
        foreach (var tile in availableToSpawnTiles)
            TrySpawnTile(tile);
    }

    private void TrySpawnTile(SpawnableTile tile)
    {
        tile.TimeLeftToSpawnTileInSeconds -= Time.deltaTime;

        if (tile.TimeLeftToSpawnTileInSeconds < 0)
        {
            SpawnTile(tile);
            tile.TimeLeftToSpawnTileInSeconds = tile.spawnIntervalInSeconds;
        }
    }

    private void SpawnTile(SpawnableTile tile)
    {
        var existingTile = tile.tile;
        Vector3Int randomPosition = default;

        var triesAvailable = 100;

        while (existingTile != null)
        {
            if (triesAvailable-- == 0)
                return;

            randomPosition = new Vector3Int(
                _random.NextInt(_minPosition.x, _maxPosition.x),
                _random.NextInt(_minPosition.y, _maxPosition.y));

            var spawnWorldPosition = TileMapReadManager.GetCellCenterWorldPosition(
                _tileMapReadManager.backgroundTilemap,
                randomPosition);

            if (safeFromObjectSpawningArea.bounds.Contains(spawnWorldPosition))
                continue;

            existingTile = _tileMapReadManager.landscapeTilemap.GetTile(randomPosition);
        }

        _tileMapReadManager.landscapeTilemap.SetTile(
            new TileChangeData(
                randomPosition,
                tile.tile,
                Color.white,
                prefabsTilemap.GetTransformMatrix(tile.prefabsTilemapPosition)),
            false);

        if (!SpawnableTilesDictionary.ContainsKey(tile.tile))
        {
            SpawnableTilesDictionary[tile.tile] = new List<Vector3Int>();
        }
        SpawnableTilesDictionary[tile.tile].Add(randomPosition);

        //Debug.Log($"Spawned [{tile.tile.name}] on coordinates {randomPosition}.");
    }
}
