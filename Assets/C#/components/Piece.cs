using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
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

    private static List<List<ElementType>> validCombinations = new List<List<ElementType>>()
    {
        // メタン (CH₄)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon },

        // 一酸化炭素 (CO)
        new List<ElementType> { ElementType.Carbon, ElementType.OxygenIon },

        // 二酸化炭素 (CO₂)
        new List<ElementType> { ElementType.Carbon, ElementType.OxygenIon, ElementType.OxygenIon },

        // 水 (H₂O)
        new List<ElementType> { ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon },

        // メタノール (CH₃OH)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon },

        // ホルムアルデヒド (H₂CO)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon },

        // ギ酸 (HCOOH)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon },

        // 酢酸 (CH₃COOH)
        new List<ElementType> { ElementType.Carbon, ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon },

        // 炭酸 (H₂CO₃)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon, ElementType.OxygenIon },

        // 炭酸水素イオン (HCO₃⁻)
        new List<ElementType> { ElementType.Carbon, ElementType.HydrogenIon, ElementType.OxygenIon, ElementType.OxygenIon, ElementType.OxygenIon },

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

        // 赤い線の削除は不要なため、ここで処理なし
    }
    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            Piece hitPiece = hit.collider.GetComponent<Piece>();
            // モノクロではないスプライトにのみ接続する
            if (hitPiece != null && hitPiece.isActive && hitPiece != lastConnectedPiece)
            {
                // 青い線を生成してスプライト同士を接続
                LineRenderer blueLine = CreateLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                allLines.Add(blueLine);

                lastConnectedPiece = hitPiece;

                connectedPieces.Add(hitPiece);
                hitPiece.SetActive(false); // isActive プロパティを変更しモノクロにする
            }
        }
    }

    private LineRenderer CreateLine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
        lineRenderer.startColor = Color.blue; // 線の色を青に設定
        lineRenderer.endColor = Color.blue;
        return lineRenderer;
    }

    void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;

        // 消去のタイミングで接続の有効性を判定
        bool validCompound = CheckIfValidCompound();

        if (validCompound)
        {
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    piece.DestroyWithEffect();
                }
            }
        }
        else
        {
            foreach (var line in allLines)
            {
                if (line != null)
                {
                    Destroy(line.gameObject);
                }
            }
        }

        connectedPieces.Clear();
        allLines.Clear();

        gridManager.ResetActiveArea();
    }

    // 接続の有効性を判定
    private bool CheckIfValidCompound()
    {
        var elementCounts = new Dictionary<ElementType, int>();
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

        foreach (var combination in validCombinations)
        {
            var tempCounts = new Dictionary<ElementType, int>(elementCounts);

            bool isMatch = true;
            foreach (var element in combination)
            {
                if (tempCounts.ContainsKey(element))
                {
                    tempCounts[element]--;
                    if (tempCounts[element] == 0)
                    {
                        tempCounts.Remove(element);
                    }
                }
                else
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch && tempCounts.Count == 0)
            {
                return true;
            }
        }

        return false;
    }

        public void DestroyWithEffect()
    {
        // エフェクトを生成する
        if (sparkleEffectPrefab != null)
        {
            GameObject effect = Instantiate(sparkleEffectPrefab, transform.position, Quaternion.identity);
            
            // パーティクルシステムを取得し再生
            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            // 一定時間後にエフェクトオブジェクトを削除
            Destroy(effect, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
        }

        // 自身の削除
        Destroy(gameObject);

        // 自身に接続されたすべての青い線を削除
        List<LineRenderer> linesToRemove = new List<LineRenderer>();
        foreach (var line in allLines)
        {
            if (line != null && (line.GetPosition(0) == transform.position || line.GetPosition(1) == transform.position))
            {
                linesToRemove.Add(line);
            }
        }
        foreach (var line in linesToRemove)
        {
            allLines.Remove(line);
            Destroy(line.gameObject);
        }
    }


    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
