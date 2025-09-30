using UnityEngine;

public class Block : MonoBehaviour
{
    public bool placed;
    public int index;
    public ParticleSystem perfectPlacement;
    public ParticleSystem missedPlacement;

    private Rigidbody2D rb;

    public void DropBlock()
    {
        transform.SetParent(null);
        rb = gameObject.AddComponent<Rigidbody2D>();
    }

    public void LaunchBlock(float displacementDifference)
    {
        int force = 3;
        if (displacementDifference < 0) force = -3; // Left

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(new Vector2(force, 3), ForceMode2D.Impulse);
        rb.AddTorque(-force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (placed) return;

        placed = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (collision.tag != "Block")
        {
            print("Game Over");
            CameraManager.instance.Shake(.1f, .1f);
            return;
        }

        if (collision.TryGetComponent(out Block block))
        {
            float surfaceDifference = Mathf.Abs(transform.position.x - block.transform.position.x);

            if (surfaceDifference < .1f) // Perfect placement
            {
                // Play effect and snap
                transform.position = new Vector3(block.transform.position.x, transform.position.y, 0);
                perfectPlacement.Play();
            }

            if (surfaceDifference > .5f) // Block falls off
            {
                CameraManager.instance.Shake(.1f, .1f);
                LaunchBlock(transform.position.x - block.transform.position.x);
                GameManager.instance.SpawnNewBlock(0);
                Destroy(this);
                return;
            }

            if (block.index != index - 1) // If block you've collided with isn't the previous block - knock all blocks above it
            {
                int blocksKnockedOver = -1;

                print("Game Over");
                CameraManager.instance.Shake(.1f, .1f);
                GameManager.instance.SpawnNewBlock(blocksKnockedOver);
                return;
            }
        }

        GameManager.instance.SpawnNewBlock(1);
    }
}
