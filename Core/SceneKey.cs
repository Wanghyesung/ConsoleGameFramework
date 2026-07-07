namespace ConsoleGameFramework.Core;

/// <summary>
/// 게임 화면을 구분하기 위한 열거형입니다.
/// 문자열로 장면 이름을 관리하면 오타가 나기 쉬우므로 enum을 사용합니다.
///
/// 새 화면을 추가할 때는 이곳에 항목을 하나 추가하고,
/// GameManager.RegisterScenes()에 해당 Scene을 등록하면 됩니다.
/// </summary>
public enum SceneKey
{
    Title,
    Battle,
    Sample,
    Main,
}
