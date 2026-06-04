using UnityEngine;

public class LanderOutOfMapChecker : MonoBehaviour {

    private float minFallY;
    private bool hasTriggered;

    public void Setup(float minFallY) {
        this.minFallY = minFallY;
        hasTriggered = false;
    }

    private void Update() {
        if (hasTriggered) {
            return;
        }

        if (Lander.Instance == null) {
            return;
        }

        if (Lander.Instance.GetState() != Lander.State.Normal) {
            return;
        }

        if (Lander.Instance.transform.position.y < minFallY) {
            hasTriggered = true;
            Lander.Instance.TriggerFellOutOfMap();
        }
    }
}
