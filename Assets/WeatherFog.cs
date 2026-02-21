using UniStorm.Effects;
using UnityEngine;

public class WeatherFog : MonoBehaviour
{
    public UniStormAtmosphericFog fog;
    public float thickness;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!fog)
        {
            fog = FindAnyObjectByType<UniStormAtmosphericFog>(); //find any object it should maybe be only one, I don't know
        }

        fog.distanceFog = true;
        fog.startDistance = thickness;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
