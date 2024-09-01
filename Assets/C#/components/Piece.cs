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
    private static LineRenderer currentRedLine;
    private static List<LineRenderer> blueLines = new List<LineRenderer>();

    private GridManager gridManager;
    private bool isActive = true;

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
        if (!isActive) return;

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

        if (currentRedLine != null)
        {
            Destroy(currentRedLine.gameObject);
        }

        GameObject lineObj = Instantiate(linePrefab);
        currentRedLine = lineObj.GetComponent<LineRenderer>();
        currentRedLine.SetPosition(0, transform.position);
        currentRedLine.SetPosition(1, transform.position);
        currentRedLine.startColor = Color.red;
        currentRedLine.endColor = Color.red;
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (currentRedLine != null)
        {
            currentRedLine.SetPosition(1, mousePosition);
        }

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
        if (hit.collider != null)
        {
            Piece hitPiece = hit.collider.GetComponent<Piece>();
            // 接続可能かの判定は行わず、接続できるなら接続
            if (hitPiece != null && hitPiece.isActive && hitPiece != lastConnectedPiece)
            {
                LineRenderer blueLine = CreateBlueLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                blueLines.Add(blueLine);

                lastConnectedPiece = hitPiece;

                if (currentRedLine != null)
                {
                    currentRedLine.SetPosition(0, lastConnectedPiece.transform.position);
                }

                connectedPieces.Add(hitPiece);
                hitPiece.SetActive(false); // isActive プロパティを変更
            }
        }

        if (connectedPieces.Count > 1)
        {
            if (currentRedLine != null)
            {
                Destroy(currentRedLine.gameObject);
                currentRedLine = null;
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
            foreach (var blueLine in blueLines)
            {
                Destroy(blueLine.gameObject);
            }
        }

        connectedPieces.Clear();
        blueLines.Clear();

        if (currentRedLine != null)
        {
            Destroy(currentRedLine.gameObject);
            currentRedLine = null;
        }

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

        return (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 4 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 1 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 4) || 
               (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 2 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 1 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 1) || 
               (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 2 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 1 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 2) || 
               (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 4 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 2 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 2) || 
               (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 2 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 1 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 3) || 
               (elementCounts.ContainsKey(ElementType.HydrogenIon) && elementCounts[ElementType.HydrogenIon] == 1 &&
                elementCounts.ContainsKey(ElementType.Carbon) && elementCounts[ElementType.Carbon] == 1 &&
                elementCounts.ContainsKey(ElementType.OxygenIon) && elementCounts[ElementType.OxygenIon] == 3);
    }

    public void DestroyWithEffect()
    {
        Instantiate(sparkleEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
