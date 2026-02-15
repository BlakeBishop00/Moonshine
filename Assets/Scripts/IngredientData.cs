using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public IngredientData(float waterValue, float yeastValue, float sweetValue, float sourValue, float bitterValue, float saltyValue, float umamiValue)
    {
        WaterValue = waterValue;
        YeastValue = yeastValue;
        SweetValue = sweetValue;
        SourValue = sourValue;
        BitterValue = bitterValue;
        SaltyValue = saltyValue;
        UmamiValue = umamiValue;
    }
    
    public float WaterValue;
    public float YeastValue;
    public float SweetValue;
    public float SourValue;
    public float BitterValue;
    public float SaltyValue;
    public float UmamiValue;
}
