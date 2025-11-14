using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerInitialize : NetworkBehaviour
{
    [SerializeField] private GameObject _chain;
    [SerializeField] private Transform _forwardJoint;
    [SerializeField] private Transform _backJoint;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Inventory _inventory;

    private Rigidbody _rigidbody;

    [HideInInspector] public List<GameObject> chainGO;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isMove = true;
    [HideInInspector] public GameObject ragdoll;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        List<PlayerInitialize> players = PlayersManager.Instance.players;
        
        if (players.Count > 0)
        {
            GameObject chainGO = Instantiate(_chain);
            Chain chain = chainGO.GetComponent<Chain>();
            List<Rigidbody> playersRb = new List<Rigidbody>()
            {
                GetComponent<Rigidbody>(),
                players[players.Count - 1].GetComponent<Rigidbody>()
            };

            chain.Initialize(players[players.Count - 1]._backJoint, _forwardJoint, playersRb);

            GetComponent<CalculateDistance>().connectedPlayers.Add(players[players.Count - 1].transform);
            players[players.Count - 1].GetComponent<CalculateDistance>().connectedPlayers.Add(transform);
        }

        if (!PlayersManager.Instance.players.Contains(this))
        {
            players.Add(this);
        }

        if (IsOwner)
        {
            PlayersManager.Instance.ownerPlayer = this;
            PlayersManager.Instance.ownerInventory = _inventory;
        }
        else
            _canvas.SetActive(false);
    }

    public void Spawn(Vector3 position)
    {
        if (IsOwner)
        {
            _rigidbody.position = position;
        }
    }

    public void DestroyChain(NetworkObjectReference diedPlayer)
    {
        DestroyChainServerRpc(diedPlayer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyChainServerRpc(NetworkObjectReference objRef)
    {
        DestroyChainClientRpc(objRef);
    }

    [ClientRpc]
    private void DestroyChainClientRpc(NetworkObjectReference diedPlayerRef)
    {
        
        if (!diedPlayerRef.TryGet(out NetworkObject diedPlayer))
            return;

        foreach (var player in PlayersManager.Instance.players)
        {
            CalculateDistance calculateDistance = player.GetComponent<CalculateDistance>();

            for (int i = 0; i < calculateDistance.connectedPlayers.Count; i++)
            {
            
                var connectedPlayer = calculateDistance.connectedPlayers[i];

                if (connectedPlayer == null)
                    continue;

                if (connectedPlayer.GetComponent<NetworkObject>().NetworkObjectId == diedPlayer.NetworkObjectId)
                {
                    calculateDistance.connectedPlayers.RemoveAt(i);
                    break;
                }
            }
        }

        for (int i = 0; i < chainGO.Count; i++)
        {
            Destroy(chainGO[i]);
        }

        chainGO.Clear();
    }

    [ClientRpc]
    public void StartAttackClientRpc()
    {
        isMove = false;
        isAlive = false;
    }
}