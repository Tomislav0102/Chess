using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TK.Enums;

public class CastlingManager : MonoBehaviour
{
    GameManager gm;
    public Button gumbCastling;
    bool castlBijeli, castlCrni;


    private void Awake()
    {
        gm = GameManager.gm;
    }
    private void Start()
    {
        gumbCastling.interactable = false;

    }

    public void Castling_Prostor()
    {
        bool mozeCastling = false;
        switch (gm.igrac)
        {
            case Igrac.White:
                if (!castlBijeli && gm.polja[4, 0].figura == Figura.Kralj && gm.polja[7, 0].figura == Figura.Top)
                {
                    mozeCastling = true;
                    for (int i = 5; i < 7; i++)
                    {
                        if (gm.polja[i, 0].Oznaka != 0) mozeCastling = false;
                    }
                    if (mozeCastling &&
                        (gm.SvaPolja(TotalFigure.MeteCrnih).Contains(new Vector2Int(4, 0)) || 
                        gm.SvaPolja(TotalFigure.PotMeteCrnih).Contains(new Vector2Int(5, 0)) || 
                        gm.SvaPolja(TotalFigure.PotMeteCrnih).Contains(new Vector2Int(6, 0)))) mozeCastling = false; 
                }
                break;
            case Igrac.Black:
                if (!castlCrni && gm.polja[4, 7].figura == Figura.Kralj && gm.polja[0, 7].figura == Figura.Top)
                {
                    mozeCastling = true;
                    for (int i = 1; i < 4; i++)
                    {
                        if (gm.polja[i, 7].Oznaka != 0)
                        {
                            mozeCastling = false;
                        }
                    }
                    if (mozeCastling &&
                        (gm.SvaPolja(TotalFigure.MeteBijelih).Contains(new Vector2Int(4, 7)) || 
                        gm.SvaPolja(TotalFigure.PotMeteBijelih).Contains(new Vector2Int(3, 7)) || 
                        gm.SvaPolja(TotalFigure.PotMeteBijelih).Contains(new Vector2Int(2, 7)))) mozeCastling = false; 

                }
                break;
        }

        gumbCastling.interactable = mozeCastling;

    }

    public void Castling_PrviPotez(Polje figOdabrana)
    {
        switch (gm.igrac)
        {
            case Igrac.White:
                if (!castlBijeli && (figOdabrana.koor == new Vector2Int(4,0) || figOdabrana.koor == new Vector2Int(7, 0)))
                {
                    castlBijeli = true;
                }
                break;
            case Igrac.Black:
                if (!castlCrni && (figOdabrana.koor == new Vector2Int(0, 7) || figOdabrana.koor == new Vector2Int(4, 7)))
                {
                    castlCrni = true;
                }
                break;
        }

    }

    public void G_Castling()
    {
        switch (gm.igrac)
        {
            case Igrac.White:
                if (!castlBijeli)
                {
                    gm.polja[4, 0].Tv_Castling();
                    castlBijeli = true;
                }
                break;
            case Igrac.Black:
                if (!castlCrni)
                {
                    gm.polja[4, 7].Tv_Castling();
                    castlCrni = true;
                }
                break;
        }
    }


}
