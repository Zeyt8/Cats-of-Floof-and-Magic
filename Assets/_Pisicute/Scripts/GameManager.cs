using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ResourcesPanel resourcesPanel;
    public BuildingDetails buildingDetails;

    // Start is called before the first frame update
    void Start()
    {
        Shader.DisableKeyword("_HEX_MAP_EDIT_MODE");
    }
}
