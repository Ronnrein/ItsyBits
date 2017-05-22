﻿namespace ItsyBits.ViewModels {
    public class UpgradeViewModel {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SpritePath { get; set; }
        public int Price { get; set; }
        public bool ForBuilding { get; set; }
        public bool ForAnimal { get; set; }

    }
}