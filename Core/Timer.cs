using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework_KR.Core
{
    public static class Timer
    {
        static private long Counter = 0; //이거도 아토믹으로 해야하지 않나? 캐시랑 메인 메모리랑 다를 수 있으니깐

        static private bool m_Running = false;

        private static Task m_refTimeTask = null; //스레드를 추적하는 객체

        //왜 테스크를 쓰냐 
        /*
         효율적인 일꾼 관리 (1초 쉬기 ..) , 해당 시간이 끝난 뒤에 남은 작업을 처리할 수 있음
        */

        public static void Init()
        {
            //여기서 스레드 풀에서 하나의 스레드를 제공받아서 해당 스레드가 UpdateTime을 따로 돌림
            m_refTimeTask = Task.Run(UpdateTime);
        }

        //최소한의 일꾼(스레드)으로 최대한의 효율을 내기 위해
        /*
        Thread.Sleep이나 lock 대기 같은 녀석들은 일꾼을 일 안 하고 놀게 만듭니다.
        반면 async/await는 대기 시간이 생기면 일꾼을 즉시 다른 작업에 투입할 수 있게 순환시켜 줍니다
        위에서 아래로 흐르는 평범한 코드처럼 편하게 짤 수 있습니다.
         */

        static async void UpdateTime()
        {
            while (true)
            {
                // 1. 정확히 1초(1000밀리초) 동안 비동기 대기
                // 스레드를 차단(Block)하지 않고 잠시 권한을 양보합니다.
                await Task.Delay(1000);

                //가시성, 원자성을 보장을 받음
                //Interlocked 연산이 하드웨어 레벨에서 '메모리 장벽(Memory Barrier)'을 치기 때문
                /*
                캐시 일관성(Cache Coherency) 발동:
                한 CPU 코어가 메모리 값을 바꾸면, 하드웨어(CPU) 수준에서 다른 모든
                코어의 캐시에 있는 해당 주소 값을 "야, 그거 무효(Invalid)"라고 마킹해 버립니다.
                */
                //얘는 디버깅용
                long lCurValue = Interlocked.Increment(ref Counter);
            }
        }
        
        public static long Time()
        {
            return Interlocked.Read(ref Counter);
        }

    }

}
