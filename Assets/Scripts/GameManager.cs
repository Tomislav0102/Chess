using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public CastlingManager casManager;
    public Button buttonCastling;

    FigManager _figManager;
    SmallFunctions _sFunctions;

    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            Cursor.visible = !_isPaused;
        }
    }
    bool _isPaused;
    [HideInInspector] public Tile figureChosen;
    public Transform carrier;
    public Sprite[] spr_Figure;
    public Tile[,] allTiles = new Tile[8, 8];
   // public TestPollje[,] testPolljes = new TestPollje[8, 8];
    HashSet<Vector2Int> _freeTileCoordinates = new HashSet<Vector2Int>();
    HashSet<Vector2Int> _targetTileCoordinates = new HashSet<Vector2Int>();
    [HideInInspector] public bool turnEndsWithEatingFigure;
    Vector2Int PosKing()
    {
        int ozz = Player == PlayerColor.White ? 15 : 25;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allTiles[i,j].Marking == ozz)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return Vector2Int.zero;
    }
    public HashSet<Vector2Int> AllTileCoordinates(TotalFigure tot)
    {
        ResetHighlight();
        HashSet<Vector2Int> hsTot = new HashSet<Vector2Int>();
        PlayerColor player = Player;
        if ((int)tot % 2 == 0) Player = PlayerColor.White;
        else Player = PlayerColor.Black;
        bool searchingTargets = (int)tot < 2;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Clicked(new Vector2Int(i, j)) == ClickedTarget.Friend) _figManager.DistributionOfHashSets(allTiles[i, j], searchingTargets, _freeTileCoordinates, _targetTileCoordinates);
            }
        }
        hsTot.UnionWith(searchingTargets ? _targetTileCoordinates : _freeTileCoordinates);
        _targetTileCoordinates.Clear();
        _freeTileCoordinates.Clear();
        Player = player;

        return hsTot;
    }
    readonly HashSet<int> _markWhite = new HashSet<int>();
    readonly HashSet<int> _markBlack = new HashSet<int>();
    public ClickedTarget Clicked(Vector2Int coor)
    {
        if (coor.x < 0 || coor.x > 7 || coor.y < 0 || coor.y > 7) return ClickedTarget.OutsideBoard;
        else if (allTiles[coor.x, coor.y].Marking == 0 || allTiles[coor.x, coor.y].Marking == 100 || allTiles[coor.x, coor.y].Marking == 200) return ClickedTarget.FreeTile;
        else if ((Player == PlayerColor.White && _markBlack.Contains(allTiles[coor.x, coor.y].Marking)) || 
            (Player == PlayerColor.Black && _markWhite.Contains(allTiles[coor.x, coor.y].Marking))) 
            return ClickedTarget.Foe;

        else if ((Player == PlayerColor.Black && _markBlack.Contains(allTiles[coor.x, coor.y].Marking) ||
            (Player == PlayerColor.White && _markWhite.Contains(allTiles[coor.x, coor.y].Marking)))) 
            return ClickedTarget.Friend;

        return ClickedTarget.OutsideBoard;
    }
    [SerializeField] TextMeshProUGUI displayCurrentPlayer;
    public PlayerColor Player
    {
        get
        {
            return _player;
        }
        set
        {
            _player = value;
            displayCurrentPlayer.text = _player.ToString() + " player's move";
        }
    }
    PlayerColor _player;
    [SerializeField] Transform gridTest;
    [SerializeField] Material backgroundMaterial;
    float _blendValue;

    private void Awake()
    {
        gm = this;
        casManager = new CastlingManager(this);
        _figManager = new FigManager(this);
        _sFunctions = GetComponent<SmallFunctions>();
    }
    IEnumerator Start()
    {
        backgroundMaterial.SetFloat("_Blend", -15f);
        for (int i = 0; i < 6; i++)
        {
            _markWhite.Add(i + 10);
            _markBlack.Add(i + 20);
        }
        yield return new WaitForSeconds(0.5f);
        BoardSetup();
    }
    private void OnEnable()
    {
        buttonCastling.onClick.AddListener(casManager.Button_Castling);
    }
    private void OnDisable()
    {
        buttonCastling.onClick.RemoveAllListeners();
    }
    private void Update()
    {
       // if (Input.GetKeyDown(KeyCode.K)) sFunctions.PromotionProvjera(polja[0, 1]);

        //for (int i = 0; i < 8; i++)
        //{
        //    for (int j = 0; j < 8; j++)
        //    {
        //      //  testPolljes[i, j].prikaz.text = polja[i, j].figura.ToString();
        //        testPolljes[i, j].prikaz.text = polja[i, j].Oznaka.ToString();
        //    }
        //}
    }
    public IEnumerator EndMove(Tile tile)
    {
        ResetHighlight();

        if (tile != null)
        {
            figureChosen.Tween_Move(tile);
            while (IsPaused)
            {
                yield return null;
            }
            if (turnEndsWithEatingFigure) _sFunctions.EatingFigures(tile.Marking);
            tile.Marking = figureChosen.Marking;
            figureChosen.Marking = 0;
            gm.figureChosen = null;
            IsPaused = true;
            Cursor.visible = true;
            _sFunctions.CheckForPromotion(Player, tile);
            while (IsPaused)
            {
                yield return null;
            }

        }
        IsPaused = true;

        //checking chess-mat
        if (AllTileCoordinates(Player == PlayerColor.White ? TotalFigure.TargetsOfBlack : TotalFigure.TargetsOfWhite).Contains(PosKing()))
        {
            UtilitiesCollection.Winner?.Invoke(Player == PlayerColor.White ? PlayerColor.Black : PlayerColor.White);
            yield break;
        }

        if (Player == PlayerColor.White)
        {
            Player = PlayerColor.Black;
            _blendValue = -15f;
            while (_blendValue < 15f)
            {
                _blendValue += Time.deltaTime * 30f;
                backgroundMaterial.SetFloat("_Blend", _blendValue);
                yield return null;
            }
        }
        else
        {
            Player = PlayerColor.White;
            _blendValue = 15f;
            while (_blendValue > -15f)
            {
                _blendValue -= Time.deltaTime * 30f;
                backgroundMaterial.SetFloat("_Blend", _blendValue);
                yield return null;
            }

        }
        //castling
        casManager.Castling_Space();
        //en passant cleanup
        for (int i = 0; i < 8; i++)
        {
            if (allTiles[i, 2].Marking == 100 && allTiles[i, 3].figura != Figure.Pawn) allTiles[i, 2].Marking = 0;
            if (allTiles[i, 5].Marking == 200 && allTiles[i, 4].figura != Figure.Pawn) allTiles[i, 5].Marking = 0;
        }
        IsPaused = false;
        figureChosen = null;
    }
    

    public void ChosenTile(Tile tile)
    {
        if (figureChosen == null)
        {
            if (Clicked(tile.coor) == ClickedTarget.Friend)
            {
                Processing(tile);
            }
        }
        else if (figureChosen != tile)
        {
            switch (Clicked(tile.coor))
            {
                case ClickedTarget.FreeTile:
                    if (_freeTileCoordinates.Contains(tile.coor))
                    {
                        Setup_EnPassant(tile, figureChosen);
                    }
                    else if (_targetTileCoordinates.Contains(tile.coor))
                    {
                        turnEndsWithEatingFigure = true;
                    }
                    StartCoroutine(EndMove(tile));
                    break;
                case ClickedTarget.Friend:
                    Processing(tile);
                    break;
                case ClickedTarget.Foe:
                    if (_targetTileCoordinates.Contains(tile.coor))
                    {
                        turnEndsWithEatingFigure = true;
                        StartCoroutine(EndMove(tile));
                    }
                    break;
            }
        }

    }

    void Processing(Tile tile)
    {
        _freeTileCoordinates.Clear();
        _targetTileCoordinates.Clear();
        ResetHighlight();
        figureChosen = tile;
        tile.HighlightTile(HighlghtTile.ChosenFigure);
        _figManager.DistributionOfHashSets(tile, true, _freeTileCoordinates, _targetTileCoordinates);
        foreach (Vector2Int item in _freeTileCoordinates)
        {
            allTiles[item.x, item.y].HighlightTile(HighlghtTile.FreeTile);
        }
        foreach (Vector2Int item in _targetTileCoordinates)
        {
            allTiles[item.x, item.y].HighlightTile(HighlghtTile.Attack);
        }
    }

    public void ResetHighlight()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                allTiles[i, j].HighlightTile(HighlghtTile.NONE);
            }
        }
    }
    void Setup_EnPassant(Tile targetTile, Tile startingTile)
    {
        if (startingTile.figura != Figure.Pawn) return;
        switch (Player)
        {
            case PlayerColor.White:
                if (startingTile.coor.y == 1 && targetTile.coor.y == 3) allTiles[targetTile.coor.x, 2].Marking = 100;
                break;
            case PlayerColor.Black:
                if (startingTile.coor.y == 6 && targetTile.coor.y == 4) allTiles[targetTile.coor.x, 5].Marking = 200;
                break;
        }
    }
    void BoardSetup()
    {
        //white
        Player = PlayerColor.White;
        for (int i = 0; i < 8; i++)
        {
            allTiles[i, 1].Marking = 10;
        }
        allTiles[0, 0].Marking = allTiles[7, 0].Marking = 11;
        allTiles[1, 0].Marking = allTiles[6, 0].Marking = 12;
        allTiles[2, 0].Marking = allTiles[5, 0].Marking = 13;
        allTiles[3, 0].Marking = 14;
        allTiles[4, 0].Marking = 15;


        //black
        Player = PlayerColor.Black;
        for (int i = 0; i < 8; i++)
        {
            allTiles[i, 6].Marking = 20;
        }
        allTiles[0, 7].Marking = allTiles[7, 7].Marking = 21;
        allTiles[1, 7].Marking = allTiles[6, 7].Marking = 22;
        allTiles[2, 7].Marking = allTiles[5, 7].Marking = 23;
        allTiles[3, 7].Marking = 24;
        allTiles[4, 7].Marking = 25;

        Player = PlayerColor.White;
    }
    public void G_TestSvaPolja(int a)
    {
        foreach (Vector2Int item in AllTileCoordinates((TotalFigure)a))
        {
            allTiles[item.x, item.y].HighlightTile(a < 2 ? HighlghtTile.PossibleAttack : HighlghtTile.PossibleFree);
        }
    }
}

