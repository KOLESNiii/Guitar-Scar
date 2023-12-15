using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<EnemyType> PossibleEnemyTypes;
    public bool isHardEnemy = false;
    public EnemyTypeManager enemyTypeManager;


    public void SpawnEnemy()
    {
        int numEnemies = Random.Range(2, 6);
        for (int i = 0; i < numEnemies; i++)
        {
            EnemyType enemyType = PossibleEnemyTypes[Random.Range(0, PossibleEnemyTypes.Count)];
            GameObject enemy = Instantiate(enemyTypeManager.getEnemy(enemyType), transform.position, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.type = enemyType;
            enemyScript.isHardEnemy = isHardEnemy;
        }
        Destroy(gameObject);
    }
}
