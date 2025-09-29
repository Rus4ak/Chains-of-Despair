using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _lamps;
    [SerializeField] private Light[] _lights;

    private float[] _lightIntensity;
    private AudioSource _audioSource;

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

        StartCoroutine(Flick());
    }

    IEnumerator Flick()
    {
        while (true)
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
    }
}
