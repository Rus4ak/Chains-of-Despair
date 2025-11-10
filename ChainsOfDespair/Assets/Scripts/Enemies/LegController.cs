using System.Collections;
using UnityEngine;

public class LegController : MonoBehaviour
{
    [SerializeField] private Transform _origin;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private float _arcHeight = 1f;
    [SerializeField] private float _maxDistance = 2f;

    private Coroutine _moveCoroutine;

    private void Update()
    {
        if (_origin == null)
            return;

        if (Vector3.Distance(transform.position, _origin.position) > _maxDistance)
        {
            if (_moveCoroutine == null)
            {
                _moveCoroutine = StartCoroutine(MoveToOrigin());
            }
        }
    }

    private IEnumerator MoveToOrigin()
    {
        Vector3 startPos = transform.position;
        float progress = 0f;

        while (progress < 1f)
        {
            if (_origin == null)
                break;

            float journeyLength = Vector3.Distance(startPos, _origin.position);

            progress += (_speed * Time.deltaTime) / journeyLength;
            progress = Mathf.Clamp01(progress);

            Vector3 nextPos = Vector3.Lerp(startPos, _origin.position, progress);

            nextPos.y += Mathf.Sin(progress * Mathf.PI) * _arcHeight;

            transform.position = nextPos;

            yield return null;
        }

        _moveCoroutine = null;
    }
}
