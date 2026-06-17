using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class UniqueID : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _id;

    [SerializeField] private static SerializableDictionary<string, GameObject> _idDatabase = 
        new SerializableDictionary<string, GameObject>();

    public string ID => _id;

    void Awake()
    {
        if (_idDatabase == null) _idDatabase = new SerializableDictionary<string, GameObject>();
        
        if (_idDatabase.ContainsKey(_id)) Generate();
        else _idDatabase.Add(_id, this.gameObject);
    }

    // void OnValidate()
    // {
    //     if (_idDatabase.ContainsKey(_id)) Generate();
    //     else _idDatabase.Add(_id, this.gameObject);
    // }

    void OnDestroy()
    {
        if (_idDatabase.ContainsKey(_id)) _idDatabase.Remove(_id);
    }

    void Generate()
    {
        _id = Guid.NewGuid().ToString();
        _idDatabase.Add(_id, this.gameObject);
    }
}
