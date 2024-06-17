using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;

    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private GameObject waterPrefab;

    private void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject prefabToInstantiate;

                // Determine which prefab to use based on the grid coordinates
                if (x % 2 == 0 && y % 2 == 0)
                {
                    prefabToInstantiate = grassPrefab;
                }
                else if (x % 2 == 1 && y % 2 == 0)
                {
                    prefabToInstantiate = dirtPrefab;
                }
                else
                {
                    prefabToInstantiate = waterPrefab;
                }

                // Instantiate the prefab at the current grid position
                GameObject spawnedPrefab = Instantiate(prefabToInstantiate, new Vector3(x, y, 0), Quaternion.identity);
                spawnedPrefab.name = $"Tile {x} {y}";
            }
        }
    }
}
