using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;

    private Animator animator;

    public LayerMask solidObjectLayers;
    public LayerMask interactableLayer;

    public LayerMask battleLayer;
    public AudioSource audiosource;
    public AudioClip audioClip;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            animator.SetFloat("moveX", input.x);
            animator.SetFloat("moveY", input.y);


            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                     StartCoroutine(Move(targetPos));
            }
        }
        //animator.SetBool("isMoving", isMoving); 
        if(Input.GetKeyDown(KeyCode.Z))
            Interact();
    }
    void Interact()
    {
        Debug.Log("Z PRESS 123");
        var facingDir = new Vector3(animator.GetFloat("moveX"),animator.GetFloat("moveY"));
        var interactPos = transform.position + facingDir;
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        Debug.Log(collider);
        if (collider != null)
        {
            //Debug.Log("Z PRESS 123");
            collider.GetComponent<Interactable>()?.Interact();
            //Debug.Log("there is an NPC here!");
        }
    }
    

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        audiosource.clip = audioClip;
        audiosource.Play();
        isMoving = false;

        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectLayers | interactableLayer) != null)
    {
            return false;
           
    }
            return true;
    }

    void CheckForEncounters(){
        if (Physics2D.OverlapCircle(transform.position, 0.01f, battleLayer) != null)
        {
            if (Random.Range(1,100)<=50)
            {
                Debug.Log("Battle has started");
            }
            
        }
    }
       
}

