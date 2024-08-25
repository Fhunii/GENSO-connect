using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(ObjectStatusManager))]
[RequireComponent(typeof(SpriteManager))]
public class DragAndDropManager : MonoBehaviour
{
    private ObjectStatusManager statusManager;
    public Sprite mainSprite;
    private SpriteManager spriteManager;
    private Vector3 screenPoint;
    
    private Vector3 offset;

    public ObjectStatusManager.ObjectStatus status { get; set; }

    void Start()
    {
        statusManager = GetComponent<ObjectStatusManager>();
        spriteManager = GetComponent<SpriteManager>();
    }

    void OnMouseDown()
    {
        StartCoroutine(Hello());
    }

    void OnMouseUp()
    {
        StartCoroutine(Sup());
    }

    public IEnumerator Hello()
    {
        screenPoint = Camera.main.WorldToScreenPoint(transform.position);
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        if (spriteManager.mainSprite.sprite == spriteManager.sq1)
        {
            UnityEngine.Debug.Log("Hello");
            spriteManager.CallSquare(ObjectStatusManager.ObjectStatus.Square2);
        }

        yield return new WaitForSeconds(0.01f);
    }

    public IEnumerator Sup()
    {
        if (spriteManager.mainSprite.sprite == spriteManager.sq2)
        {
            spriteManager.CallSquare(ObjectStatusManager.ObjectStatus.Square1);
        }

        yield return new WaitForSeconds(0.01f);
    }

    void OnMouseDrag()
    {
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
    
        Collider2D[] colliders = Physics2D.OverlapPointAll(currentPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                UniqueClassname otherObject = collider.gameObject.GetComponent<UniqueClassname>();
                if (otherObject != null)
                {
                    ObjectStatusManager.ObjectStatus otherStatus = otherObject.status;
                    if (otherStatus == ObjectStatusManager.ObjectStatus.Triangle1)
                    {
                        spriteManager.CallSquare(ObjectStatusManager.ObjectStatus.Square4);
                        UnityEngine.Debug.Log("Triangle1");
                    }
                    else
                    {
                        spriteManager.CallSquare(ObjectStatusManager.ObjectStatus.Square3);
                    }
                }
    
                break;
            }
        }
    }
}

// MonoBehaviourを継承するように修正
public class UniqueClassname : MonoBehaviour
{
    public ObjectStatusManager.ObjectStatus status { get; set; }
}
