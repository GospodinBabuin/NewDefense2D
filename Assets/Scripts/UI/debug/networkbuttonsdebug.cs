using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class networkbuttonsdebug : MonoBehaviour
{

    public void StartHostbutton()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void StartClientbutton()
    {
        NetworkManager.Singleton.StartClient();
    }
}
