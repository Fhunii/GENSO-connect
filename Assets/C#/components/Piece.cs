using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Piece : MonoBehaviour
{
    private CountdownManager countdownManager;
    public enum ElementType
    {
        HydrogenIon, Carbon, OxygenIon
    }

    public ElementType elementType;

    public Sprite hydrogenIonSprite;
    public Sprite carbonSprite;
    public Sprite oxygenIonSprite;

    public Sprite hydrogenIonMonoSprite;
    public Sprite carbonMonoSprite;
    public Sprite oxygenIonMonoSprite;

    private SpriteRenderer spriteRenderer;

    public GameObject sparkleEffectPrefab;
    public GameObject linePrefab;

    private static List<Piece> connectedPieces = new List<Piece>();
    private static Piece lastConnectedPiece;
    private static Piece firstClickedPiece;
    private static bool isDragging = false;
    private static List<LineRenderer> allLines = new List<LineRenderer>();

    private GridManager gridManager;
    private bool isActive = true;
    private static ScoreManager scoreManager;

    // 新しいスプライト生成用に座標を保存するリスト
    private static List<Vector3> savedPositions = new List<Vector3>();

    // 化合物ごとのスコアを設定
    private static Dictionary<List<ElementType>, int> compoundScores = new Dictionary<List<ElementType>, int>()
    {
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon }, 10 }, // メタン (CH₄)
        { new List<ElementType> { ElementType.Carbon, ElementType.OxygenIon }, 15 }, // 一酸化炭素 (CO)
        { new List<ElementType> { ElementType.Carbon, ElementType.OxygenIon, ElementType.OxygenIon }, 20 }, // 二酸化炭素 (CO₂)
        { new List<ElementType> { ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon }, 25 }, // 水 (H₂O)
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon }, 30 }, // メタノール (CH₃OH)
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon }, 35 }, // ホルムアルデヒド (H₂CO)
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon }, 40 }, // ギ酸 (HCOOH)
        { new List<ElementType> { ElementType.Carbon, ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon }, 45 }, // 酢酸 (CH₃COOH)
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon, ElementType.OxygenIon }, 50 }, // 炭酸 (H₂CO₃)
        { new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon, ElementType.OxygenIon }, 55 }, // 炭酸水素イオン (HCO₃⁻)
        // 他の組み合わせもここに追加する
    };

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>();
        scoreManager = FindObjectOfType<ScoreManager>(); // ScoreManager の参照を取得
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(AnimatePieceAppearance());
        countdownManager = FindObjectOfType<CountdownManager>();
        if (countdownManager != null && countdownManager.IsCountdownActive())
        {
            // カウントダウン中はこのスクリプトを無効にする
            this.enabled = false;
        }
    }

    private IEnumerator AnimatePieceAppearance()
    {
        Vector3 targetScale = new Vector3(0.06f, 0.06f, 0.06f);
        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }

    public void SetElementType(ElementType type)
    {
        elementType = type;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.sprite = isActive switch
        {
            true => elementType switch
            {
                ElementType.HydrogenIon => hydrogenIonSprite,
                ElementType.Carbon => carbonSprite,
                ElementType.OxygenIon => oxygenIonSprite,
                _ => spriteRenderer.sprite
            },
            false => elementType switch
            {
                ElementType.HydrogenIon => hydrogenIonMonoSprite,
                ElementType.Carbon => carbonMonoSprite,
                ElementType.OxygenIon => oxygenIonMonoSprite,
                _ => spriteRenderer.sprite
            }
        };
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateSprite();
    }

    void OnMouseDown()
    {
        if (countdownManager != null && countdownManager.IsCountdownActive())
        {
            return; // カウントダウン中は処理を行わない
        }
        if (!isActive) return; // モノクロの場合は処理しない

        isDragging = true;
        connectedPieces.Clear();
        connectedPieces.Add(this);
        lastConnectedPiece = this;
        firstClickedPiece = this;

        Vector2Int gridPosition = GetGridPosition();
        if (gridManager != null)
        {
            gridManager.SetActiveArea(gridPosition);
        }
    }

    void OnMouseDrag()
    {
        if (countdownManager != null && countdownManager.IsCountdownActive())
        {
            return; // カウントダウン中は処理を行わない
        }
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            Piece hitPiece = hit.collider.GetComponent<Piece>();
            if (hitPiece != null && hitPiece.isActive && hitPiece != lastConnectedPiece)
            {
                LineRenderer blueLine = CreateLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                allLines.Add(blueLine);

                lastConnectedPiece = hitPiece;
                connectedPieces.Add(hitPiece);
                hitPiece.SetActive(false); // isActive プロパティを変更しモノクロにする

                // クリックされたスプライトもモノクロにする
                SetActive(false);
            }
        }
    }

    private LineRenderer CreateLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        return lineRenderer;
    }

    void OnMouseUp()
    {
        if (countdownManager != null && countdownManager.IsCountdownActive())
        {
            return; // カウントダウン中は処理を行わない
        }
        if (!isDragging) return;

        isDragging = false;

        bool validCompound = CheckIfValidCompound(out List<ElementType> matchedCompound);

        if (validCompound)
        {
            // 座標を保存
            SavePiecePositions();

            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    piece.DestroyWithEffect();
                }
            }

            // スコアの追加
            if (matchedCompound != null && compoundScores.ContainsKey(matchedCompound))
            {
                int scoreToAdd = compoundScores[matchedCompound];
                scoreManager.AddScore(scoreToAdd);
            }

            // 新しいスプライトを生成
            CreateNewPieces();
        }
        else
        {
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    piece.SetActive(true); // すべてのスプライトを再びアクティブにする
                }
            }

            // 全ての青い線を削除
            foreach (var line in allLines)
            {
                Destroy(line.gameObject);
            }
        }

        connectedPieces.Clear();
        allLines.Clear();

        Vector2Int gridPosition = GetGridPosition();
        if (gridManager != null)
        {
            gridManager.DeactivateArea(gridPosition);
        }
    }

    private void SavePiecePositions()
    {
        savedPositions.Clear();
        foreach (var piece in connectedPieces)
        {
            savedPositions.Add(piece.transform.position);
        }
    }

    private void CreateNewPieces()
    {
        foreach (var position in savedPositions)
        {
            Piece newPiece = Instantiate(this, position, Quaternion.identity);
            newPiece.SetActive(true);
        }
    }

    public void DestroyWithEffect()
    {
        if (sparkleEffectPrefab != null)
        {
            Instantiate(sparkleEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private bool CheckIfValidCompound(out List<ElementType> matchedCompound)
    {
        matchedCompound = null;
        List<ElementType> connectedTypes = new List<ElementType>();

        foreach (var piece in connectedPieces)
        {
            connectedTypes.Add(piece.elementType);
        }

        foreach (var compound in compoundScores.Keys)
        {
            if (MatchCompound(connectedTypes, compound))
            {
                matchedCompound = compound;
                return true;
            }
        }
        return false;
    }

    private bool MatchCompound(List<ElementType> connectedTypes, List<ElementType> validCombination)
    {
        if (connectedTypes.Count != validCombination.Count) return false;

        List<ElementType> tempConnectedTypes = new List<ElementType>(connectedTypes);
        foreach (var element in validCombination)
        {
            if (tempConnectedTypes.Contains(element))
            {
                tempConnectedTypes.Remove(element);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public Vector2Int GetGridPosition()
    {
        Vector3 localPosition = transform.localPosition;
        int x = Mathf.RoundToInt(localPosition.x);
        int y = Mathf.RoundToInt(localPosition.y);
        return new Vector2Int(x, y);
    }
}
