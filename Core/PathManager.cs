using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.UI;
using ConsoleGameFramework_KR.Model;
using ConsoleGameFramework_KR.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        public struct Node
        {
            public int Y, X;
            public int G, H;
            public int F => G + H; // F = G + H 자동 계산
            public Node(int y, int x) { Y = y; X = x; }
            public Node() { Y = -1; X = -1; }
        }

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

        private List<List<int>> m_listBestG = new List<List<int>>(); //A스타 알고리즘 전용

        private List<Vec2> m_listResult = new List<Vec2>();

        private Queue<Vec2> m_refQueue = new Queue<Vec2>();  //BFS
        private int m_iMinCount = int.MaxValue;

        private PriorityQueue<Node, int> m_refOpenPQ = new PriorityQueue<Node, int>();
        private HashSet<Vec2> m_refHashClose = new HashSet<Vec2>();

        private ePathFlag Flag = ePathFlag.None;

        private int iMax;
        public void Init()
        {
            Flag |= ePathFlag.AStar;

            Vec2 vScale = SceneManager.Instance.CurrentScene.GetMapSize();
            int i = vScale.x > vScale.y ? vScale.x : vScale.y;
            iMax = i;

            m_listVis = Enumerable.Range(0, iMax)
            .Select(_ => Enumerable.Repeat(false, iMax).ToList())
            .ToList();

            m_listPath = Enumerable.Range(0, iMax)
           .Select(_ => Enumerable.Repeat(new Vec2(), iMax).ToList())
           .ToList();


            m_listBestG = Enumerable.Range(0, iMax)
           .Select(_ => Enumerable.Repeat(int.MaxValue, iMax).ToList())
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
            
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            StartAStar(vStartPos, refScene);
            stopwatch.Stop();
            ConsoleUI.WriteLine($"A* 알고리즘 실행 시간: {stopwatch.Elapsed.TotalMilliseconds} ms");

            stopwatch.Start();
            BFS(vStartPos, refScene);
            stopwatch.Stop();
            ConsoleUI.WriteLine($"BFS 알고리즘 실행 시간: {stopwatch.Elapsed.TotalMilliseconds} ms");

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

            //없으면 게임이 끝남
            if (bFind == true)
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


        /*
         F = G + H
         G (Goal): 시작점에서 현재 타일까지 오는데 걸린 실제 비용
         H (Heuristic): 현재 타일에서 목적지까지 남은 예상 비용 (직선거리 등)
         F: 최종 점수 (이 값이 가장 낮은 타일을 고르며 전진합니다) 
         */
        //휴리스틱을 기준으로 가장 가까운 사과를 찾기
        private void StartAStar(Vec2 _vStartPos, SceneBase _refSceneBase)
        {
            //맨해튼 거리고 가장 가까운 사과 찾기
            Vec2 vEndPos = new Vec2(-1,-1);
            int iMinH = int.MaxValue;
            foreach (var refObj in _refSceneBase.GetObjs(Layer.Apple))
            {
                Vec2 vTargetPos = refObj.m_vPos;
                int H = (Math.Abs(_vStartPos.x - vTargetPos.x) + Math.Abs(_vStartPos.y - vTargetPos.y)) * 10;
                if (H < iMinH)
                {
                    iMinH = H;
                    vEndPos = vTargetPos;
                }
            }

            if (vEndPos.x == -1)
                return;

            Vec2 vCur = new Vec2(-1, -1);
            m_listPath[_vStartPos.y][_vStartPos.x] = vCur;

            AStar(_vStartPos, vEndPos, _refSceneBase);
        }
        private void AStar(Vec2 _vStartPos, Vec2 _vEndPos, SceneBase _refSceneBase)
        {
            // 1. OpenList(검사 예정인 곳), ClosedList(검사 끝난 곳) 생성
            m_refOpenPQ.Clear();

            // 2. 빠른 검색을 위해 HashSet 사용
            m_refHashClose.Clear();
            Node vStartNode = new Node(_vStartPos.y, _vStartPos.x);
            m_refOpenPQ.Enqueue(vStartNode, vStartNode.F);

            while (m_refOpenPQ.Count > 0)
            {
                // 2. OpenList 안에서 F 점수가 가장 낮은 노드를 현재 노드로 선택
                Node tCur = m_refOpenPQ.Dequeue();

                //비싼 얘 먼저 나오고 나중에 싼 값이 나오면 무시
                if (m_refHashClose.Contains(new Vec2(tCur.Y, tCur.X)))
                    continue;

                // 3. 현재 노드를 OpenList에서 빼고 ClosedList에 넣음
                m_refHashClose.Add(new Vec2(tCur.Y, tCur.X));

                // 3. 목적지에 도달했으면 경로 역추적 리턴
                if (_vEndPos.x == tCur.X && _vEndPos.y == tCur.Y)
                {
                    FindPath(_vEndPos);
                    return;
                }

                // 4. 상하좌우 인접한 이웃 타일 탐색
                for (int i = 0; i < 4; ++i)
                {
                    Vec2 vNext = new Vec2(tCur.Y, tCur.X);
                    vNext.y += dir[i, 0];
                    vNext.x += dir[i, 1];

                    // 맵 범위 밖이거나, 벽(1)이거나, 이미 검사한 곳(Closed)이면 패스
                    if (_refSceneBase.CanGo(vNext, Layer.Player) == false)
                        continue;
                    if (m_refHashClose.Contains(vNext) == true)
                        continue;
                   
                    // 이동 비용 계산 (여기선 한 칸 이동당 비용 10으로 잡음) -> 현재까지 얼마나 이동했나
                    int iNextG = tCur.G + 10;

                    // 휴리스틱 대각선 제외한 맨해튼 거리 측정법 (X차이 + Y차이 * 10) -> 목적지까지 예정 비용
                    int iNextH = (Math.Abs(vNext.x - _vEndPos.x) + Math.Abs(vNext.y - _vEndPos.y)) * 10;

                    // 이미 발견된 적이 있는 타일인지 최고 기록(Best G)을 확인
                    if (iNextG < m_listBestG[vNext.y][vNext.x])
                    {
                        m_listBestG[vNext.y][vNext.x] = iNextG;

                        m_listPath[vNext.y][vNext.x] = new Vec2(tCur.Y, tCur.X);

                        // 새로운 데이터로 노드를 생성하여 큐에 대입
                        Node vNextNode = new Node(vNext.y, vNext.x)
                        {
                            G = iNextG,
                            H = iNextH
                        };
                        m_refOpenPQ.Enqueue(vNextNode, vNextNode.F);
                    }
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
            for (int i = 0; i < iMax; i++)
            {
                for (int j = 0; j < iMax; j++)
                {
                    m_listVis[i][j] = false;
                    m_listBestG[i][j] = int.MaxValue;
                    // Vec2가 구조체(struct)라면 new Vec2()는 가비지를 만들지 않고 0으로 초기화됩니다.
                    m_listPath[i][j] = new Vec2();
                }
            }
        }
    } 
}