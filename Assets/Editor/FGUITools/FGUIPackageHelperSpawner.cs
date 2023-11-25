using System.Collections.Generic;
using System.IO;
using Bright.Serialization;

namespace WGame.UIEditor
{
    public static class FGUIPackageHelperSpawner
    {
        public static void GenerateMappingFile()
        {
            var ids = new List<string>();
            var names = new List<string>();
            foreach (PackageInfo packageInfo in FGUICodeSpawner.PackageInfos.Values)
            {
                ids.Add(packageInfo.Id);
                names.Add(packageInfo.Name);
            }
            
            ByteBuf byteBuf = new ByteBuf();
            byteBuf.WriteInt(ids.Count);
            for (int i = 0; i < ids.Count; i++)
            {
                byteBuf.WriteString(ids[i]);
                byteBuf.WriteString(names[i]);
            }
            
            string filePath = $"Assets/Bundles/Gen/UIPackageMapping.bytes";
            using FileStream fs = new FileStream(filePath, FileMode.Create);
            fs.Write(byteBuf.Bytes, 0, byteBuf.Bytes.Length);
        }
    }
}