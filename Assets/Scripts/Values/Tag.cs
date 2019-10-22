namespace Values
{
    public class Tag
    {
        //TagがStringでスペルミスしやすいので，不具合抑制のため
        private static string _player = "Player";
        private static string _enemy = "Enemy";
        private static string _largeEnemy = "LargeEnemy";
        private static string _largeEnemyWepon = "LargeEnemyWepon";
        private static string _gameController = "GameController";
        private static string _uiHome = "UIHome";
        private static string _uiStageSelect = "UIStageSelect";
        private static string _uiCustom = "UICustom";
        private static string _floor = "Floor";
        private static string _groundAndWall = "GroundAndWall";
        private static string _uiSettings = "UISettings";

        public static string Player
        {
            get { return _player; }
        }

        public static string Enemy
        {
            get { return _enemy; }
        }

        public static string LargeEnemy
        {
            get { return _largeEnemy; }
        }

        public static string LargeEnemyWepon
        {
            get { return _largeEnemyWepon; }
        }

        public static string GameController
        {
            get { return _gameController; }
        }

        public static string UiHome
        {
            get { return _uiHome; }
        }

        public static string UiStageSelect
        {
            get { return _uiStageSelect; }
        }

        public static string UiCustom
        {
            get { return _uiCustom; }
        }

        public static string Floor
        {
            get { return _floor; }
        }

        public static string GroundAndWall
        {
            get { return _groundAndWall; }
        }

        public static string UiSettings
        {
            get { return _uiSettings; }
        }
    }
}