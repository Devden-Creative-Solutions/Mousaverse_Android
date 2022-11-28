using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperaSeat : MonoBehaviour
{
    [SerializeField]
    private bool isTaken = false;

    public void TakeSeat()
    {
        isTaken = true;
    }

    public void ClearSeat()
    {
        isTaken = false;
    }

    /// <summary>
    /// If return true then the seat is taken and if false then seat is empty
    /// </summary>
    /// <returns>bool</returns>
    public bool GetSeatStatus()
    {
        if (isTaken)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
