using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;
using System.Collections;

public class TilemapMask : MonoBehaviour
{
    public Tilemap maskTilemap; // Tilemap para la máscara
    public Tilemap mainTilemap; // Tu Tilemap principal
    public TileBase newMaskTile; // El tile que deseas usar como máscara
    public float delay = 0.4f;
    private Vector3Int CoordsMask;  

    public void ChangeMask(Vector3Int cellPosition, int direction)
    {
        if (direction == 0) cellPosition.x += 1;
        else if (direction == 1) cellPosition.y += 1;
        else if (direction == 2) cellPosition.x -= 1;
        else if (direction == 3) cellPosition.y -= 1;

        Vector3Int tilemapOrigin = new Vector3Int(0, 9, 0);
        Vector3Int[] adjacentCellPositions = new Vector3Int[]
        {
            cellPosition * 2 + tilemapOrigin,
            cellPosition * 2 + new Vector3Int(-1, 0, 0) + tilemapOrigin, // Derecha
            cellPosition * 2 + new Vector3Int(0, -1, 0) + tilemapOrigin, // Arriba
            cellPosition * 2 + new Vector3Int(-1, -1, 0) + tilemapOrigin  // Diagonal superior derecha
        };

        Vector3Int adjustedCellPosition = cellPosition;
        Quaternion maskRotation = Quaternion.identity;

        if (direction == 2 || direction == 0)
        {
            maskRotation = Quaternion.Euler(0, 0, -90);
        }

        foreach (var adjCellPos in adjacentCellPositions)
        {
            DisableCollider(adjCellPos);
        }

        Vector3Int maskCellPosition = new Vector3Int(Mathf.RoundToInt((float)(cellPosition.x - 1)), cellPosition.y+4, 0); // Ajusta la posición para el Tilemap de la máscara
        // Invoca la función para establecer el tile con un retraso y pasa maskCellPosition como argumento
        StartCoroutine(ApplyMaskTileWithDelay(maskCellPosition));
    }

    private IEnumerator ApplyMaskTileWithDelay(Vector3Int maskCellPosition)
    {
        // Esperar el tiempo de retraso
        yield return new WaitForSeconds(delay);

        // Establece el tile en el Tilemap de la máscara
        maskTilemap.SetTile(maskCellPosition, newMaskTile);

        GameObject maskGameObject = maskTilemap.gameObject;
        Collider2D maskCollider = maskGameObject.AddComponent<BoxCollider2D>();
        maskCollider.isTrigger = true;
        maskGameObject.tag = "TileDig"; 
    }
    private void DisableCollider(Vector3Int cellPosition)
    {
        mainTilemap.SetColliderType(cellPosition, Tile.ColliderType.None);
    }
}
