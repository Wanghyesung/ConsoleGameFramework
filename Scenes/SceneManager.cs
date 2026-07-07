using ConsoleGameFramework.Core;
using ConsoleGameFramework.Models;
using ConsoleGameFramework.Scenes;
using ConsoleGameFramework_KR.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameFramework_KR.Scenes
{
    public class SceneManager
    {
        private readonly Dictionary<SceneKey, SceneBase> _scenes = new Dictionary<SceneKey, SceneBase>();
        private SceneBase _currentScene;

        public static SceneManager Instance { get; } = new SceneManager();

        private GameContext m_refContext = null;
        private SceneManager() { }

        public SceneBase CurrentScene => _currentScene;
        private void AddScene(SceneBase scene)
        {
            _scenes[scene.Key] = scene;
        }

        public void ChangeScene(SceneKey key)
        {
            if (!_scenes.TryGetValue(key, out SceneBase nextScene))
            {
                m_refContext.AddLog($"등록되지 않은 화면입니다: {key}");
                return;
            }

            _currentScene?.Exit(m_refContext);
            _currentScene = nextScene;
            _currentScene.Enter(m_refContext);
        }

        public void Init(GameContext _refContext)
        {
            m_refContext = _refContext;

            AddScene(new TitleScene());
            AddScene(new SampleScene());
            AddScene(new MainScene());
            _currentScene = new MainScene();
            _currentScene.Init(_refContext);

        }

     

        //public void Update(GameContext _refContext)
        //{
            //CurrentScene?.Update(_refContext);
        //}
    }
}
