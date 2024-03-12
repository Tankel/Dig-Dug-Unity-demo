using UnityEngine;
using UnityEngine.Tilemaps;
public class PookaController : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocidad de movimiento del Pooka
    public float flyingSpeed = 1f; // Velocidad de vuelo del Pooka
    public GameObject player; // Referencia al GameObject del jugador
    private Animator animator;
    private Rigidbody2D rb;
    private bool isFlying = false; // Estado de vuelo del Pooka
    private Vector2 moveDirection = Vector2.up;
    private Collider2D pookaCollider;
    private bool hasBeenInsideTriggerForSomeTime = false;
    private float timeInsideTrigger = 0f;
    public float requiredTimeInsideTrigger = 1.5f;
    public Tilemap tilemap;
    private bool hasBeenDestroyed = false;

    void Start()
    {
        pookaCollider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (animator.GetBool("stopEnemy"))
        {
            //isInflating = true;
            StopEnemy();
            SetPlayerInflateState();
        }
        else if (!isFlying)
        {
            // Movimiento del Pooka
            Move();
        }
        else
        {
            // Vuelo del Pooka
            Fly();
        }
        if (hasBeenInsideTriggerForSomeTime && isFlying)
        {
            isFlying = false;
            animator.SetBool("fly", false);

            // Reactivar el collider y el Rigidbody
            pookaCollider.isTrigger = false;
            //rb.simulated = true;
            //Debug.Log("Ya no voy a volar");
            hasBeenInsideTriggerForSomeTime = false;

            MoveToNearestValidTile();
        }
    }
    void MoveToNearestValidTile()
    {
        // Obtener todas las posiciones de los tiles en el tilemap
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        // Inicializar la posición más cercana y la distancia mínima
        Vector3Int nearestTilePosition = Vector3Int.zero;
        float minDistance = Mathf.Infinity;

        // Iterar sobre todas las posiciones de los tiles en el tilemap
        foreach (var pos in bounds.allPositionsWithin)
        {
            // Obtener el tile en la posición actual
            TileBase tile = tilemap.GetTile(pos);

            // Si el tile no es nulo, calcular la distancia al enemigo
            if (tile != null)
            {
                float distance = Vector3.Distance(transform.position, tilemap.CellToWorld(pos));

                // Si la distancia es menor que la mínima actual, actualizar la posición más cercana y la distancia mínima
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestTilePosition = pos;
                }
            }
        }
        Vector3 targetPosition = tilemap.CellToWorld(nearestTilePosition);
        Debug.Log("movida a la pos " + targetPosition);
        transform.position = new Vector3(targetPosition.x + 0.3f, targetPosition.y + 0.3f, targetPosition.z);

    }
    void Move()
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Mover el Pooka en la dirección determinada
        rb.velocity = moveDirection * moveSpeed;
    }

    void Fly()
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Mover el Pooka en dirección al jugador
        Vector2 playerDirection = (player.transform.position - transform.position).normalized;
        rb.velocity = playerDirection * flyingSpeed;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Si el Pooka choca con un obstáculo, cambiar de dirección o entrar en el estado "fly"
        if (collision.gameObject.CompareTag("Tile"))
        {
            // Si no está volando, determinar si entra en el estado "fly"
            if (!isFlying && Random.value < 0.05f) // Probabilidad de entrar en el estado "fly"
            {
                Debug.Log("Voy a volar");
                isFlying = true;
                animator.SetBool("fly", true);
                rb.velocity = Vector2.zero; // Detener el movimiento

                // Desactivar el collider y el Rigidbody para volar a través de tiles
                pookaCollider.isTrigger = true;
                //rb.simulated = false;

                // Calcular la dirección hacia el jugador para el vuelo
                moveDirection = (player.transform.position - transform.position).normalized;
            }
            else
            {
                // Cambiar de dirección
                moveDirection = (player.transform.position - transform.position).normalized;
                //ChangeDirection();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Debug.Log("soy un tirgger");
        // Si el Pooka está volando y choca con un TileDig, volver al estado normal
        if (isFlying && other.gameObject.CompareTag("Tile"))
        {
            // Iniciar temporizador cuando el Pooka entra en el trigger
            timeInsideTrigger = 0f;
            hasBeenInsideTriggerForSomeTime = false;
        }
    }
    void StopEnemy()
    {
        //Debug.Log("ME DEBO DE DETENER");
        rb.velocity = Vector2.zero;

    }
    void SetPlayerInflateState()
    {
        // Busca el jugador y establece su estado de inflado
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Animator playerAnimator = player.GetComponent<Animator>();
            int inflatedValue = Mathf.CeilToInt((float)playerAnimator.GetInteger("inflate") / 2f);
            if (playerAnimator != null) // Corrección aquí
            {
                animator.SetInteger("inflate_state", inflatedValue);
            }
            if (inflatedValue == 4)
            {
                Debug.Log("llegue a 4");
                //playerAnimator.SetInteger("inflate", 0);
                Invoke("DestroyEnemy", 0.5f);
            }
            Invoke("WalkAgain", 0.5f);
        }
    }
    void WalkAgain()
    {
        if (animator.GetInteger("inflate_state") == 0)
        {
            animator.SetBool("stopEnemy", false);
        }
    }
    public void DestroyEnemy()
    {
        if (!hasBeenDestroyed)
        {
        hasBeenDestroyed = true;
        int scoreToAdd = 77;

        Vector3 enemyPosition = transform.position;
        Collider2D[] colliders = Physics2D.OverlapPointAll(enemyPosition);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Yellow Dirt"))
            {
                scoreToAdd = 100;
                break;
            }
            else if (collider.CompareTag("Orange Dirt"))
            {
                scoreToAdd = 200;
                break;
            }
            else if (collider.CompareTag("Red Dirt"))
            {
                scoreToAdd = 400;
                break;
            }
            else if (collider.CompareTag("Wine Dirt"))
            {
                scoreToAdd = 800;
                break;
            }
        }
        GlobalVariables.IncreaseScore(scoreToAdd);
        // Destruir este objeto enemigo
        Destroy(gameObject);
        GlobalVariables.DecreaseEnemyCount();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Incrementar el temporizador mientras el Pooka está dentro del trigger
        if (isFlying && other.gameObject.CompareTag("Tile"))
        {
            timeInsideTrigger += Time.deltaTime;

            // Verificar si el Pooka ha estado un cierto tiempo dentro del trigger
            if (timeInsideTrigger >= requiredTimeInsideTrigger)
            {
                // Acciones a realizar después de un cierto tiempo dentro del trigger
                hasBeenInsideTriggerForSomeTime = true;
            }
        }
        if (other.gameObject.CompareTag("Surface"))
        {
            Debug.Log("estoy en la superficie aaaaaa");
            Invoke("MoveDown",1f);
        }
    }
    void MoveDown()
    {
        moveDirection = Vector2.down;
        rb.velocity = Vector2.down * moveSpeed;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Si el Pooka sale del trigger, reiniciar el temporizador
        if (isFlying && other.gameObject.CompareTag("Tile"))
        {
            timeInsideTrigger = 0f;
            hasBeenInsideTriggerForSomeTime = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Si el Pooka choca con un obstáculo mientras vuela, cambiar de dirección
        if (!isFlying && collision.gameObject.CompareTag("Tile"))
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        if (animator.GetBool("stopEnemy")) { return; }
        // Generar un nuevo movimiento aleatorio solo en las direcciones permitidas (izquierda, derecha, arriba, abajo)
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0: // Izquierda
                moveDirection = Vector2.left;
                break;
            case 1: // Derecha
                moveDirection = Vector2.right;
                break;
            case 2: // Arriba
                moveDirection = Vector2.up;
                break;
            case 3: // Abajo
                moveDirection = Vector2.down;
                break;
        }
    }
}
