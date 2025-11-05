using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Block> lastBlocks = new List<Block>();

    public int Xoffset = 38;
    public bool gameStarted;
    public bool canDrop;
    public int towerIndex;
    public int buildings;
    public int lifes;
    public int score;
    public int color;
    public GameObject blockPrefab;
    public GameObject currentBlock;
    public Transform swingParent;
    public Transform clawPosition;
    public Transform cameraFocus;
    public GameObject groundFloorPrefab;

    [Header("UI")]
    public GameObject mainMenu;
    public GameObject retryMenu;
    public GameObject winMenu;
    public GameObject creditsMenu;
    public TextMeshProUGUI lifesText;
    public TextMeshProUGUI scoreText;
    public CanvasGroup fadePanel;

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
        if (towerIndex >= 15)
        {
            print("Game won!");
            clawPosition.GetComponent<SwingAndBob>().GetOut(Vector3.up * 5 * towerIndex);
            winMenu.SetActive(true);

            return;
        }

        if (lifes <= 0) return;
        towerIndex += towerIncrease;
        currentBlock = Instantiate(blockPrefab, clawPosition.position, clawPosition.rotation, clawPosition);
        currentBlock.GetComponent<Block>().index = towerIndex;
        clawPosition.GetComponent<SwingAndBob>().RaiseTo(new Vector3(Xoffset * buildings, 2.75f * towerIndex, 0));
        cameraFocus.position = new Vector3(Xoffset * buildings, 2.75f * towerIndex + 1, 0);

        canDrop = true;

        if (towerIndex % 5 == 0) color = Random.Range(0, 4);
    }

    public void NextBlock() => StartCoroutine(NextBlockCo());

    private IEnumerator NextBlockCo()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        // Fade out
        while(fadePanel.alpha < 1)
        {
            fadePanel.alpha += .1f;

            yield return waitForFixedUpdate;
        }

        // Reset camera and claw and tower index and such and offset them to the right a bit
        if (towerIndex >= 15)
        {
            print("tower index has reached the maximum");
            buildings++; // If building is complete
            Instantiate(groundFloorPrefab, new Vector3(Xoffset * buildings, -3, 0), Quaternion.identity);
        }
        else
        {
            for (int i = 0; i < lastBlocks.Count; i++) // Remove entire building
                Destroy(lastBlocks[i].gameObject);
        }

        towerIndex = 0;
        score = 0;
        lifes = 3;
        lifesText.text = lifes.ToString();
        scoreText.text = score.ToString();

        gameStarted = true;
        canDrop = true;

        clawPosition.GetComponent<SwingAndBob>().StopAllCoroutines();
        Vector3 offset = new Vector3(Xoffset * buildings, 3f, 0);
        clawPosition.GetComponent<SwingAndBob>().currentPos = offset;
        clawPosition.transform.position = offset;
        CameraManager.instance.target.position = offset;
        cameraFocus.position = Vector3.right * Xoffset * buildings + Vector3.up * 3 * towerIndex + Vector3.up;
        color = 0;
        SpawnNewBlock(1);
        lastBlocks.Clear();

        for (int i = 0; i < swingParent.childCount; i++)
            Destroy(swingParent.GetChild(i).GetComponent<Block>());
        swingParent.transform.DetachChildren();

        yield return new WaitForSeconds(1);

        // Fade in
        while (fadePanel.alpha > 0)
        {
            fadePanel.alpha -= .1f;

            yield return waitForFixedUpdate;
        }
    }

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

        if (lifes <= 0) StartCoroutine(TraceBuilding());
    }

    private IEnumerator TraceBuilding()
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        while (cameraFocus.position.y > 3)
        {
            cameraFocus.transform.Translate(-Vector3.up * .2f);
            yield return waitForFixedUpdate;
        }

        retryMenu.SetActive(true);
    }

    public void AddScore(int amount)
    {
        score += amount;
        scoreText.text = score.ToString();
    }
}
