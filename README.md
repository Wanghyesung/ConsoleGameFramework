# C# Console Game Framework (뼈대 버전)

C# 콘솔 게임 프로젝트를 시작하기 위한 뼈대(프레임워크)입니다.
전투, 상점, 미로 같은 게임 내용은 모두 빠져 있고, **화면 전환 구조 + 콘솔 UI 출력 기능**만 남아 있습니다.
이 위에 여러분만의 게임 로직을 자유롭게 채워 넣으면 됩니다.

## 실행 방법

```bash
dotnet run
```

Visual Studio에서는 `ConsoleGameFramework_KR.csproj` 파일을 열고 실행하면 됩니다.

## 폴더 구조

```text
ConsoleGameFramework/
├─ Program.cs
├─ Core/      # 게임 루프, 화면 전환, 공용 상태 (GameManager, GameContext, IScene, SceneBase, SceneKey)
├─ UI/        # 콘솔 출력, 입력, 더블 버퍼링, 표/박스/맵 렌더링 (ConsoleUI)
└─ Scenes/    # 화면 예시 (TitleScene, SampleScene)
```

실제 게임을 만들 때는 여기에 `Models/`(데이터), `Systems/`(로직), `Data/`(고정 데이터) 폴더를 직접 추가해서 확장하면 됩니다.

## 핵심 구조 한눈에 보기

이 프레임워크는 딱 세 가지 개념으로 돌아갑니다.

1. **GameManager** — 프로그램 전체에서 단 하나만 존재하는 관리자 (Singleton). 어떤 화면이 지금 켜져 있는지 기억하고, 화면을 전환합니다.
2. **Scene** — 화면 하나(타이틀, 전투, 상점 등)를 표현하는 단위. `Render`(그리기)와 `HandleInput`(입력 처리)만 구현하면 됩니다.
3. **GameContext** — 여러 Scene이 함께 공유해야 하는 데이터(난수, 로그, 실행 여부 등)를 담는 상자.

### 프로그램 실행 흐름

```
Program.cs
  └─ GameManager.Instance.Run()
        └─ while (게임이 실행 중이라면)
              1. 현재 Scene.Render(context)   // 화면 내용을 버퍼에 그림
              2. ConsoleUI.Present()          // 버퍼 내용을 콘솔에 한 번에 출력
              3. 현재 Scene.HandleInput(context) // 입력을 받아 다음 행동 결정
```

## 새 화면(Scene) 추가하는 법

1. `Core/SceneKey.cs`의 enum에 새 이름을 추가합니다. (예: `Battle`)
2. `Scenes/` 폴더에 `SceneBase`를 상속하는 새 클래스를 만듭니다.
   ```csharp
   public sealed class BattleScene : SceneBase
   {
       public override SceneKey Key => SceneKey.Battle;

       public override void Render(GameContext context)
       {
           ConsoleUI.Clear();
           ConsoleUI.WriteTitle("전투", "몬스터가 나타났다!");
           // ... 필요한 UI 출력
       }

       public override void HandleInput(GameContext context)
       {
           // 키 입력을 받아서 다음 화면으로 이동하거나 로직을 처리
       }
   }
   ```
3. `Core/GameManager.cs`의 `RegisterScenes()`에 한 줄 추가합니다.
   ```csharp
   AddScene(new BattleScene());
   ```
4. 다른 화면에서 `GoTo(context, SceneKey.Battle);`을 호출하면 그 화면으로 전환됩니다.

## GameContext에 데이터 추가하는 법

여러 Scene이 공유해야 하는 값(예: 플레이어 정보, 점수)이 생기면 `Core/GameContext.cs`에 프로퍼티를 추가합니다.

```csharp
public int Score { get; set; }
```

Scene에서는 `context.Score`처럼 바로 접근할 수 있습니다.

## 콘솔 UI 기능

`UI/ConsoleUI.cs`에 화면 표현 기능을 모아두었습니다.

| 기능 | 메서드 |
|---|---|
| 제목 출력 | `WriteTitle()` |
| 안내 박스 출력 | `WriteBox()` |
| 메뉴 출력 | `WriteMenu()` |
| 상태바 출력 | `WriteStatusBar()` |
| 표 출력 | `WriteTable()` |
| 로그 출력 | `WriteLog()` |
| 미로/맵 출력 | `WriteMap()` |
| 안전 입력 | `ReadMenuChoice()`, `ReadString()`, `ReadInt()` |
| 프레임 갱신 | `Clear()`, `Present()` |

## 화면 갱신 방식

각 Scene은 `Render()`에서 `ConsoleUI.Clear()`를 호출한 뒤 필요한 UI 요소를 그립니다.
`GameManager`는 `Render()`가 끝난 뒤 `ConsoleUI.Present()`를 호출해 완성된 화면을 한 번에 반영합니다.
이 방식은 `Console.Clear()`를 매번 직접 호출하는 방식보다 깜빡임이 적습니다.

## 미로를 다시 만들고 싶다면

미로/맵 시스템은 이번 뼈대 버전에서 제거했습니다. `ConsoleUI.WriteMap(IReadOnlyList<string> rows)`가 그대로 남아있으므로,
2차원 배열로 맵을 만들고 문자열 리스트로 변환해서 넘기기만 하면 오전에 만든 BFS/DFS 미로 코드를 그대로 재활용할 수 있습니다.
