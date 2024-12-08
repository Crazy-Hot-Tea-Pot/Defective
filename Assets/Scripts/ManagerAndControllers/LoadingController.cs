using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{
    private string targetScene;
    private List<NewChip> Chips = new List<NewChip>();
    private List<Item> Items = new List<Item>();
    private NewChip choosenChip;
    private Item choosenItem;
    

    public GameObject Display;
    public TextMeshProUGUI TipText;
    public Image ProgressBar;

    [Header("Loading screen settings")]
    public float rotationSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {
        targetScene = GameManager.Instance.TargetScene.ToString();

        StartCoroutine(LoadSceneAsync(targetScene));

        // Get all chips and items from their respective managers
        Chips = new List<NewChip>(ChipManager.Instance.AllChips);
        Items = new List<Item>(GearManager.Instance.AllGear);

        // Randomly choose between a chip or an item
        bool showChipTip = true; //Random.value > 0.5f;

        if (showChipTip)// && Chips.Count > 0)
        {
            // Randomly select a chip
            choosenChip = Chips[Random.Range(0, Chips.Count)];
            Display.GetComponent<Image>().sprite = choosenChip.chipImage;
            TipText.SetText("Chip Tip: " + choosenChip.ChipTip);
        }
        else if (Items.Count > 0)
        {
            // Randomly select an item
            choosenItem = Items[Random.Range(0, Items.Count)];
            TipText.SetText("Item Tip: " + choosenItem.itemTip);
        }

        Display.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        // Gradually rotate the image on the Y-axis
        Display.GetComponent<Transform>().Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Start loading the target scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // Prevents auto-activation

        float elapsedTime = 0f;
        float FillDuration = 5f;
        while(elapsedTime < FillDuration)
        {
            elapsedTime += Time.deltaTime;
            ProgressBar.fillAmount = Mathf.Lerp(0,1,elapsedTime/FillDuration);
            yield return null;
        }

        // Wait until the scene is fully loaded (progress reaches 90%)
        while (!asyncLoad.isDone)
        {
            // Display of progress goes here.
            //ProgressBar.fillAmount = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            if (asyncLoad.progress >= 0.9f)
            {
                // Scene is almost ready
                // add a delay or later we can add delay :D
                // or wait for user input to continue
                // Short delay for smoother transition
                yield return new WaitForSeconds(1f);

                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }

            // Wait for next frame
            yield return null;
        }
    }
}