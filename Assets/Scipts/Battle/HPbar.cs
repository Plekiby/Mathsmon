using System.Collections;
using UnityEngine;

public class HPbar : MonoBehaviour
{
    [SerializeField] private GameObject health;

    public void SetHP(float hpNormalized)
    {
        // Clipping the hpNormalized value to ensure it's between 0 and 1
        hpNormalized = Mathf.Clamp(hpNormalized, 0f, 1f);
        health.transform.localScale = new Vector3(hpNormalized, 1f, 1f); // Ensure Z scale is set if needed
    }

    public IEnumerator SetHPSmooth(float newHp)
    {
        float curHp = health.transform.localScale.x;
        float changeSpeed = 1f; // Change speed, adjust as needed for faster or slower updates

        // Loop condition adjusted to continue as long as the current HP is not equal to the new HP
        while (!Mathf.Approximately(curHp, newHp))
        {
            // Use Mathf.MoveTowards to ensure that curHp moves correctly towards newHp
            curHp = Mathf.MoveTowards(curHp, newHp, changeSpeed * Time.deltaTime);
            curHp = Mathf.Clamp(curHp, 0f, 1f);  // Clamping curHp to avoid negative or overly large values
            health.transform.localScale = new Vector3(curHp, 1f, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3(newHp, 1f, 1f); // Final set to make sure it exactly reaches newHp
    }
}