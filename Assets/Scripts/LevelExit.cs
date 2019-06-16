using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] ParticleSystem exitParticleSystem;
    [SerializeField] float levelExitTime = 2f;

    bool inTransition = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!inTransition)
        {
            inTransition = true;
            StartCoroutine(LoadNextScene());
        }
    }

    IEnumerator LoadNextScene()
    {
        exitParticleSystem.Play();
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        yield return new WaitForSeconds(levelExitTime);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
