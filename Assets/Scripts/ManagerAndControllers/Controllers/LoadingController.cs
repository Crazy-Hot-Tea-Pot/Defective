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
    private Dictionary<int, float> verticalOffsetTracker = new Dictionary<int, float>();

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

        switch (GameManager.Instance.TargetScene)
        {
            case Levels.Credits:
            case Levels.Win:
            case Levels.Title:
                break;
            default:
                GenerateStoryGraph();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Gradually rotate the image on the Y-axis
        Display.GetComponent<Transform>().Rotate(0f, ChipRotationSpeed * Time.deltaTime, 0f);
    }
    /// <summary>
    /// Generate graph for story progression.
    /// </summary>
    private void GenerateStoryGraph()
    {
        string currentLevelUniqueID = StoryManager.Instance.CurrentLevel.uniqueLevelID;

        // Clear previous graph before generating
        foreach (GameObject obj in nodes)
        {
            Destroy(obj);
        }
        nodes.Clear();

        Story story = StoryManager.Instance.CurrentStory;
        LevelDefinition currentLevel = StoryManager.Instance.CurrentLevel;

        Dictionary<LevelDefinition, Vector2> levelPositions = new Dictionary<LevelDefinition, Vector2>();

        bool isLinear = IsLinearPath(story.levels[0]);

        float spacingX = isLinear ? 200f : 150f;
        float spacingY = isLinear ? 0f : 60f;

        List<(Vector2, Vector2)> connectionPairs = new List<(Vector2, Vector2)>();

        // Step 1: First Create All Nodes
        void CreateNodes(LevelDefinition level, Vector2 position, int depth)
        {
            // Prevent duplicates
            if (levelPositions.ContainsKey(level)) 
                return;

            GameObject node = Instantiate(nodePrefab, graphContainer.transform);
            node.GetComponent<RectTransform>().anchoredPosition = position;
            nodes.Add(node);
            levelPositions[level] = position;

            Image nodeImage = node.GetComponent<Image>();

            bool isLocked = true;
            foreach (var prevLevel in levelPositions.Keys)
            {
                if (prevLevel.NextLevels.Contains(level))
                {
                    isLocked = false;
                    break;
                }
            }

            if (level.uniqueLevelID == currentLevelUniqueID)
            {
                nodeImage.color = currentColor;
                StartCoroutine(PulseEffect(node.transform));
            }
            else if (level.isCompleted)
            {
                nodeImage.color = completedColor;
            }
            else if (isLocked)
            {
                nodeImage.color = lockedColor;
            }
            else
            {
                nodeImage.color = futureColor;
            }

            for (int i = 0; i < level.NextLevels.Count; i++)
            {
                LevelDefinition nextLevel = level.NextLevels[i];

                //Vector2 nextPosition = new Vector2(position.x + spacingX, position.y - (i * spacingY));

                if (!verticalOffsetTracker.ContainsKey(depth + 1))
                    verticalOffsetTracker[depth + 1] = position.y;

                float yOffset = verticalOffsetTracker[depth + 1];
                Vector2 nextPosition = new Vector2(position.x + spacingX, yOffset);

                verticalOffsetTracker[depth + 1] -= spacingY;

                // Store connection pair for later
                connectionPairs.Add((position, nextPosition));

                CreateNodes(nextLevel, nextPosition, depth + 1);
            }
        }

        Vector2 startPosition = isLinear ? new Vector2(-600f, -100f) : new Vector2(-600f, 100f);
        CreateNodes(story.levels[0], startPosition, 0);

        // Step 2: Now Create Connections AFTER Nodes Exist
        foreach (var pair in connectionPairs)
        {
            GameObject connection = Instantiate(connectionPrefab, graphContainer.transform);
            RectTransform rect = connection.GetComponent<RectTransform>();

            Vector2 midPoint = (pair.Item1 + pair.Item2) / 2;
            rect.anchoredPosition = midPoint;

            float distance = Vector2.Distance(pair.Item1, pair.Item2);
            rect.sizeDelta = new Vector2(distance, rect.sizeDelta.y);

            float angle = Mathf.Atan2(pair.Item2.y - pair.Item1.y, pair.Item2.x - pair.Item1.x) * Mathf.Rad2Deg;
            rect.rotation = Quaternion.Euler(0, 0, angle);

            // Move lines behind nodes in the hierarchy
            connection.transform.SetAsFirstSibling();
        }
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

        if (GameManager.Instance.Debugging)
        {
            while (!asyncLoad.isDone)
            {
                ProgressBar.fillAmount = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                if (asyncLoad.progress >= 0.9f)
                    asyncLoad.allowSceneActivation = true;

                yield return null;
            }
        }
        else
        {
            //Fake loading time with progress bar
            float elapsedTime = 0f;
            float FillDuration = 5f;

            while (elapsedTime < FillDuration)
            {
                elapsedTime += Time.deltaTime;
                ProgressBar.fillAmount = Mathf.Lerp(0, 1, elapsedTime / FillDuration);
                yield return null;
            }
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