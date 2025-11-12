using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class JumpingEnemy : Enemy
{
    [SerializeField] private float _jumpTime = .7f;
    [SerializeField] private float _arcHeight = 5f;

    private Transform _camera;
    private bool _isJumping;

    protected override void Attack()
    {
        _camera = _attackedPlayer.GetComponent<LookAround>().Camera;
        
        Invoke(nameof(JumpServerRpc), .5f);
        
        base.Attack();
    }

    [ServerRpc(RequireOwnership = false)]
    private void JumpServerRpc()
    {
        if (!_isJumping)
        {
            StartCoroutine(Jump());
        }
    }

    private IEnumerator Jump()
    {
        _isJumping = true;

        Vector3 startPos = transform.position;
        float time = 0f;

        while (time < _jumpTime)
        {
            time += Time.deltaTime;
            float t = time / _jumpTime;
            float smoothT = t * t;

            Vector3 pos = Vector3.Lerp(startPos, _camera.position, smoothT);

            pos.y += Mathf.Sin(t * Mathf.PI) * _arcHeight;

            transform.position = pos;
            yield return null;
        }

        _isJumping = false;
        
        StopAttack();
    }
}
