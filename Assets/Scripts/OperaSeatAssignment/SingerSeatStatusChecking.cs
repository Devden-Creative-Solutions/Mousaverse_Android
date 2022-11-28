using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingerSeatStatusChecking : MonoBehaviour
{
    #region singleton

    private static SingerSeatStatusChecking _instance;

    public static SingerSeatStatusChecking Instance { get { return _instance; } }


    private void Awake()
    {

        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    [SerializeField] SingerSeat Opera1SingerSeat;
    [SerializeField] SingerSeat Opera2SingerSeat;

    public bool IsPrimaryOperaSingerSeatTaken()
    {
        return Opera1SingerSeat.GetSeatStatus();
    }

    public bool IsSecondaryOperaSingerSeatTaken()
    {
        return Opera2SingerSeat.GetSeatStatus();
    }
}
