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

    public IReadOnlyList<Entity> ListObject => m_listObject; //읽기 전용으로 넘겨주기

    protected List<List<CellInfo>> m_map = new();

    public IReadOnlyList<IReadOnlyList<CellInfo>> Map => m_map;


    protected List<List<bool>> m_listLayer = new();
    
    private Dictionary<int, Action<Vec2>> m_hashAction = new();

    private List<Entity> m_listDeleteObj = new List<Entity>();

    public virtual void Init(GameContext context)
    {
        m_listLayer = new List<List<bool>>((int)Layer.End);
        
        for (int i = 0; i < (int)Layer.End; ++i)
        {
            m_listLayer.Add(new List<bool>());
            for (int j = 0; j < (int)Layer.End; ++j)
            {
                m_listLayer[i].Add(false);
            }
        }
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

    public bool CheckMove(Layer _eLayer, Layer _eNextLayer)
    {
        int iCur = (int)_eLayer;
        int iNext = (int)_eNextLayer;   

        //작은 애가 앞으로
        if(iCur > iNext)
            return m_listLayer[iNext][iCur];
        else
            return m_listLayer[iCur][iNext];

    }

    public virtual void AddEntity(Entity _refObj)
    {
        m_listObject.Add(_refObj);
        Vec2 vPos = _refObj.m_vPos;

        if(CanGo(vPos, _refObj.m_eLayer) == false)
            Environment.FailFast("강제 크래시 발생!");

        m_map[vPos.y][(int)vPos.x].m_eLayer = _refObj.m_eLayer;
    }


    public bool Move(Vec2 _vPre, Vec2 _vNex, Layer _eLayer)
    {
        if (CanGo(_vNex, _eLayer) == false)
            return false;

        //만약 이동 성공하면 그에 맞는 액션 호출
        Layer eTargetLayer = FindLayer(_vNex);
        StartAction(_eLayer, eTargetLayer, _vNex);

        m_map[_vPre.y][_vPre.x].m_eLayer = Layer.None;
        m_map[_vNex.y][_vNex.x].m_eLayer = _eLayer;

        return true;
    }

    public bool CanGo(Vec2 _vNext,Layer _eLayer)
    {
        if(_vNext.y < 0 || _vNext.x < 0)
            return false;
        if (_vNext.y >= m_map.Count || _vNext.x >= m_map[0].Count)
            return false;

        if (CheckMove(m_map[_vNext.y][_vNext.x].m_eLayer, _eLayer) == false)
            return false;

        return true;
    }

    public void SetCellInfo(Vec2 _vPos, Layer _eLayers)
    {
         m_map[_vPos.y][_vPos.x].m_eLayer = _eLayers;
    }

    public void AddAction(Layer _eLayer, Layer _eTarget, Action<Vec2> _refAction)
    {
        int iValue = (1<<(int)_eLayer) | (1<<(int)_eTarget);

        if (m_hashAction.ContainsKey(iValue))
            m_hashAction[iValue] += _refAction;
        else
            m_hashAction[iValue] = _refAction;
    }

    public void StartAction(Layer _eLayer, Layer _eTarget, Vec2 _vStartPos)
    {
        int iValue = (1<<(int)_eLayer) | (1<<(int)_eTarget);
        if (m_hashAction.TryGetValue(iValue, out var refAction) == true)
            refAction?.Invoke(_vStartPos);
    }

    private Layer FindLayer(Vec2 _vPos)
    {
        if (_vPos.y < 0 || _vPos.x < 0)
            return Layer.End;
        if (_vPos.y >= m_map.Count || _vPos.x >= m_map[0].Count)
            return Layer.End;

        return m_map[_vPos.y][_vPos.x].m_eLayer;
    }

    public void DeleteEntity(Entity _refObj)
    {
        m_listDeleteObj.Add(_refObj);
    }

    public void UpdateDelete()
    {
        for(int i = 0; i<m_listDeleteObj.Count; ++i)
        {
            var refEntity = m_listDeleteObj[i];
            for (int j = 0; j<m_listObject.Count; ++j) 
            {
                if(m_listObject[j] == refEntity)
                {
                    //기존 자리 없애기
                    int iLast = m_listObject.Count -1;
                    Entity tem = m_listObject[j];
                    m_listObject[j] = m_listObject[iLast];
                    m_listObject[iLast] = tem;
                    m_listObject.RemoveAt(iLast);

                    break;
                }
            }
        }

        m_listDeleteObj.Clear();
    }   



}
