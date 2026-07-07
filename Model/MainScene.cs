using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework_KR.Model
{
    [Flags]
    public enum Layer
    {
        None = 0,
        Lock = 1 << 0,
        Apple = 1 << 1,
        Player = 1 << 2,
    }

    public struct Vec2
    {
        public int y;
        public int x;

        public Vec2(int _y, int _x)
        {
            y = _y; x = _x;
        }
    }

    public class CellInfo
    {
        //TODO : 이거 비트 마스크로 변경 갈수 있는지 레이어로 체크

        public Layer m_eLayer;
        public CellInfo()
        {
           
        }

        public CellInfo(int y, int x)
        {
            m_vPos.y = y;
            m_vPos.x = x;
        }

        public CellInfo(int y, int x, Layer _eLayer)
        {
            m_vPos.y = y;
            m_vPos.x = x;
            m_eLayer = _eLayer;
        }

        public Vec2 m_vPos;
    }


    public class MainScene : SceneBase
    {
        private SceneKey SceneKey = SceneKey.Main;

        private  List<MenuOption> Menu = new List<MenuOption>
    {
        new MenuOption(2, "전투 화면으로 이동", "플레이어와 적의 전투가 시작됩니다."),
        new MenuOption(1, "샘플 화면으로 이동", "ConsoleUI의 다른 기능들을 보여주는 화면으로 이동합니다."),
        new MenuOption(0, "종료", "프로그램을 종료합니다.")
    };
        const int MAX_MAP_SIZEX = 100;
        const int MAX_MAP_SIZEY = 30;
        private string m_strMap; //stringbuilder로 바꾸기
        
        public override SceneKey Key => SceneKey;

        public override void Init(GameContext context)
        {
            //Console.OutputEncoding = System.Text.Encoding.UTF8;
            m_map = new List<List<CellInfo>>(MAX_MAP_SIZEY);
            for(int i = 0; i< MAX_MAP_SIZEY; ++i)
            {
                m_map.Add(new List<CellInfo>());
                for(int j = 0; j < MAX_MAP_SIZEX; ++j)
                {
                    m_map[i].Add(new CellInfo(i,j, Layer.None));
                }
            }

            SetCellInfo(new Vec2(1, 10), Layer.Apple);
            SetCellInfo(new Vec2(12, 20), Layer.Apple);
            SetCellInfo(new Vec2(5, 15), Layer.Apple);
            SetCellInfo(new Vec2(22, 7), Layer.Apple);
            SetCellInfo(new Vec2(27, 25), Layer.Apple);

            Random refRange = new Random();

            for(int i = 0; i<MAX_MAP_SIZEY /4; ++i) 
            {
                for(int j =0; j<MAX_MAP_SIZEX / 8; ++j)
                {
                    int y = refRange.Next(0, MAX_MAP_SIZEY -1);
                    int x = refRange.Next(0, MAX_MAP_SIZEX - 1);
                    SetCellInfo(new Vec2(y, x), Layer.Lock);
                }
            }

            // 오브젝트 초기화
            m_refPlayer = new Player(new Vec2(0, 0), Layer.Player, Layer.Lock);
                AddEntity(m_refPlayer);

        }

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();

            ConsoleUI.WriteBox(new[]
            {
                "현재 맵",
        }, "ASDW를 눌러서 움직이세요", ConsoleColor.DarkCyan);

            
            //오브젝트를 먼저 그리고 그 뒤에 렌더링
            RenderObject(context);

            RenderScene(context);
          
            ConsoleUI.WriteLine(m_strMap);
           
        }

        private void RenderScene(GameContext context)
        {
            //m_refPlayer. 여기서 Queue로 플레이어 정보 가져오기

            for(int i = 0; i< MAX_MAP_SIZEY; ++i) 
            {
                for(int j = 0; j< MAX_MAP_SIZEX; ++j)
                {
                    if( (m_map[i][j].m_eLayer & Layer.Lock) != 0)
                        m_strMap+= "▣";
                    else if ((m_map[i][j].m_eLayer & Layer.Apple) != 0)
                        m_strMap += "●";
                    else if ((m_map[i][j].m_eLayer & Layer.Player) != 0)
                        m_strMap += "★";
                    else /*if ((m_map[i][j].m_eLayer & Layer.None) == 0)*/
                        m_strMap += "·";
                }
                m_strMap +='\n';
            }
        }

        //아직까지는 쓸 일이 없음
        private void RenderObject(GameContext context)
        {
            for(int i = 0; i<m_listObject.Count; ++i)
                m_listObject[i].Render(context);
        }

        public override void HandleInput(GameContext context)
        {
            int choice = ConsoleUI.ReadMenuChoice(Menu);
            
            switch (choice)
            {
                case 0:
                    context.Game.RequestQuit();
                    break;
            }
        }

        public override void Update(GameContext context)
        {
            base.Update(context);
        }

        public override Vec2 GetMapSize()
        {
            return new Vec2(MAX_MAP_SIZEY, MAX_MAP_SIZEX);
        }

    }
}
