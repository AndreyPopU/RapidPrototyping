using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Block> lastBlocks = new List<Block>();

    public int towerIndex;
    public GameObject blockPrefab;
    public GameObject currentBlock;
    public Transform clawPosition;

    // only needed if using Lerp
    public float minValue = -3;
    public float maxValue = 3;

    public float speed = 2;

    private void Awake() => instance = this;

    private void Start() => SpawnNewBlock(0);

    void Update()
    {
        var value = Mathf.PingPong(Time.time / speed, 6) - 3;
        transform.position = new Vector3(value, transform.position.y, transform.position.z);

        if (Input.GetKeyDown(KeyCode.Space) && currentBlock != null)
        {
            currentBlock.GetComponent<Block>().DropBlock();
            currentBlock = null;
        }
    }

    public void SpawnNewBlock(int towerIncrease)
    {
        towerIndex += towerIncrease;
        currentBlock = Instantiate(blockPrefab, transform.position, Quaternion.identity);
        currentBlock.transform.SetParent(transform);
        currentBlock.GetComponent<Block>().index = towerIndex;
        clawPosition.position = Vector3.up * 2.5f * towerIndex;
    }
}
