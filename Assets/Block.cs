using UnityEngine;

public class Block : MonoBehaviour
{
    public bool dropped;
    public bool placed;
    public int index;
    public ParticleSystem perfectPlacement;
    public ParticleSystem missedPlacement;

    private Rigidbody2D rb;

    public void DropBlock()
    {
        dropped = true;
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
        Destroy(GetComponent<BoxCollider2D>());
        Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (placed || !dropped) return;

        placed = true;

        if (rb == null) print(collision.gameObject.name);
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

            if (surfaceDifference < .25f) // Perfect placement
            {
                // Play effect and snap
                transform.position = new Vector3(block.transform.position.x, transform.position.y, 0);
                perfectPlacement.Play();
            }

            if (block.index != index - 1) // If block you've collided with isn't the previous block - knock all blocks above it
            {
                Debug.Log($"Previous block is: {block.index} and current block is {index}");
                int blocksKnockedOver = 0;

                for (int i = GameManager.instance.lastBlocks.Count - 1; i >= 0; i--)
                {
                    if (GameManager.instance.lastBlocks[i].index == block.index) break;

                    //Debug.Log($"Launching block {GameManager.instance.lastBlocks[i].index}");
                    GameManager.instance.lastBlocks[i].LaunchBlock(surfaceDifference);
                    blocksKnockedOver++;
                }

                Debug.Log($"Total blocks knocked over: {blocksKnockedOver}, tower index is {GameManager.instance.towerIndex}");

                // Remove knocked over blocks
                int loops = blocksKnockedOver;

                while (loops > 0)
                {
                    //Debug.Log($"Removed block {GameManager.instance.lastBlocks[GameManager.instance.lastBlocks.Count - 1].index}");
                    GameManager.instance.lastBlocks.RemoveAt(GameManager.instance.lastBlocks.Count - 1);
                    loops--;
                }

                LaunchBlock(-surfaceDifference);
                GameManager.instance.lastBlocks.Remove(this);
                GameManager.instance.towerIndex -= blocksKnockedOver; // +1 Because of the block that just fell and
                Debug.Log($"Tower index is {GameManager.instance.towerIndex}");

                CameraManager.instance.Shake(.1f, .1f);
                GameManager.instance.SpawnNewBlock(0);
                return;
            }

            if (surfaceDifference > 2f) // Block falls off
            {
                CameraManager.instance.Shake(.1f, .1f);
                LaunchBlock(transform.position.x - block.transform.position.x);
                GameManager.instance.lastBlocks.Remove(this);
                GameManager.instance.SpawnNewBlock(0);
                Destroy(this);
                return;
            }
        }

        GameManager.instance.lastBlocks.Add(this);
        GameManager.instance.SpawnNewBlock(1);
    }
}
