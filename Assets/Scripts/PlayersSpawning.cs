using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersSpawning : MonoBehaviour
{
  [SerializeField] GameObject playerPrefab;

  public void SpawnPlayer(float x, float y)
  {
    Instantiate(playerPrefab, new Vector3(x, y, 0), Quaternion.identity);
  }
}
