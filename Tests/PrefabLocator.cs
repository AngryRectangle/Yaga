using UnityEngine;
using Yaga.Test.Documentation;

namespace Yaga.Test
{
    [CreateAssetMenu(fileName = "TestPrefabLocator", menuName = "PrefabLocator", order = 0)]
    public class PrefabLocator : ScriptableObject
    {
        public Canvas canvasPrefab;
        public ModelessView modelessView;
        public SimpleTextButtonView simpleTextButtonView;
    }
}