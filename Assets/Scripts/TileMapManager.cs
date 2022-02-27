using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
  [SerializeField] Tilemap tilemap;
  [SerializeField] Tilemap collidersTilemap;
  [SerializeField] Tile[] groundTiles;
  [SerializeField] int rowNum;
  [SerializeField] int colNum;
  [SerializeField] int middleSpace = 1;

  private float _cellSizeX;
  private float _cellSizeY;
  private float _cellOffsetX;
  private PlayersSpawning _playersSpawning;
  private GameObject _tileMapMovePoint;
  private float _movementSpeed;

  void Awake()
  {
    _playersSpawning = GetComponent<PlayersSpawning>();

    _cellSizeX = tilemap.transform.localScale.x;
    _cellSizeY = tilemap.transform.localScale.y;

    // TODO: Move camera position instead
    _tileMapMovePoint = new GameObject("Tile_Map_Move_Point");
    _tileMapMovePoint.transform.parent = transform;

    if (colNum % 2 != 0)
    {
      Vector3 mapOffset = new Vector3(_cellSizeX / 2, 0, 0);
      tilemap.transform.Translate(mapOffset);
      collidersTilemap.transform.Translate(mapOffset);
      _tileMapMovePoint.transform.Translate(mapOffset);
      _cellOffsetX = _cellSizeX / 2;
    }

    GenerateTileMaps(groundTiles);
    GenerateBoundingWalls(groundTiles);
    SpawnPlayers(_cellSizeX, _cellSizeY, _cellOffsetX);
  }

  void Start()
  {
  }

  void Update()
  {
    // Reconciliation to the tile map move point
    // upTilemap, downTilemap and collidersTilemap has the same coordinates
    Vector3 tileMapMoveTarget = Vector3.MoveTowards(tilemap.transform.position, _tileMapMovePoint.transform.position, _movementSpeed * Time.deltaTime);
    tilemap.transform.position = tileMapMoveTarget;
    collidersTilemap.transform.position = tileMapMoveTarget;
  }

  public void MoveTileMap(Vector3 moveAmount, float speed)
  {
    _tileMapMovePoint.transform.position += moveAmount;
    _movementSpeed = speed;
  }

  private void GenerateTileMaps(Tile[] tiles)
  {
    if (tiles.Length == 0)
    {
      return;
    }

    int halfColNum = (int)Mathf.Ceil(colNum / 2f);
    BoundsInt upTilemapGrounds = new BoundsInt(new Vector3Int(-halfColNum, middleSpace, 1), new Vector3Int(colNum, rowNum, 1));
    BoundsInt downTilemapGrounds = new BoundsInt(new Vector3Int(-halfColNum, -(rowNum + middleSpace), 1), new Vector3Int(colNum, rowNum, 1));
    Tile[] groundTiles = Fill(rowNum * colNum, tiles[0]);
    tilemap.SetTilesBlock(upTilemapGrounds, groundTiles);
    tilemap.SetTilesBlock(downTilemapGrounds, groundTiles);
  }

  private void GenerateBoundingWalls(Tile[] tiles)
  {
    if (tiles.Length == 0)
    {
      return;
    }

    int wallNum = (rowNum + colNum) * 4 + 8;
    int halfWallNum = wallNum / 2;
    int halfColNum = (int)Mathf.Ceil(colNum / 2f);
    int oppositeOffset = rowNum + colNum + 2;

    Vector3Int[] positionArr = new Vector3Int[wallNum];
    int it = 0;
    for (int i = 0; i < colNum + 2; i++)
    {
      int xPos = i - halfColNum - 1;
      positionArr[it] = new Vector3Int(xPos, middleSpace - 1, 1);
      positionArr[it + oppositeOffset] = new Vector3Int(xPos, middleSpace + rowNum, 1);
      positionArr[it + halfWallNum] = new Vector3Int(xPos, -middleSpace, 1);
      positionArr[it + oppositeOffset + halfWallNum] = new Vector3Int(xPos, -middleSpace - 1 - rowNum, 1);
      it++;
    }

    int xRightPos = halfColNum - 1;
    int xLeftPos = -halfColNum - 1;
    for (int i = 0; i < rowNum; i++)
    {
      int yPos = i + middleSpace;
      positionArr[it] = new Vector3Int(xRightPos, yPos, 1);
      positionArr[it + oppositeOffset] = new Vector3Int(xLeftPos, yPos, 1);
      positionArr[it + halfWallNum] = new Vector3Int(xRightPos, -yPos - 1, 1);
      positionArr[it + oppositeOffset + halfWallNum] = new Vector3Int(xLeftPos, -yPos - 1, 1);
      it++;
    }

    Tile[] wallTiles = Fill(wallNum, tiles[1]);
    collidersTilemap.SetTiles(positionArr, wallTiles);
  }

  private void SpawnPlayers(float cellSizeX, float cellSizeY, float cellOffsetX)
  {
    float playerX = 0f;
    if (colNum > 1)
    {
      playerX = cellSizeX / 2 + cellOffsetX;
    }
    float playerY = (-rowNum - middleSpace + 0.5f) * cellSizeY;
    _playersSpawning.SpawnPlayer(playerX, playerY);
  }

  private static T[] Fill<T>(int size, T value)
  {
    T[] arr = new T[size];
    for (int i = 0; i < size; i++)
    {
      arr[i] = value;
    }
    return arr;
  }
}
