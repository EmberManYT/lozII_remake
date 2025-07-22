using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;
    private bool isTransitioning = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName, Vector2 spawnPosition) {
        if (!isTransitioning) {
            StartCoroutine(LoadSceneAsync(sceneName, spawnPosition));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, Vector2 spawnPosition) {
        isTransitioning = true;
        yield return FadeController.Instance.FadeOut();
        yield return SceneManager.LoadSceneAsync(sceneName);

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) {
            player.transform.position = spawnPosition;
        }

        yield return new WaitForSeconds(0.2f);
        yield return FadeController.Instance.FadeIn();
        yield return new WaitForSeconds(0.5f);
        isTransitioning = false;
    }

    public bool IsTransitioning() => isTransitioning;
}
