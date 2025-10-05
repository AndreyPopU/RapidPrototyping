using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Block> lastBlocks = new List<Block>();

    public bool gameStarted;
    public int towerIndex;
    public int lifes;
    public int color;
    public GameObject blockPrefab;
    public GameObject currentBlock;
    public Transform swingParent;
    public Transform clawPosition;

    [Header("UI")]
    public GameObject mainMenu;
    public GameObject retryMenu;
    public GameObject creditsMenu;
    public TextMeshProUGUI lifesText;

    // only needed if using Lerp
    public float minValue = -3;
    public float maxValue = 3;

    public float speed = 2;

    private float startPosition = 0;

    private void Awake() => instance = this;

    private void Start()
    {
    }

    void Update()
    {
        if (!gameStarted || lifes <= 0) return;

        float swingForce = startPosition + Mathf.Sin(Time.time * speed) * .1f * towerIndex;

        float value = Mathf.PingPong(Time.time / speed, 6) - 3;
        transform.position = new Vector3(value, transform.position.y, transform.position.z);

        if (Input.GetKeyDown(KeyCode.Space) && currentBlock != null)
        {
            currentBlock.GetComponent<Block>().DropBlock();
            currentBlock = null;
        }

        swingParent.position = new Vector3(swingForce, 0, 0);
    }

    public void SpawnNewBlock(int towerIncrease)
    {
        if (lifes <= 0) return;
        towerIndex += towerIncrease;
        currentBlock = Instantiate(blockPrefab, transform.position, Quaternion.identity);
        currentBlock.transform.SetParent(transform);
        currentBlock.GetComponent<Block>().index = towerIndex;
        clawPosition.position = Vector3.up * 2.75f * towerIndex;

        // Every 5th block - change color
        if (towerIndex % 5 == 0) color = Random.Range(0, 4);
    }

    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void Quit() => Application.Quit();

    public void StartGame() => gameStarted = true;

    public void LoseLife()
    {
        lifes--;
        lifesText.text = lifes.ToString();

        if (lifes <= 0) retryMenu.SetActive(true);
    }
}
