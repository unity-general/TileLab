using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMovement : MonoBehaviour
{
  [HideInInspector]
  public float stepSize = 0.5f;

  [SerializeField] float speed = 5f;
  [SerializeField] GameObject movePoint;

  private TileMapManager _tileMapManager;

  void Start()
  {
    movePoint.transform.parent = null;
    _tileMapManager = GameObject.FindGameObjectWithTag("TileMapSpawner").GetComponent<TileMapManager>();
  }

  void Update()
  {
    float xAmount = 0;
    float yAmount = 0;
    if (Input.GetKeyDown(KeyCode.A))
    {
      xAmount = -stepSize;
    }
    else if (Input.GetKeyDown(KeyCode.D))
    {
      xAmount = stepSize;
    }
    else if (Input.GetKeyDown(KeyCode.W))
    {
      yAmount = stepSize;
    }
    else if (Input.GetKeyDown(KeyCode.S))
    {
      yAmount = -stepSize;
    }
    else
    {
      xAmount = 0;
      yAmount = 0;
    }

    Vector3 moveAmount = new Vector3(xAmount, yAmount, 0);
    Vector3 newPos = movePoint.transform.position + moveAmount;

    Vector3 leftScreenBound = Camera.main.WorldToViewportPoint(new Vector3(newPos.x - 0.5f, 0, 0));
    Vector3 rightScreenBound = Camera.main.WorldToViewportPoint(new Vector3(newPos.x + 0.5f, 0, 0));
    if (leftScreenBound.x > 0 && rightScreenBound.x < 1)
    {
      movePoint.transform.position = newPos;
      transform.position = Vector3.MoveTowards(transform.position, movePoint.transform.position, speed * Time.deltaTime);
    }
    else
    {
      _tileMapManager.MoveTileMap(-moveAmount, speed);
    }
  }
}
