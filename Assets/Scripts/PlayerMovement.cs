using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement I;
    [HideInInspector] public bool isRunning;

    private void Awake() {
        I = this;
    }

    void Update() {
        isRunning = Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0;
    }
}
