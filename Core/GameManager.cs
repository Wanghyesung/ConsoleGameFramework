using ConsoleGameFramework.Models;
using ConsoleGameFramework.Scenes;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Core;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System.Diagnostics;

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

    private Player m_refPlayer = null;
    public Player Player => m_refPlayer;
    public void SetPlayer(Player _refPlayer) { m_refPlayer = _refPlayer; }

    private bool m_bWin = false;
    public bool IsWin => m_bWin;
    public bool SetWin
    {
        set
        {
            m_bWin = value;
            Context.IsRunning = false;
        }
    }
    
    /// <summary>
    /// 씬 등록, 맵 생성 등 무거운 초기화를 담당합니다.
    /// Program.cs에서 Run()보다 먼저 명시적으로 호출해야 합니다.
    /// </summary>
    public void Init()
    {
        Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
        SceneManager.Instance.Init(Context);
        PathManager.Instance.Init();

        ConsoleGameFramework_KR.Core.Timer.Init();
        InputManager.Init();
    }

    /// <summary>
    /// 프로그램의 메인 루프입니다.
    /// 현재 Scene을 그리고(Render), 화면에 반영(Present), 입력을 처리(HandleInput)하는 과정을 반복합니다.
    /// </summary>
    /// 
    
    public void Run()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        double dTargetFrameTime = 0.08;
        while (Context.IsRunning && SceneManager.Instance.CurrentScene is not null)
        {
            long lStartTicks = stopwatch.ElapsedTicks;

            SceneManager.Instance.CurrentScene.Render(Context);

            InputManager.Update();

            ConsoleUI.Present();

            SceneManager.Instance.CurrentScene.Update(Context);

            SceneManager.Instance.CurrentScene.UpdateDelete();

            long endTicks = stopwatch.ElapsedTicks;
            double elapsedSeconds = (double)(endTicks - lStartTicks) / Stopwatch.Frequency;
            double remainingSeconds = dTargetFrameTime - elapsedSeconds;

            // 만약 1초보다 일찍 끝났다면, 남은 시간만큼 스레드를 잠재움
            if (remainingSeconds > 0)
            {
                // 밀리초 단위로 변환해서 대기
                int sleepTimeMs = (int)(remainingSeconds * 1000);
                Thread.Sleep(sleepTimeMs);
            }
        }

        ConsoleUI.Clear();
        if (GameManager.Instance.IsWin == true)
        {
            ConsoleUI.WriteTitle("이겼습니다^0^", "고생하셨습니다.");
            ConsoleUI.WriteBox(new[]
            {
        "      _.-'''''''-._      ",
        "    .'  ^       ^  '.    ",
        "   /   ( )     ( )   \\   ",
        "  |       _____       |  ",
        "  |      /     \\      |  ",
        "   \\     \\_____/     /   ",
        "    '.             .'    ",
        "      '-._______.-'      ",
        "                         ",
        "    VICTORY CLEAR (^0^)  "
    }, "WINNER", ConsoleColor.DarkCyan);
        }
        else
        {
            ConsoleUI.WriteTitle("졌습니다 ;;", "고생하셨습니다.");
            ConsoleUI.WriteBox(new[]
    {
        "      _.-'''''''-._      ",
        "    .'  ;       ;  '.    ",
        "   /   | |     | |   \\   ",
        "  |    | |     | |    |  ",
        "  |    v v     v v    |  ",
        "   \\     .---.       /   ",
        "    '.  /     \\    .'    ",
        "      '-._______.-'      ",
        "                         ",
        "     GAME OVER (ㅠㅠ)    "
    }, "LOSER", ConsoleColor.DarkCyan);
        }
        

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
