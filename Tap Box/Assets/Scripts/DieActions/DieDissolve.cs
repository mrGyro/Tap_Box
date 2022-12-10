using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DieDissolve : MonoBehaviour, IDieAction
{
    [SerializeField] private Renderer[] renderers = new Renderer[0];
    [SerializeField] private float waitBeforeDissolve;
    [SerializeField] private float dissolveSpeed = 1.0f;
    [SerializeField] private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Queue<bool> _coroutines = new();

    private static readonly int DissolveSlider = Shader.PropertyToID("Dissolve_Slider_Reference");


    public async UniTask DieAction()
    {
        List<Material> materials = new List<Material>();

        foreach (var VARIABLE in renderers)
        {
            foreach (var VARIABLE2 in VARIABLE.materials)
            {
                materials.Add(VARIABLE2);
            }
            
        }
        await WaitAndDissolve(materials.ToArray());
    }

    private async UniTask WaitAndDissolve(Material[] materials)
    {
        if (.0f != waitBeforeDissolve)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(waitBeforeDissolve));
        }

        foreach (Material material in materials)
        {
            _coroutines.Enqueue(true);
            StartCoroutine(Dissolve(material));
        }

        while (0 != _coroutines.Count)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
        }
    }

    private IEnumerator Dissolve(Material material)
    {
        float timer = 0f;
        while (timer < 1f)
        {
            float val = Mathf.Lerp(-1.01f, 1.01f, dissolveCurve.Evaluate(timer));
            material.SetFloat(DissolveSlider, val);

            timer += Time.deltaTime * dissolveSpeed;
            yield return null;
        }

        _coroutines.Dequeue();
    }
}