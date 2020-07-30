using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Warp : MonoBehaviour
{
    [Header("Warp Effect")]
    [SerializeField] Material mat = null;
    [SerializeField] GameObject particle = null;
    [SerializeField] SkinnedMeshRenderer mesh = null;
    [SerializeField] CameraLookUp cam = null;
    [SerializeField] Image imageBlack = null;
    
    private bool canStart = false;
    private float t;
    private float t2;
    private float alpha = 1;
    private bool isAlphaActive = false;
    
    private static readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
    private static readonly int ShowEmission = Shader.PropertyToID("_ShowEmission");

    void Start()
    {
        StartCoroutine(AlphaBegin());
    }

    void Update()
    {
        if(canStart)
            WarpEffect();

    }

    public void WarpEffect()
    {

        if (t < 1)
        {
            if (t > 0.25f)
            {
                t2 += Time.deltaTime / 4f;
                mat.SetFloat(DissolveAmount, Mathf.Lerp(0, 1f, t2));
                if(t2 > 0.1)
                    particle.SetActive(true);
                if (t2 > 0.4)
                    cam.canLookUp = true;
            }
            t += Time.deltaTime / 5f;
            mat.SetFloat(ShowEmission, Mathf.Lerp(0, 12f, t));
        }

        if (t2 > 0.67)
        {
            mesh.enabled = false;
            if (!isAlphaActive)
            {
                isAlphaActive = true;
                alpha = 0;
                StartCoroutine(AlphaEnd());
            }
        }

    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(8f);
        mat.SetFloat(DissolveAmount, 0f);
        mat.SetFloat(ShowEmission, 0f);

        SceneManager.LoadScene(0);
    }

    IEnumerator AlphaEnd()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            alpha += 0.005f;
            imageBlack.color = new Color(0, 0, 0, alpha);
            
        }
    }

    IEnumerator AlphaBegin()
    {
        while (alpha >= 0)
        {
            yield return new WaitForSeconds(0.01f);
            alpha -= 0.005f;
            imageBlack.color = new Color(0, 0, 0, alpha);

        }
    }

    void OnMouseDown()
    {
        canStart = true;
        StartCoroutine(Reset());
    }

    void OnApplicationQuit()
    {
        mat.SetFloat(DissolveAmount, 0f);
        mat.SetFloat(ShowEmission, 0f);
    }
}
