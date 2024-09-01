using System.Collections.Generic;
using UnityEngine;

public class PieceDragHandler : MonoBehaviour
{
    private Camera mainCamera;
    private List<Piece> connectedPieces = new List<Piece>();
    private bool isDragging = false;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = -1.8f;

            if (connectedPieces.Count > 0)
            {
                // 赤い線の描画はPieceクラスに任せる
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
            piece.SetActive(false); // モノクロにする
            lastPiece.SetActive(false); // モノクロにする
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
