using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
    public EnemyManager.EnemyType enemyType;
    public string enemyName;

    public GameObject GetEnemyPrefab()
    {
        return EnemyManager.Instance.GetEnemyPrefab(enemyType);
    }
}