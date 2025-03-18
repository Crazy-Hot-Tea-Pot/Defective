using TMPro.EditorUtilities;
using UnityEngine;

public class SecurityDrone : Enemy
{
    [Header("Custom for Enemy type")]

    public SoundFX NeutralizeSound;
    public SoundFX RamSound;
    public SoundFX AlertSound;

    [SerializeField]
    private int intentsPerformed;

    [SerializeField]
    private int numberOfAlertDrones;

    [SerializeField]
    private bool isAlertDrone;

    private int nextIntentRoll;

    private int ramDamage;
    private int neutralizeDamage;
    private bool alertEnabled;

    /// <summary>
    /// Amount of Intents done.
    /// </summary>
    public int IntentsPerformed
    {
        get
        {
            return intentsPerformed;
        }
        private set
        {
            intentsPerformed = value;
        }
    }

    public int NumberOfAlertDrones
    {
        get
        {
            return numberOfAlertDrones;
        }
        private set
        {
            numberOfAlertDrones = value;
        }
    }
    /// <summary>
    /// If this is a drone thats been summoned by Alert.
    /// </summary>
    public bool IsAlertDrone
    {
        get
        {
            return isAlertDrone;
        }
        private set
        {
            isAlertDrone = value;
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// Any custom drop put in here.
    /// </summary>
    public override void Start()
    {
        if (EnemyName == null)
            EnemyName = "Security Drone";

        DroppedChips.Clear();

        //Add Common Chips Todrop
        var tempChips = ChipManager.Instance.GetChipsByRarity(NewChip.ChipRarity.Common);
        int tempRandom = Random.Range(1,tempChips.Count);
        DroppedChips.Add(tempChips[tempRandom]);

        EnemyType = EnemyManager.TypeOfEnemies.GangLeader;

        base.Start();
    }
    public override void CombatStart()
    {
        nextIntentRoll = Random.Range(1, 11);
        base.CombatStart();
    }

    public override void EndTurn()
    {
        nextIntentRoll = Random.Range(1, 11);

        base.EndTurn();
    }

    /// <summary>
    /// set difficulty for security bot
    /// </summary>
    protected override void SetUpEnemy()
    {
        base.SetUpEnemy();

        switch (Difficulty)
        {
            case EnemyDifficulty.None:
                Debug.LogError("Enemy difficulty not set.");
                break;
            case EnemyDifficulty.Easy:
                MaxHp = 30;
                ramDamage = 10;
                neutralizeDamage = 5;
                alertEnabled = false;
                break;
            case EnemyDifficulty.Medium:
                MaxHp = 45;
                ramDamage = 10;
                neutralizeDamage = 7;
                alertEnabled = false;
                break;
            case EnemyDifficulty.Hard:
                MaxHp = 60;
                ramDamage = 12;
                neutralizeDamage = 7;
                alertEnabled = true;
                break;
            case EnemyDifficulty.Boss:
                break;
            default:
                Debug.LogError("Enemy difficulty not set.");
                break;
        }

        CurrentHP = MaxHp;
    }
    protected override void PerformIntent()
    {
        base.PerformIntent();

        if (IntentsPerformed > 5 && alertEnabled && NumberOfAlertDrones < 3)
        {
            //Alert();
            Animator.SetTrigger("Intent 3");    
        }
        else
        {
            if (nextIntentRoll <= 3)
            {
                Animator.SetTrigger("Intent 1");
                //Neutralize();

            }
            else
            {
                Animator.SetTrigger("Intent 2");
                //Ram();            
            }
        }

        IntentsPerformed++;               
    }
    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (IntentsPerformed > 5 && NumberOfAlertDrones < 3)
        {
            return ("Alert", IntentType.Unique, 0);
        }
        else if (nextIntentRoll <= 3)
        {
            return ("Neutralize", IntentType.Attack, neutralizeDamage);
        }
        else
        {
            return ("Ram", IntentType.Attack, ramDamage);
        }
    }

    /// <summary>
    /// Deals 12 Damage.
    /// Has a 70% chance of being called.
    /// </summary>
    public void Ram()
    {
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(ramDamage);
        SoundManager.PlayFXSound(RamSound);
    }
    /// <summary>
    /// Deals 7 Damage.
    /// Applys Drain.
    /// Has a 30% chance of being called.
    /// </summary>
    public void Neutralize()
    {
        // Play Sound
        SoundManager.PlayFXSound(NeutralizeSound);

        Debug.Log(this.gameObject.name + " is Neutralizing.");

        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(neutralizeDamage);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Drained, 1);
    }
    /// <summary>
    /// Calls another Security Drone to the Combat Zone.
    /// Try to spawn one near by safely.
    /// </summary>
    public void Alert()
    {
        if (NumberOfAlertDrones < 3 && alertEnabled)
        {
            if (CombatController != null && CombatController.CombatArea != null)
            {
                // Loop to find a clear position for the new drone
                BoxCollider areaCollider = CombatController.CombatArea.GetComponent<BoxCollider>();

                if (areaCollider == null)
                {
                    Debug.LogWarning("CombatArea does not have a BoxCollider component.");
                    return;
                }

                Vector3 areaCenter = areaCollider.bounds.center;
                Vector3 areaSize = areaCollider.bounds.size;
                Vector3 spawnPosition;
                int maxAttempts = 20;
                int attempt = 0;
                bool foundValidPosition = false;

                do
                {
                    // Generate a random position within the CombatArea
                    float x = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
                    float z = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);
                    spawnPosition = new Vector3(x, areaCenter.y, z);

                    // Define a LayerMask that only includes Player (6) and Enemy (8)
                    LayerMask collisionMask = (1 << 6) | (1 << 8);

                    // Check if the position is clear
                    if (Physics.OverlapSphere(spawnPosition, 0.5f, collisionMask).Length == 0)
                    {
                        foundValidPosition = true;
                    }

                    attempt++;

                } while (!foundValidPosition && attempt < maxAttempts);

                if (foundValidPosition)
                {
                    GameObject additionalDrone = Instantiate(EnemyManager.Instance.GetEnemyPrefab(EnemyManager.TypeOfEnemies.SecurityDrone), spawnPosition, Quaternion.identity);

                    //Set up alert drone
                    additionalDrone.GetComponent<SecurityDrone>().EnemyName = "Alert Drone";
                    additionalDrone.GetComponent<SecurityDrone>().IsAlertDrone = true;
                    additionalDrone.GetComponent<SecurityDrone>().IntentsPerformed = 0;
                    additionalDrone.GetComponent<SecurityDrone>().Difficulty = EnemyDifficulty.Easy;

                    CombatController.AddEnemyToCombat(additionalDrone);
                    NumberOfAlertDrones++;

                    SoundManager.PlayFXSound(AlertSound);
                }
                else
                {
                    Debug.LogWarning("Failed to find valid spawn position Jayce -_- fix ya code.\n This is complicated I know but thats what testing is for.");
                }
            }
            else
                Debug.LogError("CombatZone Missing!!");
        }        
    }

    [ContextMenu("Force Next Intent Alert")]
    private void ForceAlertIntent()
    {
        // Ensures Alert condition is met
        IntentsPerformed = 6;
        // Any value greater than 3 to avoid Neutralize
        nextIntentRoll = 10;
        Debug.Log("Next Intent is set to ALERT.");
    }

}
