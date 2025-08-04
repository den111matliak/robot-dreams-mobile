using UnityEngine;
using UnityEngine.InputSystem;     // для Vector2 у підписці
using Dreamteck.Forever;          // клас Runner
// using Game.Input;               // НЕ потрібно, бо InputController у глобальному неймспейсі у прикладі вище

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Runner runner;        // перетягни сюди свій "Basic Runner" (Runner)
    [SerializeField] private float strafeSpeed = 5f;  // швидкість зміни offset.x
    [SerializeField] private float maxOffset = 2f;    // межа відхилення ліво/право

    private InputController _input;
    private float _inputX;  // останнє значення по X від інпуту

    private void Awake()
    {
        if (runner == null) runner = GetComponent<Runner>();

        _input = new InputController();
        _input.MovementReceived += OnMovementReceived;
    }

    private void OnDestroy()
    {
        if (_input != null)
        {
            _input.MovementReceived -= OnMovementReceived;
            _input.Dispose();
        }
    }

    private void OnMovementReceived(Vector2 movement)
    {
        _inputX = movement.x; // беремо тільки X (ліво/право)
        // Debug.Log($"Move X: {_inputX}");
    }

    private void Update()
    {
        if (runner == null) return;

        // цільовий офсет по X у межах коридору
        float target = Mathf.Clamp(_inputX * maxOffset, -maxOffset, maxOffset);

        // плавно рухаємо поточний offset.x до цілі
        var motion = runner.motion; // це reference type (клас), тож можна змінювати його поля
        motion.offset = new Vector2(
            Mathf.MoveTowards(motion.offset.x, target, strafeSpeed * Time.deltaTime),
            motion.offset.y
        );
        // НІЧОГО додатково присвоювати runner’у не треба: motion — посилальний тип
    }
}
