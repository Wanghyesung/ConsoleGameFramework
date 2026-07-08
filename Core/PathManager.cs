using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework_KR.Core
{


    [Flags]
    public enum ePathFlag
    {
        None = 1 << 0,
        DFS = 1 << 1,
        BFS = 1 << 2,
        Dijkstra = 1 << 3,
        AStar = 1 << 4,
    }
    public class PathManager
    {


        int[,] dir = new int[4, 2]
        {
            { 1, 0 },
            { -1, 0 },
            { 0, 1 },
            { 0, -1 }
        };

        public static PathManager Instance { get; } = new PathManager();

        private List<List<bool>> m_listVis = new List<List<bool>>();
        private List<List<Vec2>> m_listPath = new List<List<Vec2>>();

        private List<Vec2> m_listResult = new List<Vec2>();

        private Queue<Vec2> m_refQueue = new Queue<Vec2>();  //BFS
        private int m_iMinCount = int.MaxValue;

        private ePathFlag Flag = ePathFlag.None;
        public void Init()
        {
            Flag |= ePathFlag.BFS;


            m_listVis = Enumerable.Range(0, 100)
            .Select(_ => Enumerable.Repeat(false, 100).ToList())
            .ToList();

            m_listPath = Enumerable.Range(0, 100)
           .Select(_ => Enumerable.Repeat(new Vec2(), 100).ToList())
           .ToList();
        }

        public void Render(StringBuilder _strMap)
        {
            Vec2 vPos = GameManager.Instance.Player.m_vPos;
            FindPath(vPos, _strMap);
        }

        private void FindPath(Vec2 vStartPos, StringBuilder _strMap)
        {
            //SceneManager에서 FindObject(Layer.Apple)로 가져와서 비교해보는게 좋은데 그건 나중에
            SceneBase refScene = SceneManager.Instance.CurrentScene;

            //TODO 나중에는 플래그에 따라 길탐색

            BFS(vStartPos, refScene); 
            
            //DFSStart(vStartPos, refScene);
            
            Reset();

            Vec2 Max = refScene.GetMapSize();
            int MaxY = Max.y;
            int MaxX = Max.x;

            for (int i = 0; i<m_listResult.Count; ++i)
            {
                int y = m_listResult[i].y;
                int x = m_listResult[i].x;
                _strMap[ (y * (MaxX + 1)) + x] = '+'; // 각 행 끝의 '\n' 만큼 stride를 +1 보정
            }
           
        }

        private void BFS(Vec2 _vStartPos, SceneBase _refSceneBase)
        {
            m_refQueue.Clear();
          
            var refMap = _refSceneBase.Map;

            CellInfo refCellInfo = refMap[_vStartPos.y][_vStartPos.x];


            Vec2 vCur = new Vec2();
            vCur.y = -1;

            Vec2 vStart = refCellInfo.m_vPos;
            m_refQueue.Enqueue(vStart);
            m_listVis[vStart.y][vStart.x] = true;
            m_listPath[vStart.y][vStart.x] = vCur;

            bool bFind = false;
            while (m_refQueue.Count > 0)
            {
                vCur = m_refQueue.Dequeue();
                if (refMap[vCur.y][vCur.x].m_eLayer == Layer.Apple)
                {
                    bFind = true;
                    break;
                }

                for (int i = 0; i < 4; ++i)
                {
                    Vec2 vNext = vCur;
                    vNext.y += dir[i, 0];
                    vNext.x += dir[i, 1];

                    if (_refSceneBase.CanGo(vNext, Layer.Player) == true && m_listVis[vNext.y][vNext.x] == false)
                    {
                        m_listVis[vNext.y][vNext.x] = true;
                        m_listPath[vNext.y][vNext.x] = vCur;
                        m_refQueue.Enqueue(vNext);
                    }
                }
            }

            if(bFind == true)
            {
                FindPath(vCur);
            }

        }

        //범위가 너무 많아서 안 쓰기로 했습니다
        private void DFSStart(Vec2 _vCurPos, SceneBase _refSceneBase)
        {
            Vec2 vCur = new Vec2();
            vCur.y = -1;
            m_listVis[_vCurPos.y][_vCurPos.x] = true;
            m_listPath[_vCurPos.y][_vCurPos.x] = vCur;

            m_iMinCount = int.MaxValue;
            DFS(0, _vCurPos, _refSceneBase);
        }
        private void DFS(int _iCount, Vec2 _vCurPos, SceneBase _refSceneBase)
        {
            var refMap = _refSceneBase.Map;
            if (refMap[_vCurPos.y][_vCurPos.x].m_eLayer == Layer.Apple && m_iMinCount > _iCount)
            {
                m_iMinCount = _iCount;
                FindPath(_vCurPos);
                return;
            }

            for (int i = 0; i<4; ++i)
            {
                Vec2 vNext = _vCurPos;

                vNext.y += dir[i, 0];    
                vNext.x += dir[i, 1];

                if (_refSceneBase.CanGo(vNext, Layer.Player) == true && m_listVis[vNext.y][vNext.x] == false)
                {
                    m_listVis[vNext.y][vNext.x] = true;
                    m_listPath[vNext.y][vNext.x] = _vCurPos;
                    DFS(_iCount + 1, vNext, _refSceneBase);
                    m_listVis[vNext.y][vNext.x] = false;
                    m_listPath[vNext.y][vNext.x] = Vec2.Zero;

                }
            }
        }


        private void FindPath(Vec2 _vCur)
        {
            m_listResult.Clear();

            while (_vCur.y > -1)
            {
                m_listResult.Add(_vCur);
                _vCur = m_listPath[_vCur.y][_vCur.x];
            }

            //마지막 인덱스 제거 (플레이어 위치)
            m_listResult.RemoveAt(m_listResult.Count - 1);

            int iCount = m_listResult.Count - 1;
            Vec2 vTem = m_listResult[0];
            m_listResult[0] = m_listResult[iCount];
            m_listResult[iCount] = vTem;
            m_listResult.RemoveAt(iCount);
        }

        private void Reset()
        {
            // 100x100 크기를 유지한 채 내부 값만 덮어씁니다.
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    m_listVis[i][j] = false;

                    // Vec2가 구조체(struct)라면 new Vec2()는 가비지를 만들지 않고 0으로 초기화됩니다.
                    m_listPath[i][j] = new Vec2();
                }
            }
        }
    } 
}