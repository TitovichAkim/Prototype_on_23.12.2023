using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PortraitInTheQueue : MonoBehaviour
{
    [Header("SetInInspector")]
    public Image characterIcon;
    public TMP_Text initiativeNumber;

    [Header("SetDynamically")]
    private Character _character;

    public Character character
    {

        get
        {
            return (_character);
        }
        set
        {
            _character = value;
            ApplyInitialSettings();
        }
    }

    public void ApplyInitialSettings ()
    {
        characterIcon.sprite = character.characterSO.characterIcon;
        ApplyCurrentSettings();
    }

    public void ApplyCurrentSettings ()
    {
        initiativeNumber.text = character.initiative.ToString();
    }
}
