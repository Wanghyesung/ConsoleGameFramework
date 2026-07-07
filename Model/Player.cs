using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ConsoleGameFramework.Models;



public class Player : Entity
{
    public class PlayerPos
    {
        public PlayerPos m_refNext = null;
        public PlayerPos m_refPre = null;

        public Vec2 m_vDir;
        public Vec2 m_vPos;
    }

    private PlayerPos m_refHead = null;

    private string m_strInput;

    private bool m_bGrow = false;

    public Player(Vec2 pos, Layer layer) : base(pos, layer)
    {
        m_refHead = new PlayerPos();
        m_vPos = pos;
        m_refHead.m_vPos = m_vPos;
    }


    public override void Init(GameContext context)
    {
        var IScene = SceneManager.Instance.CurrentScene;
        if(IScene != null)
        {

            if (IScene is SceneBase refScene)
                refScene.AddAction(m_eLayer, Layer.Apple, AddBody);
            else
                Environment.FailFast("플레이어 함수 콜백이 안됨");
        }
    }
    public override void Update(GameContext context)
    {
        UpdatePos();
    }
    public override void Render(GameContext _refContext)
    {

    }

    private void UpdatePos()
    {
        //내 위치 미리 잡고 다음 칸 갈 수 있는지 체크
        Vec2 vNextPos = m_vPos;
        Vec2 vDir;
        while (true)
        {
            m_strInput = ConsoleUI.ReadString("ASDW를 입력하여 움직이세요");
            switch (m_strInput)
            {
                case "W":
                case "w":
                    vNextPos.y--;
                    vDir = new Vec2(-1, 0);
                    break;
                case "S":
                case "s":
                    vNextPos.y++;
                    vDir = new Vec2(1, 0);
                    break;
                case "A":
                case "a":
                    vNextPos.x--;
                    vDir = new Vec2(0, -1);
                    break;
                case "D":
                case "d":
                    vNextPos.x++;
                    vDir = new Vec2(0, 1);
                    break;
                default:
                    ConsoleUI.WriteLine("잘못된 입력입니다. 다시 입력해주세요.");
                    continue;
            }

            if (SceneManager.Instance.CurrentScene.CanGo(vNextPos,m_eLayer) == true)
            {
                m_refHead.m_vDir = vDir;
                MovePos(vNextPos);
                break;
            }
            else
            {
                vNextPos = m_vPos;
                ConsoleUI.WriteLine("못가여. 다시 입력해주세요.");
            }
        }
    }

    private bool MovePos(Vec2 _vNextPos)
    {
        if (SceneManager.Instance.CurrentScene.Move(m_refHead.m_vPos, _vNextPos, m_eLayer) == false)
            return false;

        //Head가 이번에 있던 자리와 방향을 다음 마디에게 넘겨줄 값으로 저장
        Vec2 vTrailPos = m_refHead.m_vPos;
        Vec2 vTrailDir = m_refHead.m_vDir;

        m_refHead.m_vPos = _vNextPos;
        m_vPos = _vNextPos;

        PlayerPos refCur = m_refHead;
        while (refCur.m_refPre != null)
        {
            refCur = refCur.m_refPre;

            //다음 마디로 넘기기 전에 내 현재 자리/방향을 먼저 보관
            Vec2 vCurPos = refCur.m_vPos;
            Vec2 vCurDir = refCur.m_vDir;

            //앞 마디가 있던 자리로 이동하면서, 앞 마디가 오던 방향을 그대로 참고해서 따라감
            if (SceneManager.Instance.CurrentScene.Move(refCur.m_vPos, vTrailPos, m_eLayer) == false)
                break;

            refCur.m_vPos = vTrailPos;
            refCur.m_vDir = vTrailDir;

            vTrailPos = vCurPos;
            vTrailDir = vCurDir;
        }

        if(m_bGrow == true)
        {
            m_bGrow = false;

            PlayerPos refTail = new PlayerPos();
            refCur.m_refPre = refTail;
            refTail.m_refNext = refCur;

            refTail.m_vDir = vTrailDir;
            refTail.m_vPos = vTrailPos;

            SceneManager.Instance.CurrentScene.Move(refTail.m_vPos, vTrailPos, m_eLayer);
        }

        return true;
    }

    //CallBack
    private void AddBody(Vec2 _vPos)
    {
        m_bGrow = true;
    }

}
