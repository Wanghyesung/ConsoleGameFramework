//using ConsoleGameFramework.Core;
//using ConsoleGameFramework.UI;
//using System;

//public class BattleScene : SceneBase
//{
//    private static readonly List<MenuOption> Menu = new List<MenuOption>
//    {
//        new MenuOption(1, "공격","몬스터를 공격합니다."),
//        new MenuOption(0, "포기하고 종료")
//    };

//    public override SceneKey Key => SceneKey.Battle;

//    public override void Render(GameContext context)
//    {
//        ConsoleUI.Clear();
//        ConsoleUI.WriteTitle("전투씬", "스테이지 : 1");
//        ConsoleUI.WriteStatusBar(BattleManager.Instance.Player.Name, BattleManager.Instance.Player.Hp, BattleManager.Instance.Player.MaxHp); // 플레이어의 이름과 HP를 가져와야한다.
//        ConsoleUI.WriteStatusBar(BattleManager.Instance.Enemy.Name, BattleManager.Instance.Enemy.Hp, BattleManager.Instance.Enemy.MaxHp,fillColor:ConsoleColor.Red); // 적의 이름과 HP를 가져와야한다.

//        ConsoleUI.WriteMenu(Menu, "행동 메뉴");
//        ConsoleUI.WriteLog(context.Logs);
        
//    }

//    public override void HandleInput(GameContext context)
//    {
//        int choice = ConsoleUI.ReadMenuChoice(Menu);
//        switch (choice)
//        {
//            case 1:
//                // 플레이어의 공격 Enemy의 Take 데미지
//                BattleManager.BattleOutcome result = BattleManager.Instance.PlayerAttack();
//                if (result == BattleManager.BattleOutcome.Continuing)
//                    context.AddLog($"적이 반격했습니다 : 데미지 {BattleManager.Instance.Enemy.Attack}");
//                else if (result == BattleManager.BattleOutcome.Victory)
//                {
//                    context.AddLog($"Victory: {result}");
//                    // 마을씬으로 이동
//                }
//                else if (result == BattleManager.BattleOutcome.Defeat)
//                {
//                    context.AddLog("패배했습니다.");
//                    // 게임종료
//                }

//                break;

//            case 0:
//                context.Game.RequestQuit();
//                break;
//        }
//    }
//}
