using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Yaga.Test;
using Object = UnityEngine.Object;

namespace Tests
{
    public class BaseUiTest
    {
        private PrefabLocator _locator;

        public PrefabLocator Locator =>
            _locator ? _locator : _locator = GetFirstWithName<PrefabLocator>("TestPrefabLocator.asset");

        private T GetFirstWithName<T>(string name)
            where T : Object
        {
            var path = AssetDatabase.FindAssets("TestPrefabLocator").Select(AssetDatabase.GUIDToAssetPath).Single();
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
    }
}