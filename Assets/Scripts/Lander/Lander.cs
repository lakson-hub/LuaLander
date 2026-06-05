using System;
using UnityEngine;

public class Lander : MonoBehaviour {

    private const float GRAVITY_NORMAL = 0.7f;
    
    public static Lander Instance { get; private set; }

    public event EventHandler OnUpForce;
    public event EventHandler OnRightForce;
    public event EventHandler OnLeftForce;
    public event EventHandler OnBeforeForce;
    public event EventHandler OnCoinPickup;
    public event EventHandler OnFuelPickup;
    public event EventHandler<OnStateChangedEventsArgs> OnStateChanged;
    public class OnStateChangedEventsArgs : EventArgs {
        public State state;
    }
    public event EventHandler<OnLandedEventArgs> OnLanded;
    public event EventHandler OnFellOutOfMap;
    public event EventHandler OnWindZoneEntered;
    public event EventHandler OnWindZoneExited;

    public class OnLandedEventArgs : EventArgs {
        public LandingType landingType;
        public int score;
        public float dotVector;
        public float landingSpeed;
        public float scoreMultiplier;
    }
    
    public enum LandingType {
        Success,
        WrongLandingArea,
        TooSteepAngle,
        TooFastLanding,
    }
    
    public enum State {
        WaitingToStart,
        Normal,
        GameOver,
    }
    
    private Rigidbody2D landerRigidbody2D;
    private float fuelAmount;
    private float fuelAmountMax = 10f;
    private State state;
    private WindZone activeWindZone;
    
    private void Awake() {
        Instance = this;
        
        fuelAmount = fuelAmountMax;
        state = State.WaitingToStart;
        
        landerRigidbody2D = GetComponent<Rigidbody2D>();
        landerRigidbody2D.gravityScale = 0f;
    }

    private void FixedUpdate() {
        OnBeforeForce?.Invoke(this, EventArgs.Empty);

        switch (state) {
            default:
            case State.WaitingToStart:
                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
                    // Pressing any input
                    landerRigidbody2D.gravityScale = GRAVITY_NORMAL;
                    SetState(State.Normal);
                }
                
                break;
            case State.Normal:
                ApplyWindForce();
                
                if (fuelAmount <= 0f) {
                    // No fuel
                    return;
                }
        
                if (GameInput.Instance.IsUpActionPressed() ||
                    GameInput.Instance.IsRightActionPressed() ||
                    GameInput.Instance.IsLeftActionPressed() ||
                    GameInput.Instance.GetMovementInputVector2() != Vector2.zero) {
                    // Pressing any input
                    ConsumeFuel();
                }

                float gamepadDeadzone = .4f;
                if (GameInput.Instance.IsUpActionPressed() || GameInput.Instance.GetMovementInputVector2().y > gamepadDeadzone) {
                    float force = 700f;
                    landerRigidbody2D.AddForce(transform.up * (force * Time.deltaTime));
                    OnUpForce?.Invoke(this, EventArgs.Empty);
                }
        
                if (GameInput.Instance.IsLeftActionPressed() || GameInput.Instance.GetMovementInputVector2().x < -gamepadDeadzone) {
                    float turnSpeed = +100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnLeftForce?.Invoke(this, EventArgs.Empty);
                }
        
                if (GameInput.Instance.IsRightActionPressed() || GameInput.Instance.GetMovementInputVector2().x > gamepadDeadzone) {
                    float turnSpeed = -100f;
                    landerRigidbody2D.AddTorque(turnSpeed * Time.deltaTime);
                    OnRightForce?.Invoke(this, EventArgs.Empty);
                }
                
                break;
            case State.GameOver:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision2D) {
        if (!collision2D.gameObject.TryGetComponent(out LandingPad landingPad)) {
            Debug.Log("Crashed on the Terrain!");
            OnLanded?.Invoke(this, new OnLandedEventArgs {
                landingType = LandingType.WrongLandingArea,
                dotVector = 0f,
                landingSpeed = 0f,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }
        
        float softLandingVelocityMagnitude = 4f;
        float relativeVelocityMagnitude = collision2D.relativeVelocity.magnitude;
        if (relativeVelocityMagnitude > softLandingVelocityMagnitude) {
            // Landed too hard!
            Debug.Log("Landed too hard!");
            OnLanded?.Invoke(this, new OnLandedEventArgs {
                landingType = LandingType.TooFastLanding,
                dotVector = 0f,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }

        float dotVector = Vector2.Dot(Vector2.up, transform.up);
        float minDotVector = .90f;
        if (dotVector < minDotVector) {
            // Landed on a too steep angle!
            Debug.Log("Landed on a too steep angle!");
            OnLanded?.Invoke(this, new OnLandedEventArgs {
                landingType = LandingType.TooSteepAngle,
                dotVector = dotVector,
                landingSpeed = relativeVelocityMagnitude,
                scoreMultiplier = 0,
                score = 0,
            });
            SetState(State.GameOver);
            return;
        }
        
        Debug.Log("Successful landing!");

        float maxScoreAmountLandingAngle = 100;
        float scoreDotVectorMultiplier = 10f;
        float landingAngleScore = maxScoreAmountLandingAngle - Math.Abs(dotVector - 1f) * scoreDotVectorMultiplier * maxScoreAmountLandingAngle;

        float maxScoreAmountLandingSpeed = 100;
        float landingSpeedScore =
            (softLandingVelocityMagnitude - relativeVelocityMagnitude) * maxScoreAmountLandingSpeed;
        
        Debug.Log("Landing angle score: " + landingAngleScore);
        Debug.Log("Landing speed score: " + landingSpeedScore);

        int score = Mathf.RoundToInt((landingAngleScore + landingSpeedScore) * landingPad.GetScoreMultiplier());
        
        Debug.Log("Score: " + score);
        OnLanded?.Invoke(this, new OnLandedEventArgs {
            landingType = LandingType.Success,
            dotVector = dotVector,
            landingSpeed = relativeVelocityMagnitude,
            scoreMultiplier = landingPad.GetScoreMultiplier(),
            score = score,
        });
        SetState(State.GameOver);
    }

    public void SetWindZone(WindZone windZone) {
        activeWindZone = windZone;
        OnWindZoneEntered?.Invoke(this, EventArgs.Empty);
    }

    public void ClearWindZone(WindZone windZone) {
        if (activeWindZone != windZone) {
            return;
        }

        activeWindZone = null;
        OnWindZoneExited?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyWindForce() {
        if (activeWindZone == null) {
            return;
        }
        
        landerRigidbody2D.AddForce(activeWindZone.GetWindForce() * Time.deltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collider2D) {
        if (collider2D.gameObject.TryGetComponent(out FuelPickup fuelPickup)) {
            float addFuelAmount = 10f;
            fuelAmount += addFuelAmount;
            if (fuelAmount > fuelAmountMax) {
                fuelAmount = fuelAmountMax;
            }
            OnFuelPickup?.Invoke(this, EventArgs.Empty);
            fuelPickup.DestroySelf();
        }
        
        if (collider2D.gameObject.TryGetComponent(out CoinPickup coinPickup)) {
            OnCoinPickup?.Invoke(this, EventArgs.Empty);
            coinPickup.DestroySelf();
        }
    }

    private void SetState(State state) {
        this.state = state;
        OnStateChanged?.Invoke(this, new OnStateChangedEventsArgs {
            state = state
        });
    }
    
    private void ConsumeFuel() {
        float fuelConsumptionAmount = 1f;
        fuelAmount -= fuelConsumptionAmount * Time.deltaTime;
    }

    public float GetFuelAmountNormalized() {
        return fuelAmount / fuelAmountMax;
    }
    
    public float GetSpeedX() {
        return landerRigidbody2D.linearVelocityX;
    }
    
    public float GetSpeedY() {
        return landerRigidbody2D.linearVelocityY;
    }

    public State GetState() {
        return state;
    }

    public void TriggerFellOutOfMap() {
        if (state != State.Normal) {
            return;
        }
        
        SetState(State.GameOver);
        OnFellOutOfMap?.Invoke(this, EventArgs.Empty);
    }
}
