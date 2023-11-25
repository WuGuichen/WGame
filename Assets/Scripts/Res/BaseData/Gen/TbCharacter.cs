//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace BaseData
{ 

public sealed partial class TbCharacter
{
    private readonly Dictionary<int, CharacterData> _dataMap;
    private readonly List<CharacterData> _dataList;
    
    public TbCharacter(JSONNode _json)
    {
        _dataMap = new Dictionary<int, CharacterData>();
        _dataList = new List<CharacterData>();
        
        foreach(JSONNode _row in _json.Children)
        {
            var _v = CharacterData.DeserializeCharacterData(_row);
            _dataList.Add(_v);
            _dataMap.Add(_v.Id, _v);
        }
        PostInit();
    }

    public Dictionary<int, CharacterData> DataMap => _dataMap;
    public List<CharacterData> DataList => _dataList;

    public CharacterData GetOrDefault(int key) => _dataMap.TryGetValue(key, out var v) ? v : null;
    public CharacterData Get(int key) => _dataMap[key];
    public CharacterData this[int key] => _dataMap[key];

    public void Resolve(Dictionary<string, object> _tables)
    {
        foreach(var v in _dataList)
        {
            v.Resolve(_tables);
        }
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        foreach(var v in _dataList)
        {
            v.TranslateText(translator);
        }
    }
    
    
    partial void PostInit();
    partial void PostResolve();
}

}