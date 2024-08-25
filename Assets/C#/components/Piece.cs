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
            if (hitPiece != null && hitPiece.isActive && hitPiece != lastConnectedPiece && CanConnect(hitPiece))
            {
                LineRenderer blueLine = CreateBlueLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                blueLines.Add(blueLine);

                lastConnectedPiece = hitPiece;

                if (currentRedLine != null)
                {
                    currentRedLine.SetPosition(0, lastConnectedPiece.transform.position);
                }

                connectedPieces.Add(hitPiece);
                hitPiece.isActive = false;
                hitPiece.UpdateSprite();
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

        bool isValidCompound = CheckIfValidCompound();

        if (connectedPieces.Count > 1)
        {
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    if (isValidCompound)
                    {
                        piece.DestroyWithEffect();
                    }
                    else
                    {
                        piece.SetActive(true); // スプライトを再表示
                    }
                }
            }
            connectedPieces.Clear();
        }

        if (currentRedLine != null)
        {
            Destroy(currentRedLine.gameObject);
            currentRedLine = null;
        }

        foreach (var blueLine in blueLines)
        {
            Destroy(blueLine.gameObject);
        }
        blueLines.Clear();

        gridManager.ResetActiveArea();
    }

    private bool CheckIfValidCompound()
    {
        Dictionary<ElementType, int> elementCounts = new Dictionary<ElementType, int>();

        foreach (Piece piece in connectedPieces)
        {
            if (!elementCounts.ContainsKey(piece.elementType))
            {
                elementCounts[piece.elementType] = 0;
            }
            elementCounts[piece.elementType]++;
        }

        return IsValidCompound(elementCounts);
    }

    private bool IsValidCompound(Dictionary<ElementType, int> elementCounts)
    {
        // 各化合物の検証
        if (elementCounts.TryGetValue(ElementType.Carbon, out int carbonCount) &&
            elementCounts.TryGetValue(ElementType.HydrogenIon, out int hydrogenCount) &&
            elementCounts.TryGetValue(ElementType.OxygenIon, out int oxygenCount))
        {
            // メタン (CH₄)
            if (carbonCount == 1 && hydrogenCount == 4 && oxygenCount == 0)
                return true;

            // 一酸化炭素 (CO)
            if (carbonCount == 1 && hydrogenCount == 0 && oxygenCount == 1)
                return true;

            // 二酸化炭素 (CO₂)
            if (carbonCount == 1 && hydrogenCount == 0 && oxygenCount == 2)
                return true;

            // 水 (H₂O)
            if (carbonCount == 0 && hydrogenCount == 2 && oxygenCount == 1)
                return true;

            // メタノール (CH₃OH)
            if (carbonCount == 1 && hydrogenCount == 4 && oxygenCount == 1)
                return true;

            // ホルムアルデヒド (H₂CO)
            if (carbonCount == 1 && hydrogenCount == 2 && oxygenCount == 1)
                return true;

            // ギ酸 (HCOOH)
            if (carbonCount == 1 && hydrogenCount == 2 && oxygenCount == 2)
                return true;

            // 酢酸 (CH₃COOH)
            if (carbonCount == 2 && hydrogenCount == 4 && oxygenCount == 2)
                return true;

            // 炭酸 (H₂CO₃)
            if (carbonCount == 1 && hydrogenCount == 2 && oxygenCount == 3)
                return true;

            // 炭酸水素イオン (HCO₃⁻)
            if (carbonCount == 1 && hydrogenCount == 1 && oxygenCount == 3)
                return true;
        }

        return false;
    }

    public Vector2Int GetGridPosition()
    {
        string[] nameParts = gameObject.name.Split(' ');
        int x = int.Parse(nameParts[1]);
        int y = int.Parse(nameParts[2]);
        return new Vector2Int(x, y);
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateSprite();
    }

    public void DestroyWithEffect()
    {
        if (sparkleEffectPrefab != null)
        {
            Vector3 effectPosition = transform.position;
            effectPosition.z = -0.2f;
            GameObject effect = Instantiate(sparkleEffectPrefab, effectPosition, Quaternion.identity);

            var particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            StartCoroutine(DestroyEffectAfterPlay(effect, particleSystem));
        }
        StartCoroutine(DestroyAnimation());
    }

    private IEnumerator DestroyEffectAfterPlay(GameObject effect, ParticleSystem particleSystem)
    {
        yield return new WaitUntil(() => !particleSystem.isPlaying);
        Destroy(effect);
    }

    private IEnumerator DestroyAnimation()
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = endScale;
        Destroy(gameObject);
    }

    public bool CanConnect(Piece otherPiece)
    {
        return Vector3.Distance(transform.position, otherPiece.transform.position) <= 1.5f;
    }
}
