namespace Domain.Enums;

/// <summary>
/// Типы приказов по движению студентов
/// </summary>
public enum OrderTypes
{
    /// <summary>
    /// Зачисление в образовательное учреждение 
    /// </summary>
    Enrollment = 1,
    
    /// <summary>
    /// Перевод из другого образовательного учреждения
    /// </summary>
    TransferFromOtherInstitution = 2,
    
    /// <summary>
    /// Перевод в другое образовательное учреждение
    /// </summary>
    TransferToOtherInstitution = 3,
    
    /// <summary>
    /// Восстановление после отчисления
    /// </summary>
    ReinstatementFromExpelled = 4,
    
    /// <summary>
    /// Восстановление из академического отпуска
    /// </summary>
    ReinstatementFromAcademy = 5,
    
    /// <summary>
    /// Академический отпуск 
    /// </summary>
    AcademicLeave = 6,
    
    /// <summary>
    /// Перевод между группами внутри учреждения
    /// </summary>
    TransferBetweenGroups = 7,
    
    /// <summary>
    /// Отчисление из образовательного учреждения
    /// </summary>
    Expulsion = 8,
    
    /// <summary>
    /// Выпуск 
    /// </summary>
    Graduation = 9
}