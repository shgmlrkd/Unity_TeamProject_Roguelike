using UnityEngine;

namespace Bfree
{
    public class Movement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float maxSpeed = 6f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 50f;

        private Rigidbody2D rb;
        private Vector2 moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // typical top-down / no-gravity 2D movement
        }

        private void Update()
        {
            // Raw gives snappy movement for top-down / arcade.
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(x, y);
            if (moveInput.sqrMagnitude > 1f) moveInput.Normalize(); // prevent faster diagonals

            if (Mathf.Abs(moveInput.x) > 0.01f)
            {
                Vector3 s = transform.localScale;
                s.x = Mathf.Sign(moveInput.x) * Mathf.Abs(s.x);
                transform.localScale = s;
            }
        }

        private void FixedUpdate()
        {
            float targetSpeed = maxSpeed;

            Vector2 targetVelocity = moveInput * targetSpeed;

            // Choose accel or decel depending on whether player is trying to move
            float rate = (moveInput.sqrMagnitude > 0.001f) ? acceleration : deceleration;

            rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        }
    }
}