using UnityEngine;

public class WindZone : MonoBehaviour {
    
    public enum WindDirection {
        Up,
        Down,
        Right,
        Left,
    }

    [SerializeField] private WindDirection windDirection;
    [SerializeField] private float windStrength = 200f;
    [SerializeField] private Transform arrowVisual;

    private void OnValidate() {
        UpdateArrowVisual();
    }

    private void Awake() {
        UpdateArrowVisual();
    }

    private void OnTriggerEnter2D(Collider2D collider2D) {
        if (!collider2D.TryGetComponent(out Lander lander)) {
            return;
        }
        
        lander.SetWindZone(this);
    }

    private void OnTriggerExit2D(Collider2D collider2D) {
        if (!collider2D.TryGetComponent(out Lander lander)) {
            return;
        }
        
        lander.ClearWindZone(this);
    }

    public Vector2 GetWindForce() {
        return GetDirectionVector() * windStrength;
    }
    
    private Vector2 GetDirectionVector() {
        switch (windDirection) {
            default:
                return Vector2.right;
            case WindDirection.Left:
                return Vector2.left;
            case WindDirection.Up:
                return Vector2.up;
            case WindDirection.Down:
                return Vector2.down;
        }
    }

    private void UpdateArrowVisual() {
        if (arrowVisual == null) {
            return;
        }

        arrowVisual.localEulerAngles = new Vector3(0f, 0f, GetArrowZRotation());
    }

    private float GetArrowZRotation() {
        switch (windDirection) {
            default:
                return 0f;
            case WindDirection.Up:
                return 90f;
            case WindDirection.Left:
                return 180f;
            case WindDirection.Down:
                return 270f;
        }
    }
}
