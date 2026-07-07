using ConsoleGameFramework.Models;
using ConsoleGameFramework_KR.Model;
using System.Numerics;

namespace ConsoleGameFramework.Core;

/// <summary>
/// 모든 화면(Scene)이 반드시 가져야 하는 기능을 정의합니다.
/// TitleScene, SampleScene 등이 이 인터페이스를 구현합니다.
/// </summary>
public interface IScene
{
    /// <summary>
    /// 이 화면을 구분하는 키입니다.
    /// GameManager가 Dictionary에 화면을 등록할 때 사용합니다.
    /// </summary>
    SceneKey Key { get; }

    /// <summary>
    /// 화면에 처음 들어왔을 때 한 번 실행됩니다.
    /// 예: 전투 화면에 들어오면 몬스터 생성
    /// </summary>
    void Enter(GameContext context);

    /// <summary>
    /// 콘솔에 화면을 그립니다.
    /// 예: 제목, 상태창, 메뉴, 맵, 로그 출력
    /// </summary>
    void Render(GameContext context);


    void Update(GameContext context);

    void Init(GameContext context);
    
    /// <summary>
    /// 사용자 입력을 받고 처리합니다.
    /// 예: 메뉴 번호 입력, WASD 이동 입력
    /// </summary>
    void HandleInput(GameContext context);

    /// <summary>
    /// 다른 화면으로 나가기 직전에 실행됩니다.
    /// 정리할 데이터가 있을 때 사용합니다.
    /// </summary>
    void Exit(GameContext context);

    //항상 Y축이 앞에 나오게

    Vec2 GetMapSize();

    bool Move(Vec2 _vPre, Vec2 _vNex, Layer _eLayer);
    bool CanGo(Vec2 _vNext, Layer _eLayerMask);

    void AddEntity(Entity _refObj);

    public void DeleteEntity(Entity _refObj);

    void UpdateDelete();
}
