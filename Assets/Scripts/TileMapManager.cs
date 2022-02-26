using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
  [SerializeField] Tilemap upTilemap;
  [SerializeField] Tilemap downTilemap;
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

    _cellSizeX = upTilemap.transform.localScale.x;
    _cellSizeY = upTilemap.transform.localScale.y;

    _tileMapMovePoint = new GameObject("Tile_Map_Move_Point");
    _tileMapMovePoint.transform.parent = transform;

    if (colNum % 2 != 0)
    {
      Vector3 mapOffset = new Vector3(_cellSizeX / 2, 0, 0);
      upTilemap.transform.Translate(mapOffset);
      downTilemap.transform.Translate(mapOffset);
      collidersTilemap.transform.Translate(mapOffset);
      _tileMapMovePoint.transform.Translate(mapOffset);
      _cellOffsetX = _cellSizeX / 2;
    }

    GenerateTileMaps(groundTiles, _cellSizeX, _cellSizeY, _cellOffsetX);
    GenerateBoundingWalls(groundTiles, _cellSizeX, _cellSizeY);
  }

  void Start()
  {
  }

  void Update()
  {
    // Reconciliation to the tile map move point
    // upTilemap, downTilemap and collidersTilemap has the same coordinates
    Vector3 tileMapMoveTarget = Vector3.MoveTowards(upTilemap.transform.position, _tileMapMovePoint.transform.position, _movementSpeed * Time.deltaTime);
    upTilemap.transform.position = tileMapMoveTarget;
    downTilemap.transform.position = tileMapMoveTarget;
    collidersTilemap.transform.position = tileMapMoveTarget;
  }

  public void MoveTileMap(Vector3 moveAmount, float speed)
  {
    _tileMapMovePoint.transform.position += moveAmount;
    _movementSpeed = speed;
  }

  private void GenerateTileMaps(Tile[] tiles, float cellSizeX, float cellSizeY, float cellOffsetX)
  {
    if (tiles.Length == 0)
    {
      return;
    }

    float xPos;
    float yPos;
    float halfColNum = colNum / 2;
    Vector3 tilePosTemp;
    Vector3 yNegativeOffsetMask = new Vector3(0, cellSizeY, 1);
    Vector3 xMirrorMask = new Vector3(1, -1, 1);
    for (int i = 0; i < rowNum; i++)
    {
      for (int j = 0; j < colNum; j++)
      {
        xPos = (j - halfColNum) * cellSizeX;
        yPos = (i + middleSpace) * cellSizeY;
        tilePosTemp = new Vector3(xPos, yPos);
        upTilemap.SetTile(upTilemap.WorldToCell(tilePosTemp), tiles[0]);
        downTilemap.SetTile(upTilemap.WorldToCell(Vector3.Scale(tilePosTemp, xMirrorMask) - yNegativeOffsetMask), tiles[0]);
      }
    }

    float playerX = 0f;
    if (colNum > 1)
    {
      playerX = cellSizeX / 2 + cellOffsetX;
    }
    float playerY = (-rowNum - middleSpace + 0.5f) * cellSizeY;
    _playersSpawning.SpawnPlayer(playerX, playerY);
  }

  private void GenerateBoundingWalls(Tile[] tiles, float cellSizeX, float cellSizeY)
  {
    if (tiles.Length == 0)
    {
      return;
    }

    float xPos;
    float yPos;
    float halfColNum = colNum / 2 + 1;
    Vector3 tilePosTemp;

    // Set wall tiles horizontally
    Vector3 yUpperOffset = new Vector3(0, (rowNum + 1) * cellSizeY, 0);
    yPos = (middleSpace - 1) * cellSizeY;
    for (int i = 0; i < colNum + 2; i++)
    {
      xPos = (i - halfColNum) * cellSizeX;
      tilePosTemp = new Vector3(xPos, yPos);
      Vector3 lowerPos = tilePosTemp;
      Vector3 upperPos = tilePosTemp + yUpperOffset;
      GenerateBoundingWallSides(tiles, lowerPos, upperPos, cellSizeY);
    }

    // Set wall tiles vertically
    Vector3 xRightOffset = new Vector3((colNum + 1) * cellSizeX, 0, 0);
    xPos = -halfColNum * cellSizeX;
    for (int i = 0; i < rowNum; i++)
    {
      yPos = (i + middleSpace) * cellSizeY;
      tilePosTemp = new Vector3(xPos, yPos);
      Vector3 leftPos = tilePosTemp;
      Vector3 rightPos = tilePosTemp + xRightOffset;
      GenerateBoundingWallSides(tiles, leftPos, rightPos, cellSizeY);
    }
  }

  private void GenerateBoundingWallSides(Tile[] tiles, Vector3 sidePos, Vector3 oppositeSidePos, float cellSizeY)
  {
    Vector3 xMirrorMask = new Vector3(1, -1, 1);
    Vector3 yNegativeOffsetMask = new Vector3(0, cellSizeY, 1);
    collidersTilemap.SetTile(collidersTilemap.WorldToCell(sidePos), tiles[1]);
    collidersTilemap.SetTile(collidersTilemap.WorldToCell(oppositeSidePos), tiles[1]);
    collidersTilemap.SetTile(collidersTilemap.WorldToCell(Vector3.Scale(sidePos, xMirrorMask) - yNegativeOffsetMask), tiles[1]);
    collidersTilemap.SetTile(collidersTilemap.WorldToCell(Vector3.Scale(oppositeSidePos, xMirrorMask) - yNegativeOffsetMask), tiles[1]);
  }
}
