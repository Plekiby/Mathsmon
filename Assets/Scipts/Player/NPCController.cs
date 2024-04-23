using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;  // Dialogue à afficher lors de l'interaction
    [SerializeField] List<Vector2> movementPattern;  // Modèle de déplacement du NPC
    [SerializeField] float timeBetweenPattern;  // Temps entre chaque changement de pattern de déplacement

    NPCState state;
    float idleTimer = 0f;
    int currentPattern = 0;

    Character character;
    MathsmonGiver itemGiver;
    MathsmonHealer healer;
    Vendeur vendeur;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<MathsmonGiver>();
        healer = GetComponent<MathsmonHealer>();
        vendeur = GetComponent<Vendeur>();
    }

    // Interaction avec le joueur
    public void Interact(Transform initiator)
    {
        if (state == NPCState.Idle)
        {
            StartCoroutine(InteractCoroutine(initiator));
        }
    }

    // Coroutine qui gère l'interaction spécifique basée sur les composants du NPC
    private IEnumerator InteractCoroutine(Transform initiator)
    {
        state = NPCState.Dialog;
        character.LookTowards(initiator.position);

        // Distribution de Pokémon, soins ou vente selon le composant disponible
        if (itemGiver != null && !itemGiver.use())
        {
            yield return itemGiver.GivePokemon(initiator.GetComponent<PlayerControllers>());
        }
        else if (itemGiver == null || itemGiver.use() && (healer == null && vendeur == null))
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
        }
        else if (healer != null)
        {
            healer.HealPokemon(initiator.GetComponent<PlayerControllers>());
            yield return DialogManager.Instance.ShowDialog(dialog);
        }
        else if (vendeur != null)
        {
            vendeur.AddMathsball(initiator.GetComponent<PlayerControllers>());
            yield return DialogManager.Instance.ShowDialog(dialog);
        }
        idleTimer = 0f;
        state = NPCState.Idle;
    }

    // Mise à jour et gestion du modèle de déplacement
    private void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                    StartCoroutine(Walk());
            }
        }

        character.HandleUpdate();
    }

    // Coroutine de déplacement selon le modèle prédéfini
    IEnumerator Walk()
    {
        state = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
            currentPattern = (currentPattern + 1) % movementPattern.Count;

        state = NPCState.Idle;
    }
}

// État possible d'un NPC
public enum NPCState { Idle, Walking, Dialog }