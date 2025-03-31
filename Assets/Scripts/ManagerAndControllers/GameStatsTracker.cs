using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsTracker : MonoBehaviour
{
    public static GameStatsTracker Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public float SessionStartTime
    {
        get
        {
            return sessionStartTime;
        }
        set
        {
            sessionStartTime = value;
        }
    }
    public float SessionPlayTime
    {
        get
        {
            return sessionPlayTime;
        }
        private set
        {
            sessionPlayTime = value;
        }
    }
    public float FastestCombatTime
    {
        get
        {
            return fastestCombatTime;
        }
        private set
        {
            fastestCombatTime = value;
        }
    }
    public float HighestDamageDealt
    {
        get
        {
            return highestDamageDealt;
        }
        private set
        {
            highestDamageDealt = value;
        }
    }
    public int TotalScrapCollected
    {
        get
        {
            return totalScrapCollected;
        }
        private set
        {
            totalScrapCollected = value;
        }
    }
    public List<NewChip> TotalChipsCollected
    {
        get
        {
            return totalChipsCollected;
        }
        set
        {
            totalChipsCollected = value;
        }
    }

    private int totalScrapCollected;
    private float sessionPlayTime=0f;
    private float fastestCombatTime = Mathf.Infinity;
    private float combatStartTime;
    private float highestDamageDealt;
    private List<NewChip> totalChipsCollected= new List<NewChip>();
    private static GameStatsTracker instance;
    private float sessionStartTime = 0;

    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Start Combat Timer
    /// </summary>
    public void StartCombatTimer()
    {
        combatStartTime = Time.time;
    }
    /// <summary>
    /// End Combat Timer and Saves if fastest
    /// </summary>
    public void EndCombatTimer()
    {
        float duration = Time.time - combatStartTime;

        duration = Mathf.Round(duration * 100f) / 100f;

        if (duration < FastestCombatTime)
        {
            FastestCombatTime = duration;
        }
    }
    /// <summary>
    /// Report damage done amount for highest damage
    /// </summary>
    /// <param name="damage"></param>
    public void ReportDamage(float damage)
    {
        if (damage > highestDamageDealt)
        {
            HighestDamageDealt = damage;
        }
    }
    /// <summary>
    /// Add scrap to totale scrap collected
    /// </summary>
    /// <param name="scrap"></param>
    public void AddScrap(int scrap)
    {
        TotalScrapCollected += scrap;
    }    
    /// <summary>
    /// Starting time for game
    /// </summary>
    public void StartSession()
    {
        SessionStartTime = Time.time;
    }
    /// <summary>
    /// Track time for the current session
    /// </summary>
    public void UpdatePlayTime()
    {
        if (SessionStartTime > 0)
        {            
            SessionPlayTime = Time.time - SessionStartTime;
        }
    }
    [ContextMenu("Generate Dummy Data")]
    public void GenerateDummyData()
    {
        Debug.Log("Generating dummy data for testing...");

        // Generate random dummy data for testing
        FastestCombatTime = UnityEngine.Random.Range(10f, 120f); // Random combat time between 10 and 120 seconds
        HighestDamageDealt = UnityEngine.Random.Range(50f, 500f); // Random damage between 50 and 500
        TotalScrapCollected = UnityEngine.Random.Range(100, 1000); // Random scrap between 100 and 1000

        // Simulate a completion time
        SessionStartTime = Time.time - UnityEngine.Random.Range(600f, 3600f); // Random session from 10 minutes to 1 hour ago

        Debug.Log("Dummy data generated!");
    }

}
