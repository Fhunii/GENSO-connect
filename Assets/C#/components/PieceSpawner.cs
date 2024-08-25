using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    public GameObject piecePrefab;
    public Transform spawnPoint;

    public void SpawnHydrogenIon()
    {
        SpawnPiece(Piece.ElementType.HydrogenIon);
    }

    public void SpawnHydroxideIon()
    {
        SpawnPiece(Piece.ElementType.HydroxideIon);
    }

    public void SpawnCarbon()
    {
        SpawnPiece(Piece.ElementType.Carbon);
    }

    public void SpawnOxygen()
    {
        SpawnPiece(Piece.ElementType.Oxygen);
    }

    private void SpawnPiece(Piece.ElementType elementType)
    {
        GameObject newPieceObject = Instantiate(piecePrefab, spawnPoint.position, Quaternion.identity);
        Piece newPiece = newPieceObject.GetComponent<Piece>();
        if (newPiece != null)
        {
            newPiece.SetElementType(elementType);
        }
        else
        {
            Debug.LogError("Spawned object does not have a Piece component!");
        }
    }
}