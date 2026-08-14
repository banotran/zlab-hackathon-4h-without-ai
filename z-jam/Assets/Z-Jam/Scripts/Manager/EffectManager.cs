using System.Collections;
using System.Collections.Generic;
using MeowgaByte.Utils;
using UnityEngine;

namespace Game.Manager
{
    public class EffectManager : MonoSingleton<EffectManager>
    {
        [Header("Ref")]
        [SerializeField] private ColorDatabase _colorDb;
        [SerializeField] private ParticleSystem _cubeBlastParticle;
        [SerializeField] private int _poolSize = 40;
        [SerializeField] private Transform _container;

        private Queue<ParticleSystem> _blastEffectPool = new();
        
        void Start()
        {
            WarmUpPools();
        }

        void Update()
        {
            
        }

        private void WarmUpPools()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                CreateNewPar();
            }
        }

        public ParticleSystem PlayBlastParticle(Vector3 pos, Color color)
        {
            ParticleSystem p = _blastEffectPool.Count > 0 ? _blastEffectPool.Dequeue() : CreateNewPar();
            var m = p.main;
            m.startColor = color;
            p.transform.position = pos;
            p.gameObject.SetActive(true);
            p.Play();
            StartCoroutine(ReturnToPool(p));

            return p;
        }

        private IEnumerator ReturnToPool(ParticleSystem p)
        {
            yield return new WaitForSeconds(p.main.duration);
            p.Stop();
            p.gameObject.SetActive(false);
            _blastEffectPool.Enqueue(p);
        }

        private ParticleSystem CreateNewPar()
        {
            ParticleSystem p = Instantiate(_cubeBlastParticle, _container);
            _blastEffectPool.Enqueue(p);
            p.gameObject.SetActive(false);
            return p;
        }
    }


}
