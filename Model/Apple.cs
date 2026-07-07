using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework_KR.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework_KR.Model
{
    public class Apple : Entity
    {
        public Apple(Vec2 pos) : base(pos, Layer.Apple)
        {

        }
        public override void Init(GameContext context)
        {
            
        }
        
        private void Delete(Vec2 _vDummy)
        {
            SceneManager.Instance.CurrentScene.DeleteEntity(this);
        }
    }
}
