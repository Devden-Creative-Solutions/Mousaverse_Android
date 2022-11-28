using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    public int playerTeleportScene;

    public bool operaEntrance;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.parent.GetComponent<PhotonView>().IsMine)
            {
                other.GetComponent<AssignSeat>()._mainCamera.GetComponent<AudioListener>().enabled = false;
                if (other.GetComponent<AssignSeat>().currentPlayerType == PlayerType.Singer)
                {
                    OperaHouseID operaHouseID = (OperaHouseID)playerTeleportScene;

                    switch (operaHouseID)
                    {
                        case OperaHouseID.OperaHouse1:
                            if (SingerSeatStatusChecking.Instance.IsPrimaryOperaSingerSeatTaken())
                            {
                                print("Already there is a singer inside the opera house!!");
                                WarningMessage.Instance.ShowWarningMessage();
                            }
                            else
                            {
                                SceneChangeController.Instance.TeleportTo(playerTeleportScene, operaEntrance);
 
                            }
                            break;

                        case OperaHouseID.OperaHouse2:
                            if (SingerSeatStatusChecking.Instance.IsSecondaryOperaSingerSeatTaken())
                            {
                                print("Already there is a singer inside the opera house!!");
                                WarningMessage.Instance.ShowWarningMessage();
                            }
                            else
                            {
                                SceneChangeController.Instance.TeleportTo(playerTeleportScene, operaEntrance);
                              
                            }
                            break;
                    }
                }
                else
                {
                    SceneChangeController.Instance.TeleportTo(playerTeleportScene, operaEntrance);
                   // this.GetComponent<AudioSource>().Play(); //This is for testing need to delete it later
                }


            }
        }
    }

}
