using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TK.Enums;

public class FigManager : MonoBehaviour
{
    GameManager gm;
    Vector2Int vZamjenski;
    List<Vector2Int> lSlobodno = new List<Vector2Int>();
    List<Vector2Int> lmete = new List<Vector2Int>();
    List<Vector2Int> lZamjenska = new List<Vector2Int>();
    int Smjer()
    {
        return (gm.igrac == Igrac.White) ? 1 : -1;
    }
    bool pomocni;

    private void Awake()
    {
        gm = GameManager.gm;
    }

    public void RaspodjelaHashsetova(Polje pljlj, bool ukljuciPijuna, HashSet<Vector2Int> sl, HashSet<Vector2Int> me)
    {
        Figura fig = pljlj.figura;
        Vector2Int koor = pljlj.koor;
        vZamjenski = Vector2Int.zero;
        lSlobodno.Clear();
        lmete.Clear();
        lZamjenska.Clear();
        pomocni = false;
        switch (fig)
        {
            case Figura.BEZ:
                break;
            case Figura.Pijun:
                if(ukljuciPijuna) F_Pijun(koor, sl, me);
                break;
            case Figura.Top:
                F_Top(koor, sl, me);
                break;
            case Figura.Konj:
                F_Konj(koor, sl, me);
                break;
            case Figura.Lovac:
                F_Lovac(koor, sl, me);
                break;
            case Figura.Kraljica:
                F_Top(koor, sl, me);
                F_Lovac(koor, sl, me);
                break;
            case Figura.Kralj:
                F_Kralj(koor, sl, me);
                break;
        }

    }
    void F_Pijun(Vector2Int koor, HashSet<Vector2Int> _sl, HashSet<Vector2Int> _me)
    {
        vZamjenski = koor + Smjer() * Vector2Int.up;

        lSlobodno.Add(vZamjenski);
        if ((gm.igrac == Igrac.White && koor.y == 1 && gm.polja[koor.x, 2].Oznaka == 0) || (gm.igrac == Igrac.Black && koor.y == 6 && gm.polja[koor.x, 5].Oznaka == 0))//prvi potez (dva polja)
        {
            lSlobodno.Add(koor + Smjer() * 2 * Vector2Int.up);
        }
        for (int i = 0; i < lSlobodno.Count; i++)
        {
            switch (gm.Klik(lSlobodno[i]))
            {
                case KlikMeta.PraznoPolje:
                    _sl.Add(lSlobodno[i]);
                    break;
            }
        }
        lmete.Add(vZamjenski + Vector2Int.right);
        lmete.Add(vZamjenski - Vector2Int.right);
        for (int i = 0; i < lmete.Count; i++)
        {
            switch (gm.Klik(lmete[i]))
            {
                case KlikMeta.PraznoPolje:
                    if (gm.igrac == Igrac.White && gm.polja[lmete[i].x, lmete[i].y].Oznaka == 200 ||
                    gm.igrac == Igrac.Black && gm.polja[lmete[i].x, lmete[i].y].Oznaka == 100) _me.Add(lmete[i]);
                    break;

                case KlikMeta.Neprijatelj:
                    _me.Add(lmete[i]);
                    break;
            }
        }

    }
    void F_Top(Vector2Int koor, HashSet<Vector2Int> _sl, HashSet<Vector2Int> _me)
    {
        for (int i = 0; i < 4; i++) //smjerovi
        {
            for (int j = 1; j < 8; j++)
            {
                switch (i)
                {
                    case 0:
                        vZamjenski = new Vector2Int(koor.x + j, koor.y);
                        break;
                    case 1:
                        vZamjenski = new Vector2Int(koor.x - j, koor.y);
                        break;
                    case 2:
                        vZamjenski = new Vector2Int(koor.x, koor.y + j);
                        break;
                    case 3:
                        vZamjenski = new Vector2Int(koor.x, koor.y - j);
                        break;
                }

                switch (gm.Klik(vZamjenski))
                {
                    case KlikMeta.IzvanMreze:
                        pomocni = true;
                        break;
                    case KlikMeta.PraznoPolje:
                        _sl.Add(vZamjenski);
                        break;
                    case KlikMeta.Svoje:
                        pomocni = true;
                        break;
                    case KlikMeta.Neprijatelj:
                        _me.Add(vZamjenski);
                        pomocni = true;
                        break;
                }

                if (pomocni)
                {
                    pomocni = false;
                    break;
                }
            }
        }
    }
    void F_Konj(Vector2Int koor, HashSet<Vector2Int> _sl, HashSet<Vector2Int> _me)
    {
        lZamjenska.Add(new Vector2Int(koor.x - 1, koor.y + 2));
        lZamjenska.Add(new Vector2Int(koor.x + 1, koor.y + 2));
        lZamjenska.Add(new Vector2Int(koor.x - 1, koor.y - 2));
        lZamjenska.Add(new Vector2Int(koor.x + 1, koor.y - 2));
        lZamjenska.Add(new Vector2Int(koor.x + 2, koor.y + 1));
        lZamjenska.Add(new Vector2Int(koor.x + 2, koor.y - 1));
        lZamjenska.Add(new Vector2Int(koor.x - 2, koor.y + 1));
        lZamjenska.Add(new Vector2Int(koor.x - 2, koor.y - 1));
        for (int i = 0; i < 8; i++) //lZamjenska.Count je uvijek 8
        {
            switch (gm.Klik(lZamjenska[i]))
            {
                case KlikMeta.PraznoPolje:
                    _sl.Add(lZamjenska[i]);
                    break;
                case KlikMeta.Neprijatelj:
                    _me.Add(lZamjenska[i]);
                    break;
            }
        }
    }
    void F_Lovac(Vector2Int koor, HashSet<Vector2Int> _sl, HashSet<Vector2Int> _me)
    {
        for (int i = 0; i < 4; i++) //smjerovi
        {
            for (int j = 1; j < 8; j++)
            {
                switch (i)
                {
                    case 0:
                        vZamjenski = new Vector2Int(koor.x + j, koor.y + j);
                        break;
                    case 1:
                        vZamjenski = new Vector2Int(koor.x - j, koor.y - j);
                        break;
                    case 2:
                        vZamjenski = new Vector2Int(koor.x + j, koor.y - j);
                        break;
                    case 3:
                        vZamjenski = new Vector2Int(koor.x - j, koor.y + j);
                        break;
                }

                switch (gm.Klik(vZamjenski))
                {
                    case KlikMeta.IzvanMreze:
                        pomocni = true;
                        break;
                    case KlikMeta.PraznoPolje:
                        _sl.Add(vZamjenski);
                        break;
                    case KlikMeta.Svoje:
                        pomocni = true;
                        break;
                    case KlikMeta.Neprijatelj:
                        _me.Add(vZamjenski);
                        pomocni = true;
                        break;
                }

                if (pomocni)
                {
                    pomocni = false;
                    break;
                }
            }
        }

    }
    void F_Kralj(Vector2Int koor, HashSet<Vector2Int> _sl, HashSet<Vector2Int> _me)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                lZamjenska.Add(new Vector2Int(koor.x + i, koor.y + j));
            }
        }
        for (int i = 0; i < lZamjenska.Count; i++) 
        {
            switch (gm.Klik(lZamjenska[i]))
            {
                case KlikMeta.PraznoPolje:
                    _sl.Add(lZamjenska[i]);
                    break;
                case KlikMeta.Neprijatelj:
                    _me.Add(lZamjenska[i]);
                    break;
            }
        }

    }

}
