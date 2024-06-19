using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 128;
    [SerializeField] private int height = 128;

    [SerializeField] private GameObject grassPrefab;
    [SerializeField] private GameObject dirtPrefab;
    [SerializeField] private GameObject waterPrefab;

    private GameObject[,] matrix;
    private List<GameObject> instObjects = new List<GameObject>(); // Initialize the list

    private void Start()
    {
        matrix = new GameObject[height, width];

        GenerateMatrix();
        matrix[20, 15] = waterPrefab;
        RenderMatrix();
    }

    void GenerateMatrix()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (y % 2 == 0)
                {
                    if (y == 10) matrix[y, x] = waterPrefab;
                    else if (x % 2 == 0) matrix[y, x] = grassPrefab;
                    else matrix[y, x] = dirtPrefab;
                }
                else
                {
                    if (x % 2 == 0) matrix[y, x] = dirtPrefab;
                    else matrix[y, x] = grassPrefab;
                }
            }
        }
    }

    void RenderMatrix()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Instantiate and add to instObjects list
                instObjects.Add(Instantiate(matrix[y, x], new Vector3(x, y, 0), Quaternion.identity));
            }
        }
    }

    void DestroyMatrix()
    {
        foreach (GameObject gameObject in instObjects)
        {
            Destroy(gameObject);
        }
        instObjects.Clear(); // Clear the list after destroying objects
    }

    public CellTile.CellType GetCellTypeInLocation(Vector3 location)
    {
        if (location.x >= 0 && location.x < width && location.y >= 0 && location.y < height)
        {
            CellTile tile = matrix[(int)location.y, (int)location.x].GetComponent<CellTile>();
            if (tile != null) return tile.GetCellType();
            else return CellTile.CellType.none;
        }
        else return CellTile.CellType.none;
    }
}
