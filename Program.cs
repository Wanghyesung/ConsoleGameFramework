using System.Text;
using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;

// 한글이 깨지지 않도록 콘솔 입출력 인코딩을 UTF-8로 맞춥니다.
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

// 콘솔 창 제목입니다. Visual Studio / Windows Terminal에서 확인할 수 있습니다.
Console.Title = "C# Console Game Framework";

// 메뉴 화면이 깔끔하게 보이도록 기본 커서를 숨깁니다.
ConsoleUI.Initialize();

try
{
    // 프로그램의 시작점은 GameManager입니다.
    // Program.cs는 짧게 유지하고 실제 게임 흐름은 Core/Scenes에서 관리합니다.
    GameManager.Instance.Init();
    GameManager.Instance.Run();
}
finally
{
    // 프로그램이 예외로 종료되더라도 콘솔 상태를 원래대로 되돌립니다.
    ConsoleUI.Shutdown();
}
