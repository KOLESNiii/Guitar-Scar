using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Spawner class, used to spawn enemies
public class Spawner : MonoBehaviour
{
    public List<EnemyType> PossibleEnemyTypes;
    public bool isHardEnemy = false;
    public EnemyTypeManager enemyTypeManager;


    public void SpawnEnemy()
    {
        int numEnemies = Random.Range(2, 6); //random number of enemies between 2 and 5
        for (int i = 0; i < numEnemies; i++) 
        {
            EnemyType enemyType = PossibleEnemyTypes[Random.Range(0, PossibleEnemyTypes.Count)]; //random enemy type from possible enemy types
            //Contruct enemy
            GameObject enemy = Instantiate(enemyTypeManager.getEnemy(enemyType), transform.position, Quaternion.identity);
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.type = enemyType; //set enemy type
            enemyScript.isHardEnemy = isHardEnemy; //set enemy difficulty
        }
        Destroy(gameObject); //destroy spawner when finished spawning
    }
}
