using UnityEngine;
using System.Collections.Generic;

public class PieceDragHandler : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Piece> connectedPieces = new List<Piece>();
    private Vector2Int lastPosition;
    private GridManager gridManager;
    private bool isDragging;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    void OnMouseDown()
    {
        isDragging = true;
        connectedPieces.Clear();
        lastPosition = GetGridPosition(transform.position);
        AddPieceToConnectedList(transform.GetComponent<Piece>());
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector2Int currentPosition = GetGridPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (gridManager.IsAdjacent(lastPosition, currentPosition))
            {
                Piece piece = gridManager.grid[currentPosition.x, currentPosition.y];
                if (piece != null && !connectedPieces.Contains(piece) && piece.GetComponent<SpriteRenderer>().color != Color.black)
                {
                    AddPieceToConnectedList(piece);
                    lastPosition = currentPosition;
                }
            }
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (connectedPieces.Count > 0)
        {
            // チェックして消去するかどうか決定する
            if (CheckIfValidCompound(connectedPieces))
            {
                foreach (var piece in connectedPieces)
                {
                    Destroy(piece.gameObject);
                }
            }
            else
            {
                // 無効な場合は青い線を消去する
                if (lineRenderer != null)
                {
                    lineRenderer.positionCount = 0;
                }
            }
        }
    }

    private void AddPieceToConnectedList(Piece piece)
    {
        connectedPieces.Add(piece);
        lineRenderer.positionCount = connectedPieces.Count;
        lineRenderer.SetPosition(connectedPieces.Count - 1, piece.transform.position);
    }

    private Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector2Int gridPosition = new Vector2Int(
            Mathf.RoundToInt((worldPosition.x - gridManager.startPosition.x) / gridManager.tileSize.x),
            Mathf.RoundToInt((worldPosition.y - gridManager.startPosition.y) / gridManager.tileSize.y)
        );
        return gridPosition;
    }

    private bool CheckIfValidCompound(List<Piece> pieces)
    {
        Dictionary<Piece.ElementType, int> elementCounts = new Dictionary<Piece.ElementType, int>();
        foreach (var piece in pieces)
        {
            Piece.ElementType type = piece.GetElementType();
            if (!elementCounts.ContainsKey(type))
            {
                elementCounts[type] = 0;
            }
            elementCounts[type]++;
        }

        return ValidateCompound(elementCounts);
    }

    private bool ValidateCompound(Dictionary<Piece.ElementType, int> elementCounts)
    {
        // ここに化合物の検証ロジックを実装する
        return false; // デフォルトで無効とする
    }
}
