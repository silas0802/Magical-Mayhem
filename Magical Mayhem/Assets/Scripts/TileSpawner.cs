using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    [SerializeField] Vector2Int dimensions = new Vector2Int(10,10);
    [SerializeField] GameObject tile;
    [SerializeField] Transform player;


    // Start is called before the first frame update
    void Start()
    {
        SpawnTiles();
    }

    void SpawnTiles()
    {
        for (int i = 0; i < dimensions.x; i++)
        {
            for (int j = 0; j < dimensions.y; j++)
            {
                Instantiate(tile, new Vector3(i, 0, j), Quaternion.identity);
            }
        }
        player.position = new Vector3 (dimensions.x/2, 0.5f, dimensions.y/2);
    }
}
