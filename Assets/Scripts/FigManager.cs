using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FigManager 
{
    GameManager _gm;
    Vector2Int _bufferVector;
    readonly List<Vector2Int> _free = new List<Vector2Int>();
    readonly List<Vector2Int> _targets = new List<Vector2Int>();
    readonly List<Vector2Int> _buffers = new List<Vector2Int>();
    int Direction()
    {
        return (_gm.Player == PlayerColor.White) ? 1 : -1;
    }
    bool _helperSwitch;

    public FigManager(GameManager gameManager)
    {
        _gm = gameManager;
    }

    public void DistributionOfHashSets(Tile tile, bool includePawn, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        Figure fig = tile.figura;
        Vector2Int coordinate = tile.coor;
        _bufferVector = Vector2Int.zero;
        _free.Clear();
        _targets.Clear();
        _buffers.Clear();
        _helperSwitch = false;
        switch (fig)
        {
            case Figure.NONE:
                break;
            case Figure.Pawn:
                if(includePawn) F_Pawn(coordinate, freeCoor, targetCoor);
                break;
            case Figure.Rook:
                F_Rook(coordinate, freeCoor, targetCoor);
                break;
            case Figure.Knight:
                F_Knight(coordinate, freeCoor, targetCoor);
                break;
            case Figure.Bishop:
                F_Bishop(coordinate, freeCoor, targetCoor);
                break;
            case Figure.Queen:
                F_Rook(coordinate, freeCoor, targetCoor);
                F_Bishop(coordinate, freeCoor, targetCoor);
                break;
            case Figure.King:
                F_King(coordinate, freeCoor, targetCoor);
                break;
        }

    }
    void F_Pawn(Vector2Int coor, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        _bufferVector = coor + Direction() * Vector2Int.up;

        _free.Add(_bufferVector);
        if ((_gm.Player == PlayerColor.White && coor.y == 1 && _gm.allTiles[coor.x, 2].Marking == 0) || (_gm.Player == PlayerColor.Black && coor.y == 6 && _gm.allTiles[coor.x, 5].Marking == 0))//prvi potez (dva polja)
        {
            _free.Add(coor + Direction() * 2 * Vector2Int.up);
        }
        for (int i = 0; i < _free.Count; i++)
        {
            switch (_gm.Clicked(_free[i]))
            {
                case ClickedTarget.FreeTile:
                    freeCoor.Add(_free[i]);
                    break;
            }
        }
        _targets.Add(_bufferVector + Vector2Int.right);
        _targets.Add(_bufferVector - Vector2Int.right);
        for (int i = 0; i < _targets.Count; i++)
        {
            switch (_gm.Clicked(_targets[i]))
            {
                case ClickedTarget.FreeTile:
                    if (_gm.Player == PlayerColor.White && _gm.allTiles[_targets[i].x, _targets[i].y].Marking == 200 ||
                    _gm.Player == PlayerColor.Black && _gm.allTiles[_targets[i].x, _targets[i].y].Marking == 100) targetCoor.Add(_targets[i]);
                    break;

                case ClickedTarget.Foe:
                    targetCoor.Add(_targets[i]);
                    break;
            }
        }

    }
    void F_Rook(Vector2Int coor, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        for (int i = 0; i < 4; i++) //smjerovi
        {
            for (int j = 1; j < 8; j++)
            {
                switch (i)
                {
                    case 0:
                        _bufferVector = new Vector2Int(coor.x + j, coor.y);
                        break;
                    case 1:
                        _bufferVector = new Vector2Int(coor.x - j, coor.y);
                        break;
                    case 2:
                        _bufferVector = new Vector2Int(coor.x, coor.y + j);
                        break;
                    case 3:
                        _bufferVector = new Vector2Int(coor.x, coor.y - j);
                        break;
                }

                switch (_gm.Clicked(_bufferVector))
                {
                    case ClickedTarget.OutsideBoard:
                        _helperSwitch = true;
                        break;
                    case ClickedTarget.FreeTile:
                        freeCoor.Add(_bufferVector);
                        break;
                    case ClickedTarget.Friend:
                        _helperSwitch = true;
                        break;
                    case ClickedTarget.Foe:
                        targetCoor.Add(_bufferVector);
                        _helperSwitch = true;
                        break;
                }

                if (_helperSwitch)
                {
                    _helperSwitch = false;
                    break;
                }
            }
        }
    }
    void F_Knight(Vector2Int coor, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        _buffers.Add(new Vector2Int(coor.x - 1, coor.y + 2));
        _buffers.Add(new Vector2Int(coor.x + 1, coor.y + 2));
        _buffers.Add(new Vector2Int(coor.x - 1, coor.y - 2));
        _buffers.Add(new Vector2Int(coor.x + 1, coor.y - 2));
        _buffers.Add(new Vector2Int(coor.x + 2, coor.y + 1));
        _buffers.Add(new Vector2Int(coor.x + 2, coor.y - 1));
        _buffers.Add(new Vector2Int(coor.x - 2, coor.y + 1));
        _buffers.Add(new Vector2Int(coor.x - 2, coor.y - 1));
        for (int i = 0; i < 8; i++) //lZamjenska.Count is always 8
        {
            switch (_gm.Clicked(_buffers[i]))
            {
                case ClickedTarget.FreeTile:
                    freeCoor.Add(_buffers[i]);
                    break;
                case ClickedTarget.Foe:
                    targetCoor.Add(_buffers[i]);
                    break;
            }
        }
    }
    void F_Bishop(Vector2Int coor, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        for (int i = 0; i < 4; i++) //directions
        {
            for (int j = 1; j < 8; j++)
            {
                switch (i)
                {
                    case 0:
                        _bufferVector = new Vector2Int(coor.x + j, coor.y + j);
                        break;
                    case 1:
                        _bufferVector = new Vector2Int(coor.x - j, coor.y - j);
                        break;
                    case 2:
                        _bufferVector = new Vector2Int(coor.x + j, coor.y - j);
                        break;
                    case 3:
                        _bufferVector = new Vector2Int(coor.x - j, coor.y + j);
                        break;
                }

                switch (_gm.Clicked(_bufferVector))
                {
                    case ClickedTarget.OutsideBoard:
                        _helperSwitch = true;
                        break;
                    case ClickedTarget.FreeTile:
                        freeCoor.Add(_bufferVector);
                        break;
                    case ClickedTarget.Friend:
                        _helperSwitch = true;
                        break;
                    case ClickedTarget.Foe:
                        targetCoor.Add(_bufferVector);
                        _helperSwitch = true;
                        break;
                }

                if (_helperSwitch)
                {
                    _helperSwitch = false;
                    break;
                }
            }
        }

    }
    void F_King(Vector2Int coor, HashSet<Vector2Int> freeCoor, HashSet<Vector2Int> targetCoor)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                _buffers.Add(new Vector2Int(coor.x + i, coor.y + j));
            }
        }
        for (int i = 0; i < _buffers.Count; i++) 
        {
            switch (_gm.Clicked(_buffers[i]))
            {
                case ClickedTarget.FreeTile:
                    freeCoor.Add(_buffers[i]);
                    break;
                case ClickedTarget.Foe:
                    targetCoor.Add(_buffers[i]);
                    break;
            }
        }

    }

}
