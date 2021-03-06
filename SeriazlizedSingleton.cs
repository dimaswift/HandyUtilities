﻿using UnityEngine;
using System.Collections;

namespace HandyUtilities
{
    public abstract class SerializedSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    var path = "Assets/" + typeof(T).Name + ".asset";
                    _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    if (_instance == null)
                    {
                        UnityEditor.AssetDatabase.CreateAsset(CreateInstance<T>(), path);
                        _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                    }
                }
#else
                 _instance = Resources.Load<T>("Resources/" + typeof(T).Name + ".asset");
#endif
                return _instance;
            }
        }
    }

}

