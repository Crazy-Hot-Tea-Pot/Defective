using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingController : MonoBehaviour
{       

    public GameObject Display;
    public TextMeshProUGUI TipText;
    public Image ProgressBar;

    [Header("Loading screen settings")]
    public float ChipRotationSpeed = 30f;

    [Header("Story Progress UI")]
    public GameObject graphContainer; 
    public GameObject nodePrefab; 
    public GameObject connectionPrefab;
    [Header("Node Colors")]
    // Past levels
    public Color completedColor = Color.blue;
    // Current level
    public Color currentColor = Color.green;
    // Future levels
    public Color futureColor = Color.gray;
    // Unreachable levels
    public Color lockedColor = Color.red;


    private List<GameObject> nodes = new List<GameObject>();

    private string targetScene;
    private List<NewChip> Chips = new List<NewChip>();
    private NewChip choosenChip;

    // Start is called before the first frame update
    void Start()
    {
        targetScene = GameManager.Instance.TargetScene.ToString();

        StartCoroutine(LoadSceneAsync(targetScene));

        // Get all chips and items from their respective managers
        Chips = new List<NewChip>(ChipManager.Instance.AllChips);

        // Randomly select a chip
        choosenChip = Instantiate(Chips[Random.Range(0, Chips.Count)]);

        Display.GetComponent<Image>().sprite = choosenChip.chipImage;
            TipText.SetText("Chip Tip:\n" + choosenChip.ChipTip);


        Display.SetActive(true);

        //Generate Story Graph
        GenerateStoryGraph();
    }

    // Update is called once per frame
    void Update()
    {
        // Gradually rotate the image on the Y-axis
        Display.GetComponent<Transform>().Rotate(0f, ChipRotationSpeed * Time.deltaTime, 0f);
    }
    private void GenerateStoryGraph()
    {
        // Clear previous graph before generating
        foreach (GameObject obj in nodes)
        {
            Destroy(obj);
        }
        nodes.Clear();

        Story story = StoryManager.Instance.CurrentStory;
        LevelDefinition currentLevel = StoryManager.Instance.CurrentLevel;

        // Dictionary to store positions for each level to prevent overlapping
        Dictionary<LevelDefinition, Vector2> levelPositions = new Dictionary<LevelDefinition, Vector2>();

        bool isLinear = IsLinearPath(story.levels[0]);

        // Horizontal distance between levels
        float spacingX = isLinear ? 200f : 150f;
        // Vertical spacing for branching paths
        float spacingY = isLinear ? 0f : 60f;

        // Recursive function to create nodes for branching paths
        void CreateNodes(LevelDefinition level, Vector2 position, int depth)
        {
            if (levelPositions.ContainsKey(level)) return; // Prevent duplicates

            // Create the node
            GameObject node = Instantiate(nodePrefab, graphContainer.transform);
            node.GetComponent<RectTransform>().anchoredPosition = position;
            nodes.Add(node);
            levelPositions[level] = position;

            // Get the Image component for coloring
            Image nodeImage = node.GetComponent<Image>();

            // Determine if the level is unreachable (locked)
            bool isLocked = true;
            foreach (var prevLevel in levelPositions.Keys)
            {
                if (prevLevel.NextLevels.Contains(level))
                {
                    // If any previous level leads to this, it's reachable
                    isLocked = false;
                    break;
                }
            }

            // Set color based on level status
            if (level == currentLevel)
            {
                // Highlight current level
                nodeImage.color = currentColor;
                // Make it pulse
                StartCoroutine(PulseEffect(node.transform));
            }
            else if (level.isCompleted)
            {
                // Past levels
                nodeImage.color = completedColor;
            }
            else if (isLocked)
            {
                // Unreachable path (player didn't take this branch)
                nodeImage.color = lockedColor;
            }
            else
            {
                // Future levels
                nodeImage.color = futureColor;
            }


            // Create connections for all branches
            for (int i = 0; i < level.NextLevels.Count; i++)
            {
                LevelDefinition nextLevel = level.NextLevels[i];

                // Calculate position for the next node (spread branches vertically)
                Vector2 nextPosition = new Vector2(position.x + spacingX, position.y - (i * spacingY));

                // Recursively create nodes for next levels
                CreateNodes(nextLevel, nextPosition, depth + 1);

                // Create a connection line
                GameObject connection = Instantiate(connectionPrefab, graphContainer.transform);
                RectTransform rect = connection.GetComponent<RectTransform>();

                // Position in the middle of the two nodes
                Vector2 midPoint = (position + nextPosition) / 2;
                rect.anchoredPosition = midPoint;

                // Set width to match distance between nodes
                float distance = Vector2.Distance(position, nextPosition);
                rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);

                // Rotate the line to match the angle between nodes
                float angle = Mathf.Atan2(nextPosition.y - position.y, nextPosition.x - position.x) * Mathf.Rad2Deg;
                rect.rotation = Quaternion.Euler(0, 0, angle);

            }
        }

        // Start from the first level and build the branching paths
        Vector2 startPosition = IsLinearPath(story.levels[0]) ? new Vector2(-600f, -100f) : new Vector2(-600f, 100f);
        CreateNodes(story.levels[0], startPosition, 0);
    }

    private bool IsLinearPath(LevelDefinition levelDefinition)
    {
        int totalBranches = 0;

        void CountBranches(LevelDefinition currentLevel)
        {
            if(currentLevel.NextLevels.Count>1)
                totalBranches++;

            foreach (var next in currentLevel.NextLevels)
                CountBranches(next);
        }

        CountBranches(levelDefinition);
        return totalBranches == 0;
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

    private IEnumerator PulseEffect(Transform obj, float scaleAmount = 1.2f, float duration = 0.5f)
    {
        Vector3 originalScale = obj.localScale;
        Vector3 targetScale = originalScale * scaleAmount;

        while (true)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                obj.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                yield return null;
            }

            elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                obj.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
                yield return null;
            }
        }
    }

}