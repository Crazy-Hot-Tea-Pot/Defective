using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleRange : MonoBehaviour
{
    private float currentHealth;
    public float maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PuzzleManager.Instance.OpenPuzzle(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PuzzleManager.Instance.ClosePuzzle(this.gameObject);
        }
    }

    public void TakeDamage(int number)
    {
        currentHealth = currentHealth - number;
        if (currentHealth <= 0)
        {
            DestroyMe();
        }
    }

    public void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
