using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace UnityFixes
{

    public class DisabledMenusFixer
    {

        [InitializeOnLoadMethod]
        static void SetupCallback()
        {
            EditorApplication.playmodeStateChanged -= PlayModeChanged;
            EditorApplication.playmodeStateChanged += PlayModeChanged;
        }

        static void PlayModeChanged()
        {
            if (!EditorApplication.isPlaying)
            {
                Fix();
            }
        }

        [DidReloadScripts]
        static void Fix()
        {
            // Nothing inspected? try later
            if (Selection.activeGameObject == null)
            {
                EditorApplication.delayCall += Fix;
                return;
            }
            // Do the trick
            EditorApplication.ExecuteMenuItem("Component/Add...");
            if (EditorWindow.focusedWindow != null)
            {
                EditorWindow.focusedWindow.SendEvent(Event.KeyboardEvent("escape"));
            }
        }

    }

}