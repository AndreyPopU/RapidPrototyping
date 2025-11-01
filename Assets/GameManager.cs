using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Block> lastBlocks = new List<Block>();

    public bool gameStarted;
    public bool canDrop;
    public int towerIndex;
    public int lifes;
    public int score;
    public int color;
    public GameObject blockPrefab;
    public GameObject currentBlock;
    public Transform swingParent;
    public Transform clawPosition;
    public Transform cameraFocus;

    [Header("UI")]
    public GameObject mainMenu;
    public GameObject retryMenu;
    public GameObject creditsMenu;
    public TextMeshProUGUI lifesText;
    public TextMeshProUGUI scoreText;

    // only needed if using Lerp
    public float minValue = -3;
    public float maxValue = 3;

    public float speed = 2;

    private float startPosition = 0;

    private void Awake() => instance = this;

    void Update()
    {
        if (!gameStarted || lifes <= 0) return;

        float swingForce = startPosition + Mathf.Sin(Time.time * speed) * .1f * towerIndex;
        swingParent.position = new Vector3(swingForce, 0, 0);

        if (Input.GetKeyDown(KeyCode.Space) && currentBlock != null && canDrop)
        {
            currentBlock.GetComponent<Block>().DropBlock();
            currentBlock = null;
        }

        float value = Mathf.PingPong(Time.time / speed, 6) - 3;
        transform.position = new Vector3(value, transform.position.y, transform.position.z);
    }

    public void SpawnNewBlock(int towerIncrease)
    {
        if (towerIndex >= 20)
        {
            print("Game won!");
            clawPosition.GetComponent<SwingAndBob>().GetOut(Vector3.up * 5 * towerIndex);

            return;
        }

        if (lifes <= 0) return;
        towerIndex += towerIncrease;
        currentBlock = Instantiate(blockPrefab, clawPosition.position, clawPosition.rotation, clawPosition);
        currentBlock.GetComponent<Block>().index = towerIndex;
        clawPosition.GetComponent<SwingAndBob>().RaiseTo(Vector3.up * 2.75f * towerIndex);
        cameraFocus.position = Vector3.up * 2.75f * towerIndex + Vector3.up;

        // Every 5th block - change color
        if (towerIndex % 5 == 0) color = Random.Range(0, 4);

        canDrop = true;
    }

    public void Retry() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void Quit() => Application.Quit();

    public void StartGame()
    {
        gameStarted = true;
        canDrop = true;
    }

    public void LoseLife()
    {
        lifes--;
        lifesText.text = lifes.ToString();

        if (lifes <= 0) retryMenu.SetActive(true);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }
}
