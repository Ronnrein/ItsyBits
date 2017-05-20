using System.Collections.Generic;

namespace ItsyBits.Models.ViewModels.Store {

    /// <summary>
    /// View model for the store
    /// </summary>
    public class StoreIndexViewModel {
        
        /// <summary>
        /// Animals to buy
        /// </summary>
        public IEnumerable<AnimalType> AnimalTypes { get; set; }

        /// <summary>
        /// Buildings to buy
        /// </summary>
        public IEnumerable<BuildingType> BuildingTypes { get; set; }
        
        /// <summary>
        /// Upgrades to buy
        /// </summary>
        public IEnumerable<Upgrade> Upgrades { get; set; }

    }
}