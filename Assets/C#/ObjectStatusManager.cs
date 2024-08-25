using UnityEngine;

public class ObjectStatusManager : MonoBehaviour
{
    public enum ObjectStatus
    {
        Square1 = 1,
        Square2 = 2,
        Square3 = 3,
        Square4 = 4,
        Triangle1 = 5,
        Triangle2 = 6,
        Triangle3 = 7,
        Triangle4 = 8
    }

    public ObjectStatus status = ObjectStatus.Square1;
}
