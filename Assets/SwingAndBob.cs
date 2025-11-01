using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class SwingAndBob : MonoBehaviour
{
    [Header("Swing Settings")]
    public float baseSwingAmplitude = 10f;     // Degrees of rotation
    public float swingSpeed = 2f;          // Speed of swinging

    [Header("Bob Settings")]
    public float baseBobAmplitude = 0.1f;      // Vertical movement range
    public float bobSpeed = 2f;            // Speed of bobbing

    public Vector3 currentPos;
    private float timeOffset;

    // Current smoothed amplitudes
    private float currentSwingAmplitude;
    private float currentBobAmplitude;

    void Start()
    {
        currentPos = transform.position;
        timeOffset = Random.Range(0f, Mathf.PI * 2); // Randomize phase for variation
    }

    void Update()
    {
        // Target amplitudes (based on towerIndex)
        float targetSwingAmplitude = baseSwingAmplitude * GameManager.instance.towerIndex / 10;
        float targetBobAmplitude = baseBobAmplitude * GameManager.instance.towerIndex / 10;

        // Smoothly interpolate to avoid snapping
        currentSwingAmplitude = Mathf.Lerp(currentSwingAmplitude, targetSwingAmplitude, Time.deltaTime * 5f);
        currentBobAmplitude = Mathf.Lerp(currentBobAmplitude, targetBobAmplitude, Time.deltaTime * 5f);

        // Apply the smooth motion
        float angle = Mathf.Sin(Time.time * swingSpeed + timeOffset) * currentSwingAmplitude;
        float bob = Mathf.Sin(Time.time * bobSpeed + timeOffset) * currentBobAmplitude;

        transform.rotation = Quaternion.Euler(0, 0, angle);
        transform.position = currentPos + new Vector3(bob * 30, bob, 0);
    }

    public void RaiseTo(Vector3 level) => StartCoroutine(RaiseToCo(level));

    private IEnumerator RaiseToCo(Vector3 level)
    {
        YieldInstruction waitForFixedTime = new WaitForFixedUpdate();

        while (Mathf.Abs(currentPos.y - level.y) > 0.05f)
        {
            currentPos = Vector3.Lerp(currentPos, level, .25f);
            yield return waitForFixedTime;
        }

        currentPos = level;
    }

    public void GetOut(Vector3 level) => StartCoroutine(GetOutCo(level));

    private IEnumerator GetOutCo(Vector3 level)
    {
        YieldInstruction waitForFixedTime = new WaitForFixedUpdate();

        while (Mathf.Abs(currentPos.y - level.y) > 0.05f)
        {
            currentPos = Vector3.Lerp(currentPos, level, .05f);
            yield return waitForFixedTime;
        }

        currentPos = level;
    }
}
