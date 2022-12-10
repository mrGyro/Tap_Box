using Cysharp.Threading.Tasks;
using UnityEngine;

public class VFXDieAction : MonoBehaviour, IDieAction
{
    [SerializeField] private ParticleSystem _particleSystem;
    public async UniTask DieAction()
    {
        _particleSystem.gameObject.SetActive(true);
        _particleSystem.Play();
        await UniTask.WaitUntil(() => !_particleSystem.IsAlive());
    }
}
