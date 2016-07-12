using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance = null;
    public vp_FPPlayerDamageHandler DamangeHandler;

    void Awake()
    {
        Instance = this;
        DamangeHandler = GetComponent<vp_FPPlayerDamageHandler>();
    }

}
