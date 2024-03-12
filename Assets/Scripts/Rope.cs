using UnityEngine;
using System.Collections;
public class Rope : MonoBehaviour
{
    //private bool hitTilemapMask = false;
    private float timer = 0f;
    public float lifetime = 1.7f;

    public float colissionLifetime = 0.5f;
    private Collider2D pookaCollider;
    private GameObject player;
    private GameObject enemyHit;
    private bool inColission = false;
    public float maxDistance = 0.5f; 
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator.GetInteger("inflate")==0)
        {

            //Invoke("DestroyRope", 0.5f);
        }
    }
    void DestroyRope()
    {
        // Destruir este objeto enemigo
        //Destroy(gameObject);
    }
    private void Update()
    {
        // Actualizar el temporizador
        timer += Time.deltaTime;

        // Si el temporizador supera el tiempo de vida, destruir la cuerda
        if (timer >= lifetime && !inColission)
        {
            Destroy(gameObject);
        }

        if (player != null && Vector2.Distance(player.transform.position, transform.position) > maxDistance)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            inColission = true;
            enemyHit = collision.gameObject;

            // paralizar enemigo
            //Rigidbody2D enemyRB = enemyHit.GetComponent<Rigidbody2D>();
            Animator enemyAnimator = enemyHit.GetComponent<Animator>();
            if (enemyHit != null)
            {
                //Debug.Log("se supone que me paralizo");
                //enemyRB.velocity = Vector2.zero;
                enemyAnimator.SetBool("stopEnemy", true);
            }
            else
            {
                Debug.Log("no pude detenerlo");
            }

            //activar modo inflar al player
            if (player != null)
            {
                Animator playerAnimator = player.GetComponent<Animator>();
                if (playerAnimator != null)
                {
                    playerAnimator.SetInteger("inflate", 1);
                }
            }
            //Destroy(collision.gameObject); // Destruir el enemigo
            //Destroy(gameObject, colissionLifetime); // Destruir la cuerda después de medio segundo
        }
        else if (collision.gameObject.GetComponent<TilemapMask>() != null)
        {
            inColission = true;
            //hitTilemapMask = true; // Indicar que la cuerda ha chocado con el TilemapMask
            StartCoroutine(DelayedDisableAttack()); // Llamar al método DelayedDisableAttack con un pequeño retraso
        }
    }

    // Método para desactivar el ataque del jugador con un pequeño retraso
    private IEnumerator DelayedDisableAttack()
    {
        yield return new WaitForSeconds(0.1f); // Esperar 0.1 segundos
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.DisableAttack();
        }
        Destroy(gameObject); // Destruir la cuerda
    }

}
