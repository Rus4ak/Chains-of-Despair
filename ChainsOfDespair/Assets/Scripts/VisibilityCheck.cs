using System.Collections;
using UnityEngine;

public class VisibilityCheck : MonoBehaviour
{
    [SerializeField] private GameObject _disableObject;
    [SerializeField] private float _visibilityDistance = 100f;
    [SerializeField] private LayerMask _wallLayer;

    private Transform _player;

    private void OnEnable()
    {
        StartCoroutine(Check());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Check()
    {
        while (true)
        {
            if (_player == null)
            {
                GameObject playerGO = GameObject.FindWithTag("Player");
                if (playerGO != null)
                {
                    _player = playerGO.transform;
                }

                else
                {
                    yield return null;
                    continue;
                }
            }

            if (Vector3.Distance(transform.position, _player.position) > _visibilityDistance)
            {
                _disableObject.SetActive(false);
                yield return new WaitForSeconds(1);
                continue;
            }

            Vector3 direction = _player.position - transform.position;
            float distance = direction.magnitude;

            if (Physics.Raycast(transform.position, direction, distance, _wallLayer))
            {
                _disableObject.SetActive(false);
            }

            else
            {
                _disableObject.SetActive(true);
            }

            yield return new WaitForSeconds(1);
        }
    }

    private void OnDrawGizmos()
    {
        if (_player != null)
        {
            Vector3 direction = _player.position - transform.position;
            float distance = direction.magnitude;

            if (Vector3.Distance(transform.position, _player.position) > _visibilityDistance)
                return;

            if (Physics.Raycast(transform.position, direction, distance, _wallLayer))
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Gizmos.DrawRay(transform.position, direction.normalized * distance);
        }
    }
}
