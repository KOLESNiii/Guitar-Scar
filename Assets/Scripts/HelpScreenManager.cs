using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Class for managing the help screen
public class HelpScreenManager : MonoBehaviour
{
    //UI elements assigned in unity editor
    [SerializeField]
    GameObject Text;
    private TextMeshProUGUI text;
    [SerializeField]
    GameObject Moving;
    [SerializeField]
    GameObject Attacking;
    [SerializeField]
    GameObject Blocking;
    [SerializeField]
    GameObject Damage;
    [SerializeField]
    GameObject AttackingExtra;
    private int selectedText = 0;
    public void Start() //Sets text to the movement help text as default
    {
        text = Text.GetComponent<TextMeshProUGUI>();
        UpdateText();
    }
    //Logic for toggling the help text
    public void ToggleMoving(bool newValue) //Triggered when toggle state is changed
    {
        if (Moving.GetComponent<UnityEngine.UI.Toggle>().isOn) //If the toggle is on, set the selected text to the movement help text
        {
            Debug.Log("Moving");
            selectedText = 0;
            UpdateText();
        }
    }
    //The following functions are the same as the above, but for different help text
    public void ToggleAttacking(bool newValue)
    {
        if (Attacking.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Attacking");
            selectedText = 1;
            UpdateText();
        }
    }
    public void ToggleDamage(bool newValue)
    {
        if (Damage.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Damage");
            selectedText = 2;
            UpdateText();
        }
    }
    public void ToggleAttackingExtra(bool newValue)
    {
        if (AttackingExtra.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Attacking Extra");
            selectedText = 3;
            UpdateText();
        }
    }
    public void ToggleBlocking(bool newValue)
    {
        if (Blocking.GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            Debug.Log("Blocking");
            selectedText = 4;
            UpdateText();
        }
    }
    //Updates the help text based on the selected text
    public void UpdateText()  
    {
        if (selectedText == 0)
        {
            text.text = "Use the arrow keys to move your character around the map.<br>Upon coming close to an enemy, you will be entered into a battle with it.<br>Reach the exit portal to save the game and move to the next level.";   
        }
        else if (selectedText == 1) 
        {
            text.text = "Use a guitar to attack. Play the correct chords to deal damage to the enemy.<br>The enemy will also attack you, so be careful!<br>The major chords you can use are: C, D flat, D, E flat, E, F, G, A<br>You can also use the relative minor chords of the major chords above.";
        }
        else if (selectedText == 2)
        {
            text.text = "Any damage you take will be dealt to your armour first.<br>This will be repaired at the end of each battle, but if it is broken, you will take damage directly to your health instead.<br>Health can only be restored by levelling up.";
        }
        else if (selectedText == 3)
        {
            text.text = "Each enemy is strong and weak against different chords, so be sure to experiment with different combinations and use the best ones.<br>Be careful, the enemy will adapt to your attacks and take less damage if you use the same attack several times in a row.";
        }
        else 
        {
            text.text = "You and the enemy can both block attacks.<br>You can block the enemy's attacks by playing the relative minor or major chord of the chord the enemy is attacking with.<br>If an attack is blocked, no damage is taken.";
        }
    }
}
