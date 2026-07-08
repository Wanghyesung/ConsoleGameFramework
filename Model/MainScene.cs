using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Core;
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
        Lock = 1 ,
        Apple = 2,
        Player = 3,
        End = 4,
    }

    public struct Vec2
    {
        public int y;
        public int x;

        public Vec2(int _y, int _x)
        {
            y = _y; x = _x;
        }

        public static Vec2 Zero =>  new Vec2(0, 0);
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
        private StringBuilder m_strMap = new StringBuilder();
        
        public override SceneKey Key => SceneKey;

    

        public override void Init(GameContext context)
        {
            base.Init(context);

            //Console.OutputEncoding = System.Text.Encoding.UTF8;

            #region CellInfo
            m_listLayer[(int)Layer.None][(int)Layer.Player] = true;
            m_listLayer[(int)Layer.Apple][(int)Layer.Player] = true;
            m_listLayer[(int)Layer.None][(int)Layer.Apple] = true;

            m_map = new List<List<CellInfo>>(MAX_MAP_SIZEY);
            for(int i = 0; i< MAX_MAP_SIZEY; ++i)
            {
                m_map.Add(new List<CellInfo>());
                for(int j = 0; j < MAX_MAP_SIZEX; ++j)
                {
                    m_map[i].Add(new CellInfo(i,j, Layer.None));
                }
            }

            AddEntity(new Apple(new Vec2(1, 10)));
            AddEntity(new Apple(new Vec2(12, 20)));
            AddEntity(new Apple(new Vec2(5, 15)));
            AddEntity(new Apple(new Vec2(22, 7) ));
            AddEntity(new Apple(new Vec2(27, 25)));

           
            #endregion

            #region CreateLock
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

            #endregion

            // 오브젝트 초기화
            Player refPlayer = new Player(new Vec2(0, 0));
            AddEntity(refPlayer);
            GameManager.Instance.SetPlayer(refPlayer);

            //TODO : 나중에 오브젝트만 따로 Init하는 코드 추가
            for (int i = 0; i < m_listObject.Count; ++i)
                m_listObject[i].Init(context);
        }

        public override void Render(GameContext context)
        {
            ConsoleUI.Clear();
            
            //오브젝트를 먼저 그리고 그 뒤에 렌더링
            RenderObject(context);
            
            RenderScene(context);

            //Console.WriteLine(m_strMap.ToString());
            
            ConsoleUI.Write(m_strMap.ToString());
        }

        private void RenderScene(GameContext context)
        {
            m_strMap.Clear();
            //m_refPlayer. 여기서 Queue로 플레이어 정보 가져오기

            for (int i = 0; i< MAX_MAP_SIZEY; ++i)
            {
                for(int j = 0; j< MAX_MAP_SIZEX; ++j)
                {
                    if( (m_map[i][j].m_eLayer == Layer.Lock))
                        m_strMap.Append("▣");
                    else if ((m_map[i][j].m_eLayer == Layer.Apple))
                        m_strMap.Append("●");
                    else if ((m_map[i][j].m_eLayer == Layer.Player))
                        m_strMap.Append("★");
                    else /*if ((m_map[i][j].m_eLayer & Layer.None) == 0)*/
                        m_strMap.Append("·");
                }
                m_strMap.Append('\n');
               
            }

            PathManager.Instance.Render(m_strMap);
        }

        //아직까지는 쓸 일이 없음
        private void RenderObject(GameContext context)
        {
            for(int i = 0; i<m_listObject.Count; ++i)
                m_listObject[i].Render(context);
        }

        public override void HandleInput(GameContext context)
        {
            //int choice = ConsoleUI.ReadMenuChoice(Menu);
            //
            //switch (choice)
            //{
            //    case 0:
            //        context.Game.RequestQuit();
            //        break;
            //}
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
