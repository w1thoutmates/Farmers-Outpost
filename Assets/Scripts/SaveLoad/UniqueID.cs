using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [ReadOnly] [SerializeField] private string _id;

    private static SerializableDictionary<string, GameObject> _idDatabase = 
        new SerializableDictionary<string, GameObject>();

    public string ID => _id;

    void Awake()
    {
        InitID();
    }

    void OnValidate()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += InitID;
        #endif
    }

    void OnDestroy()
    {
        if (_idDatabase != null && _idDatabase.ContainsKey(_id)) 
        {
            if (_idDatabase[_id] == this.gameObject)
            {
                _idDatabase.Remove(_id);
            }
        }
    }

    private void InitID()
    {
        if (this == null) return; 

        if (_idDatabase == null) 
            _idDatabase = new SerializableDictionary<string, GameObject>();

        if (string.IsNullOrEmpty(_id) || (_idDatabase.ContainsKey(_id) && _idDatabase[_id] != this.gameObject))
        {
            Generate();
        }
        else if (!_idDatabase.ContainsKey(_id))
        {
            _idDatabase.Add(_id, this.gameObject);
        }
    }

    void Generate()
    {
        if (_idDatabase != null && _idDatabase.ContainsKey(_id) && _idDatabase[_id] == this.gameObject)
        {
            _idDatabase.Remove(_id);
        }

        _id = Guid.NewGuid().ToString();
        _idDatabase.Add(_id, this.gameObject);
        
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            string currentId = _id;
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(this.gameObject.scene);
        }
        #endif
    }
}