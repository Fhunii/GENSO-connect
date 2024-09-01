using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public enum ElementType { HydrogenIon, Carbon, OxygenIon }

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
    private static List<LineRenderer> blueLines = new List<LineRenderer>();

    private GridManager gridManager;
    private bool isActive = true;

    private List<List<Piece.ElementType>> validCombinations = new List<List<Piece.ElementType>>()
    {
        new List<Piece.ElementType> { Piece.ElementType.HydrogenIon, Piece.ElementType.Carbon, Piece.ElementType.OxygenIon }, // 例: H2O
        // メタン (CH₄)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon },

        // 一酸化炭素 (CO)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.OxygenIon },

        // 二酸化炭素 (CO₂)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon },

        // 水 (H₂O)
        new List<Piece.ElementType> { Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon },

        // メタノール (CH₃OH)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon },

        // ホルムアルデヒド (H₂CO)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon },

        // ギ酸 (HCOOH)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon },

        // 酢酸 (CH₃COOH)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon },

        // 炭酸 (H₂CO₃)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon },

        // 炭酸水素イオン (HCO₃⁻)
        new List<Piece.ElementType> { Piece.ElementType.Carbon, Piece.ElementType.HydrogenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon, Piece.ElementType.OxygenIon },

        // 他の組み合わせもここに追加する
    };

    public ElementType GetElementType()
    {
        return elementType;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = FindObjectOfType<GridManager>();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(AnimatePieceAppearance());
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
        if (spriteRenderer == null)
        {
            return;
        }

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
        if (!isActive) return;  // モノクロの場合は処理しない

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

        // 赤い線のコードは削除しました

        // 最初のクリックされたスプライトとマウスの間に青い線を引く
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        // 青い線を引く
        if (lastConnectedPiece != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                Piece hitPiece = hit.collider.GetComponent<Piece>();
                if (hitPiece != null && hitPiece.isActive && hitPiece != lastConnectedPiece)
                {
                    LineRenderer blueLine = CreateBlueLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                    blueLines.Add(blueLine);

                    lastConnectedPiece = hitPiece;

                    connectedPieces.Add(hitPiece);
                    hitPiece.SetActive(false); // モノクロにする
                }
            }
        }
    }

    private LineRenderer CreateBlueLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer blueLine = lineObj.GetComponent<LineRenderer>();
        blueLine.SetPosition(0, startPosition);
        blueLine.SetPosition(1, endPosition);
        blueLine.startColor = Color.blue;
        blueLine.endColor = Color.blue;
        return blueLine;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // スプライトの組み合わせを検証
        List<Piece> validCombination = GetValidCombination();

        if (validCombination != null)
        {
            foreach (var piece in validCombination)
            {
                if (piece != null)
                {
                    piece.DestroyWithEffect();
                }
            }
        }
        else
        {
            foreach (var blueLine in blueLines)
            {
                Destroy(blueLine.gameObject);
            }
        }

        connectedPieces.Clear();
        blueLines.Clear();

        gridManager.ResetActiveArea();
    }

    private List<Piece> GetValidCombination()
    {
        var elementCounts = new Dictionary<Piece.ElementType, int>();
        foreach (var piece in connectedPieces)
        {
            if (piece != null)
            {
                if (!elementCounts.ContainsKey(piece.elementType))
                {
                    elementCounts[piece.elementType] = 0;
                }
                elementCounts[piece.elementType]++;
            }
        }

        foreach (var validCombination in validCombinations)
        {
            var validElementCounts = new Dictionary<Piece.ElementType, int>();
            foreach (var type in validCombination)
            {
                if (!validElementCounts.ContainsKey(type))
                {
                    validElementCounts[type] = 0;
                }
                validElementCounts[type]++;
            }

            if (elementCounts.Count == validElementCounts.Count)
            {
                bool match = true;
                foreach (var kvp in validElementCounts)
                {
                    if (!elementCounts.ContainsKey(kvp.Key) || elementCounts[kvp.Key] != kvp.Value)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return connectedPieces;
                }
            }
        }

        return null;
    }

    public void DestroyWithEffect()
    {
        if (sparkleEffectPrefab != null)
        {
            Instantiate(sparkleEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
