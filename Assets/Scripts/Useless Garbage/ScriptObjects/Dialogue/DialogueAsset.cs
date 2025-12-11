using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class DialogueAsset : ScriptableObject
{
    [TextArea]
    public string[] dialogue;
}
