using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{

    public float duration = 0.06f; // how long freeze lasts
    private bool isStopping = false;

    public void Stop()
    {
        if(!isStopping) StartCoroutine(DoStop());
    }
    IEnumerator DoStop()
    {
        isStopping = true;
        Time.timeScale = 0f; // freeze game
        yield return new WaitForSecondsRealtime(duration); // wait in real time
        Time.timeScale = 1f; // resume
        isStopping = false;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
