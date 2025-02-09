
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SecurityDrone : Enemy
{
    [Header("Custom for Enemy type")]

    [SerializeField]
    private int intentsPerformed;

    [SerializeField]
    private int numberOfAlertDrones;

    [SerializeField]
    private bool isAlertDrone;

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
    protected override void PerformIntent()
    {
        IntentsPerformed++;

        if(IntentsPerformed > 5 && NumberOfAlertDrones < 3)
            Alert();
        else
        {            
            if(Random.Range(1,11) <= 3)
                Neutralize();
            else
                Ram();
        }

       //THIS IS NEEDED DON'T REMOVE. don't want that again.
       base.PerformIntent();

    }
    protected override Intent GetNextIntent()
    {
        if (IntentsPerformed > 5 && NumberOfAlertDrones < 3)
        {
            return new Intent("Alert", Color.blue, 0, "Summons another Security Drone");
        }
        else if (Random.Range(1, 11) <= 3)
        {
            return new Intent("Neutralize", Color.red, 7, "Deals damage and applies Drained");
        }
        else
        {
            return new Intent("Ram", Color.red, 12, "Deals heavy damage");
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
            int maxAttempts = 10;
            int attempt = 0;
            bool foundValidPosition = false;

            do
            {
                // Generate a random position within the CombatArea
                float x = Random.Range(areaCenter.x - areaSize.x / 2, areaCenter.x + areaSize.x / 2);
                float z = Random.Range(areaCenter.z - areaSize.z / 2, areaCenter.z + areaSize.z / 2);
                spawnPosition = new Vector3(x, areaCenter.y, z);

                // Check if the position is clear
                if (Physics.OverlapSphere(spawnPosition, 1f).Length == 0)
                {
                    foundValidPosition = true;
                }

                attempt++;

            } while (!foundValidPosition && attempt < maxAttempts);

            if (foundValidPosition)
            {
                GameObject additionalDrone = Instantiate(this.gameObject, spawnPosition, Quaternion.identity);
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
}
