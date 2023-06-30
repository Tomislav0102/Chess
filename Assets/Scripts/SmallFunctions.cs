using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SmallFunctions : MonoBehaviour
{
    GameManager _gm;
    [SerializeField] GameObject gameOverWindow;
    [SerializeField] TextMeshProUGUI displayGameOver;
    [SerializeField] Image prefabEatenFigure;
    [SerializeField] Transform contentEatenFigures;
    const float CONST_SPACING = 60f;
    int _indexWhite, _indexBlack;

    [SerializeField] GameObject promotionWindow;
    [SerializeField] TMP_Dropdown promotionDropDown;
    Tile _newTile;
    PlayerColor _newPlayer;
    readonly int[] _distributionDD = { 10, 14, 11, 13, 12};
    bool _gameOver;

    private void Awake()
    {
        _gm = GameManager.gm;
    }
    private void Start()
    {
        promotionWindow.SetActive(false);
        gameOverWindow.SetActive(false);

    }
    private void OnEnable()
    {
        UtilitiesCollection.Winner += GameEnd;
    }
    private void OnDisable()
    {
        UtilitiesCollection.Winner -= GameEnd;
    }
    void GameEnd(PlayerColor player)
    {
        _gameOver = true;
        promotionWindow.SetActive(false);
        gameOverWindow.SetActive(true);
        displayGameOver.text = (player == PlayerColor.White ? "White player" : "Black player") + " has won!";
        _gm.IsPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_gameOver) return;
            gameOverWindow.SetActive(!gameOverWindow.activeInHierarchy);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            _gm.ResetHighlight();
            _gm.figureChosen = null;
        }

    }
    public void EatingFigures(int marking)
    {
        int mark = marking;
        if (mark == 100) mark = 10;
        else if (mark == 200) mark = 20;

        int direction = 0;
        int siblingIndex = 0;
        int index = 0;
        switch (_gm.Player)
        {
            case PlayerColor.White:
                siblingIndex = 0;
                direction = 1;
                index = _indexWhite;
                _indexWhite++;
                break;
            case PlayerColor.Black:
                siblingIndex = 1;
                direction = -1;
                index = _indexBlack;
                _indexBlack++;
                break;
        }
        Image img = Instantiate(prefabEatenFigure, contentEatenFigures.GetChild(siblingIndex)); //no need for pooling. This happens only once per figure.
        img.rectTransform.localPosition = direction * index * CONST_SPACING * Vector2.up;
        img.sprite = _gm.spr_Figure[mark];

        _gm.turnEndsWithEatingFigure = false;
    }


    //promotion
    public void CheckForPromotion(PlayerColor player, Tile tagetTile)
    {
        _newPlayer = player;
        if (tagetTile.figura == Figure.Pawn && ((tagetTile.coor.y == 7 && _newPlayer == PlayerColor.White) || (tagetTile.coor.y == 0 && _newPlayer == PlayerColor.Black)))
        {
            _newTile = tagetTile;
            promotionWindow.SetActive(true);
        }
        else _gm.IsPaused = false;
    }
    public void G_Dropdown()
    {
        int player = _newPlayer == PlayerColor.White ? 0 : 10;
        _newTile.Marking = player + _distributionDD[promotionDropDown.value];
    }
    public void G_OK()
    {
        promotionWindow.SetActive(false);
        _gm.IsPaused = false;
    }
    public void G_Restart()
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }
    public void G_QuitGame()
    {
        Application.Quit();
    }
}
