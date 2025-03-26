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
    public int totalScrapCollected;

    private float fastestCombatTime = Mathf.Infinity;
    private float combatStartTime;
    private float highestDamageDealt;
    private static GameStatsTracker instance;

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

        if (duration < FastestCombatTime)
        {
            FastestCombatTime = duration;
        }
    }
    public void ReportDamage(float damage)
    {
        if (damage > highestDamageDealt)
        {
            HighestDamageDealt = damage;
        }
    }
    public void AddScrap(int scrap)
    {
        totalScrapCollected += scrap;
    }
}
