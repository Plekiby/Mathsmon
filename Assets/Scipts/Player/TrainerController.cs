using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour
{
    [SerializeField] Dialog dialog;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;

    Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        // Show Exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            Debug.Log("Starting Trainer Battle");
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right)
            angle = 90f;
        else if (dir == FacingDirection.Up)
            angle = 180f;
        else if (dir == FacingDirection.Left)
            angle = 270;

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
