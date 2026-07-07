namespace ConsoleGameFramework.UI;

/// <summary>
/// 콘솔 메뉴 한 줄을 표현하는 모델입니다.
/// 예: 1. 전투 시작 - 몬스터와 턴제 전투를 진행합니다.
/// </summary>
public readonly struct MenuOption
{
    public int Number { get; }
    public string Label { get; }
    public string Description { get; }
    public bool IsEnabled { get; }

    public MenuOption(int number, string label, string description = "", bool isEnabled = true)
    {
        Number = number;
        Label = label;
        Description = description;
        IsEnabled = isEnabled;
    }
}