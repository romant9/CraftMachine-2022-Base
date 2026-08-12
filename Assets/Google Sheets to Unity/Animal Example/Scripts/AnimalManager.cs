using GoogleSheetsToUnity;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// example script to show realtime updates of multiple items
/// </summary>
public class AnimalManager : MonoBehaviour
{
    public enum SheetStatus
    {
        PUBLIC,
        PRIVATE
    }
    public SheetStatus sheetStatus;

    [HideInInspector]
    public string associatedSheet = "1_LO2NMbiMd1izu0pu4EUZ3D1O5kME2PpLCl2p6Ay1T8";
    [HideInInspector]
    public string associatedWorksheet = "List12";

    public List<AnimalObject> animalObjects = new List<AnimalObject>();
    public AnimalContainer container;
    

    public bool updateOnPlay;

    void Awake()
    {
        if(updateOnPlay)
        {
           UpdateStats();
        }
    }

    void UpdateStats()
    {
        if (sheetStatus == SheetStatus.PRIVATE)
        {
            SpreadsheetManager.Read(new GSTU_Search(associatedSheet, associatedWorksheet), UpdateAllAnimals);
        }
        else if(sheetStatus == SheetStatus.PUBLIC)
        {
            SpreadsheetManager.ReadPublicSpreadsheet(new GSTU_Search(associatedSheet, associatedWorksheet), UpdateAllAnimals);
        }
    }

    void UpdateAllAnimals(GstuSpreadSheet ss)
    {
        foreach (Animal animal in container.allAnimals)
        {
            animal.UpdateStats(ss);
        }

        foreach(AnimalObject animalObject in animalObjects)
        {
            animalObject.BuildAnimalInfo();
        }
    }

}
