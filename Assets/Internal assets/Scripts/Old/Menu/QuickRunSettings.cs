namespace Old.Menu
{
    public class QuickRunSettings
    {
        private readonly UIQuickRunSettings _uiQuickRunSettings = new();

        private bool _isToggleMobe = false;
        private bool _isTimer = false;
        private readonly int[] _sizeMaze = new int[2];


        private QuickRunSettings()
        {
            this._isToggleMobe = _uiQuickRunSettings.isToggleMobe;
            this._isTimer = _uiQuickRunSettings.isTimer;
            this._sizeMaze[0] = SizeMazeY();
            this._sizeMaze[1] = SizeMazeX();
        }

        private int SizeMazeY()
        {
            var random = new System.Random();
            return _uiQuickRunSettings.sizeMaze switch
            {
                "Small" => 10,
                "Middle" => 15,
                "Large" => 20,
                "Return" => random.Next(10, 20),
                "Custom" => 0,
                _ => 0
            };
        }
        private int SizeMazeX()
        {
            var random = new System.Random();
            return _uiQuickRunSettings.sizeMaze switch
            {
                "Small" => 10,
                "Middle" => 15,
                "Large" => 20,
                "Return" => random.Next(10, 20),
                "Custom" => 0,
                _ => 0
            };
        }
    }
}
