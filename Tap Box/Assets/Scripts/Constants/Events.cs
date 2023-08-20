namespace Constants
{
    public static class Events
    {
        public const string OnBoxRemoveFromGameField = "OnBoxRemoveFromGameField";
        public const string OnBoxClicked = "OnBoxClicked";
        public const string OnLevelCreated = "OnLevelCreated";
        public const string OnGameLoose = "OnGameLoose";
        public const string OnGetRandomSkin = "OnGetRandomSkin";
        public const string OnRewardedVideoReward = "OnRewardedVideoReward";
        
        public const string OnGetNewSkin = "OnGetNewSkin";
        public const string OnEquipSkin = "OnEquipSkin";

        
        public const string OnBackgroundSpriteChanged = "OnBackgroundSpriteChanged";
        public const string OnBackgroundMaterialChanged = "OnBackgroundMaterialChanged";
        
        
        public const string OnTapSkinChanged = "OnTapSkinChanged";
        public const string OnTapShow = "OnTapShow";
        
        public const string OnTailSkinChanged = "OnTailSkinChanged";
        public const string OnTailStart = "OnTailStart";
        
        public const string OnPlaySound = "OnPlaySound";


    }

    public static class AnalyticsEvent
    {
        public const string FirstOpenLevel = "first_level_open";
        public const string OpenLevel = "level_open";
        public const string FirstWinLevel = "first_level_finish";
        public const string WinLevel = "level_finish";
        
        
        public const string LevelIdParameter = "level_name";

    }
    
    public static class IAP
    {
        public const string PurchaseSuccess = "PurchaseSuccess";
        public const string NoAds = "com.gyrogame.tapbox.noads";

    }

    public static class Sounds
    {
        public static class UI
        {
            public const string WinWindowShow = "WinWindowShow";
            public const string LoseWindowShow = "LoseWindowShow";
            public const string NewLevelWindowShow = "NewLevelWindowShow";
            public const string WinWindowGetReward = "WinWindowGetReward";
            public const string WinWindowGetSkinReward = "WinWindowGetSkinReward";
        }
        
        public static class Game
        {
            public const string TapOnBox = "TapOnBox";

        }

        
    }
}