using UnityEngine;

public class SecurityDrone : Enemy
{
    [Header("Custom for Enemy type")]

    public GameObject AdditionalDrone;

    [SerializeField]
    private int intentsPerformed;

    [SerializeField]
    private int numberOfAlertDrones;

    [SerializeField]
    private bool isAlertDrone;

    private int nextIntentRoll;

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
    public override void PerformIntentTrigger(string intentName)
    {
        base.PerformIntentTrigger(intentName);

        switch (intentName) {
            case "Ram":
                Ram();
                break;
            case "Neutralize":
                Neutralize(); 
            break;
            case "Alert":
                Alert();
                break;
            default:
                Debug.LogWarning($"Intent '{intentName}' not handled in {EnemyName}.");
                break;

        }

    }
    protected override void PerformIntent()
    {

        if (IntentsPerformed > 5 && NumberOfAlertDrones < 3)
            Animator.SetTrigger("Intent 3");
            //Alert();
        else
        {
            if (nextIntentRoll <= 3)
                Animator.SetTrigger("Intent 1");
            //Neutralize();
            else
                Animator.SetTrigger("Intent 2");
                //Ram();            
        }

        IntentsPerformed++;

        //THIS IS NEEDED DON'T REMOVE. don't want that again.
        base.PerformIntent();

    }
    protected override (string intentText, IntentType intentType, int value) GetNextIntent()
    {
        if (IntentsPerformed > 5 && NumberOfAlertDrones < 3)
        {
            return ("Alert", IntentType.Unique, 0);
        }
        else if (nextIntentRoll <= 3)
        {
            return ("Neutralize", IntentType.Attack, 7);
        }
        else
        {
            return ("Ram", IntentType.Attack, 12);
        }
    }

    /// <summary>
    /// Deals 12 Damage.
    /// Has a 70% chance of being called.
    /// </summary>
    private void Ram()
    {
        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(12);
    }
    /// <summary>
    /// Deals 7 Damage.
    /// Applys Drain.
    /// Has a 30% chance of being called.
    /// </summary>
    private void Neutralize()
    {
        // Play Sound
        SoundManager.PlayFXSound(SoundFX.NeutralizeSecurityDrone,this.gameObject.transform);

        Debug.Log(this.gameObject.name + " is Neutralizing.");

        EnemyTarget.GetComponent<PlayerController>().DamagePlayerBy(7);
        EnemyTarget.GetComponent<PlayerController>().AddEffect(Effects.Debuff.Drained, 1);
    }
    /// <summary>
    /// Calls another Security Drone to the Combat Zone.
    /// Try to spawn one near by safely.
    /// </summary>
    private void Alert()
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

                //Leaving this here for in the future if this intent give problems again.
                //Collider[] hitColliders = Physics.OverlapSphere(spawnPosition, 0.5f);

                //if (hitColliders.Length > 0)
                //{
                //    foreach (var col in hitColliders)
                //    {
                //        Debug.Log($"Overlap detected with: {col.gameObject.name} on Layer: {LayerMask.LayerToName(col.gameObject.layer)} at {spawnPosition}");
                //    }
                //}

                attempt++;

            } while (!foundValidPosition && attempt < maxAttempts);

            if (foundValidPosition)
            {
                GameObject additionalDrone = Instantiate(AdditionalDrone, spawnPosition, Quaternion.identity);
                additionalDrone.GetComponent<SecurityDrone>().IAmAlertDrone();

                CombatController.AddEnemyToCombat(additionalDrone);
                NumberOfAlertDrones++;

                SoundManager.PlayFXSound(SoundFX.AlertSecurityDrone);
            }
            else
            {
                Debug.LogWarning("Failed to find valid spawn position Jayce -_- fix ya code.\n This is complicated I know but thats what testing is for.");
            }
        }
        else
            Debug.LogError("CombatZone Missing!!");
    }
    /// <summary>
    /// Different stuff for Alerted Drone
    /// </summary>
    public void IAmAlertDrone()
    {
        maxHP = 60;
        EnemyName = "Alert Drone";
        IsAlertDrone = true;
        IntentsPerformed = 0;

        base.Initialize();
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
