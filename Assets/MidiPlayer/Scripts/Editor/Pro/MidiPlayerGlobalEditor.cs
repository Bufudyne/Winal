﻿using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using MEC;

namespace MidiPlayerTK
{
    /// <summary>@brief
    /// Inspector for the midi global player component
    /// </summary>
    [CustomEditor(typeof(MidiPlayerGlobal))]
    [InitializeOnLoad]
    public class MidiPlayerGlobalEditor : Editor
    {
        private SerializedProperty CustomOnEventPresetLoaded;
        private bool showEvents;
        private static MidiPlayerGlobal instance;
        private MidiCommonEditor commonEditor;
        // Manage skin
        public CustomStyle myStyle;

        private static string lastDirectory = "";
        private Texture buttonIconFolder;
        private Texture buttonIconDelete;
        private static GUILayoutOption miniButtonWidth = GUILayout.Width(20f);
        private MessagesEditor messages;

        void OnEnable()
        {
            try
            {
                //Debug.Log("OnEnable MidiFilePlayerEditor");
                CustomOnEventPresetLoaded = serializedObject.FindProperty("InstanceOnEventPresetLoaded");
                instance = (MidiPlayerGlobal)target;
                buttonIconFolder = Resources.Load<Texture2D>("Textures/Folders");
                buttonIconDelete = Resources.Load<Texture2D>("Textures/Delete_32x32");
                messages = new MessagesEditor();

                if (!Application.isPlaying)
                {
                    // Load description of available soundfont
                    if (MidiPlayerGlobal.CurrentMidiSet == null || MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo == null)
                    {
                        MidiPlayerGlobal.InitPath();
                        ToolsEditor.LoadMidiSet();
                        ToolsEditor.CheckMidiSet();
                    }
                }

            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
        public override void OnInspectorGUI()
        {
            if (instance == null)
                return;
            try
            {
                GUI.changed = false;
                GUI.color = Color.white;
                // Set custom Style. Good for background color 3E619800
                if (myStyle == null) myStyle = new CustomStyle();

                string soundFontSelected = ".";
                if (commonEditor == null) commonEditor = ScriptableObject.CreateInstance<MidiCommonEditor>();
                commonEditor.DrawCaption("Midi Player Global - Manage all global features.", "https://paxstellar.fr/midiplayerglobal/", "d7/dc4/class_midi_player_t_k_1_1_midi_player_global.html#details");

                // Not efficient ... see more later
                //string pathR = EditorGUILayout.TextField(new GUIContent("Path To Resources", ""), instance.PathToResources);
                //if (pathR != instance.PathToResources)
                //{
                //    instance.PathToResources = pathR;
                //    MidiPlayerGlobal.InitPath();
                //}
                //EditorGUILayout.BeginHorizontal();
                //if (GUILayout.Button(new GUIContent("Set to default",""), EditorStyles.miniButtonRight))//, miniButtonWidth, GUILayout.Height(20f)))
                //{
                //    instance.PathToResources =null;
                //    MidiPlayerGlobal.InitPath();
                //}
                //if (GUILayout.Button(new GUIContent("Delete", ""), EditorStyles.miniButtonRight))//, miniButtonWidth, GUILayout.Height(20f)))
                //{
                //}
                //EditorGUILayout.EndHorizontal();


                if (MidiPlayerGlobal.CurrentMidiSet != null && MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo != null)
                {
                    SoundFontInfo sfi = MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo;

                    EditorGUILayout.Separator();
                    // Display popup to change SoundFont
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Current SoundFont", "SoundFont selected to play sound"), GUILayout.Width(150));

                    soundFontSelected = sfi.Name;
                    // SF is loaded in a coroutine, forbidden in edit mode
                    int selectedSFIndex = MidiPlayerGlobal.MPTK_ListSoundFont.FindIndex(s => s == soundFontSelected);
                    int newSelectSF = EditorGUILayout.Popup(selectedSFIndex, MidiPlayerGlobal.MPTK_ListSoundFont.ToArray());
                    if (newSelectSF != selectedSFIndex)
                    {
                        MidiPlayerGlobal.MPTK_SelectSoundFont(MidiPlayerGlobal.MPTK_ListSoundFont[newSelectSF]);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                instance.LoadSoundFontAtStartup = EditorGUILayout.Toggle(
                    new GUIContent("Load SoundFont at startup", "If enabled load the default SoundFont is loaded when application is started."),
                    instance.LoadSoundFontAtStartup);

                instance.LoadWaveAtStartup = EditorGUILayout.Toggle(
                    new GUIContent("Load Samples at startup (no core mode)", "Load all samples for non core mode when application is started else load when needed at playing."),
                    instance.LoadWaveAtStartup);

                string helpuri = "Define an url (prefix with http:// or https:/) or a full path to a local file (prefix with file://).";
                EditorGUILayout.LabelField(new GUIContent("SoundFont URL or file path:", helpuri));

                EditorGUILayout.BeginHorizontal();
                // Select a soundfont from the desktop
                bool setByBrowser = false;
                EditorGUILayout.BeginVertical(GUILayout.Width(22));
                if (GUILayout.Button(new GUIContent(buttonIconFolder, "Browse"), EditorStyles.miniButtonRight, miniButtonWidth, GUILayout.Height(20f)))
                {
                    string path = EditorUtility.OpenFilePanel("Select a SoundFont file", lastDirectory, "sf2");
                    if (!string.IsNullOrEmpty(path))
                    {
                        lastDirectory = Path.GetDirectoryName(path);
                        instance.MPTK_LiveSoundFont = "file://" + path;
                        setByBrowser = true;
                    }
                }
                if (GUILayout.Button(new GUIContent(buttonIconDelete, "Reset"), EditorStyles.miniButtonRight, miniButtonWidth, GUILayout.Height(20f)))
                {
                    instance.MPTK_LiveSoundFont = "";
                }
                EditorGUILayout.EndVertical();

                string newsf = EditorGUILayout.TextField(instance.MPTK_LiveSoundFont, myStyle.TextFieldMultiLine, GUILayout.Height(40f));
                if (newsf != instance.MPTK_LiveSoundFont || setByBrowser)
                {
                    instance.MPTK_LiveSoundFont = newsf;
                }
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button(new GUIContent("Load SoundFont", "Load SF")))
                {
                    if (string.IsNullOrEmpty(instance.MPTK_LiveSoundFont))
                        messages.Add("No SoundFont defined.");
                    else if (!Application.isPlaying)
                        messages.Add("Load SoundFont only when playing.");
                    else
                    {
                        MidiPlayerGlobal.MPTK_LoadLiveSF(instance.MPTK_LiveSoundFont);
                    }
                }

                EditorGUILayout.Separator();
                showEvents = EditorGUILayout.Foldout(showEvents, "Show Global Events");
                if (showEvents)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(CustomOnEventPresetLoaded);
                    serializedObject.ApplyModifiedProperties();
                    EditorGUI.indentLevel--;
                }

                messages.Display();

                showDefault = EditorGUILayout.Foldout(showDefault, "Show default editor");
                if (showDefault)
                {
                    EditorGUI.indentLevel++;
                    DrawDefaultInspector();
                    EditorGUI.indentLevel--;
                }

                if (GUI.changed) EditorUtility.SetDirty(instance);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
        private static bool showDefault = false;


    }

}
