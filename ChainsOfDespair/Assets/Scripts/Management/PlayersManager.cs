using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance;

    [HideInInspector] public List<PlayerInitialize> players = new List<PlayerInitialize>();
    [HideInInspector] public PlayerInitialize ownerPlayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
}
