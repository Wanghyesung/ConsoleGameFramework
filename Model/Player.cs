using ConsoleGameFramework.Core;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System;
using System.Numerics;

namespace ConsoleGameFramework.Models;



public class Player : Entity
{
    public class PlayerPos
    {
        public PlayerPos m_refNext = null;
        public PlayerPos m_refPre = null;

        public Vec2 m_vDir;
    }

    private PlayerPos Head = null;

    private string m_strInput;

    public Player(Vec2 pos, Layer layer, Layer _eLayerMaks) : base(pos, layer, _eLayerMaks)
    {
        Head = new PlayerPos();
        m_vPos = pos;
    }

    
    public override void Update(GameContext context) //Lock
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

            if (SceneManager.Instance.CurrentScene.CanGo(vNextPos, m_eLayerMask))
            {
                Head.m_vDir = vDir;
                MovePos();

                break;
            }
            else
                vNextPos = m_vPos;
        }
   
    }
    private void MovePos()
    {
        PlayerPos refCur = Head;
        PlayerPos prePre = null;

        while(refCur != null)
        {
            
        }

    }

}
