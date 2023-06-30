using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    GameManager gm;
    public Vector2Int coor;
    public int Marking
    {
        get
        {
            return _marking;
        }
        set
        {
            _marking = value;
            if (_marking > 25) return; //en passant
            imgFigure.sprite = gm.spr_Figure[_marking];
            int broj = (gm.Player == PlayerColor.White) ? 9 : 19;
            if (_marking >= 10) figura = (Figure)_marking - broj;
            else figura = Figure.NONE;
        }
    }
    int _marking;

    [SerializeField] Image imgTileColor, imgSelectFigure, hl, imgFigure;
    [HideInInspector] public Transform figureTransform;
    Transform _myTransform;
    [SerializeField] Color[] tileColors;
    HighlghtTile _currentHL;
    public Figure figura;

    const float CONST_TWEENSPEED = 800f;
    const Ease CONST_TWEENEASE = Ease.Linear;

    private void Awake()
    {
        gm = GameManager.gm;
        _myTransform = transform;
    }
    private void Start()
    {
        coor.x = transform.GetSiblingIndex() % 8;
        coor.y = transform.GetSiblingIndex() / 8;
        if ((coor.x + coor.y) % 2 == 0) imgTileColor.color = tileColors[0];
        else imgTileColor.color = tileColors[1];
        gm.allTiles[coor.x, coor.y] = this;
        figureTransform = imgFigure.transform;
    }

    public void Tween_Move(Tile tile)
    {
        gm.IsPaused = true;
        gm.casManager.Castling_FirstMove(this);

        figureTransform.SetParent(gm.carrier);
        figureTransform.DOMove(tile.transform.position, CONST_TWEENSPEED)
            .SetSpeedBased(true)
            .SetEase(CONST_TWEENEASE)
            .OnComplete(End_Move);
    }
    void End_Move()
    {
        gm.IsPaused = false;
        figureTransform.SetParent(_myTransform);
        figureTransform.localPosition = Vector3.zero;
    }
    public void Tween_Castling()
    {
        gm.IsPaused = true;

        Tile targetPolje = gm.Player == PlayerColor.White ? gm.allTiles[6, 0] : gm.allTiles[2, 7];
        figureTransform.SetParent(gm.carrier);
        figureTransform.DOMove(targetPolje.transform.position, CONST_TWEENSPEED)
            .SetSpeedBased(true)
            .SetEase(CONST_TWEENEASE)
            .OnComplete(() => End_Castling_King(targetPolje));

    }
    void End_Castling_King(Tile targetTile)
    {
        figureTransform.SetParent(_myTransform);
        figureTransform.localPosition = Vector3.zero;
        targetTile.Marking = gm.Player == PlayerColor.White ? 15 : 25;
        Marking = 0;

        Tile rook = gm.Player == PlayerColor.White ? gm.allTiles[7, 0] : gm.allTiles[0, 7];
        Vector3 endPOz = gm.Player == PlayerColor.White ? gm.allTiles[5, 0].transform.position : gm.allTiles[3, 7].transform.position;
        rook.figureTransform.SetParent(gm.carrier);
        rook.figureTransform.DOMove(endPOz, CONST_TWEENSPEED)
            .SetSpeedBased(true)
            .SetEase(CONST_TWEENEASE)
            .OnComplete(End_Castling_Rook);

    }
    void End_Castling_Rook()
    {
        switch (gm.Player)
        {
            case PlayerColor.White:
                gm.allTiles[4, 0].Marking = 0;
                gm.allTiles[7, 0].Marking = 0;
                gm.allTiles[5, 0].Marking = 11;
                gm.allTiles[6, 0].Marking = 15;
                gm.allTiles[7, 0].figureTransform.SetParent(gm.allTiles[7, 0].transform);
                break;
            case PlayerColor.Black:
                gm.allTiles[0, 7].Marking = 0;
                gm.allTiles[4, 7].Marking = 0;
                gm.allTiles[3, 7].Marking = 21;
                gm.allTiles[2, 7].Marking = 25;
                gm.allTiles[0, 7].figureTransform.SetParent(gm.allTiles[0, 7].transform);
                break;
        }
        gm.buttonCastling.interactable = false;
        StartCoroutine(gm.EndMove(null));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gm.IsPaused) return;
        gm.ChosenTile(this);
    }


    public void HighlightTile(HighlghtTile highlight)
    {
        _currentHL = highlight;
        imgSelectFigure.sprite = gm.spr_Figure[0];
        switch (_currentHL)
        {
            case HighlghtTile.NONE:
                hl.color = Color.clear;
                break;
            case HighlghtTile.ChosenFigure:
                imgSelectFigure.sprite = gm.spr_Figure[2];
                break;
            case HighlghtTile.FreeTile:
                hl.color = Color.green;
                break;
            case HighlghtTile.Attack:
                hl.color = Color.red;
                break;
            case HighlghtTile.PossibleFree:
                hl.color = Color.cyan;
                break;
            case HighlghtTile.PossibleAttack:
                hl.color = Color.magenta;
                break;
        }
    }
}
