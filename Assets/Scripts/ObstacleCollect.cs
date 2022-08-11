using System.Collections;
using System.Collections.Generic;
using RVO;
using UnityEngine;
using Vector2 = RVO.Vector2;

/// <summary>
/// 外置障碍物集合
/// </summary>
public class ObstacleCollect : MonoBehaviour
{
    void Awake()
    {
        BoxCollider2D[] boxColliders = GetComponentsInChildren<BoxCollider2D>();
        for (int i = 0; i < boxColliders.Length; i++)
        {
            float minX = boxColliders[i].transform.position.x -
                         boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
            float minY = boxColliders[i].transform.position.y -
                         boxColliders[i].size.y*boxColliders[i].transform.lossyScale.y*0.5f;
            float maxX = boxColliders[i].transform.position.x +
                         boxColliders[i].size.x*boxColliders[i].transform.lossyScale.x*0.5f;
            float maxY = boxColliders[i].transform.position.y +
                         boxColliders[i].size.y*boxColliders[i].transform.lossyScale.y*0.5f;

            IList<Vector2> obstacle = new List<Vector2>();
            obstacle.Add(new Vector2(maxX, maxY));
            obstacle.Add(new Vector2(minX, maxY));
            obstacle.Add(new Vector2(minX, minY));
            obstacle.Add(new Vector2(maxX, minY));
            Simulator.Instance.addObstacle(obstacle);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}