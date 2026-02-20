using System.Runtime.CompilerServices;
using UnityEngine;

public class Bottle : MonoBehaviour, IInspectable
{
    private IngredientData _currentMixture;
    private int _ageLevel = 0;
    private Renderer _renderer;
    
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _currentMixture = ScriptableObject.CreateInstance<IngredientData>();
    }

    public void SetMixture(IngredientData mixture, int ageLevel)
    {
        // For now just darken the color based on the age level
        _renderer.material.color = IngredientData.GetFlavorColor(mixture) * Mathf.Pow(0.8f, _ageLevel);
        _currentMixture = mixture;
        _ageLevel = ageLevel;
    }

    public void GetMixture(out IngredientData mixture, out int ageLevel)
    {
        mixture = _currentMixture;
        ageLevel = _ageLevel;
    }

    public IngredientData GetStats()
    {
        return _currentMixture;
    }
}
