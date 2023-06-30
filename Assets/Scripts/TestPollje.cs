using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestPollje : MonoBehaviour
{
    GameManager gm;
    public Vector2Int koor;
    public TextMeshProUGUI prikaz;

    private void Awake()
    {
        gm = GameManager.gm;
    }
    private void Start()
    {
        koor.x = transform.GetSiblingIndex() % 8;
        koor.y = transform.GetSiblingIndex() / 8;
      //  gm.testPolljes[koor.x, koor.y] = this;
    }

}
