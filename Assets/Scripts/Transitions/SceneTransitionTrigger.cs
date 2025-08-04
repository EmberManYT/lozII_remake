using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    public string targetScene;
    public Vector2 spawnPositionInNewScene;
    public string triggerID;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !SceneTransitionManager.Instance.IsTransitioning()) {
            SceneTransitionManager.Instance.TransitionToScene(targetScene, spawnPositionInNewScene);
        }
    }
}
