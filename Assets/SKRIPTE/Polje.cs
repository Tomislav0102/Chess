using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TK;
using TK.Enums;
using DG.Tweening;

public class Polje : MonoBehaviour, IPointerClickHandler
{
    GameManager gm;
    public Vector2Int koor;
    int _oznaka;
    public int Oznaka
    {
        get
        {
            return _oznaka;
        }
        set
        {
            _oznaka = value;
            if (_oznaka > 25) return; //zbog en passant
            lik.sprite = gm.spr_Figure[_oznaka];
            int broj = (gm.igrac == Igrac.White) ? 9 : 19;
            if (_oznaka >= 10) figura = (Figura)_oznaka - broj;
            else figura = Figura.BEZ;
        }
    }
    [SerializeField] Image imgBojaPolja, imgSelectFigure, hl, lik;
    [HideInInspector] public Transform likTR;
    Transform myTransform;
    [SerializeField] Color[] bojice;
    HL hlTrenutni;
    public Figura figura;

    const float tv_Speed = 800f;
    const Ease izy = Ease.Linear;

    private void Awake()
    {
        gm = GameManager.gm;
        myTransform = transform;
    }
    private void Start()
    {
        koor.x = transform.GetSiblingIndex() % 8;
        koor.y = transform.GetSiblingIndex() / 8;
        if ((koor.x + koor.y) % 2 == 0) imgBojaPolja.color = bojice[0];
        else imgBojaPolja.color = bojice[1];
        gm.polja[koor.x, koor.y] = this;
        likTR = lik.transform;
    }

    public void Tv_Pomicanje(Polje _polje)
    {
        gm.Pauza = true;
        gm.casManager.Castling_PrviPotez(this);

        likTR.SetParent(gm.nosacFigura);
        likTR.DOMove(_polje.transform.position, tv_Speed)
            .SetSpeedBased(true)
            .SetEase(izy)
            .OnComplete(End_Pomicanje);
    }
    void End_Pomicanje()
    {
        gm.Pauza = false;
        likTR.SetParent(myTransform);
        likTR.localPosition = Vector3.zero;
    }
    public void Tv_Castling()
    {
        gm.Pauza = true;

        Polje targetPolje = gm.igrac == Igrac.White ? gm.polja[6, 0] : gm.polja[2, 7];
        likTR.SetParent(gm.nosacFigura);
        likTR.DOMove(targetPolje.transform.position, tv_Speed)
            .SetSpeedBased(true)
            .SetEase(izy)
            .OnComplete(() => End_Castling_Kralj(targetPolje));

    }
    void End_Castling_Kralj(Polje targetPolje)
    {
        likTR.SetParent(myTransform);
        likTR.localPosition = Vector3.zero;
        targetPolje.Oznaka = gm.igrac == Igrac.White ? 15 : 25;
        Oznaka = 0;

        Polje top = gm.igrac == Igrac.White ? gm.polja[7, 0] : gm.polja[0, 7];
        Vector3 endPOz = gm.igrac == Igrac.White ? gm.polja[5, 0].transform.position : gm.polja[3, 7].transform.position;
        top.likTR.SetParent(gm.nosacFigura);
        top.likTR.DOMove(endPOz, tv_Speed)
            .SetSpeedBased(true)
            .SetEase(izy)
            .OnComplete(End_Castling_Top);

    }
    void End_Castling_Top()
    {
        switch (gm.igrac)
        {
            case Igrac.White:
                gm.polja[4, 0].Oznaka = 0;
                gm.polja[7, 0].Oznaka = 0;
                gm.polja[5, 0].Oznaka = 11;
                gm.polja[6, 0].Oznaka = 15;
                gm.polja[7, 0].likTR.SetParent(gm.polja[7, 0].transform);
                break;
            case Igrac.Black:
                gm.polja[0, 7].Oznaka = 0;
                gm.polja[4, 7].Oznaka = 0;
                gm.polja[3, 7].Oznaka = 21;
                gm.polja[2, 7].Oznaka = 25;
                gm.polja[0, 7].likTR.SetParent(gm.polja[0, 7].transform);
                break;
        }
        gm.casManager.gumbCastling.interactable = false;
        StartCoroutine(gm.ZavrsetakPoteza(null));
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (gm.Pauza) return;
        gm.OdabranoPolje(this);
    }


    public void HajlajtajMe(HL hajlajt)
    {
        hlTrenutni = hajlajt;
        imgSelectFigure.sprite = gm.spr_Figure[0];
        switch (hlTrenutni)
        {
            case HL.Nema:
                hl.color = Color.clear;
                break;
            case HL.Odabrana_Figura:
                imgSelectFigure.sprite = gm.spr_Figure[2];
                break;
            case HL.Slobodno_Polje:
                hl.color = Color.green;
                break;
            case HL.Napad:
                hl.color = Color.red;
                break;
            case HL.Pot_Slobodno:
                hl.color = Color.cyan;
                break;
            case HL.Pot_Napad:
                hl.color = Color.magenta;
                break;
        }
    }
}
