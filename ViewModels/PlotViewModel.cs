namespace ItsyBits.ViewModels {
    public class PlotViewModel {

        public int Id { get; set; }
        public string Description { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public BuildingViewModel Building { get; set; }

    }
}