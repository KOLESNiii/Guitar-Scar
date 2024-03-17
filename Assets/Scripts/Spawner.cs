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
            SpriteRenderer enemySprite = enemy.GetComponent<SpriteRenderer>();
            if (isHardEnemy) //Hard enemy colour
            {
                if (Global.colourblindMode == 0) //Normal hard enemy colour
                {
                    enemySprite.color = new Color(1f, 0, 1f, 1f); 
                }
                else if (Global.colourblindMode == 1) //Protanopia & deuteranopia
                {
                    enemySprite.color = new Color(0.819f, 0, 1f, 1f); //set colourblind mode 1
                }
                else if (Global.colourblindMode == 2) //Tritanopia
                {
                    enemySprite.color = new Color(1f, 0.286f, 0.420f, 1f);
                }
            }
            else //Normal enemy colour
            {
                if (Global.colourblindMode == 1) //Protanopia & deuteranopia
                {
                    enemySprite.color = new Color(0.392f, 0.631f, 1f, 1f); //set colourblind mode 1
                }
                else if (Global.colourblindMode == 2) //Tritanopia
                {
                    enemySprite.color = new Color(0.741f, 0.439f, 0.980f, 1f); //set colourblind mode 2
                }
            }
        }
        Destroy(gameObject); //destroy spawner when finished spawning
    }
}
