using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D rockCollider;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rockCollider = GetComponent<Collider2D>();
    }
    void Update()
    {
        // Verificar si se hace clic
        if (Input.GetMouseButtonDown(0))
        {
            // Obtener la posición del clic en coordenadas de pantalla
            Vector3 mousePosition = Input.mousePosition;
            // Convertir la posición del clic a coordenadas del mundo
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            // Imprimir las coordenadas del mundo en la consola
            Debug.Log("Coordenadas del mundo: " + worldPosition);
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (animator.GetBool("isTouched") || animator.GetBool("isFalling"))
        {
            Debug.Log("entré aquiiiiiiiii");
            if (other.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("destruir enemigo");
                PookaController pookaController = other.gameObject.GetComponent<PookaController>();
                if (pookaController != null)
                {
                    pookaController.DestroyEnemy();
                }
                //GlobalVariables.DecreaseEnemyCount();
            }
            else if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("destruir player aaaaaaaa");
                PlayerController player = other.gameObject.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.Die();
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void RockTouched()
    {
        Invoke("ChangeToDynamic", 1.43f);
    }
    private void ChangeToDynamic()
    {
        animator.SetBool("isFalling", true);
        rb.bodyType = RigidbodyType2D.Dynamic; // Cambia el tipo de cuerpo a dinámico
        rockCollider.enabled = false;
        Invoke("ReactivateCollider", 0.2f);
        Invoke("BrokeRock", 0.3f);
    }

    private void BrokeRock()
    {
        animator.SetBool("isInGround", true);
        //rb.bodyType = RigidbodyType2D.Static;
        //collider.enabled = false;
        Destroy(gameObject, 2.0f);

    }
    private void ReactivateCollider()
    {
        rockCollider.enabled = true;
    }
}
