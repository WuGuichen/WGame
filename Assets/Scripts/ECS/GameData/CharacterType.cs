
namespace TWY.GameData
{
    public class CharacterType
    {
        public const int Player = 0;
        public const int Skeleton_Male = 1;
        public const int Default = 2;
        public const int MaxTypeID = 4;

        private static CharacterResourcePath path = new CharacterResourcePath();
        public static CharacterResourcePath Path => path;

        public struct CharacterResourcePath
        {
            public string this[int i]
            {
                get
                {
                    return i switch
                    {
                        Player => "Assets/Prefabs/Character/Player/Player.prefab",
                        Skeleton_Male => "Assets/Prefabs/Character/Skeleton_Male/Skeleton_Male.prefab", 
                        Default => "Assets/Prefabs/Character/BaseCharacter/BaseCharacter.prefab",
                        _ => ""
                    };
                }
            }
        }
    }
}