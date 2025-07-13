using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dartwint.UnityExtensions.IOPackageManager
{
    public class AssetTrackerLoggerWindow : EditorWindow
    {
        private static Dictionary<Package, List<Message>> _messagesDicrionary = new();

        private static Vector2 _windowScrollPosition;

        private static List<Vector2> _scrollPositions = new();

        private struct Message
        {
            public MessageType Type { get; private set; }

            private string _text;
            private string _sender;
            private DateTime _dateTime;

            public Message(string text, string sender, DateTime dateTime, MessageType type)
            {
                _text = text;
                _sender = sender;
                _dateTime = dateTime;
                Type = type;
            }

            public override string ToString()
            {
                return _text + " [" + _dateTime.ToString("HH:mm:ss") + "]";
            }
        }

        private class MessageComparer : IComparer<Message>
        {
            public int Compare(Message x, Message y)
            {
                return (int) x.Type - (int) y.Type;
            }
        }

        public enum MessageType { None = 1, Warning = 0 };

        [MenuItem("Tools/Debug/Asset Tracker Logger")]
        public static void ShowWindow()
        {
            GetWindow<AssetTrackerLoggerWindow>("Asset Tracker Logger", typeof(SceneView));
        }

        public static void LogMessage(string messageText, Package sender, MessageType messageType = MessageType.None)
        {
            if (sender == null) return;

            if (!_messagesDicrionary.ContainsKey(sender))
                _messagesDicrionary.Add(sender, new List<Message>());

            _messagesDicrionary[sender].Add(new Message(messageText, sender.name, DateTime.Now, messageType));

            MessageComparer comparer = new MessageComparer();
            _messagesDicrionary[sender].Sort(comparer);

            UpdateViews();
        }

        private void OnEnable() => EditorApplication.update += OnEditorUpdate;

        private void OnDisable() => EditorApplication.update -= OnEditorUpdate;

        private void OnEditorUpdate() => Repaint();

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            bool result;
            if (result = GUILayout.Button("Clear"))
            {
                _messagesDicrionary.Clear();
            }
            GUILayout.EndHorizontal();
            if (result) return;

            _windowScrollPosition = GUILayout.BeginScrollView(_windowScrollPosition, GUILayout.ExpandHeight(true));

            DrawFoldouts();

            GUILayout.EndScrollView();
        }

        private static List<bool> _foldouts = new();
        private static void DrawFoldouts()
        {
            int i = 0;
            foreach (Package package in _messagesDicrionary.Keys)
            {
                _foldouts[i] = EditorGUILayout.Foldout(_foldouts[i], package.name);
                if (_foldouts[i])
                {
                    _scrollPositions[i] = GUILayout.BeginScrollView(_scrollPositions[i]);

                    foreach (Message message in _messagesDicrionary[package])
                    {
                        GUILayout.BeginHorizontal();

                        if (message.Type == MessageType.Warning)
                        {
                            GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon.sml"), GUILayout.Width(15));
                        }

                        GUILayout.Label(new GUIContent(message.ToString()), GUILayout.ExpandWidth(true));

                        GUILayout.EndHorizontal();

                        GUILayout.Space(2);
                    }

                    GUILayout.EndScrollView();
                }

                i++;
            }
        }

        private static void UpdateViews()
        {
            if (_foldouts.Count < _messagesDicrionary.Keys.Count)
            {
                for (int i = 0; i < _messagesDicrionary.Keys.Count - _foldouts.Count; i++)
                {
                    _foldouts.Add(true);
                }
            }

            if (_scrollPositions.Count < _messagesDicrionary.Keys.Count)
            {
                for (int i = 0; i < _messagesDicrionary.Keys.Count - _scrollPositions.Count; i++)
                {
                    _scrollPositions.Add(new Vector2());
                }
            }
        }
    }
}