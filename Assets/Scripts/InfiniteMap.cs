using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteMap : MonoBehaviour
{
    [SerializeField] private int _width = 128;
    [SerializeField] private int _height = 128;
    [SerializeField] private int _rows = 3;
    [SerializeField] private int _cols = 3;
    [SerializeField] private GameObject _gridPrefab;
    
    private Transform[] _grids;
    
    int CoordToIndex(int row, int col)
    {
        return row * _cols + col;
    }

    private void Awake()
    {
        _grids = new Transform[_rows * _cols];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _cols; col++)
            {
                GameObject gridGameObject = Instantiate(_gridPrefab, transform.position, Quaternion.identity);
                gridGameObject.transform.parent = transform;

                _grids[CoordToIndex(row, col)] = gridGameObject.transform;
            }
        }
    }

    Vector2Int WorldPositionToCoord(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.y / _height), Mathf.RoundToInt(pos.x / _width));
    }

    Vector3 CoordToWorldPosition(Vector2Int coord, float z = 0.0f)
    {
        int row = coord.x;
        int col = coord.y;
        return new Vector3(col * _width, row * _height, z);
    }

    private void LateUpdate()
    {
        Vector3 camPos = Camera.main.transform.position;
        Vector2Int coord = WorldPositionToCoord(camPos);
        int currentRow = coord.x;
        int currentCol = coord.y;
        int minRow = currentRow - _rows / 2;
        int maxRow = currentRow + _rows / 2;
        int minCol = currentCol - _cols / 2;
        int maxCol = currentCol + _cols / 2;
        for (int row = minRow; row <= maxRow; row++)
        {
            for (int col = minCol; col <= maxCol; col++)
            {
                int effectiveRow = (((row % _rows) + _rows) % _rows);
                int effectiveCol = (((col % _cols) + _cols) % _cols);
                Vector3 pos = CoordToWorldPosition(new Vector2Int(row, col), 0.0f);
                _grids[CoordToIndex(effectiveRow, effectiveCol)].localPosition = pos;
            }
        }
    }
}
