using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public EnemyManager.TypeOfEnemies enemyType;
    public string enemyName;
    public Enemy.EnemyDifficulty difficulty=Enemy.EnemyDifficulty.Medium;

    public GameObject GetEnemyPrefab()
    {
        Debug.Log("Fetching prefab for "+ enemyType+": "+ enemyName);

        return EnemyManager.Instance.GetEnemyPrefab(enemyType);
    }
}