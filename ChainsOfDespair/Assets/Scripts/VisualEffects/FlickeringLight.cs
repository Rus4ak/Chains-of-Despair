using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _lamps;
    [SerializeField] private Light[] _lights;

    private float[] _lightIntensity;
    private AudioSource _audioSource;

    private Transform _player;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        _lightIntensity = new float[_lights.Length];

        for (int i = 0; i < _lights.Length; i++)
        {
            _lightIntensity[i] = _lights[i].intensity;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(Flick());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Flick()
    {
        while (true)
        {
            if (_player == null)
            {
                GameObject playerGO = GameObject.FindWithTag("Player");

                if (playerGO != null)
                    _player = playerGO.transform;
                else
                {
                    yield return null;
                    continue;
                }
            }

            foreach (var l in _lights)
            {
                if (!l.gameObject.activeInHierarchy)
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }
            }

            if (Vector3.Distance(_player.position, transform.position) < 50)
            {
                yield return new WaitForSeconds(Random.Range(.2f, 5f));

                foreach (var light in _lights)
                {
                    light.intensity = 0;
                }

                foreach (var l in _lamps)
                {
                    l.material.DisableKeyword("_EMISSION");
                }

                _audioSource.Play();

                yield return new WaitForSeconds(.2f);

                for (int i = 0; i < _lights.Length; i++)
                {
                    _lights[i].intensity = _lightIntensity[i];
                }

                foreach (var l in _lamps)
                {
                    l.material.EnableKeyword("_EMISSION");
                }
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }
}
