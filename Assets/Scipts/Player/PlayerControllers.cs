using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine;

public class PlayerControllers : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private Sprite playerSprite;

    public float moveSpeed = 5.0f;
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask grassLayer;
    public LayerMask fovLayer;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainersView;

    private bool isMoving;
    private Vector2 input;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving )
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                Vector3 targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space))
            Interact();
    }

    private IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;

        CheckForEncounters();
        CheckIfInTrainersView();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        return Physics2D.OverlapCircle(targetPos, 0.1f, solidObjectsLayer | interactableLayer) == null;
    }

    private void Interact()
    {
        Vector3 facingDir = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        Vector3 interactPos = transform.position + facingDir;

        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null && UnityEngine.Random.Range(1, 101) <= 10)
        {
            OnEncountered?.Invoke();
        }
    }

    private void CheckIfInTrainersView()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.2f, fovLayer);
        if (collider != null)
        {
            OnEnterTrainersView?.Invoke(collider);
        }
    }

    public string Name => playerName;
    public Sprite Sprite => playerSprite;
}
