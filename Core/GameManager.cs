using ConsoleGameFramework.Scenes;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;

namespace ConsoleGameFramework.Core;

/// <summary>
/// 게임의 전체 흐름을 담당하는 클래스입니다.
///
/// 역할:
/// 1. Scene 등록
/// 2. 현재 Scene 실행
/// 3. Scene 전환
/// 4. 프로그램 종료 요청 처리
///
/// 참고: 이 클래스는 프로그램 전체에서 단 하나만 존재해야 하므로 Singleton 패턴으로 작성되어 있습니다.
/// (생성자를 private으로 막고, 정적 Instance 프로퍼티로만 접근을 허용)
/// </summary>
public class GameManager
{
    /// <summary>
    /// 프로그램 전체에서 하나만 사용하는 GameManager 인스턴스입니다.
    /// </summary>
    public static GameManager Instance { get; } = new GameManager();

    private GameManager()
    {
        Context = new GameContext(this);
    }

    public GameContext Context { get; }

    /// <summary>
    /// 씬 등록, 맵 생성 등 무거운 초기화를 담당합니다.
    /// Program.cs에서 Run()보다 먼저 명시적으로 호출해야 합니다.
    /// </summary>
    public void Init()
    {
        SceneManager.Instance.Init(Context);
    }

    /// <summary>
    /// 프로그램의 메인 루프입니다.
    /// 현재 Scene을 그리고(Render), 화면에 반영(Present), 입력을 처리(HandleInput)하는 과정을 반복합니다.
    /// </summary>
    /// 
    
    public void Run()
    {

        while (Context.IsRunning && SceneManager.Instance.CurrentScene is not null)
        {
            SceneManager.Instance.CurrentScene.Render(Context);
            SceneManager.Instance.CurrentScene.Update(Context);
            ConsoleUI.Present();
            SceneManager.Instance.CurrentScene.HandleInput(Context);
        }

        ConsoleUI.Clear();
        ConsoleUI.WriteTitle("프로그램 종료", "수고하셨습니다.");
        ConsoleUI.WriteBox(new[]
        {
            "C# 콘솔 게임 프레임워크가 종료되었습니다.",
            "Core, UI, Scenes 구조를 기준으로 기능을 확장할 수 있습니다."
        }, "Good Bye", ConsoleColor.DarkCyan);
        ConsoleUI.Present();
    }


    /// <summary>
    /// 다른 화면으로 전환합니다.
    /// </summary>
    

    /// <summary>
    /// 게임 종료를 요청합니다.
    /// </summary>
    public void RequestQuit()
    {
        Context.IsRunning = false;
    }
}
