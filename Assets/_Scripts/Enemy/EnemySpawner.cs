using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public PlayerEvent PlayerIn;
    public PlayerEvent playerOut;

    private void Awake()
    {
        PlayerIn = new PlayerEvent();
        playerOut = new PlayerEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Consts.PlayerTag) && PlayerIn != null)
        {
            PlayerIn.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Consts.PlayerTag) && playerOut != null)
        {
            playerOut.Invoke();
        }
    }


    public class PlayerEvent : UnityEvent { }
}
