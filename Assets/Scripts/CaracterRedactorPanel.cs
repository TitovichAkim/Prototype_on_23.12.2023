using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CaracterRedactorPanel:MonoBehaviour
{
    public Image characterIcon;
    public Transform effectsPanel;
    private Character _character;

    public TMP_InputField characterNameIF;
    public TMP_InputField enduranceIF;
    public TMP_InputField currentEduranceIF;
    public TMP_InputField healthIF;
    public TMP_InputField currentHealthIF;
    public TMP_InputField manaIF;
    public TMP_InputField currentManaIF;
    public TMP_InputField speedIF;
    public TMP_InputField movementPointsIF;
    public TMP_InputField attackPowerIF;
    public TMP_InputField attackRangeIF;
    public TMP_InputField initiativeIF;

    public TMP_Text characterNamePH;
    public TMP_Text endurancePH;
    public TMP_Text currentEdurancePH;
    public TMP_Text healthPH;
    public TMP_Text currentHealthPH;
    public TMP_Text manaPH;
    public TMP_Text currentManaPH;
    public TMP_Text speedPH;
    public TMP_Text movementPointsPH;
    public TMP_Text attackPowerPH;
    public TMP_Text attackRangePH;
    public TMP_Text initiativePH;


    public Image currentHealthProgressBar;
    public Image currentManaProgressBar;
    public Character character
    {
        get
        {
            return (_character);
        }
        set
        {
            _character = value;
            UpdateTextFields();
        }
    }
    private void Start ()
    {
        OpenPanel(GameManager.currentGameState == GameManager.GameState.LevelRedactor);
        currentHealthProgressBar.fillAmount = character.originCurrentHealth / character.health;
        currentManaProgressBar.fillAmount = character.currentMana / character.mana;
    }
    public void OpenPanel (bool redacting)
    {
        characterNameIF.interactable = redacting;
        enduranceIF.interactable = redacting;
        currentEduranceIF.interactable = redacting;
        healthIF.interactable = redacting;
        currentHealthIF.interactable = redacting;
        manaIF.interactable = redacting;
        currentManaIF.interactable = redacting;
        speedIF.interactable = redacting;
        movementPointsIF.interactable = redacting;
        attackPowerIF.interactable = redacting;
        attackRangeIF.interactable = redacting;
        initiativeIF.interactable = redacting;
        if (redacting)
        {
            speedIF.gameObject.GetComponent<Image>().color = Color.white;
            attackPowerIF.gameObject.GetComponent<Image>().color = Color.white;
            attackRangeIF.gameObject.GetComponent<Image>().color = Color.white;
            initiativeIF.gameObject.GetComponent<Image>().color = Color.white;
        }
        else
        {
            speedIF.gameObject.GetComponent<Image>().color =  new Color(1,1,1,0);
            attackPowerIF.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            attackRangeIF.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            initiativeIF.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }
    public void UpdateTextFields ()
    {
        characterNamePH.text = character.characterName;
        endurancePH.text = character.endurance.ToString();
        currentEdurancePH.text = character.currentEdurance.ToString();
        healthPH.text = character.health.ToString();
        currentHealthPH.text = character.originCurrentHealth.ToString();
        manaPH.text = character.mana.ToString();
        currentManaPH.text = character.currentMana.ToString();
        speedPH.text = character.speed.ToString();
        //movementPointsPH.text = character.movementPoints.ToString();
        attackPowerPH.text = character.attackPower.ToString();
        attackRangePH.text = character.attackRange.ToString();
        initiativePH.text = character.initiative.ToString();
    }
    public void GetCharacterName ()
    {
        character.characterName = characterNameIF.text;
    }
    public void GetEndurance (float value)
    {
        if(value == 0)
        {
            character.endurance = float.Parse(enduranceIF.text);
        }
        else
        {
            character.endurance += value;
            enduranceIF.text = character.endurance.ToString();
        }
    }
    public void GetCurrentEdurance (float value)
    {
        if(value == 0)
        {
            character.currentEdurance = float.Parse(currentEduranceIF.text);
        }
        else
        {
            character.currentEdurance += value;
            currentEduranceIF.text = character.currentEdurance.ToString();
        }
    }
    public void GetHealth (float value)
    {
        if(value == 0)
        {
            character.health = float.Parse(healthIF.text);
        }
        else
        {
            character.health += value;
            healthIF.text = character.health.ToString();
        }
    }
    public void GetCurrentHealth (float value)
    {
        if(value == 0)
        {
            character.originCurrentHealth = float.Parse(currentHealthIF.text);

        }
        else
        {
            character.originCurrentHealth += value;
            currentHealthIF.text = character.originCurrentHealth.ToString();
        }
    }
    public void GetMana (float value)
    {
        if(value == 0)
        {
            character.mana = float.Parse(manaIF.text);

        }
        else
        {
            character.mana += value;
            manaIF.text = character.mana.ToString();
        }
    }
    public void GetCurrentMana (float value)
    {
        if(value == 0)
        {
            character.currentMana = float.Parse(currentManaIF.text);

        }
        else
        {
            character.currentMana += value;
            currentManaIF.text = character.currentMana.ToString();
        }
    }
    public void GetSpeed (float value)
    {
        if(value == 0)
        {
            character.speed = float.Parse(speedIF.text);
        }
        else
        {
            character.speed += value;
            speedIF.text = character.speed.ToString();
        }
    }
    public void GetMovementPoints (float value)
    {
        if(value == 0)
        {
            character.movementPoints = float.Parse(movementPointsIF.text);

        }
        else
        {
            character.movementPoints += value;
            movementPointsIF.text = character.movementPoints.ToString();
        }
    }
    public void GetAttackPower (float value)
    {
        if(value == 0)
        {
            character.attackPower = float.Parse(attackPowerIF.text);

        }
        else
        {
            character.attackPower += value;
            attackPowerIF.text = character.attackPower.ToString();
        }
    }
    public void GetAttackRange (float value)
    {
        if(value == 0)
        {
            character.attackRange = float.Parse(attackRangeIF.text);
        }
        else
        {
            character.attackRange += value;
            attackRangeIF.text = character.attackRange.ToString();
        }
    }
    public void GetInitiative (float value)
    {
        if(value == 0)
        {
            character.initiative = float.Parse(initiativeIF.text);
        }
        else
        {
            character.initiative += value;
            initiativeIF.text = character.initiative.ToString();
        }
    }

    public void DestroyCharacterRedactor ()
    {
        LevelRedactor.redactingCharacter = null;
        character.caracterRedactorPanel = null;
        Destroy(this.gameObject);
    }
}
