using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerController : MonoBehaviour, Interactable
{
    [SerializeField] string name; 
    [SerializeField] Sprite sprite; 
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterBattle; 
    [SerializeField] GameObject exclamation;  
    [SerializeField] GameObject fov; 

    bool battleLost = false;  

    Character character;

    private void Awake()
    {
        character = GetComponent<Character>();  
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);  // Initialisation de la rotation du champ de vision
    }

    private void Update()
    {
        character.HandleUpdate();  // Mise à jour des animations et du positionnement
    }

    // Gère l'interaction avec le joueur
    public void Interact(Transform initiator)
    {
        character.LookTowards(initiator.position);  // Le dresseur regarde vers le joueur

        if (!battleLost)
        {
            // Lance un dialogue et un combat si le dresseur n'a pas encore perdu
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
            {
                GameController.Instance.StartTrainerBattle(this);
            }));
        }
        else
        {
            // Affiche un dialogue différent si le dresseur a déjà perdu
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterBattle));
        }
    }

    // Déclencheur pour commencer un combat de dresseur
    public IEnumerator TriggerTrainerBattle(PlayerControllers player)
    {
        exclamation.SetActive(true);  // Affichage de l'icône d'exclamation
        yield return new WaitForSeconds(0.8f);
        exclamation.SetActive(false);

        // Calcul du déplacement vers le joueur et déclenchement du combat
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Affichage du dialogue avant le début du combat
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog, () =>
        {
            GameController.Instance.StartTrainerBattle(this);
        }));
    }

    // Marque le dresseur comme ayant perdu le combat
    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);  // Désactive le champ de vision après la défaite
    }

    // Régle la rotation du champ de vision selon la direction
    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        switch (dir)
        {
            case FacingDirection.Right:
                angle = 90f;
                break;
            case FacingDirection.Up:
                angle = 180f;
                break;
            case FacingDirection.Left:
                angle = 270;
                break;
        }
        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public string Name
    {
        get => name;  // Accesseur pour le nom du dresseur
    }

    public Sprite Sprite
    {
        get => sprite;  // Accesseur pour le sprite du dresseur
    }
}
