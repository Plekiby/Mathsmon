using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;

public class PlayerControllers : MonoBehaviour
{
    [SerializeField] private string playerName;  // Nom du personnage joueur
    [SerializeField] private Sprite playerSprite;  // Sprite du personnage joueur
    [SerializeField] private NewSystem system;
    [SerializeField] private PokemonParty pokemonParty;
    [SerializeField] private PlayerControllers player;

    public float moveSpeed = 5.0f;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;
    public LayerMask fovLayer;

    public event Action OnEncountered;   // Déclenché lors de rencontres aléatoires
    public event Action<Collider2D> OnEnterTrainersView;  // Déclenché lorsque le joueur entre dans le champ de vision d'un dresseur
    private Character character;

    private bool isMoving;
    private int nbMathsball = 10;
    private Vector2 input;
    private Animator animator;  // Composant pour contrôler les animations
    private string name;
   

    private void Awake()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        if (listeners.Length > 1)
        {
            for (int i = 1; i < listeners.Length; i++)
            {
                Destroy(listeners[i]);
            }
        }
        character = GetComponent<Character>();
        animator = GetComponent<Animator>();  // Initialisation du composant Animator
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Éviter le mouvement diagonal
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                // Configuration de l'animation en fonction du mouvement
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                Vector3 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                // Démarrage de la coroutine de mouvement si la cible est accessible
                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        // Interaction avec les objets ou personnages
        if (Input.GetKeyDown(KeyCode.E))
            Interact();
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    /*private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        // Déplacement progressif vers la position cible
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        // Vérification des rencontres et si le joueur est vu par un dresseur
        CheckForEncounters();
        CheckIfInTrainersView();
    }*/

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        animator.SetBool("isMoving", true);

        // Déplacement progressif vers la position cible
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
        animator.SetBool("isMoving", false);

        // Vérification des interactions et déclenchements après le mouvement
        PostMoveCheck();
    }

    private void PostMoveCheck()
    {
        // Vérifier les rencontres potentielles
        CheckForEncounters();
        CheckIfInTrainersView();

        // Vérifier d'autres déclencheurs potentiels
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffsetY), 0.2f, GameLayers.i.TriggerableLayers);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break; // Interrompre la boucle après la première interaction valide pour éviter les multiples déclenchements
            }
        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // Vérifie si la position cible n'est pas bloquée par des objets solides ou interactifs
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) == null;
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3 interactPos = transform.position + facingDir;

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            // Déclenchement de l'interaction
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void CheckForEncounters()
    {
        // Déclenche une rencontre si le joueur marche sur de l'herbe
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null && UnityEngine.Random.Range(1, 101) <= 10)
        {
            OnEncountered?.Invoke();
        }
    }

    private void CheckIfInTrainersView()
    {
        // Déclenche un événement si un dresseur voit le joueur
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, fovLayer);
        if (collider != null)
        {
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name()
    {
        if (system == null)
        {
            return playerName;
        }
        else { 
            playerName = system.name; 
            return playerName;
        }
        return "chef";
    }
    public Sprite Sprite => playerSprite;

    public int getNbMathsball()
    {
        return nbMathsball;
    }

    public void setNbMathsball(int nb)
    {
        nbMathsball = nb;
    }

    public void NbMathsballm()
    {
        nbMathsball--;
    }

    public void NbMathsballp()
    {
        nbMathsball++;
    }
   
    public Character Character => character;



    public void SavePlayer()
    {
        Debug.Log("Sauvegarde");

        SaveSystemTrue.SavePlayer(player, pokemonParty.Pokemons);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystemTrue.LoadPlayer();
        if (data != null)
        {
            Debug.Log("Data loaded successfully.");

            moveSpeed = data.speed;

            if (data.position != null && data.position.Length >= 3)
            {
                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                transform.position = position;
                Debug.Log($"Player position set to {position}");
                SceneManager.LoadScene(data.currentScene);
          
            }
            else
            {
                Debug.LogError("Position data is either null or does not contain 3 elements.");
            }

            pokemonParty.Pokemons.Clear();
            foreach (var pokemonData in data.pokemonTeam)
            {
                // Vous devrez peut-être convertir PlayerData.Pokemon en votre autre type Pokemon, si ce sont des types distincts
                Pokemon pokemon = ConvertPlayerDataPokemonToPokemon(pokemonData);
                pokemonParty.Pokemons.Add(pokemon);
            }
        }
        else
        {
            Debug.LogError("Failed to load data.");
        }
    }
        Pokemon ConvertPlayerDataPokemonToPokemon(PlayerData.Pokemon pokemonData)
        {
            Pokemon pokemon = new Pokemon(pokemonData.Base, pokemonData.Level, pokemonData.CurrentWins,pokemonData.WinsRequiredForNextLevel);
            return pokemon;
        }

}
