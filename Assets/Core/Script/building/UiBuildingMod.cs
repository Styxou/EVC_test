using UnityEngine;
using UnityEngine.UI;


public class UiBuildingMod : MonoBehaviour
{
    [Header("References")]
    BuildingMod BuildingMod;

    [Header("UI")]
    [SerializeField] GameObject UIBuilding;
    [SerializeField] GameObject UIMods;
    bool uiBuildActivated = false;

    [Header("Wall")]
    [SerializeField] GameObject WallUi;
    bool uiWallActivated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildingMod = GetComponent<BuildingMod>();
    }

    public void EnableUiBuild()
    {
        //Build mod
        if (BuildingMod.bIsBuilding == true && uiBuildActivated == false)
        {
            UIBuilding.transform.GetChild(1).GetComponent<Image>().enabled = true;
            UIMods.SetActive(true);
            uiBuildActivated = true;
        }
        else
        {
            UIBuilding.transform.GetChild(1).GetComponent<Image>().enabled = false;
            UIMods.SetActive(false);
            uiBuildActivated = false;

        }

      
    }

    public void EnableUiWal()
    {
        //Wall
        if (BuildingMod.state == BuildingMod.BuildingState.wall && uiWallActivated == false)
        {
            WallUi.transform.GetChild(1).GetComponent<Image>().enabled = true;
            uiWallActivated = true;


        }
        else
        {
            WallUi.transform.GetChild(1).GetComponent<Image>().enabled = false;
            uiWallActivated = false;
        }
    } 
}

