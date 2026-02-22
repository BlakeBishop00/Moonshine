using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Ingredient Data")]
public class IngredientData : ScriptableObject
{
    public IngredientData(float yeastValue, float sweetValue, float sourValue, float bitterValue, float saltyValue, float umamiValue)
    {
        YeastValue = yeastValue;
        SweetValue = sweetValue;
        SourValue = sourValue;
        BitterValue = bitterValue;
        SaltyValue = saltyValue;
        UmamiValue = umamiValue;
    }

    public float YeastValue;
    public float SweetValue;
    public float SourValue;
    public float BitterValue;
    public float SaltyValue;
    public float UmamiValue;

    public static IngredientData operator +(IngredientData a, IngredientData b)
    {
        if (a == null) return b;
        if (b == null) return a;

        IngredientData result = CreateInstance<IngredientData>();

        result.YeastValue = a.YeastValue + b.YeastValue;
        result.SweetValue = a.SweetValue + b.SweetValue;
        result.SourValue = a.SourValue + b.SourValue;
        result.BitterValue = a.BitterValue + b.BitterValue;
        result.SaltyValue = a.SaltyValue + b.SaltyValue;
        result.UmamiValue = a.UmamiValue + b.UmamiValue;

        return result;
    }

    public static Color GetFlavorColor(IngredientData mixture)
    {
        float r = mixture.SweetValue;
        float g = mixture.SourValue + mixture.BitterValue;
        float b = mixture.SaltyValue + mixture.UmamiValue;

        Color baseColor = new Color(r / 100f, g / 100f, b / 100f);

        return baseColor;
    }
}
