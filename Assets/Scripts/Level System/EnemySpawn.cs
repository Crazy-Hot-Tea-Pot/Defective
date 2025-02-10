using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public EnemyManager.EnemyType enemyType;
    public string enemyName;

    public GameObject GetEnemyPrefab()
    {
        Debug.Log("Fetching prefab for "+ enemyType+": "+ enemyName);

        return EnemyManager.Instance.GetEnemyPrefab(enemyType);
    }
}