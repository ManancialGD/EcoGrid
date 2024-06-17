using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTile : MonoBehaviour
{
    public enum CellType { grass, dirt, stone, water, path, }
    [SerializeField] private CellType thisCellType;

    public CellType GetCellType()
    {
        return thisCellType;
    }
}
