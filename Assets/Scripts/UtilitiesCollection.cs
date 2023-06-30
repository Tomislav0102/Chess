using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class UtilitiesCollection
    {
        public static System.Action<PlayerColor> Winner;

        public static T[] GetAllChildren<T>(Transform parentTransform)
        {
            T[] tempArray = new T[parentTransform.childCount];
            for (int i = 0; i < parentTransform.childCount; i++)
            {
                tempArray[i] = parentTransform.GetChild(i).GetComponent<T>();
            }
            return tempArray;
        }
        public static void ChooseOneChild(GameObject[] children, bool isRandom, int index)
        {
            for (int i = 0; i < children.Length; i++)
            {
                children[i].SetActive(false);
            }
            if (isRandom) children[Random.Range(0, children.Length - 1)].SetActive(true);
            else children[index].SetActive(true);
        }

        public static List<int> RandomizeList(int size)
        {
            List<int> startingList = new List<int>();
            List<int> endingList = new List<int>();
            for (int i = 0; i < size; i++)
            {
                startingList.Add(i);
            }
            int num = startingList.Count;
            for (int i = 0; i < num; i++)
            {
                int rdn = Random.Range(0, startingList.Count);
                endingList.Add(startingList[rdn]);
                startingList.RemoveAt(rdn);
            }
            return endingList;
        }

    }

        public enum PlayerColor
        {
            White, 
            Black
        }
        public enum ClickedTarget
        {
            OutsideBoard,
            FreeTile,
            Foe,
            Friend
        }
        public enum Figure
        {
            NONE,
            Pawn,
            Rook,
            Knight,
            Bishop,
            Queen,
            King
        }
        public enum HighlghtTile
        {
            NONE,
            ChosenFigure,
            FreeTile,
            Attack,
            PossibleFree,
            PossibleAttack
        }
        public enum TotalFigure
        {
            TargetsOfWhite,
            TargetsOfBlack,
            PotentialTargetsOfWhite,
            PotentialTargetsOfBlack
        }
    /*
     0 -> prazno
    10-15 ->bijele
    100 ->bijeli en passant
    20-25 ->crne
    200 ->crni en passant
     */
