using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour
{
    public enum ElementType { HydrogenIon, HydroxideIon, Carbon, Oxygen }
    
    public ElementType elementType;

    public Sprite hydrogenIonSprite;
    public Sprite hydroxideIonSprite;
    public Sprite carbonSprite;
    public Sprite oxygenSprite;
    
    public Sprite hydrogenIonMonoSprite;
    public Sprite hydroxideIonMonoSprite;
    public Sprite carbonMonoSprite;
    public Sprite oxygenMonoSprite;

    private SpriteRenderer spriteRenderer;

    public GameObject sparkleEffectPrefab;
    public GameObject linePrefab;

    private static List<Piece> connectedPieces = new List<Piece>();
    private static Piece lastConnectedPiece;
    private static Piece firstClickedPiece; // 追加: 最初にクリックされたスプライト
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
                ElementType.HydroxideIon => hydroxideIonSprite,
                ElementType.Carbon => carbonSprite,
                ElementType.Oxygen => oxygenSprite,
                _ => spriteRenderer.sprite
            },
            false => elementType switch
            {
                ElementType.HydrogenIon => hydrogenIonMonoSprite,
                ElementType.HydroxideIon => hydroxideIonMonoSprite,
                ElementType.Carbon => carbonMonoSprite,
                ElementType.Oxygen => oxygenMonoSprite,
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
        firstClickedPiece = this; // 追加: 最初にクリックされたスプライトを記録

        Vector2Int gridPosition = GetGridPosition();
        if (gridManager != null)
        {
            gridManager.SetActiveArea(gridPosition);
        }

        // 赤い線の初期設定
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

        // 赤い線を更新
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
                // 青い線を描画
                LineRenderer blueLine = CreateBlueLine(lastConnectedPiece.transform.position, hitPiece.transform.position);
                blueLines.Add(blueLine);

                // "終点"を"二つ目"に移動
                lastConnectedPiece = hitPiece;

                // 赤い線の始点を"終点"の位置に変更
                if (currentRedLine != null)
                {
                    currentRedLine.SetPosition(0, lastConnectedPiece.transform.position);
                }

                connectedPieces.Add(hitPiece);
                hitPiece.isActive = false;
                hitPiece.UpdateSprite();
            }
        }

        // 二つ以上つないだときに赤い線を消す
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

        // 接続されたスプライトと青い線を削除
        if (connectedPieces.Count > 1)
        {
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    piece.DestroyWithEffect();
                }
            }
            connectedPieces.Clear();
        }

        // 赤い線を削除
        if (currentRedLine != null)
        {
            Destroy(currentRedLine.gameObject);
            currentRedLine = null;
        }

        // 青い線をすべて削除
        foreach (var blueLine in blueLines)
        {
            Destroy(blueLine.gameObject);
        }
        blueLines.Clear();

        gridManager.ResetActiveArea();
    }

    public bool CanConnect(Piece otherPiece)
    {
        if (!isActive || !otherPiece.isActive)
        {
            return false;
        }
        return (elementType == ElementType.HydrogenIon && otherPiece.elementType == ElementType.HydroxideIon) ||
               (elementType == ElementType.HydroxideIon && otherPiece.elementType == ElementType.HydrogenIon) ||
               (elementType == ElementType.Carbon && otherPiece.elementType == ElementType.Oxygen) ||
               (elementType == ElementType.Oxygen && otherPiece.elementType == ElementType.Carbon) ||
               (elementType == ElementType.HydroxideIon && otherPiece.elementType == ElementType.Oxygen) ||
               (elementType == ElementType.Oxygen && otherPiece.elementType == ElementType.HydroxideIon);
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
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break; // SpriteRenderer が存在しない場合は早期リターン

        Color startColor = sr.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        float duration = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            sr.color = Color.Lerp(startColor, endColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sr.color = endColor;
        Destroy(gameObject);
    }
}
