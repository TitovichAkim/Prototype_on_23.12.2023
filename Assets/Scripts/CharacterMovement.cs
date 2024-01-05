using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterMovement : MonoBehaviour
{
    [Header("SetInInspector")]
    public Character character;
    public Transform characterTransform;
    public float movementSpeed;
    [Header("SetDynamically")]
    public List<LandscapeCell> targetCells;


    private void Update ()
    {
        if (targetCells.Count > 0)
        {
            Move();
        }
    }

    private void Move ()
    {
        Vector2 charPos = characterTransform.position;
        Vector2 targetPos = targetCells[0].gameObject.transform.position;
        if (Vector2.Distance(charPos, targetPos) > 0.05f)
        {
            charPos += (targetPos - charPos).normalized * movementSpeed * Time.deltaTime;
            characterTransform.position = charPos;
        }
        else
        {
            characterTransform.position = targetPos;
            character.currentLandscapeCell.currentCharacter = null;
            character.currentLandscapeCell = targetCells[0];
            targetCells[0].currentCharacter = character;
            targetCells.RemoveAt(0);
            if (targetCells.Count == 0)
            {
                if (character.currentLandscapeCell.cellState == LandscapeCell.CellState.EnoughPoints)
                {
                    character.movementPoints -= character.currentLandscapeCell.minimumMovementCosts;
                }
                else
                {
                    Debug.Log($"Ќе хватает поинтов, минимальна€ цена перехода = {character.currentLandscapeCell.minimumMovementCosts}, текуща€ выносливость: {character.currentEdurance}");
                    character.currentEdurance -= character.currentLandscapeCell.minimumMovementCosts - character.movementPoints;
                    Debug.Log($"¬ыносливость: {character.currentEdurance}");
                    character.movementPoints = 0;
                }
                character.characterState = Character.CharacterState.Readiness;
                character.gameManager.ChangeCellsStates(LandscapeCell.CellState.Expectation);
            }
        }
    }
}
