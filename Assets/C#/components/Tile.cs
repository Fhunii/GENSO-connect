using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Piece currentPiece;

    public void SetPiece(Piece piece)
    {
        currentPiece = piece;
        piece.transform.position = transform.position;
    }
}
