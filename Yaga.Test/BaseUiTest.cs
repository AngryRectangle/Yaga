using NUnit.Framework;
using UnityEditor;

namespace Yaga.Test
{
    public class BaseUiTest
    {
        private PrefabLocator _locator;

        public PrefabLocator Locator
        {
            get
            {
                if(_locator is null)
                    _locator = AssetDatabase.LoadAssetAtPath<PrefabLocator>("Assets/Yaga.Test/TestPrefabLocator.asset");
                return _locator;
            }
        }
        
        [SetUp]
        public void SetUp()
        {
            UiBootstrap.InitializeSingleton();
            UiControl.InitializeSingleton(Locator.canvasPrefab);
        }
    }
}