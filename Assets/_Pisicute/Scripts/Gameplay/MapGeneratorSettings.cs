using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorSettings", menuName = "Scriptable Objects/MapGeneratorSettings", order = 2)]
public class MapGeneratorSettings : ScriptableObject
{
    public int seed;
    public bool useFixedSeed;
    [Range(0, 0.5f)] public float jitterProbability = 0.25f;
    [Header("Meta")]
    [Range(20, 200)] public int chunkSizeMin = 30;
    [Range(20, 200)] public int chunkSizeMax = 100;
    [Range(0, 10)] public int mapBorderX = 5;
    [Range(0, 10)] public int mapBorderZ = 5;
    [Range(0, 10)] public int regionBorder = 5;
    [Range(1, 4)] public int regionCount = 1;
    [Header("Elevation")]
    [Range(0f, 1f)] public float highRiseProbability = 0.25f;
    [Range(0, 0.4f)] public float sinkProbability = 0.2f;
    [Range(0, 100)] public int landPercentage = 50;
    [Range(1, 5)] public int waterLevel = 3;
    [Range(-5, 0)] public int elevationMinimum = -2;
    [Range(0, 15)] public int elevationMaximum = 8;
    [Range(0, 100)] public int erosionPercentage = 50;
    [Range(0, 8)] public int rockDesertElevation;
    [Header("Humidity")]
    [Range(0f, 1f)] public float precipitationFactor = 0.25f;
    [Range(0f, 1f)] public float evaporationFactor = 0.5f;
    [Range(0f, 1f)] public float runoffFactor = 0.25f;
    [Range(0f, 1f)] public float seepageFactor = 0.125f;
    [Range(0f, 1f)] public float startingMoisture = 0.1f;
    [Range(0, 40)] public int riverPercentage = 10;
    [Range(0f, 1f)] public float extraLakeProbability = 0.25f;
    [Header("Wind")]
    public HexDirection windDirection = HexDirection.NW;
    [Range(1f, 10f)] public float windStrength = 4f;
    [Header("Temperature")]
    [Range(0f, 1f)] public float lowTemperature = 0f;
    [Range(0f, 1f)] public float highTemperature = 1f;
    [Range(0f, 1f)] public float temperatureJitter = 0.1f;
    public HemisphereMode hemisphere;
}
