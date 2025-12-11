using UnityEngine;
using UnityEngine.UI;

public class GlitchUIController : MonoBehaviour
{
    public Material glitchMaterial;

    [Header("Animation")]
    public bool animate = true;
    public float minGlitch = 0f;
    public float maxGlitch = 1f;
    public float speed = 2f;

    private float t = 0f;

    void Update()
    {
        if (!animate || glitchMaterial == null) 
            return;

        t += Time.deltaTime * speed;
        float strength = Mathf.Lerp(minGlitch, maxGlitch, (Mathf.Sin(t) + 1f) / 2f);

        glitchMaterial.SetFloat("_GlitchStrength", strength);
    }
}