using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;

    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private GameObject waterPrefab;

    [SerializeField] private GameObject[,] matrix;

    CellTile.CellType cellType;

    private void Start()
    {
        matrix = new GameObject[height, width];

        GenerateMatrix();
    }

    void GenerateMatrix()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y % 2 == 0)
                {
                    if (x % 2 == 0) matrix[y, x] = Instantiate(grassPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    else matrix[y, x] = Instantiate(dirtPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else
                {
                    if (x % 2 == 0) matrix[y, x] = Instantiate(dirtPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    else matrix[y, x] = Instantiate(grassPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }

            }
        }
    }
    void DestroyMatrix()
    {
        foreach (GameObject gameObject in matrix)
        {
            Destroy(gameObject);
        }
    }

    public CellTile.CellType GetCellTypeInLocation(Vector3 location)
    {
        if (location.y >= 0 && location.x >= 0 && location.x <= width && location.y <= height)
        {
            CellTile tile = matrix[(int)location.y, (int)location.x].GetComponent<CellTile>();
            if (tile != null) return tile.GetCellType();
            else return CellTile.CellType.none;
        }
        else return CellTile.CellType.none;
    }
}
