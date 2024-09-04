using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    // 消去されたスプライトの位置を保存するリスト
    private static List<Vector2> destroyedPositions = new List<Vector2>();

    // 化合物ごとのスコアを設定
    private static Dictionary<List<ElementType>, int> compoundScores = new Dictionary<List<ElementType>, int>()
    {
        // 各化合物のスコア設定（例）
        { new List<ElementType> { ElementType.HydrogenIon, ElementType.Carbon, ElementType.OxygenIon }, 10 },
        // 他の化合物とスコア設定
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
        else
        {
            // ゲーム開始時に消去されたスプライトの位置に新しいスプライトを配置
            StartCoroutine(SpawnNewPieces());
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
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    Debug.Log($"Destroying piece: {piece.name}");
                    piece.DestroyWithEffect();
                }
            }

            // スコアの追加
            if (matchedCompound != null && compoundScores.ContainsKey(matchedCompound))
            {
                int scoreToAdd = compoundScores[matchedCompound];
                scoreManager.AddScore(scoreToAdd);
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

            // すべてのスプライトを有効に戻す
            foreach (var piece in connectedPieces)
            {
                if (piece != null)
                {
                    piece.SetActive(true);
                }
            }
        }

        connectedPieces.Clear();
        allLines.Clear();

        gridManager.ResetActiveArea();
    }

    private bool CheckIfValidCompound(out List<ElementType> matchedCompound)
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

        foreach (var combination in compoundScores.Keys)
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
                matchedCompound = combination;
                return true;
            }
        }

        matchedCompound = null;
        return false;
    }

    public void DestroyWithEffect()
    {
        // 消去されたスプライトの位置を保存する
        destroyedPositions.Add(transform.position);

        // エフェクトを生成する
        if (sparkleEffectPrefab != null)
        {
            GameObject effect = Instantiate(sparkleEffectPrefab, transform.position, Quaternion.identity);
            ParticleSystem particleSystem = effect.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
                Destroy(effect, particleSystem.main.duration);
            }
        }
        else
        {
            Debug.LogWarning("Sparkle effect prefab is not assigned.");
        }

        // スプライトを消去する
        Debug.Log("Destroying piece: " + gameObject.name);
        Destroy(gameObject);
    }

    private IEnumerator SpawnNewPieces()
    {
        yield return new WaitForSeconds(1f); // 1秒待機

        foreach (var position in destroyedPositions)
        {
            // ランダムにスプライトを選択
            ElementType randomElement = (ElementType)Random.Range(0, 3);
            GameObject newPieceObj = Instantiate(gameObject, position, Quaternion.identity);
            Piece newPiece = newPieceObj.GetComponent<Piece>();
            newPiece.SetElementType(randomElement);
            newPiece.SetActive(true);
        }

        // リストをクリア
        destroyedPositions.Clear();
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
    }
}
