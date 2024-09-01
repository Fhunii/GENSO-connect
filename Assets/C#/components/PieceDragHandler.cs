using System.Collections.Generic;
using UnityEngine;

public class PieceDragHandler : MonoBehaviour
{
    private Camera mainCamera;
    private List<Piece> connectedPieces = new List<Piece>();
    private LineRenderer lineRenderer;
    private bool isDragging = false;

    void Awake()
    {
        mainCamera = Camera.main;
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        // 線の色を背景に対して目立つ色に変更（例えば、赤色）
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = Color.red };
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = -1.8f;

            if (connectedPieces.Count > 0)
            {
                lineRenderer.positionCount = connectedPieces.Count + 1;
                for (int i = 0; i < connectedPieces.Count; i++)
                {
                    lineRenderer.SetPosition(i, connectedPieces[i].transform.position);
                }
                lineRenderer.SetPosition(connectedPieces.Count, mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            CheckConnections();
        }
    }

    void OnMouseDown()
    {
        Piece piece = GetPieceUnderMouse();
        if (piece != null)
        {
            connectedPieces.Clear();
            connectedPieces.Add(piece);
            isDragging = true;
        }
    }

    void OnMouseDrag()
    {
        Piece piece = GetPieceUnderMouse();
        if (piece != null && !connectedPieces.Contains(piece))
        {
            Piece lastPiece = connectedPieces[connectedPieces.Count - 1];
            
            connectedPieces.Add(piece);
            
        }
    }

    void CheckConnections()
    {
        if (connectedPieces.Count > 1)
        {
            foreach (Piece piece in connectedPieces)
            {
                piece.DestroyWithEffect();
            }
        }

        connectedPieces.Clear();
        lineRenderer.positionCount = 0;
    }

    Piece GetPieceUnderMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            return hit.collider.GetComponent<Piece>();
        }
        return null;
    }
}
