public interface IInput
{
    // События изменения векторов
    event System.Action<UnityEngine.Vector2> MoveChanged;   // Вектор движения (обычно от -1 до 1)
    event System.Action<UnityEngine.Vector2> AimChanged;    // Вектор направления атаки/прицеливания

    // События действий (кнопки)
    event System.Action AttackPressed;                       // Начало атаки
    event System.Action<UnityEngine.Vector2> AttackReleased;                      // Окончание атаки (если нужно)
    event System.Action AttackCancelled;                     // Взаимодействие
    // При необходимости можно добавить другие события: Pause, Reload и т.д.

    // Управление активностью ввода (опционально)
    void Enable();
    void Disable();
}
