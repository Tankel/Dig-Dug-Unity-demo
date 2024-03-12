using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private float movementX;
    private float movementY;

    public float normalSpeed = 9f; // Velocidad normal del jugador
    public float diggingSpeed = 3f; // Velocidad del jugador mientras cava
    private float currentSpeed; // Velocidad actual del jugador
    private Animator animator;
    private bool isOnSurface = false;
    private bool isAttacking = false;
    private bool wasWalking = false;
    private float maxHeight;
    private bool canDig = true;
    public float digCooldown = 0.5f; // Tiempo de espera antes de poder cavar nuevamente
    private bool isDigging = false;
    private bool isAlive = true;
    public float ropeSpeed = 0.3f;
    public float ropeDuration = 0.3f;
    public float ropeStopDelay = 0.5f;
    public GameObject ropePrefab; 
    private GameObject currentRope; 
    private int inflateState = 0; // Contador para el estado de inflado
    private bool isInflating = false; // Booleano para el estado de inflar
    private Coroutine decreaseInflateStateCoroutine; 

    public GameObject live_1; 
    public GameObject live_2; 

    public AudioSource movementAudio; // Referencia al AudioSource
    private bool isMoving = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        maxHeight = rb.transform.position.y;
        animator = GetComponent<Animator>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (!isAlive) {return; }
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        wasWalking = Mathf.Abs(moveHorizontal) > 0 || Mathf.Abs(moveVertical) > 0;
        isMoving = wasWalking;
        animator.SetBool("walking", wasWalking);
        bool isAtMaxHeight = rb.transform.position.y >= maxHeight;

        if (isAtMaxHeight && moveVertical > 0)
        {
            moveVertical = 0;
        }

        if (wasWalking)
        {
            if (moveHorizontal > 0) animator.SetInteger("direction", 0);
            if (moveVertical > 0 && !isOnSurface) animator.SetInteger("direction", 1);
            if (moveHorizontal < 0) animator.SetInteger("direction", 2);
            if (moveVertical < 0) animator.SetInteger("direction", 3);
        }
        else
        {
            isDigging = false;
            animator.SetBool("digging", false);
        }

        if (!isAttacking)
        {
            movementX = moveHorizontal;
            movementY = moveVertical;
        }
        if (isDigging)
        {
            currentSpeed = diggingSpeed;
        }
        else
        {
            currentSpeed = normalSpeed;
        }
        if (animator.GetInteger("inflate")  == 8)
        {
            Invoke("WalkAgain", 0.1f);
            inflateState = 0;
            //animator.SetBool("inflate", isInflating);
        }
        else if (animator.GetInteger("inflate") > 0)
        {
            isInflating = true;
            //animator.SetBool("inflate", isInflating);
        }
        animator.SetInteger("inflate", inflateState);
    }
    void WalkAgain()
    {
        isAttacking = false;
        isInflating = false;
        animator.SetInteger("inflate", 0);
        animator.SetBool("attack", false);

        GameObject objectToDestroy = GameObject.FindGameObjectWithTag("Weapon");
        if (objectToDestroy != null)
        {
            // Destruir el objeto
            Destroy(objectToDestroy);
        }
    }
    void FixedUpdate()
    {
        if (!isAlive) {return; }
        if (animator.GetInteger("inflate") == 0)
        {
            Vector2 movement = new Vector2(movementX, movementY);
            rb.velocity = currentSpeed * movement;
        }
        else
        {
            Debug.Log("No puedo caminar");
            rb.velocity = Vector2.zero;
        }
        if (isMoving)
        {
            if (!movementAudio.isPlaying)
            {
                movementAudio.Play();
            }
        }
        else
        {
            if (movementAudio.isPlaying)
            {
                movementAudio.Pause();
            }
        }
        animator.SetBool("digging", isDigging);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) {return; }
        if (other.gameObject.CompareTag("Surface"))
        {
            isOnSurface = true;
        }
        if (other.gameObject.CompareTag("TileDig"))
        {
            //Debug.Log("Soy un celda cavada");
            Invoke("desactivateDig", 0.2f);
        }
    }

    private void desactivateDig()
    {
        isDigging = false;
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!isAlive) {return; }
        if (other.gameObject.CompareTag("Enemy")) //|| other.gameObject.CompareTag("Rock"))
        {
            if (other.gameObject.GetComponent<Animator>().GetInteger("inflate_state") == 0)
            {
                Die();
            }
        }
        else if (other.gameObject.GetComponent<TilemapMask>() != null)
        {
            if (wasWalking && canDig)
            {
                Dig(other);
                canDig = false; // Desactivar la capacidad de cavar temporalmente
                StartCoroutine(ResetDigCooldown());
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!isAlive) {return; }
        if (other.gameObject.GetComponent<TilemapMask>() != null)
        {
            if (wasWalking && canDig)
            {
                Dig(other);
                canDig = false; // Desactivar la capacidad de cavar temporalmente
                StartCoroutine(ResetDigCooldown());
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<TilemapMask>() != null)
        {
            // Restablecer el estado de cavado
            //isDigging = false;
        }
    }

    private IEnumerator ResetDigCooldown()
    {
        yield return new WaitForSeconds(digCooldown);
        canDig = true; // Permitir cavar nuevamente después de que haya pasado el tiempo de espera
    }

    private void Dig(Collision2D collision)
    {
        if (!isAlive) {return; }
        TilemapMask tilemapMask = collision.gameObject.GetComponent<TilemapMask>();

        if (tilemapMask != null)
        {
            int direction = animator.GetInteger("direction");
            Vector3Int positionInt = new Vector3Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), 0);

            if (isOnSurface && direction != 3)
            {
                //Debug.Log("Solo puedes cavar para abajo!");
                return;
            }
            //animator.SetBool("digging", true);
            isDigging = true;
            tilemapMask.ChangeMask(positionInt, direction);
        }
        else
        {
            //Debug.Log("El objeto TilemapMask es nulo.");
        }
    }
public void Die()
{
    isAlive = false;
    animator.SetBool("die", true);
    GlobalVariables.DecreaseLives();

    if (GlobalVariables.lives == 2)
    {
        Destroy(live_2);
    }
    else if (GlobalVariables.lives == 1)
    {
        Destroy(live_1);
    }
    // Pausar el juego
    if (GlobalVariables.lives <= 0)
    {
        Destroy(gameObject);
        // Aquí puedes añadir código para el final del juego o reinicio total del nivel
    }
    else
    {
        Invoke("DieDelay", 1f); 
    }
}

    void DieDelay()
    {
        gameObject.SetActive(false);
        Invoke("ResetPlayerAfterDelay", 2f);
        //Time.timeScale = 0; 
    }

    void ResetPlayerAfterDelay()
    {
        inflateState = 0;
        animator.SetInteger("direction",0);
        animator.SetInteger("inflate",0);
        animator.SetBool("walking",false);
        animator.SetBool("attack",false);
        animator.SetBool("digging",false);
        animator.SetBool("inflate_state",false);
        isInflating = false;
        isDigging = false;
        isAttacking = false;
        isOnSurface = true;

        animator.SetBool("die", false); // Asumiendo que "die" es el nombre del parámetro del animator que controla la animación de muerte
        Debug.Log("Se va a reiniciar el jugador");
        //Time.timeScale = 1; // Reanudar el juego
        gameObject.SetActive(true); // Reactivar el GameObject del jugador
        isAlive = true;
        transform.position = new Vector3(0, 3.94f, 0); // Resetea la posición del jugador
        // Aquí puedes añadir más líneas para resetear otras propiedades del jugador si es necesario

        //Time.timeScale = 1; 
    }
    void OnAttack(InputValue inputValue)
    {
        if (!isAlive) { return; }
        if (isInflating)
        {
            //Debug.Log("INFLAR FUAAAA");
            isAttacking = true;
            animator.SetBool("attack", isAttacking);
            inflateState++;
            animator.SetInteger("inflate", inflateState);
            animator.SetBool("inflate_state", !animator.GetBool("inflate_state")); // Alternar el estado de inflado

            // Iniciar la corrutina solo si no está activa
            if (decreaseInflateStateCoroutine == null)
            {
                decreaseInflateStateCoroutine = StartCoroutine(DecreaseInflateState());
            }
        }
        else if (!isAttacking)
        {
            isAttacking = true;
            animator.SetBool("attack", isAttacking);
            animator.SetBool("walking", wasWalking);
            movementX = 0;
            movementY = 0;
            Invoke("DisableAttack", ropeDuration);

            LaunchRope();
        }
        if (inflateState >= 8)
        {
            isAttacking = false;
            isInflating = false;
            animator.SetBool("attack", isAttacking);
            inflateState = 0; // Reiniciar el contador de inflado
        }
    }


    public void DisableAttack()
    {
        isAttacking = false;
        animator.SetBool("attack", isAttacking);
    }
    void LaunchRope()
    {
        if (currentRope != null)
        {
            Destroy(currentRope);
        }

        int direction = animator.GetInteger("direction");
        Vector2 launchDirection = Vector2.zero;
        float angleZ = 0f;

        switch (direction)
        {
            case 0: // Derecha
                launchDirection = Vector2.right;
                angleZ = 0f;
                break;
            case 1: // Arriba
                launchDirection = Vector2.up;
                angleZ = 90f;
                break;
            case 2: // Izquierda
                launchDirection = Vector2.left;
                angleZ = 180f; 
                break;
            case 3: // Abajo
                launchDirection = Vector2.down;
                angleZ = -90f; 
                break;
            default:
                break;
        }

        GameObject ropeInstance = Instantiate(ropePrefab, transform.position, Quaternion.Euler(0f, 0f, angleZ));
        Rigidbody2D ropeRigidbody = ropeInstance.GetComponent<Rigidbody2D>();
        ropeRigidbody.velocity = launchDirection * ropeSpeed; 

        currentRope = ropeInstance;
        StartCoroutine(StopRopeMovement(ropeRigidbody, ropeStopDelay));
    }

    private IEnumerator StopRopeMovement(Rigidbody2D ropeRigidbody, float stopDelay)
    {
        // Esperar el tiempo especificado antes de detener el movimiento
        yield return new WaitForSeconds(stopDelay);
        
        // Verificar si el componente Rigidbody2D aún existe antes de acceder a él
        if (ropeRigidbody != null)
        {
            // Detener el movimiento de la cuerda
            ropeRigidbody.simulated  = false;
            ropeRigidbody.velocity = Vector2.zero;
        }
    }

    private IEnumerator DestroyRopeWithDelay(GameObject rope, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(rope);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isAlive) {return; }
        if (other.gameObject.CompareTag("Surface"))
        {
            isOnSurface = false;
            //isDigging = false;
        }
    }
    private IEnumerator DecreaseInflateState()
    {
        yield return new WaitForSeconds(1.0f); // Esperar 1 segundo antes de empezar a disminuir inflateState

        // Bucle para disminuir inflateState mientras sea mayor que 0
        while (inflateState > 0)
        {
            inflateState--; // Disminuir inflateState en 1
            if (inflateState == 0)
            {
                WalkAgain();
            }
            animator.SetInteger("inflate", inflateState); // Actualizar el valor en el animador
            yield return new WaitForSeconds(1.0f); // Esperar 1 segundo antes de la siguiente disminución
        }

        // Reiniciar la referencia a la corrutina
        decreaseInflateStateCoroutine = null;
    }
}
