using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlingManager 
{
    GameManager _gm;
    bool _castlWhite, _castlBlack;

    public CastlingManager(GameManager gameManager)
    {
        _gm = gameManager;
        _gm.buttonCastling.interactable = false;
    }

    public void Castling_Space()
    {
        bool canCastling = false;
        switch (_gm.Player)
        {
            case PlayerColor.White:
                if (!_castlWhite && _gm.allTiles[4, 0].figura == Figure.King && _gm.allTiles[7, 0].figura == Figure.Rook)
                {
                    canCastling = true;
                    for (int i = 5; i < 7; i++)
                    {
                        if (_gm.allTiles[i, 0].Marking != 0) canCastling = false;
                    }
                    if (canCastling &&
                        (_gm.AllTileCoordinates(TotalFigure.TargetsOfBlack).Contains(new Vector2Int(4, 0)) || 
                        _gm.AllTileCoordinates(TotalFigure.PotentialTargetsOfBlack).Contains(new Vector2Int(5, 0)) || 
                        _gm.AllTileCoordinates(TotalFigure.PotentialTargetsOfBlack).Contains(new Vector2Int(6, 0)))) canCastling = false; 
                }
                break;
            case PlayerColor.Black:
                if (!_castlBlack && _gm.allTiles[4, 7].figura == Figure.King && _gm.allTiles[0, 7].figura == Figure.Rook)
                {
                    canCastling = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (_gm.allTiles[i, 7].Marking != 0)
                        {
                            canCastling = false;
                        }
                    }
                    if (canCastling &&
                        (_gm.AllTileCoordinates(TotalFigure.TargetsOfWhite).Contains(new Vector2Int(4, 7)) || 
                        _gm.AllTileCoordinates(TotalFigure.PotentialTargetsOfWhite).Contains(new Vector2Int(3, 7)) || 
                        _gm.AllTileCoordinates(TotalFigure.PotentialTargetsOfWhite).Contains(new Vector2Int(2, 7)))) canCastling = false; 

                }
                break;
        }

        _gm.buttonCastling.interactable = canCastling;

    }

    public void Castling_FirstMove(Tile tile)
    {
        switch (_gm.Player)
        {
            case PlayerColor.White:
                if (!_castlWhite && (tile.coor == new Vector2Int(4,0) || tile.coor == new Vector2Int(7, 0)))
                {
                    _castlWhite = true;
                }
                break;
            case PlayerColor.Black:
                if (!_castlBlack && (tile.coor == new Vector2Int(0, 7) || tile.coor == new Vector2Int(4, 7)))
                {
                    _castlBlack = true;
                }
                break;
        }

    }

    public void Button_Castling()
    {
        switch (_gm.Player)
        {
            case PlayerColor.White:
                if (!_castlWhite)
                {
                    _gm.allTiles[4, 0].Tween_Castling();
                    _castlWhite = true;
                }
                break;
            case PlayerColor.Black:
                if (!_castlBlack)
                {
                    _gm.allTiles[4, 7].Tween_Castling();
                    _castlBlack = true;
                }
                break;
        }
    }


}
