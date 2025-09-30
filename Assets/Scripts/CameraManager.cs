using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public Transform target;

    public float smoothness;
    public Vector3 offset;
    public bool shaking;

    void Awake() => instance = this;

    void FixedUpdate()
    {
        if (!shaking && target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothness);
        }
    }

    public void Shake(float duration, float force) => StartCoroutine(ShakeCO(duration, force));

    public IEnumerator ShakeCO(float duration, float force)
    {
        YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();

        Vector3 originalPos = transform.position;
        Vector3 shakePos;

        while (duration > 0)
        {
            duration -= Time.deltaTime;
            float randomX = Random.Range(originalPos.x - 1 * force, originalPos.x + 1 * force);
            float randomY = Random.Range(originalPos.y - 1 * force, originalPos.y + 1 * force);
            shakePos = new Vector3(randomX, randomY, transform.position.z);
            transform.position = shakePos;
            yield return waitForFixedUpdate;
        }

        transform.position = originalPos;
    }
}
