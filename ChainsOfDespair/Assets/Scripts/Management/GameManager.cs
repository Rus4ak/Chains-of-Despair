using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action OnLeave;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void Leave()
    {
        OnLeave?.Invoke();
    }
}
