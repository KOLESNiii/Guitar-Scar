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
        player.takeDamage(0);
        enemy.takeDamage(0);
    }

    public static void makeBattleScene(int[] environmentInts, GameObject player, GameObject enemy)
    {
        SceneManager.LoadScene("BattleScene", LoadSceneMode.Additive);
        Scene battleScene = SceneManager.GetSceneByName("BattleScene");
        GameObject battleBackground = new GameObject("BattleBackground");
        SpriteRenderer spriteRenderer = battleBackground.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingLayerName = "BattleBackground";
        int environmentInt = environmentInts[Random.Range(0, environmentInts.Length)];
        spriteRenderer.sprite = sprites[environmentInt];
        battleBackground.transform.position = new Vector3(-0.57f, 0, 0);
        battleBackground.transform.localScale = new Vector3(9, 9, 1);
        SceneManager.MoveGameObjectToScene(battleBackground, battleScene);
        OriginalPlayerPosition = player.transform.position;
        OriginalPlayerScale = player.transform.localScale; 
        SceneManager.MoveGameObjectToScene(player, battleScene);
        SceneManager.MoveGameObjectToScene(enemy, battleScene);
        player.transform.position = new Vector3(-6, -2, 0);
        enemy.transform.position = new Vector3(4, 2, 0);
        player.transform.localScale = new Vector3(4, 4, 1);
        enemy.transform.localScale = new Vector3(-3, 3, 1);
        BattleUIManager.player = player.GetComponent<Player>();
        BattleUIManager.enemy = enemy.GetComponent<Enemy>();
    }

    public static void loadRestOfBattleScene()
    {
        Debug.Log("Starting assignment");
        PlayerHealthBar = GameObject.Find("BattleUICanvas/Player/HealthBar/Health");
        Debug.Log(PlayerHealthBar.transform.position);
        PlayerHealthBarExtra = GameObject.Find("BattleUICanvas/Player/HealthBar/Extra");
        PlayerArmourBar = GameObject.Find("BattleUICanvas/Player/ArmourBar/Armour");
        PlayerArmourBarExtra = GameObject.Find("BattleUICanvas/Player/ArmourBar/Extra");
        EnemyHealthBar = GameObject.Find("BattleUICanvas/Enemy/HealthBar/Health");
        EnemyAttackGO = GameObject.Find("BattleUICanvas/Enemy/Attacks/Attack");
        EnemyBlockAnimator = GameObject.Find("BattleUICanvas/Enemy/BlockSplash").GetComponent<Animator>();
        PlayerBlockAnimator = GameObject.Find("BattleUICanvas/Player/BlockSplash").GetComponent<Animator>();
        BattleUIManager.AssignBars();
    }

    public static void closeBattleScene(GameObject player)
    {
        Scene mainDungeon = SceneManager.GetSceneByName("Dungeon");
        SceneManager.MoveGameObjectToScene(player, mainDungeon);
        DontDestroyOnLoad(player);
        SceneManager.UnloadSceneAsync("BattleScene");
        player.transform.position = OriginalPlayerPosition;
        player.transform.localScale = OriginalPlayerScale;
    }

    public static void closeBattleScene()
    {
        SceneManager.UnloadSceneAsync("BattleScene");
    }

}
