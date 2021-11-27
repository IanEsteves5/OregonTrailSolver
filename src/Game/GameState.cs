using System;

namespace OregonTrail.Game
{
    public class GameState
    {
        public int ShootingExpertise_D9 { get; }

        public int Animals_A { get; set; }
        public int Food_F { get; set; }
        public int Bullets_B { get; set; }
        public int Clothing_C { get; set; }
        public int MiscSupplies_M1 { get; set; }
        public int Cash_T { get; set; }

        public int Mileage_M { get; set; }
        public int PreviousMileage_M2 { get; set; }

        public int ChoiceOfEating_E { get; set; }

        public bool Illness_S4 { get; set; }
        public bool Injury_K8 { get; set; }
        public bool ClearingSouthPassSettingMileage_M9 { get; set; }
        public bool AtFort_X1 { get; set; }
        public bool SouthPassCleared_F1 { get; set; }
        public bool BlueMountainsCleared_F2 { get; set; }

        public int TurnNumber_D3 { get; set; }
        public DateTime CurrentDate { get; set; }

        public bool GameOver { get; set; }
        public bool GameWon { get; set; }

        public GameState(int shootingExpertise, int animals, int food,
            int bullets, int clothing, int miscSupplies, int cash)
        {
            ShootingExpertise_D9 = shootingExpertise;

            Animals_A = animals;
            Food_F = food;
            Bullets_B = bullets;
            Clothing_C = clothing;
            MiscSupplies_M1 = miscSupplies;
            Cash_T = cash;

            TurnNumber_D3 = -1;
            CurrentDate = new DateTime(1847, 3, 29);
        }
    }
}
