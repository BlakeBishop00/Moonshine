using UnityEngine;

public class Bottle : MonoBehaviour
{
    private IngredientData _currentMixture;
    private int _ageLevel = 0;
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    public void SetMixture(IngredientData mixture, int ageLevel)
    {
        // For now just darken the color based on the age level
        _renderer.material.color = IngredientData.GetFlavorColor(mixture) * Mathf.Pow(0.8f, _ageLevel);
        _currentMixture = mixture;
        _ageLevel = ageLevel;
    }


}
