namespace PropertyGrid.Demo.Model
{



    public class Model
    {
        //public Guid Guid => Guid.Parse("1edf3c6d-1424-4771-8672-0b77d7c44342");

        public Game CurrentGame { get; set; }
        public Game PreviousGame { get; set; }

        public Animation Animation { get; set; }
        public Game StartGame { get; set; }
        public Game EndGame { get; set; }

        public PrizeWheelUpdate PrizeWheelUpdate { get; set; }

        public ScreenSaver ScreenSaver { get; set; }
    }


    public class Game
    {
        public string Name { get; set; }

        public List<Hole> Holes { get; set; }
    }

    public class Hole
    {
        public string Course { get; set; }
        public string HoleNumber { get; set; }
        public string HoleName { get; set; }
        public int Points { get; set; }
        public bool Supertube { get; set; }
        public bool Hazard { get; set; }
        public bool HoleInOne { get; set; }


    }

    public class Animation
    {
        public bool SuperTube { get; set; }
        public bool HoleInOne { get; set; }
        public bool Hazard { get; set; }
        public PointCalculator PointCalculator { get; set; }
        public bool ShotTaken { get; set; }
    }

    public class PointCalculator
    {
        public int Points { get; set; }
        public int Sequence { get; set; }
    }

    public class PrizeWheelUpdate
    {
        public int SegmentSequence { get; set; }
        public string Name { get; set; }
        public int ColourScheme { get; set; }
        public bool PrintTicket { get; set; }
        public int Size { get; set; }
        public bool AllowWin { get; set; }
    }

    public class ScreenSaver
    {
        public string FileName { get; set; }
    }
}
