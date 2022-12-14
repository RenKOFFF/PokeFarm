﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterMouseInteractController : MonoBehaviour
{
    private static void Interact()
    {
        var currentGridPosition = MarkerManager.Instance.markedCellPosition;

        var backgroundTilemap = TileMapReadManager.Instance.backgroundTilemap;
        var plantsTilemap = TileMapReadManager.Instance.plantsTilemap;

        var backgroundTileData = TileMapReadManager.Instance.GetBackgroundTileDataByGridPosition(currentGridPosition);
        var plantsTileData = TileMapReadManager.Instance.GetPlantsTileDataByGridPosition(currentGridPosition);

        if (backgroundTileData.IsPlantingCycleTile)
            InteractWithPlantingCycleTile(backgroundTileData, plantsTilemap, currentGridPosition);

        if (plantsTileData.IsBreakable)
        {
            InteractWithBreakableTile(plantsTileData, plantsTilemap, currentGridPosition);
            return;
        }
    }

    private static void InteractWithPlantingCycleTile(
        TileData backgroundTileData,
        Tilemap plantsTilemap,
        Vector3Int currentGridPosition)
    {
        var plantingCycleTile = backgroundTileData.PlantingCycleTile;

        var existingTile = plantsTilemap.GetTile(currentGridPosition);

        if (!plantingCycleTile.availableForPlant || existingTile != null)
            return;

        var itemOnTheHand = ToolbarManager.Instance.ItemOnTheHand;

        if (itemOnTheHand.type != ItemType.Seed)
            return;

        plantsTilemap.SetTile(
            currentGridPosition,
            itemOnTheHand.growCycleStartTile.tile);
    }

    private static void InteractWithBreakableTile(
        TileData plantsTileData,
        Tilemap plantsTilemap,
        Vector3Int currentGridPosition)
    {
        var breakableTile = plantsTileData.BreakableTile;

        if (!breakableTile.breaksByAny)
            return;

        plantsTilemap.SetTile(
            currentGridPosition,
            null);

        SpawnManager.Instance.SpawnPickUpItemsInArea(
            TileMapReadManager.GetCellCenterWorldPosition(plantsTilemap, currentGridPosition),
            breakableTile.dropsItem,
            breakableTile.dropAmount);
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Interact();
    }
}