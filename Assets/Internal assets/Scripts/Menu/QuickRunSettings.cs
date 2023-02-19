namespace Menu
{
    public class QuickRunSettings
    {
        UIQuickRunSettings uiQuickRunSettings = new UIQuickRunSettings();

        public bool isToggleMobe = false;
        public bool isTimer = false;
        public int[] sizeMaze = new int[2];


        QuickRunSettings()
        {
            this.isToggleMobe = uiQuickRunSettings.isToggleMobe;
            this.isTimer = uiQuickRunSettings.isTimer;
            this.sizeMaze[0] = SizeMazeY();
            this.sizeMaze[1] = SizeMazeX();
        }

        private int SizeMazeY()
        {
            System.Random random = new System.Random();
            switch (uiQuickRunSettings.SizeMaze)
            {
                case "Small":
                    return 10;
                case "Middle":
                    return 15;
                case "Large":
                    return 20;
                case "Return":
                    return random.Next(10, 20); 
                case "Custom":
                    return 0;
            }
            return 0;
        }
        private int SizeMazeX()
        {
            System.Random random = new System.Random();
            switch (uiQuickRunSettings.SizeMaze)
            {
                case "Small":
                    return 10;
                case "Middle":
                    return 15;
                case "Large":
                    return 20;
                case "Return":
                    return random.Next(10, 20);
                case "Custom":
                    return 0;
            }
            return 0;
        }
    }
}
