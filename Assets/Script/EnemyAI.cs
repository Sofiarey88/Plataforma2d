using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Detección y timing")]
    [Tooltip("Segundos en estado idle (pero seguirá detectando al player)")]
    public float idleDuration = 2f;
    [Tooltip("Radio de detección del player")]
    public float detectionRadius = 4f;
    [Tooltip("Tag usado por el player")]
    public string playerTag = "Player";

    [Header("Movimiento")]
    [Tooltip("Velocidad al perseguir al player")]
    public float chaseSpeed = 3f;

    [Header("Animator")]
    [Tooltip("Nombre del parámetro bool que activa la animación de run (por ejemplo 'enMovim' o 'isRunning')")]
    public string runParam = "enMovim";

    Animator anim;
    Rigidbody2D rb;
    Transform player;
    float idleTimer;
    bool isRunning;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        var p = GameObject.FindGameObjectWithTag(playerTag);
        player = p != null ? p.transform : null;
        idleTimer = 0f;
        isRunning = false;

        // Asegura estado inicial en animador (idle)
        if (anim != null) anim.SetBool(runParam, false);
    }

    void Update()
    {
        idleTimer += Time.deltaTime;

        if (player == null)
        {
            // intentar recuperar el player si fue null al Start
            var p = GameObject.FindGameObjectWithTag(playerTag);
            player = p != null ? p.transform : null;
            return;
        }

        // Detectar player en rango: si detecta, iniciar chase inmediatamente
        float dist = Vector2.Distance(transform.position, player.position);
        if (!isRunning && dist <= detectionRadius)
        {
            StartChase();
        }

        // si quieres que siempre espere al menos idleDuration antes de comenzar a patrullar
        // mantendremos idle visualmente, pero detection activa ya permite iniciar chase inmediato.
        // No se hace nada especial aquí si no se detectó.
    }

    void FixedUpdate()
    {
        if (!isRunning || player == null) return;

        // Mover hacia el player en el eje X
        Vector2 dir = (player.position - transform.position);
        float sign = Mathf.Sign(dir.x);
        rb.linearVelocity = new Vector2(sign * chaseSpeed, rb.linearVelocity.y);

        // Opcional: voltear sprite por escala X
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * sign;
        transform.localScale = s;
    }

    void StartChase()
    {
        isRunning = true;
        if (anim != null) anim.SetBool(runParam, true);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}