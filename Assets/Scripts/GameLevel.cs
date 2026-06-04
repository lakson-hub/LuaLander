using UnityEngine;

public class GameLevel : MonoBehaviour {

    [SerializeField] private int levelNumber;
    [SerializeField] private Transform landerStartPositionTransform;
    [SerializeField] private Transform cameraStartTargetTransform;
    [SerializeField] private float zoomedOutOrthographicSize;
    [SerializeField] private float minFallY = -25f;

    public int GetLevelNumber() {
        return levelNumber;
    }

    public Vector3 GetLanderStartPosition() {
        return landerStartPositionTransform.position;
    }

    public Transform GetCameraStartTargetTransform() {
        return cameraStartTargetTransform;
    }

    public float GetZoomedOutOrthographicSize() {
        return zoomedOutOrthographicSize;
    }

    public float GetMinFallY() {
        return minFallY;
    }
}
