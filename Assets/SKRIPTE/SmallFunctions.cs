using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TK;
using TK.Enums;
using TMPro;
using UnityEngine.SceneManagement;

public class SmallFunctions : MonoBehaviour
{
    GameManager gm;
    [SerializeField] GameObject goWindow;
    [SerializeField] TextMeshProUGUI prikazKraj;
    [SerializeField] Image pojedeniPrefab;
    [SerializeField] Transform par_Zeludac;
    const float razmak = 60f;
    int RB_eBijeli, RB_eCrni;

    [SerializeField] GameObject promWindow;
    [SerializeField] TMP_Dropdown promDropDown;
    Polje promPolje;
    Igrac promIgrac;
    readonly int[] rasporedDD = { 10, 14, 11, 13, 12};
    bool gameOver;
    private void Awake()
    {
        gm = GameManager.gm;
    }
    private void Start()
    {
        promWindow.SetActive(false);
        goWindow.SetActive(false);

    }
    private void OnEnable()
    {
        Magic.Pobjednik += Metoda_Kraj;
    }
    private void OnDisable()
    {
        Magic.Pobjednik -= Metoda_Kraj;
    }
    void Metoda_Kraj(Igrac igg)
    {
        gameOver = true;
        promWindow.SetActive(false);
        goWindow.SetActive(true);
        prikazKraj.text = (igg == Igrac.White ? "White player" : "Black player") + " has won!";
        gm.Pauza = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameOver) return;
            goWindow.SetActive(!goWindow.activeInHierarchy);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            gm.ResetirajHL();
            gm.FigOdabrana = null;
        }

    }
    public void Jedenje(int _oznaka)
    {
        int ozz = _oznaka;
        if (ozz == 100) ozz = 10;
        else if (ozz == 200) ozz = 20;

        int smjer = 0;
        int sibIndex = 0;
        int RB = 0;
        switch (gm.igrac)
        {
            case Igrac.White:
                sibIndex = 0;
                smjer = 1;
                RB = RB_eBijeli;
                RB_eBijeli++;
                break;
            case Igrac.Black:
                sibIndex = 1;
                smjer = -1;
                RB = RB_eCrni;
                RB_eCrni++;
                break;
        }
        Image img = Instantiate(pojedeniPrefab, par_Zeludac.GetChild(sibIndex));
        img.rectTransform.localPosition = smjer * RB * razmak * Vector2.up;
        img.sprite = gm.spr_Figure[ozz];

        gm.zavrsavaJedenjem = false;
    }


    //promotion
    public void PromotionProvjera(Igrac igg, Polje _targetPolje)
    {
        promIgrac = igg;
        if (_targetPolje.figura == Figura.Pijun && ((_targetPolje.koor.y == 7 && promIgrac == Igrac.White) || (_targetPolje.koor.y == 0 && promIgrac == Igrac.Black)))
        {
            promPolje = _targetPolje;
            promWindow.SetActive(true);
        }
        else gm.Pauza = false;
    }
    public void G_Dropdown()
    {
        int dodIgrac = promIgrac == Igrac.White ? 0 : 10;
        promPolje.Oznaka = dodIgrac + rasporedDD[promDropDown.value];
    }
    public void G_OK()
    {
        promWindow.SetActive(false);
        gm.Pauza = false;
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
