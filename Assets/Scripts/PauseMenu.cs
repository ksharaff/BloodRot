using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private PlayerMovement movement;
    private PlayerAttack attack;
    private bool isPaused = false;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        movement = player.GetComponent<PlayerMovement>();
        attack = player.GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused) Resume();
            else if(movement.enabled) Pause();
        }
    }

    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // freezes everything
        movement.enabled = false; // stops movement inputs
        attack.enabled = false; // stops attack inputs
        isPaused = true;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // unfreezes everything
        movement.enabled = true; // movement inputs
        attack.enabled = true; // attack inputs
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
