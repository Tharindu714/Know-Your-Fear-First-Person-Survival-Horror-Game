using UnityEngine;
using UnityEngine.InputSystem;

public class Interaction : MonoBehaviour
{
    public float interactionDistance;
    public GameObject Interaction_Text;
    public GameObject Interaction_Toys;
    public GameObject Open_box;
    public GameObject candlePrompt;
    public GameObject Instructions; // New: "[T] Instructions" UI
    public GameObject getUpPrompt; // New: "[E] Get Up" UI
    public LayerMask interactionLayers;

    private CameraFall _cameraFall;
    private StaggerMovement _stagger;

    void Start()
    {
        _cameraFall = FindObjectOfType<CameraFall>();
        _stagger = FindObjectOfType<StaggerMovement>();
        getUpPrompt.SetActive(false);
    }

    void Update()
    {
        // If player is down, show Get Up prompt
        if (_cameraFall != null && _cameraFall.IsDown)
        {
            getUpPrompt.SetActive(true);
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                _cameraFall.StandUp();
                if (_stagger != null)
                    StartCoroutine(_stagger.DoStagger());
                getUpPrompt.SetActive(false);
            }
            return; // skip other interactions while down
        }

        // --- Default interactions ---
        if (Physics.Raycast(transform.position, transform.forward,
            out var hit, interactionDistance, interactionLayers))
        {
            // Instructions
            var InstructionsVar = hit.collider.GetComponent<Instruction>();
            if (InstructionsVar != null)
            {
                Instructions.SetActive(true);
                if (Keyboard.current.tKey.wasPressedThisFrame)
                    InstructionsVar.ToggleInstruction();
            }
            else Instructions.SetActive(false);

            // Letters
            var letter = hit.collider.GetComponent<Letter>();
            if (letter != null)
            {
                Interaction_Text.SetActive(true);
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    letter.OpenCloseLetter();
            }
            else Interaction_Text.SetActive(false);

            // Toys
            var toys = hit.collider.GetComponent<Toys>();
            if (toys != null)
            {
                Interaction_Toys.SetActive(true);
                if (Keyboard.current.gKey.wasPressedThisFrame)
                    toys.OpenCloseBox();
            }
            else Interaction_Toys.SetActive(false);

            // Gift Boxes
            var giftBox = hit.collider.GetComponent<GiftBox>();
            if (giftBox != null && giftBox.gameObject.CompareTag("Close"))
            {
                Open_box.SetActive(true);
                if (Keyboard.current.oKey.wasPressedThisFrame)
                    giftBox.OpenBox();
            }
            else Open_box.SetActive(false);

            // Candles
            var candle = hit.collider.GetComponentInParent<CandleController>();
            if (candle != null)
            {
                candlePrompt.SetActive(true);
                if (Keyboard.current.lKey.wasPressedThisFrame)
                    candle.ToggleCandle();
            }
            else candlePrompt.SetActive(false);
        }
        else
        {
            Interaction_Text.SetActive(false);
            Interaction_Toys.SetActive(false);
            Open_box.SetActive(false);
            candlePrompt.SetActive(false);
            getUpPrompt.SetActive(false);
        }
    }
}

