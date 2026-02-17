using UnityEngine;

public class Bottle : MonoBehaviour
{
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetMixture(IngredientData mixture)
    {
        // For now just change the color based on the mixtures properties
        _renderer.material.color = GetFlavorColor(mixture);
    }

    public Color GetFlavorColor(IngredientData mixture)
    {
        float r = mixture.SweetValue;
        float g = mixture.SourValue;
        float b = mixture.BitterValue;

        Color baseColor = new Color(r, g, b);

        baseColor *= Mathf.Lerp(0.5f, 1.5f, mixture.SaltyValue);

        Color.RGBToHSV(baseColor, out float h, out float s, out float v);
        s = Mathf.Clamp01(s + mixture.UmamiValue * 0.5f);
        return Color.HSVToRGB(h, s, v);
    }


}
