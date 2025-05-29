namespace Domain.Enums;

/// <summary>
///     Статусы студентов в образовательном процессе
/// </summary>
public enum StudentStatuses
{
    /// <summary>
    ///     Активно обучается
    /// </summary>
    Active = 1,

    /// <summary>
    ///     Отчислен из образовательного учреждения
    /// </summary>
    Expelled,

    /// <summary>
    ///     В академическом отпуске
    /// </summary>
    Vacation,

    /// <summary>
    ///     Выпустился из образовательного учреждения
    /// </summary>
    Released,

    /// <summary>
    ///     Переведён в другое образовательное учреждение
    /// </summary>
    TransferToOtherInstitution,

    /// <summary>
    ///     Не активен
    /// </summary>
    NotActive
}