using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    private static GameObject EnemyHealthBar;
    private static GameObject PlayerHealthBar;
    private static GameObject PlayerHealthBarExtra;
    private static GameObject PlayerArmourBar;
    private static GameObject PlayerArmourBarExtra;
    private static GameObject EnemyAttackGO;
    private static Animator EnemyBlockAnimator;
    private static Animator PlayerBlockAnimator;
    [SerializeField]
    private GameObject[] gameObjects;
    private static Sprite[] sprites;
    [SerializeField]
    private Sprite[] Sprites;
    private static Vector3 OriginalPlayerPosition;
    private static Vector3 OriginalPlayerScale;
    private static Player player;
    private static Enemy enemy;
    void Start()
    {
        sprites = Sprites;
    }
    //Assigning UI elements to the player and enemy to be accessed dynamically during battle
    public static void AssignBars()
    {
        player.armourBar = PlayerArmourBar.GetComponent<HealthBar>();
        player.armourBar.isPlayer = true;
        player.healthBar = PlayerHealthBar.GetComponent<HealthBar>();
        player.healthBar.isPlayer = true;
        player.healthBarExtra = PlayerHealthBarExtra.GetComponent<ExtraBar>();
        player.armourBarExtra = PlayerArmourBarExtra.GetComponent<ExtraBar>();
        enemy.healthBar = EnemyHealthBar.GetComponent<HealthBar>();
        enemy.healthBar.isPlayer = false;
        enemy.attackTextBox = EnemyAttackGO.GetComponent<TextMeshProUGUI>();
        enemy.BlockAnimator = EnemyBlockAnimator;
        player.BlockAnimator = PlayerBlockAnimator;
        player.takeDamage(0); //update health bars
        enemy.takeDamage(0);
    }

    //dynamically generates the battle scene
    public static void makeBattleScene(int[] environmentInts, GameObject player, GameObject enemy)
    {
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive); //load battle scene on top of dungeon scene
        Scene battleScene = SceneManager.GetSceneByName("BattleScene");
        GameObject battleBackground = new GameObject("BattleBackground"); //create battle background
        SpriteRenderer spriteRenderer = battleBackground.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "BattleBackground";
        int environmentInt = environmentInts[Random.Range(0, environmentInts.Length)]; //randomly choose an environment out of choices given
        spriteRenderer.sprite = sprites[environmentInt];
        battleBackground.transform.position = new Vector3(-0.57f, 0, 0); //set position and scale of battle background to fit screen
        battleBackground.transform.localScale = new Vector3(9, 9, 1);
        SceneManager.MoveGameObjectToScene(battleBackground, battleScene); //move battle background to battle scene, so can all be deleted together later
        OriginalPlayerPosition = player.transform.position; //save original player position and scale, to be restored later
        OriginalPlayerScale = player.transform.localScale; 
        SceneManager.MoveGameObjectToScene(player, battleScene); //move player and enemy to battle scene
        SceneManager.MoveGameObjectToScene(enemy, battleScene);
        player.transform.position = new Vector3(-6, -2, 0); //set position and scale of player and enemy, to be placed properly in scene
        enemy.transform.position = new Vector3(4, 2, 0);
        player.transform.localScale = new Vector3(4, 4, 1);
        enemy.transform.localScale = new Vector3(-3, 3, 1);
        BattleUIManager.player = player.GetComponent<Player>(); //assign player and enemy to static variables, to be accessed later
        BattleUIManager.enemy = enemy.GetComponent<Enemy>();
    }

    //loads the rest of the battle scene after the battle UI is loaded

    public static void loadRestOfBattleScene()
    {
        Debug.Log("Starting assignment");
        PlayerHealthBar = GameObject.Find("BattleUICanvas/Player/HealthBar/Health"); //find all UI elements
        PlayerHealthBarExtra = GameObject.Find("BattleUICanvas/Player/HealthBar/Extra");
        PlayerArmourBar = GameObject.Find("BattleUICanvas/Player/ArmourBar/Armour");
        PlayerArmourBarExtra = GameObject.Find("BattleUICanvas/Player/ArmourBar/Extra");
        EnemyHealthBar = GameObject.Find("BattleUICanvas/Enemy/HealthBar/Health");
        EnemyAttackGO = GameObject.Find("BattleUICanvas/Enemy/Attacks/Attack");
        EnemyBlockAnimator = GameObject.Find("BattleUICanvas/Enemy/BlockSplash").GetComponent<Animator>();
        PlayerBlockAnimator = GameObject.Find("BattleUICanvas/Player/BlockSplash").GetComponent<Animator>();
        BattleUIManager.AssignBars();
    }
    //closes the battle scene and restores the player to the dungeon scene
    public static void closeBattleScene(GameObject player)
    {
        Scene mainDungeon = SceneManager.GetSceneByName("Dungeon");
        SceneManager.MoveGameObjectToScene(player, mainDungeon);
        DontDestroyOnLoad(player); //don't destroy player when another dungeon scene is unloaded
        SceneManager.UnloadSceneAsync("BattleScene");
        player.transform.position = OriginalPlayerPosition; //restore player position and scale
        player.transform.localScale = OriginalPlayerScale;
    }

    //closes the battle scene without restoring the player to the dungeon scene, so deletes the player, used for game over
    public static void closeBattleScene()
    {
        SceneManager.UnloadSceneAsync("BattleScene");
    }
}
