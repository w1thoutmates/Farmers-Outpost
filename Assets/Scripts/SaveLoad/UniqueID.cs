using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _id;

    public string ID => _id;

    void Awake()
    {
        if (Application.isPlaying)
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString();
            }
            return;
        }

#if UNITY_EDITOR
        VerifyIDInEditor();
#endif
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;

        UnityEditor.EditorApplication.delayCall += VerifyIDInEditor;
    }

    private void VerifyIDInEditor()
    {
        if (this == null) return;

        if (string.IsNullOrEmpty(_id))
        {
            _id = Guid.NewGuid().ToString();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
            return;
        }

        UniqueID[] allIds = FindObjectsByType<UniqueID>();
        
        foreach (var other in allIds)
        {
            if (other != this && other._id == this._id)
            {
                if (UnityEditor.Selection.activeGameObject == this.gameObject)
                {
                    _id = Guid.NewGuid().ToString();
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
                    Debug.Log($"[UniqueID] Обнаружен дубликат объекта! Сгенерирован новый ID: {_id}", this.gameObject);
                    break;
                }
            }
        }
    }
#endif
}