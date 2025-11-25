using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : NetworkBehaviour
{
    [SerializeField] private Transform _pauseMenu;
    [SerializeField] private Transform _pauseTarget;
    [SerializeField] private float _animationTime;
    [SerializeField] private PlayerInitialize _playerInitialize;
    [SerializeField] private PlayerDied _playerDied;

    private bool _isActive;

    private void Update()
    {
        if (!IsOwner)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && !MenuTransition.Instance.isAnimate)
        {
            if (_isActive)
                DisablePauseMenu();
            else
                ActivePauseMenu();
        }
    }

    private void ActivePauseMenu()
    {
        StartCoroutine(MenuTransition.Instance.AnimationRoutine(_pauseMenu, _pauseTarget, _animationTime, 0, 0));
        Cursor.lockState = CursorLockMode.None;
        _playerInitialize.isMove = false;
        _isActive = true;
    }

    public void DisablePauseMenu()
    {
        StartCoroutine(MenuTransition.Instance.AnimationRoutine(_pauseTarget, _pauseMenu, _animationTime, 0, 0));
        Cursor.lockState = CursorLockMode.Locked;
        _playerInitialize.isMove = true;
        _isActive = false;
    }

    public void Leave()
    {
        if (IsServer)
        {
            LoadMainMenuClientRpc();
        }
        else if (IsOwner)
        {
            _playerDied.isLeave = true;
            _playerDied.OnDeath += ShutDownSession;
            LeaveServerRpc();
        }
    }

    [ServerRpc]
    private void LeaveServerRpc()
    {
        _playerDied.Died();
    }

    private void ShutDownSession()
    {
        Cursor.lockState = CursorLockMode.None;
        GameManager.Instance.Leave();
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        SceneManager.LoadScene("MainMenu");
    }

    [ClientRpc]
    private void LoadMainMenuClientRpc()
    {
        ShutDownSession();
    }
}
