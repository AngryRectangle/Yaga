using UnityEditor;
using Yaga.Test;

namespace Tests
{
    public class BaseUiTest
    {
        private PrefabLocator _locator;

        public PrefabLocator Locator
        {
            get
            {
                if(_locator is null)
                    _locator = AssetDatabase.LoadAssetAtPath<PrefabLocator>("Assets/Tests/TestPrefabLocator.asset");
                return _locator;
            }
        }
    }
}