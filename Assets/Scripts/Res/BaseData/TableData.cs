// using BaseData;
// using SimpleJSON;
// using UnityEngine;
//
// public class TableData
// {
//     private static Tables _tables;
//
//     public static Tables Tables
//     {
//         get
//         {
//             if(_tables == null)
//                 InitData();
//             return _tables;
//         }
//     }
//
//     public static void InitData()
//     {
//         _tables = new Tables(name =>
//         {
//             var str = Addressables.LoadAssetAsync<TextAsset>(name);
//             var o = str.WaitForCompletion();
//             var res = JSONNode.Parse(o.ToString());
//             Addressables.Release(str);
//             return res;
//             return null;
//         });
//     }
// }
