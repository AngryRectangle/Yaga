﻿using System;
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
            var files = Directory.GetFiles(Application.dataPath, name, SearchOption.AllDirectories);
            foreach (var file in files.Select(file => file.Replace("\\", "/")
                         .Remove(0, Application.dataPath.Length - Application.dataPath.Split('/').Last().Length)))
            {
                var t = AssetDatabase.LoadAssetAtPath<T>(file);
                if (t != null)
                    return t;
            }

            throw new Exception($"Asset with name {name} not found");
        }
    }
}