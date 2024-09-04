using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefab;
    public GameObject piecePrefab;
    public Vector2 startPosition;
    public GameObject tilesParent;
    public GameObject piecesParent;
    public Piece[,] grid;
    public Vector2 tileSize; // 追加: tileSize を公開するプロパティ

    void Start()
    {
        SpriteRenderer spriteRenderer = tilePrefab.GetComponent<SpriteRenderer>();
        tileSize = spriteRenderer.bounds.size; // tileSize を初期化
        grid = new Piece[width, height];
        StartCoroutine(GenerateGrid());
    }

    IEnumerator GenerateGrid()
    {
        Vector2 gridSize = new Vector2(tileSize.x * width, tileSize.y * height);
        Vector2 gridOrigin = startPosition - gridSize / 2;

        float delay = 0f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = gridOrigin + new Vector2(x * tileSize.x, y * tileSize.y);

                // タイルの生成
                var tile = Instantiate(tilePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity, tilesParent.transform);
                tile.name = $"Tile {x} {y}";

                TileAnimation tileAnimation = tile.GetComponent<TileAnimation>();
                if (tileAnimation != null)
                {
                    tileAnimation.AnimateTile(delay);
                    delay += 0.05f;
                }

                // ピースの生成
                var piece = Instantiate(piecePrefab, new Vector3(position.x, position.y, -0.1f), Quaternion.identity, piecesParent.transform);
                piece.name = $"Piece {x} {y}";
                var pieceComponent = piece.GetComponent<Piece>();

                // 変更箇所: ランダムなElementTypeの選択
                Piece.ElementType randomType = (Piece.ElementType)Random.Range(0, 3);
                pieceComponent.SetElementType(randomType);

                grid[x, y] = pieceComponent;

                PieceAnimation pieceAnimation = piece.GetComponent<PieceAnimation>();
                if (pieceAnimation != null)
                {
                    pieceAnimation.FadeIn(delay);
                    delay += 0.05f;
                }

                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    public void SetActiveArea(Vector2Int center)
    {
        // クリック位置を右に2マス、上に4マス移動
        Vector2Int adjustedCenter = new Vector2Int(center.x + 2, center.y + 4);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool isActive = Mathf.Abs(x - adjustedCenter.x) <= 1 && Mathf.Abs(y - adjustedCenter.y) <= 1;
                grid[x, y].SetActive(isActive);
            }
        }
    }

    public void ResetActiveArea()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y].SetActive(true);
            }
        }
    }

    public bool IsAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        return (Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y)) == 1;
    }
}
