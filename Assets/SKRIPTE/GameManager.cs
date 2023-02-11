using System.Collections;
using System.Collections.Generic;
using TK;
using TK.Enums;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    bool _pauza;
    public bool Pauza
    {
        get
        {
            return _pauza;
        }
        set
        {
            _pauza = value;
            Cursor.visible = !_pauza;
        }
    }
    [HideInInspector] public Polje FigOdabrana;
    public Transform nosacFigura;
    public Sprite[] spr_Figure;
    [HideInInspector] public CastlingManager casManager;
    FigManager figManager;
    SmallFunctions sFunctions;
    public Polje[,] polja = new Polje[8, 8];
    public TestPollje[,] testPolljes = new TestPollje[8, 8];
    HashSet<Vector2Int> slobodnaPolja = new HashSet<Vector2Int>();
    HashSet<Vector2Int> mete = new HashSet<Vector2Int>();
    [HideInInspector] public bool zavrsavaJedenjem;
    Vector2Int PozKralja()
    {
        int ozz = igrac == Igrac.White ? 15 : 25;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (polja[i,j].Oznaka == ozz)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        return Vector2Int.zero;
    }
    public HashSet<Vector2Int> SvaPolja(TotalFigure tot)
    {
        ResetirajHL();
        HashSet<Vector2Int> hsTot = new HashSet<Vector2Int>();
        Igrac igrTemp = igrac;
        if ((int)tot % 2 == 0) igrac = Igrac.White;
        else igrac = Igrac.Black;
        bool trazimMete = (int)tot < 2;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Klik(new Vector2Int(i, j)) == KlikMeta.Svoje) figManager.RaspodjelaHashsetova(polja[i, j], trazimMete, slobodnaPolja, mete);
            }
        }
        hsTot.UnionWith(trazimMete ? mete : slobodnaPolja);
        mete.Clear();
        slobodnaPolja.Clear();
        igrac = igrTemp;

        return hsTot;
    }
    readonly HashSet<int> ozBijeli = new HashSet<int>();
    readonly HashSet<int> ozCrni = new HashSet<int>();
    public KlikMeta Klik(Vector2Int vv)
    {
        if (vv.x < 0 || vv.x > 7 || vv.y < 0 || vv.y > 7) return KlikMeta.IzvanMreze;
        else if (polja[vv.x, vv.y].Oznaka == 0 || polja[vv.x, vv.y].Oznaka == 100 || polja[vv.x, vv.y].Oznaka == 200) return KlikMeta.PraznoPolje;
        else if ((igrac == Igrac.White && ozCrni.Contains(polja[vv.x, vv.y].Oznaka)) || 
            (igrac == Igrac.Black && ozBijeli.Contains(polja[vv.x, vv.y].Oznaka))) 
            return KlikMeta.Neprijatelj;

        else if ((igrac == Igrac.Black && ozCrni.Contains(polja[vv.x, vv.y].Oznaka) ||
            (igrac == Igrac.White && ozBijeli.Contains(polja[vv.x, vv.y].Oznaka)))) 
            return KlikMeta.Svoje;

        return KlikMeta.IzvanMreze;
    }
    [SerializeField] TextMeshProUGUI prikazIgraca;
    Igrac _igr;
    public Igrac igrac
    {
        get
        {
            return _igr;
        }
        set
        {
            _igr = value;
            prikazIgraca.text = _igr.ToString() + " player's move";
        }
    }
    [SerializeField] Transform par_Test;
    [SerializeField] Material bkgMat;
    float blendVar;

    private void Awake()
    {
        gm = this;
        casManager = GetComponent<CastlingManager>();
        figManager = GetComponent<FigManager>();
        sFunctions = GetComponent<SmallFunctions>();
    }
    IEnumerator Start()
    {
        bkgMat.SetFloat("_Blend", -15f);
        for (int i = 0; i < 6; i++)
        {
            ozBijeli.Add(i + 10);
            ozCrni.Add(i + 20);
        }
        yield return new WaitForSeconds(0.5f);
        PostavljanjePloce();
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
    public IEnumerator ZavrsetakPoteza(Polje plj)
    {
        ResetirajHL();

        if (plj != null)
        {
            FigOdabrana.Tv_Pomicanje(plj);
            while (Pauza)
            {
                yield return null;
            }
            if (zavrsavaJedenjem) sFunctions.Jedenje(plj.Oznaka);
            plj.Oznaka = FigOdabrana.Oznaka;
            FigOdabrana.Oznaka = 0;
            gm.FigOdabrana = null;
            Pauza = true;
            Cursor.visible = true;
            sFunctions.PromotionProvjera(igrac, plj);
            while (Pauza)
            {
                yield return null;
            }

        }
        Pauza = true;
        //provjera sah-mat
        if (SvaPolja(igrac == Igrac.White ? TotalFigure.MeteCrnih : TotalFigure.MeteBijelih).Contains(PozKralja()))
        {
            Magic.Pobjednik?.Invoke(igrac == Igrac.White ? Igrac.Black : Igrac.White);
            yield break;
        }

        if (igrac == Igrac.White)
        {
            igrac = Igrac.Black;
            blendVar = -15f;
            while (blendVar < 15f)
            {
                blendVar += Time.deltaTime * 30f;
                bkgMat.SetFloat("_Blend", blendVar);
                yield return null;
            }
        }
        else
        {
            igrac = Igrac.White;
            blendVar = 15f;
            while (blendVar > -15f)
            {
                blendVar -= Time.deltaTime * 30f;
                bkgMat.SetFloat("_Blend", blendVar);
                yield return null;
            }

        }
        //castling
        casManager.Castling_Prostor();
        //en passant ciscenje
        for (int i = 0; i < 8; i++)
        {
            if (polja[i, 2].Oznaka == 100 && polja[i, 3].figura != Figura.Pijun) polja[i, 2].Oznaka = 0;
            if (polja[i, 5].Oznaka == 200 && polja[i, 4].figura != Figura.Pijun) polja[i, 5].Oznaka = 0;
        }
        Pauza = false;
        gm.FigOdabrana = null;
    }
    

    public void OdabranoPolje(Polje _polje)
    {
        if (FigOdabrana == null)
        {
            if (Klik(_polje.koor) == KlikMeta.Svoje)
            {
                Obrada(_polje);
            }
        }
        else if (FigOdabrana != _polje)
        {
            switch (Klik(_polje.koor))
            {
                case KlikMeta.PraznoPolje:
                    if (slobodnaPolja.Contains(_polje.koor))
                    {
                        Postavljanje_EnPassant(_polje, FigOdabrana);
                    }
                    else if (mete.Contains(_polje.koor))
                    {
                        zavrsavaJedenjem = true;
                    }
                    StartCoroutine(ZavrsetakPoteza(_polje));
                    break;
                case KlikMeta.Svoje:
                    Obrada(_polje);
                    break;
                case KlikMeta.Neprijatelj:
                    if (mete.Contains(_polje.koor))
                    {
                        zavrsavaJedenjem = true;
                        StartCoroutine(ZavrsetakPoteza(_polje));
                    }
                    break;
            }
        }

    }

    void Obrada(Polje _polje)
    {
        slobodnaPolja.Clear();
        mete.Clear();
        ResetirajHL();
        FigOdabrana = _polje;
        _polje.HajlajtajMe(HL.Odabrana_Figura);
        figManager.RaspodjelaHashsetova(_polje, true, slobodnaPolja, mete);
        foreach (Vector2Int item in slobodnaPolja)
        {
            polja[item.x, item.y].HajlajtajMe(HL.Slobodno_Polje);
        }
        foreach (Vector2Int item in mete)
        {
            polja[item.x, item.y].HajlajtajMe(HL.Napad);
        }
    }

    public void ResetirajHL()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                polja[i, j].HajlajtajMe(HL.Nema);
            }
        }
    }
    void Postavljanje_EnPassant(Polje dolaznoPolje, Polje figOdabrana)
    {
        if (figOdabrana.figura != Figura.Pijun) return;
        switch (igrac)
        {
            case Igrac.White:
                if (figOdabrana.koor.y == 1 && dolaznoPolje.koor.y == 3) polja[dolaznoPolje.koor.x, 2].Oznaka = 100;
                break;
            case Igrac.Black:
                if (figOdabrana.koor.y == 6 && dolaznoPolje.koor.y == 4) polja[dolaznoPolje.koor.x, 5].Oznaka = 200;
                break;
        }
    }
    void PostavljanjePloce()
    {
        //bijele
        igrac = Igrac.White;
        for (int i = 0; i < 8; i++)
        {
            polja[i, 1].Oznaka = 10;
        }
        polja[0, 0].Oznaka = polja[7, 0].Oznaka = 11;
        polja[1, 0].Oznaka = polja[6, 0].Oznaka = 12;
        polja[2, 0].Oznaka = polja[5, 0].Oznaka = 13;
        polja[3, 0].Oznaka = 14;
        polja[4, 0].Oznaka = 15;


        //crne
        igrac = Igrac.Black;
        for (int i = 0; i < 8; i++)
        {
            polja[i, 6].Oznaka = 20;
        }
        polja[0, 7].Oznaka = polja[7, 7].Oznaka = 21;
        polja[1, 7].Oznaka = polja[6, 7].Oznaka = 22;
        polja[2, 7].Oznaka = polja[5, 7].Oznaka = 23;
        polja[3, 7].Oznaka = 24;
        polja[4, 7].Oznaka = 25;

        igrac = Igrac.White;
    }
    public void G_TestSvaPolja(int a)
    {
        foreach (Vector2Int item in SvaPolja((TotalFigure)a))
        {
            polja[item.x, item.y].HajlajtajMe(a < 2 ? HL.Pot_Napad : HL.Pot_Slobodno);
        }
    }
}

