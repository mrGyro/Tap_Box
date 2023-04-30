namespace Constants
{
    public static class Events
    {
        public static string OnBoxRemoveFromGameField = "OnBoxRemoveFromGameField";
        public static string OnBoxClicked = "OnBoxClicked";
        public static string OnLevelCreated = "OnLevelCreated";
        public static string OnGameLoose = "OnGameLoose";
        public static string OnGetRandomSkin = "OnGetRandomSkin";
        public static string OnRewardedVideoReward = "OnRewardedVideoReward";
        
        
        public static string OnBackgroundSpriteChanged = "OnBackgroundSpriteChanged";
        public static string OnBackgroundMaterialChanged = "OnBackgroundMaterialChanged";
        
        
        public static string OnTapSkinChanged = "OnTapSkinChanged";
        public static string OnTapShow = "OnTapShow";
        
        public static string OnTailSkinChanged = "OnTailSkinChanged";
        public static string OnTailStart = "OnTailStart";
        
        public static string OnPlaySound = "OnPlaySound";


    }

    public static class Sounds
    {
        public static class UI
        {
            public static string WinWindowShow = "WinWindowShow";
            public static string LoseWindowShow = "LoseWindowShow";
            public static string NewLevelWindowShow = "NewLevelWindowShow";
        }
        
        public static class Game
        {
            public static string TapOnBox = "TapOnBox";

        }

        
    }
}