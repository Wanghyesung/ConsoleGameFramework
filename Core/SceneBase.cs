using ConsoleGameFramework.Models;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System.Numerics;

namespace ConsoleGameFramework.Core;

/// <summary>
/// Scene 구현을 조금 더 편하게 만들기 위한 추상 클래스입니다.
/// Enter/Exit은 기본 동작이 없어도 되므로 빈 메서드로 제공합니다.
/// </summary>
public abstract class SceneBase : IScene
{
    public abstract SceneKey Key { get; }

    protected List<Entity> m_listObject = new List<Entity>();

    protected List<List<CellInfo>> m_map = new();

    protected Player m_refPlayer;
    public virtual void Init(GameContext context)
    {

    }

    public virtual void Enter(GameContext context)
    {
        // 필요할 때만 자식 Scene에서 override 합니다.
    }

    public virtual void Update(GameContext context)
    {
        for(int i = 0; i < m_listObject.Count; i++)
            m_listObject[i].Update(context);
    }

    public abstract void Render(GameContext context);

    public abstract void HandleInput(GameContext context);
    public abstract Vec2 GetMapSize();

    public virtual void Exit(GameContext context)
    {
        // 필요할 때만 자식 Scene에서 override 합니다.
    }
   
    protected static void GoTo(GameContext context, SceneKey nextScene)
    {
        SceneManager.Instance.ChangeScene(nextScene);
    }


    public virtual void AddEntity(Entity _refObj)
    {
        m_listObject.Add(m_refPlayer);
        Vec2 vPos = _refObj.m_vPos;

        if(CanGo(vPos, _refObj.m_eLayer) == false)
            Environment.FailFast("강제 크래시 발생!");

        m_map[vPos.y][(int)vPos.x].m_eLayer = _refObj.m_eLayer;
    }


    public bool Move(Vec2 _vPre, Vec2 _vNex, Layer _eLayer)
    {
        if (CanGo(_vNex, _eLayer) == false)
            return false;

        m_map[_vPre.y][_vPre.x].m_eLayer = Layer.None;
        m_map[_vNex.y][_vNex.x].m_eLayer = _eLayer;

        return true;
    }

    public bool CanGo(Vec2 _vNext,Layer _eLayerMask)
    {
        if(_vNext.y < 0 || _vNext.x < 0)
            return false;
        if (_vNext.y >= m_map.Count || _vNext.x >= m_map[0].Count)
            return false;
        if ((m_map[(int)_vNext.y][(int)_vNext.x].m_eLayer & _eLayerMask) != 0)
            return false;

        return true;
    }

    public void SetCellInfo(Vec2 _vPos, Layer _eLayers)
    {
          m_map[_vPos.y][_vPos.x].m_eLayer = _eLayers;
    }

    public bool Move(Entity _refEntity, Vec2 _vNex, Layer _eLayer)
    {
        throw new NotImplementedException();
    }
}
