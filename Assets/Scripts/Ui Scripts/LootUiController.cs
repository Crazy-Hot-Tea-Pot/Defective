using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootUiController : UiController
{
    public int LootScrap
    {
        get
        {
            return lootScrap;
        }
        set
        {
            lootScrap = value;
        }
    }
    public List<NewChip> LootChips
    {
        get
        {
            return lootChips;
        }
        set
        {
            lootChips = value;
        }
    }
    public List<Item> LootItems
    {
        get
        {
            return lootItems;
        }
        set
        {
            lootItems = value;
        }
    }

    [Header("Prefabs")]
    public GameObject ChipPrefab;
    public GameObject LootPrefab;

    [Header("Loot Stuff")]
    public GameObject LootGainDisplay;
    public TextMeshProUGUI LootGainInfo;
    public GameObject LootContainer;

    [Header("Selection stuff")]
    public GameObject SelectionDisplay;
    public GameObject SelectionContainer;
    public GameObject SelectedChipInfo;
    public Button CancelButton;

    [Header("Animation values")]
    [Range(0.01f,10f)]
    public float Duration = 1f;

    public bool distributionCheck = true;
    //When dropping items and cards we need to know how many times we have run through without a rare
    public int lootRunThroughCounter = 0;

    public List<NewChip> LootTempChip
    {
        get
        {
            return lootTempChip;
        }
        set
        {
            lootTempChip = value;
        }
    }
    public List<Item> lootTempItems = new();

    private int lootScrap;
    private List<NewChip> lootChips=new();
    private List<Item> lootItems=new();
    private float elapsedTime=0f;
    private readonly int startValue = 0;
    private NewChip selectedChip;    
    private List<NewChip> distributedChips=new();
    private List<Item> distributedItem=new();
    private List<NewChip> lootTempChip = new();

    private bool firstTimeLootAppear = true;
    


    public override void Initialize()
    {
        Debug.Log("Loot Ui initialized");
        CancelButton.onClick.AddListener(()=>CancelSelection());
    }
    /// <summary>
    /// Put information into the screen.
    /// make sure to call after you have sent info to the Controller.
    /// </summary>
    public void UpdateLootScreen()
    {
        foreach (Transform child in LootContainer.transform)
        {
            Destroy(child.gameObject);
        }

        if (lootChips.Count == 0 && lootItems.Count == 0)
        {
            GameManager.Instance.UpdateGameMode(GameManager.GameMode.Roaming);
        }

        LootTempChip.Clear();
        lootTempItems.Clear();

        LootTempChip.AddRange(LootChips);
        lootTempItems.AddRange(LootItems);

        StartCoroutine(UpdateScrapLootGainInfo());
        if (firstTimeLootAppear)
        {
            StartCoroutine(SpawnLootWithDelay());
            firstTimeLootAppear = false;
        }
        else
        {
            SpawnLootInstantly();
        }

    }
    /// <summary>
    /// Bring up the selection so user can replace chip for new chip.
    /// </summary>
    /// <param name="selectedChip"></param>
    public void BringUpChipSelection(NewChip selectedChip)
    {
        this.selectedChip = selectedChip;

        SelectionDisplay.SetActive(true);

        SelectedChipInfo.GetComponent<ChipInfoController>().ChipImage.sprite=selectedChip.chipImage;
        SelectedChipInfo.GetComponent<ChipInfoController>().ChipName.SetText(selectedChip.chipName);
        SelectedChipInfo.GetComponent<ChipInfoController>().ChipDescription.SetText(selectedChip.ChipDescription);
        SelectedChipInfo.GetComponent<ChipInfoController>().ChipType.SetText(selectedChip.ChipType.ToString());

        foreach (Transform child in SelectionContainer.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(NewChip newChip in ChipManager.Instance.PlayerDeck)
        {
            GameObject Chip = Instantiate(ChipPrefab, SelectionContainer.transform);

            Chip.GetComponent<Chip>().NewChip=newChip;

            Chip.GetComponent<Chip>().SetChipModeTo(global::Chip.ChipMode.Delete);
        }
    }

    /// <summary>
    /// Replace replaceChip with selectedChip and refresh LootScreen.
    /// </summary>
    /// <param name="replaceChip"></param>
    public void ReplaceChip(NewChip replaceChip)
    {
        ChipManager.Instance.DeleteChip(replaceChip);

        NewChip CloneOfSelectedChip=Instantiate(selectedChip);

        ChipManager.Instance.AddNewChipToDeck(CloneOfSelectedChip);

        LootChips.Remove(selectedChip);


        selectedChip = null;

        SelectionDisplay.SetActive(false);

        UpdateLootScreen();
    }

    /// <summary>
    /// Generates a random number based on the size of our chipdroplist
    /// </summary>
    /// <returns></returns>
    public float RandomNumberForSelection(int count)
    {
        float randomNumber = UnityEngine.Random.Range(0, count - 1);
        return randomNumber;
    }
    /// <summary>
    /// Instead of delay spawn loot instantly.
    /// </summary>
    private void SpawnLootInstantly()
    {
        if (distributionCheck)
        {
            distributedChips.Clear();
            distributedItem.Clear();

            int counter = Mathf.Min(LootTempChip.Count, 3); 
            for (int i = 0; i < counter; i++)
            {
                // Select a random chip
                int index = UnityEngine.Random.Range(0, LootTempChip.Count);
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(LootTempChip[index]);
                distributedChips.Add(LootTempChip[index]);
                LootTempChip.RemoveAt(index);
            }

            counter = Mathf.Min(lootTempItems.Count, 3);
            for (int i = 0; i < counter; i++)
            {
                // Select a random item
                int index = UnityEngine.Random.Range(0, lootTempItems.Count);
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(lootTempItems[index]);
                distributedItem.Add(lootTempItems[index]);
                lootTempItems.RemoveAt(index);
            }
        }
        else
        {
            // Repopulate distributed loot
            foreach (NewChip chip in distributedChips)
            {
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(chip);
            }

            foreach (Item item in distributedItem)
            {
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(item);
            }
        }
    }
    private void CancelSelection()
    {
        SelectionDisplay.SetActive(false);

        UpdateLootScreen ();
    }
    private IEnumerator UpdateScrapLootGainInfo()
    {
        int targetValue=LootScrap;

        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;

            // Interpolate the value
            int currentScrap = Mathf.RoundToInt(Mathf.Lerp(startValue, targetValue, elapsedTime / Duration));

            // Update the text
            LootGainInfo.SetText("They have dropped <color=#FFFF00>" + currentScrap + "</color> Scrap and the following chips and items.");

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        LootGainInfo.SetText("They have dropped <color=#FFFF00>" + LootScrap + "</color> Scrap and the following chips and items.");
    }
    private IEnumerator SpawnLootWithDelay()
    {
        //If not distributed
        if (distributionCheck == true)
        {
            //Clear the saved list
            distributedChips.Clear();
            distributedItem.Clear();
            
            #region RandomDistributionForChips
            //Temp variable for top of equation
            float sum = 0;
            //Temp variable for bottom of equation
            float sumWeight = 0;
            int counter = LootTempChip.Count;

            //Stop counter at 3
            if (counter > 3)
            {
                counter = 3;
            }

            #region changingRarityRates
            //Loop through to know how many times this has passed through
            lootRunThroughCounter++;
            //Change the rarity if rare
            foreach (NewChip chip in lootTempChip)
            {
                //Check if the chip is rare
                if (chip.chipRarity != NewChip.ChipRarity.Rare)
                {
                    //If it's rare then make sure to change it's rare
                    //0% chance if first time, 10% if if second time and 30% chance if still not rare
                    if (lootRunThroughCounter == 0)
                    {
                        chip.ChipRareityWeight = 0.0f;
                    }
                    else if (lootRunThroughCounter == 1)
                    {
                        chip.ChipRareityWeight = 0.10f;
                    }
                    else if (lootRunThroughCounter > 1)
                    {
                        chip.ChipRareityWeight = 0.30f;
                    }
                }
                else if (chip.chipRarity == NewChip.ChipRarity.Rare)
                {
                    lootRunThroughCounter = 0;
                }

            }
            #endregion
            //We need to loot 3 chips
            for (int i = 0; i < counter; i++)
            {
                //Loop through and perform our formula for distrapuation of random chips based on weights for each card as a top and bottom
                foreach (NewChip selection in LootTempChip)
                {
                    //Sum = (random * weight)(random2 * weight2)...
                    sum += (RandomNumberForSelection(LootTempChip.Count) * selection.ChipRareityWeight);
                    //sumWeigh = weight1 + weight2...
                    sumWeight += selection.ChipRareityWeight;
                }
                //Sum/sumWeights
                float answer = sum / sumWeight;
                if (sum == 0 && sumWeight == 0)
                {
                    answer = 0;
                }
                //Make it a solid int
                //If the converted value is larger (has been rounded up) subtract the result by 1 also do this if it's equal as we would normally round up on the 0.5
                if (System.Convert.ToInt32(answer) >= answer && System.Convert.ToInt32(answer) != 0)
                {
                    answer = System.Convert.ToInt32(answer);
                    answer -= 1;
                }
                //Else just convert it normal it will round down
                else
                {
                    answer = System.Convert.ToInt32(answer);
                }

                // Instantiate the LootPrefab
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);

                // Populate the loot
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(LootTempChip[(int)answer]);
                distributedChips.Add(lootTempChip[(int)answer]);
                LootTempChip.Remove(LootTempChip[(int)answer]);



                // Wait for 1 second before spawning the next
                yield return new WaitForSeconds(Duration);
            }
            #endregion

            #region RandomDistributionForItems
            //Temp variable for top of equation
            sum = 0;
            //Temp variable for bottom of equation
            sumWeight = 0;
            counter = 0;
            counter = lootTempItems.Count;
            if (counter > 3)
            {
                counter = 3;
            }
            //We need to loot 3 cards
            for (int i = 0; i < counter; i++)
            {
                //Loop through and perform our formula for distrapuation of random cards based on weights for each card as a top and bottom
                foreach (Item item in lootTempItems)
                {
                    //Sum = (random * weight)(random2 * weight2)...
                    sum += (RandomNumberForSelection(lootTempItems.Count) * item.ItemRarityWeight);
                    Debug.Log("Chip Value Sum for random drops: " + sum);
                    //sumWeigh = weight1 + weight2...
                    sumWeight += item.ItemRarityWeight;
                }
                //Sum/sumWeights
                float answer = sum / sumWeight;
                if (sum == 0 && sumWeight == 0)
                {
                    answer = 0;
                }

                //Make it a solid int
                //If the converted value is larger (has been rounded up) subtract the result by 1 also do this if it's equal as we would normally round up on the 0.5
                if (System.Convert.ToInt32(answer) >= answer && System.Convert.ToInt32(answer) != 0)
                {
                    answer = System.Convert.ToInt32(answer);
                    answer -= 1;
                }
                //Else just convert it normal it will round down
                else
                {
                    answer = System.Convert.ToInt32(answer);
                }

                // Instantiate the LootPrefab
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);

                // Populate the loot
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(lootTempItems[(int)answer]);
                distributedItem.Add(lootTempItems[(int)answer]);
                lootTempItems.Remove(lootTempItems[(int)answer]);

                // Wait for 1 second before spawning the next
                yield return new WaitForSeconds(Duration);
            }
            #endregion
        }
        //If already distributed then just repopulate the distributed list
        else if(distributionCheck == false)
        {
            foreach (NewChip chip in distributedChips)
            {
                // Instantiate the LootPrefab
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);

                // Populate the loot
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(chip);

                // Wait for 1 second before spawning the next
                yield return new WaitForSeconds(Duration);
            }

            foreach (Item item in distributedItem)
            {
                // Instantiate the LootPrefab
                GameObject newLoot = Instantiate(LootPrefab, LootContainer.transform);

                // Populate the loot
                LootController lootController = newLoot.GetComponent<LootController>();
                lootController.PopulateLoot(item);

                // Wait for 1 second before spawning the next
                yield return new WaitForSeconds(Duration);
            }
        }
    }

    void OnDestroy()
    {
        CancelButton.onClick.RemoveAllListeners();
    }
}
