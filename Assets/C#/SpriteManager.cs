using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(ObjectStatusManager))]
public class SpriteManager : MonoBehaviour
{
    private ObjectStatusManager statusManager;
    public SpriteRenderer mainSprite;

    public Sprite sq1;
    public Sprite sq2;
    public Sprite sq3;
    public Sprite sq4;
    public Sprite tr1;
    public Sprite tr2;
    public Sprite tr3;
    public Sprite tr4;

    void Start()
    {
        statusManager = GetComponent<ObjectStatusManager>();
        mainSprite = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        switch (statusManager.status)
        {
            case ObjectStatusManager.ObjectStatus.Square1:
                mainSprite.sprite = sq1;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                break;
            case ObjectStatusManager.ObjectStatus.Square2:
                mainSprite.sprite = sq2;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                break;
            case ObjectStatusManager.ObjectStatus.Square3:
                mainSprite.sprite = sq3;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                break;
            case ObjectStatusManager.ObjectStatus.Square4:
                mainSprite.sprite = sq4;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                break;
            case ObjectStatusManager.ObjectStatus.Triangle1:
                mainSprite.sprite = tr1;
                transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                break;
            case ObjectStatusManager.ObjectStatus.Triangle2:
                mainSprite.sprite = tr2;
                transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                break;
            case ObjectStatusManager.ObjectStatus.Triangle3:
                mainSprite.sprite = tr3;
                transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                break;
            case ObjectStatusManager.ObjectStatus.Triangle4:
                mainSprite.sprite = tr4;
                transform.localScale = new Vector3(0.13f, 0.13f, 0.13f);
                break;
            default:
                Debug.LogError("Invalid status!");
                break;
        }
    }

    public void CallSquare(ObjectStatusManager.ObjectStatus squareNumber)
    {
        statusManager.status = squareNumber;
        UpdateSprite();
    }

    public void CallTriangle(ObjectStatusManager.ObjectStatus triangleNumber)
    {
        statusManager.status = triangleNumber;
        UpdateSprite();
    }
}
