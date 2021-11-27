using OregonTrail.Shared;
using System;
using System.Globalization;

namespace OregonTrail.Game
{
    public class GameLogic : IGame
    {
        private const decimal TwoThirds = 2m / 3m;

        private static readonly string[] ShootingInputs = new string[] { "BANG", "BLAM", "POW", "WHAM" };
        
        private readonly Random _random;
        private readonly InitialGameStateProvider _initialGameStateProvider;

        public GameLogic(Random random)
        {
            _random = random;
            _initialGameStateProvider = new InitialGameStateProvider();
        }

        public bool Play(IMessageChannel messageChannel)
        {
            ShowInstructions(messageChannel);
            var gameState = _initialGameStateProvider.GetInitialGameState(messageChannel);

            return GameLoop(gameState, messageChannel,
                StartNewDay,
                VisitDoctor,
                ShowStats,
                DailyAction,
                Eating,
                RidersAttack,
                DailyEvent);
        }

        private void ShowInstructions(IMessageChannel messageChannel)
        {
            string input;
            do
            {
                messageChannel.WriteLine("YOU NEED INSTRUCTIONS (YES/NO)");
                input = messageChannel.Read().ToUpper();
            }
            while (input != "YES" && input != "NO");
            
            if (input == "NO")
                return;

            messageChannel.WriteLine(
@"THIS PROGRAM SIMULATES A TRIP OVER THE OREGON TRAIL FROM
INDEPENDENCE, MISSOURI TO OREGON CITY, OREGON IN 1847.
YOUR FAMILY OF FIVE WILL COVER THE 2040 MILE OREGON TRAIL
IN 5-6 MONTHS --- IF YOU MAKE IT ALIVE.

YOU HAD SAVED $900 TO SPEND FOR THE TRIP, AND YOU'VE JUST
   PAID $200 FOR A WAGON.
YOU WILL NEED TO SPEND THE REST OF YOUR MONEY ON THE
   FOLLOWING ITEMS:

     OXEN - YOU CAN SPEND $200-$300 ON YOUR TEAM
            THE MORE YOU SPEND, THE FASTER YOU'LL GO
               BECAUSE YOU'LL HAVE BETTER ANIMALS

     FOOD - THE MORE YOU HAVE, THE LESS CHANCE THERE
               IS OF GETTING SICK

     AMMUNITION - $1 BUYS A BELT OF 50 BULLETS
            YOU WILL NEED BULLETS FOR ATTACKS BY ANIMALS
               AND BANDITS, AND FOR HUNTING FOOD

     CLOTHING - THIS IS ESPECIALLY IMPORTANT FOR THE COLD
               WEATHER YOU WILL ENCOUNTER WHEN CROSSING
               THE MOUNTAINS

     MISCELLANEOUS SUPPLIES - THIS INCLUDES MEDICINE AND
               OTHER THINGS YOU WILL NEED FOR SICKNESS
               AND EMERGENCY REPAIRS


YOU CAN SPEND ALL YOUR MONEY BEFORE YOU START YOUR TRIP -
OR YOU CAN SAVE SOME OF YOUR CASH TO SPEND AT FORTS ALONG
THE WAY WHEN YOU RUN LOW. HOWEVER, ITEMS COST MORE AT
THE FORTS. YOU CAN ALSO GO HUNTING ALONG THE WAY TO GET
MORE FOOD.
WHENEVER YOU HAVE TO USE YOUR TRUSTY RIFLE ALONG THE WAY,
YOU WILL BE TOLD TO TYPE IN A WORD (ONE THAT SOUNDS LIKE A 
GUN SHOT). THE FASTER YOU TYPE IN THAT WORD AND HIT THE
""RETURN"" KEY, THE BETTER LUCK YOU'LL HAVE WITH YOUR GUN.

AT EACH TURN, ALL ITEMS ARE SHOWN IN DOLLAR AMOUNTS
EXCEPT BULLETS
WHEN ASKED TO ENTER MONEY AMOUNTS, DON'T USE A ""$"".

GOOD LUCK!!!");
        }

        private bool GameLoop(GameState gameState, IMessageChannel messageChannel, params Action<GameState, IMessageChannel>[] actions)
        {
            for (;;)
            {
                foreach (var a in actions)
                {
                    a(gameState, messageChannel);
                    if (gameState.GameOver)
                        return false;
                    if (gameState.GameWon)
                        return true;
                }
            }
        }

        private void StartNewDay(GameState gameState, IMessageChannel messageChannel)
        {
            if (gameState.Mileage_M >= 2040)
            {
                gameState.GameWon = true;
                return;
            }

            gameState.TurnNumber_D3++;
            if (gameState.TurnNumber_D3 > 0)
                gameState.CurrentDate = gameState.CurrentDate.AddDays(7);

            messageChannel.WriteLine();
            messageChannel.WriteLine(gameState.CurrentDate.ToString("dddd MMMM d yyyy", CultureInfo.InvariantCulture).ToUpper());
            messageChannel.WriteLine();

            if (gameState.TurnNumber_D3 >= 20)
            {
                messageChannel.WriteLine(
@"YOU HAVE BEEN ON THE TRAIL TOO LONG  ------
YOUR FAMILY DIES IN THE FIRST BLIZZARD OF WINTER");
                gameState.GameOver = true;
                return;
            }

            if (gameState.Animals_A < 0)
                gameState.Animals_A = 0;

            if (gameState.Food_F < 0)
                gameState.Food_F = 0;

            if (gameState.Bullets_B < 0)
                gameState.Bullets_B = 0;

            if (gameState.Clothing_C < 0)
                gameState.Clothing_C = 0;

            if (gameState.MiscSupplies_M1 < 0)
                gameState.MiscSupplies_M1 = 0;

            if (gameState.Food_F < 13)
                messageChannel.WriteLine("YOU'D BETTER DO SOME HUNTING OR BUY FOOD AND SOON!!!!");

            gameState.PreviousMileage_M2 = gameState.Mileage_M;
        }

        private void VisitDoctor(GameState gameState, IMessageChannel messageChannel)
        {
            if (!gameState.Illness_S4 && !gameState.Injury_K8)
                return;

            gameState.Cash_T -= 20;

            if (gameState.Cash_T < 0)
            {
                messageChannel.WriteLine("YOU CAN'T AFFORD A DOCTOR");
                gameState.GameOver = true;
                return;
            }

            messageChannel.WriteLine("DOCTOR'S BILL IS $20");

            gameState.Illness_S4 = false;
            gameState.Injury_K8 = false;
        }

        private void ShowStats(GameState gameState, IMessageChannel messageChannel)
        {
            if (gameState.ClearingSouthPassSettingMileage_M9)
            {
                messageChannel.WriteLine("TOTAL MILEAGE: 950");
                gameState.ClearingSouthPassSettingMileage_M9 = false;
            }
            else
                messageChannel.WriteLine("TOTAL MILEAGE: {0}", gameState.Mileage_M);

            messageChannel.WriteLine("FOOD: {0}", gameState.Food_F);
            messageChannel.WriteLine("BULLETS: {0}", gameState.Bullets_B);
            messageChannel.WriteLine("CLOTHING: {0}", gameState.Clothing_C);
            messageChannel.WriteLine("MISC. SUPPLIES: {0}", gameState.MiscSupplies_M1);
            messageChannel.WriteLine("CASH: {0}", gameState.Cash_T);
        }
        
        private void DailyAction(GameState gameState, IMessageChannel messageChannel)
        {
            int input;
            
            do
            {
                if (gameState.AtFort_X1)
                {
                    messageChannel.WriteLine("DO YOU WANT TO (1) HUNT, OR (2) CONTINUE");
                    if (!messageChannel.TryReadInt(out input) || input < 1)
                        input = -1;
                    else
                        input++;
                }
                else
                {
                    messageChannel.WriteLine(
@"DO YOU WANT TO (1) STOP AT THE NEXT FORT, (2) HUNT, 
OR (3) CONTINUE");
                    if (!messageChannel.TryReadInt(out input) || input < 1)
                        input = -1;
                }
                
                if (input == 2 && gameState.Bullets_B < 40)
                    messageChannel.WriteLine("TOUGH---YOU NEED MORE BULLETS TO GO HUNTING");
            }
            while (input < 1 || input > 3 || input == 2 && gameState.Bullets_B < 40);

            switch (input)
            {
                case 1:
                    ActStopAtFort(gameState, messageChannel);
                    break;
                case 2:
                    ActHunting(gameState, messageChannel);
                    break;
            }
        }

        private void ActStopAtFort(GameState gameState, IMessageChannel messageChannel)
        {
            gameState.AtFort_X1 = true;

            messageChannel.WriteLine("ENTER WHAT YOU WISH TO SPEND ON THE FOLLOWING");

            int spent = SpendCash(gameState, messageChannel, "FOOD");
            if (spent > 0)
                gameState.Food_F += Convert.ToInt32(Math.Floor(spent * TwoThirds));

            spent = SpendCash(gameState, messageChannel, "AMMUNITION");
            if (spent > 0)
                gameState.Bullets_B += Convert.ToInt32(Math.Floor(spent * 50 * TwoThirds));

            spent = SpendCash(gameState, messageChannel, "CLOTHING");
            if (spent > 0)
                gameState.Clothing_C += Convert.ToInt32(Math.Floor(spent * TwoThirds));

            spent = SpendCash(gameState, messageChannel, "MISCELLANEOUS SUPPLIES");
            if (spent > 0)
                gameState.MiscSupplies_M1 += Convert.ToInt32(Math.Floor(spent * TwoThirds));

            gameState.Mileage_M -= 45;
        }

        private int SpendCash(GameState gameState, IMessageChannel messageChannel, string msg)
        {
            int input;
            do
            {
                messageChannel.WriteLine(msg);
                if (!messageChannel.TryReadInt(out input))
                    input = -1;
            }
            while (input < 0);
            
            if (input <= 0)
                return 0;

            int newCash = gameState.Cash_T - input;
            if (newCash < 0)
            {
                messageChannel.WriteLine("YOU DON'T HAVE THAT MUCH--KEEP YOUR SPENDING DOWN");
                input = 0;
            }
            else
                gameState.Cash_T = newCash;

            return input;
        }

        private void ActHunting(GameState gameState, IMessageChannel messageChannel)
        {
            gameState.Mileage_M -= 45;
            
            if (Shoot(messageChannel))
            {
                messageChannel.WriteLine(
@"RIGHT BETWEEN THE EYES---YOU GOT A BIG ONE!!!!
FULL BELLIES TONIGHT!");

                gameState.Food_F += 52 + _random.Next(7);
                gameState.Bullets_B -= 10 + _random.Next(5);
            }
            else
                messageChannel.WriteLine("YOU MISSED---AND YOUR DINNER GOT AWAY.....");
        }

        private bool Shoot(IMessageChannel messageChannel)
        {
            var expectedInput = _random.From(ShootingInputs);
            messageChannel.WriteLine("TYPE {0}", expectedInput);

            var input = messageChannel.Read();
            return string.Equals(expectedInput, input, StringComparison.InvariantCultureIgnoreCase);
        }

        private void Eating(GameState gameState, IMessageChannel messageChannel)
        {
            if (gameState.Food_F < 13)
            {
                messageChannel.WriteLine("YOU RAN OUT OF FOOD AND STARVED TO DEATH");
                gameState.GameOver = true;
                return;
            }

            int input;
            int newFood;

            do
            {
                newFood = gameState.Food_F;

                messageChannel.WriteLine(
@"DO YOU WANT TO EAT (1) POORLY  (2) MODERATELY
OR (3) WELL");

                if (!messageChannel.TryReadInt(out input))
                {
                    input = -1;
                    continue;
                }

                newFood -= 8 + 5 * input;

                if (newFood < 0)
                    messageChannel.WriteLine("YOU CAN'T EAT THAT WELL");
            }
            while (input < 1 || input > 3 || newFood < 0);

            gameState.ChoiceOfEating_E = input;
            gameState.Mileage_M += 200 + _random.Next(11) + (gameState.Animals_A - 220) / 5;
        }

        private void RidersAttack(GameState gameState, IMessageChannel messageChannel)
        {
            var x = gameState.Mileage_M / 100.0 - 4.0;
            var x2 = x * x;
            if (_random.NextDouble(10) > (x2 + 72.0) / (x2 + 12.0) - 1.0)
                return;

            messageChannel.Write("RIDERS AHEAD.  THEY ");
            var hostilityOfRiders_S5 = false;

            if (_random.NextDouble() <= 0.2)
            {
                messageChannel.Write("DON'T ");
                hostilityOfRiders_S5 = true;
            }

            messageChannel.WriteLine("LOOK HOSTILE");

            int input;

            do
            {
                messageChannel.WriteLine(
@"TACTICS
(1) RUN  (2) ATTACK  (3) CONTINUE  (4) CIRCLE WAGONS");

                if (_random.NextDouble() <= 0.8)
                    hostilityOfRiders_S5 = !hostilityOfRiders_S5;

                if (!messageChannel.TryReadInt(out input))
                    input = -1;
            }
            while (input < 1 || input > 4);

            if (hostilityOfRiders_S5)
                ActRidersAttackFriendly(gameState, messageChannel, input);
            else
                ActRidersAttackHostile(gameState, messageChannel, input);
        }

        private void ActRidersAttackFriendly(GameState gameState, IMessageChannel messageChannel, int action)
        {
            switch (action)
            {
                case 1:
                    gameState.Mileage_M += 15;
                    gameState.Animals_A -= 10;
                    break;
                case 2:
                    gameState.Mileage_M -= 5;
                    gameState.Bullets_B -= 100;
                    break;
                case 4:
                    gameState.Mileage_M -= 20;
                    break;
            }

            messageChannel.WriteLine("RIDERS WERE FRIENDLY, BUT CHECK FOR POSSIBLE LOSSES");
        }

        private void ActRidersAttackHostile(GameState gameState, IMessageChannel messageChannel, int action)
        {
            switch (action)
            {
                case 1:
                    gameState.Mileage_M += 20;
                    gameState.MiscSupplies_M1 -= 15;
                    gameState.Bullets_B -= 150;
                    gameState.Animals_A -= 40;
                    break;
                case 2:
                case 4:
                    if (action == 2)
                        gameState.Bullets_B -= 120;
                    else
                    {
                        gameState.Bullets_B -= 110;
                        gameState.Mileage_M -= 25;
                    }

                    if (Shoot(messageChannel))
                        messageChannel.WriteLine("NICE SHOOTING---YOU DROVE THEM OFF");
                    else
                    {
                        messageChannel.WriteLine(
@"LOUSY SHOT---YOU GOT KNIFED
YOU HAVE TO SEE OL' DOC BLANCHARD");
                        gameState.Injury_K8 = true;
                    }
                    break;
                case 3:
                    if (_random.NextDouble() <= 0.2)
                        messageChannel.WriteLine("THEY DID NOT ATTACK");
                    else
                    {
                        gameState.Bullets_B -= 150;
                        gameState.MiscSupplies_M1 -= 15;
                    }
                    break;
            }

            messageChannel.WriteLine("RIDERS WERE HOSTILE--CHECK FOR LOSES");
            if (gameState.Bullets_B < 0)
            {
                messageChannel.WriteLine("YOU RAN OUT OF BULLETS AND GOT MASSACRED BY THE RIDERS");
                gameState.GameOver = true;
            }
        }

        private void DailyEvent(GameState gameState, IMessageChannel messageChannel)
        {
            bool s;

            switch (_random.Next(17))
            {
                case 0:
                    messageChannel.WriteLine("WAGON BREAKS DOWN--LOSE TIME AND SUPPLIES FIXING IT");
                    gameState.Mileage_M -= 15 + _random.Next(6);
                    gameState.MiscSupplies_M1 -= 8;
                    break;
                case 1:
                    messageChannel.WriteLine("0X INJURES LEG---SLOWS YOU DOWN REST OF TRIP");
                    gameState.Mileage_M -= 25;
                    gameState.Animals_A -= 20;
                    break;
                case 2:
                    messageChannel.WriteLine(
@"BACK LUCK---YOUR DAUGHTER BROKE HER ARM
YOU HAD TO STOP AND USE SUPPLIES TO MAKE A SLING");
                    gameState.Mileage_M -= 5 + _random.Next(5);
                    gameState.MiscSupplies_M1 -= 2 + _random.Next(4);
                    break;
                case 3:
                    messageChannel.WriteLine("OX WANDERS OFF---SPEND TIME LOOKING FOR IT");
                    gameState.Mileage_M -= 17;
                    break;
                case 4:
                    messageChannel.WriteLine("YOUR SON GETS LOST---SPEND HALF THE DAY LOOKING FOR HIM");
                    gameState.Mileage_M -= 10;
                    break;
                case 5:
                    messageChannel.WriteLine("UNSAFE WATER--LOSE TIME LOOKING FOR CLEAN SPRING");
                    gameState.Mileage_M -= 2 + _random.Next(11);
                    break;
                case 6:
                    if (gameState.Mileage_M > 950)
                    {
                        messageChannel.Write("COLD WEATHER---BRRRRRRR!---YOU ");

                        var insufficientClothing_C1 = gameState.Clothing_C <= 22 + _random.Next(5);
                        if (insufficientClothing_C1)
                            messageChannel.Write("DONT'T ");

                        messageChannel.WriteLine("HAVE ENOUGH CLOTHING TO KEEP YOU WARM");

                        if (insufficientClothing_C1)
                            Illness(gameState, messageChannel);
                    }
                    else
                    {
                        messageChannel.WriteLine("HEAVY RAINS---TIME AND SUPPLIES LOST");
                        gameState.Food_F -= 10;
                        gameState.Bullets_B -= 500;
                        gameState.MiscSupplies_M1 -= 15;
                        gameState.Mileage_M -= 5 + _random.Next(11);
                    }
                    break;
                case 7:
                    messageChannel.WriteLine("BANDITS ATTACK");
                    s = Shoot(messageChannel);

                    gameState.Bullets_B -= 20;
                    if (gameState.Bullets_B < 0)
                    {
                        messageChannel.WriteLine("YOU RAN OUT OF BULLETS---THEY GET LOTS OF CASH");
                        gameState.Cash_T /= 3;
                    }

                    if (s && gameState.Bullets_B >= 0)
                    {
                        messageChannel.WriteLine(
@"QUICKEST DRAW OUTSIDE OF DODGE CITY!!!
YOU GOT 'EM!");
                    }
                    else
                    {
                        messageChannel.WriteLine(
@"YOU GOT SHOT IN THE LEG AND THEY TOOK ONE OF YOUR OXEN
BETTER HAVE A DOC LOOK AT YOUR WOUND");
                        gameState.Injury_K8 = true;
                        gameState.MiscSupplies_M1 -= 5;
                        gameState.Animals_A -= 20;
                    }
                    break;
                case 8:
                    messageChannel.WriteLine("THERE WAS A FIRE IN YOUR WAGON--FOOD AND SUPPLIES DAMAGE!");
                    gameState.Food_F -= 40;
                    gameState.Bullets_B -= 400;
                    gameState.MiscSupplies_M1 -= 3 + _random.Next(9);
                    gameState.Mileage_M -= 15;
                    break;
                case 9:
                    messageChannel.WriteLine("LOSE YOUR WAY IN HEAVY FOG---TIME IS LOST");
                    gameState.Mileage_M -= 10 + _random.Next(6);
                    break;
                case 10:
                    messageChannel.WriteLine("YOU KILLED A POISONOUS SNAKE AFTER IT BIT YOU");
                    gameState.Bullets_B -= 10;
                    gameState.MiscSupplies_M1 -= 5;

                    if (gameState.MiscSupplies_M1 < 0)
                    {
                        messageChannel.WriteLine("YOU DIE OF SNAKEBITE SINCE YOU HAVE NO MEDICINE");
                        gameState.GameOver = true;
                    }
                    break;
                case 11:
                    messageChannel.WriteLine("WAGON GETS SWAMPED FORDING RIVER--LOSE FOOD AND CLOTHES");
                    gameState.Food_F -= 30;
                    gameState.Clothing_C -= 20;
                    gameState.Mileage_M -= 20 + _random.Next(21);
                    break;
                case 12:
                    messageChannel.WriteLine("WILD ANIMALS ATTACK!");
                    s = Shoot(messageChannel);

                    if (gameState.Bullets_B < 40)
                    {
                        messageChannel.WriteLine(
@"YOU WERE TOO LOW ON BULLETS--
THE WOLVES OVERPOWERED YOU");
                        gameState.Injury_K8 = true;
                        gameState.GameOver = true;
                        return;
                    }

                    if (s)
                    {
                        messageChannel.WriteLine("NICE SHOOTIN' PARTNER---THEY DIDN'T GET MUCH");
                        gameState.Bullets_B -= 20;
                        gameState.Clothing_C -= 4;
                        gameState.Food_F -= 8;
                    }
                    else
                    {
                        messageChannel.WriteLine("SLOW ON THE DRAW---THEY GOT AT YOUR FOOD AND CLOTHES");
                        gameState.Bullets_B -= 40;
                        gameState.Clothing_C -= 8;
                        gameState.Food_F -= 16;
                    }
                    break;
                case 13:
                    messageChannel.WriteLine("HAIL STORM---SUPPLIES DAMAGED");
                    gameState.Mileage_M -= 5 + _random.Next(11);
                    gameState.Bullets_B -= 200;
                    gameState.MiscSupplies_M1 -= 4 + _random.Next(4);
                    break;
                case 14: // illness
                    break;
                case 15:
                    messageChannel.WriteLine("HELPFUL INDIANS SHOW YOU WHERE TO FIND MORE FOOD");
                    gameState.Food_F += 14;
                    break;
            }
        }

        private void Mountains(GameState gameState, IMessageChannel messageChannel)
        {
            if (gameState.Mileage_M <= 950)
                return;

            var x = gameState.Mileage_M / 100.0 - 15.0;
            var x2 = x * x;
            if (_random.NextDouble(10.0) <= (x2 + 72)/(x2 + 12))
            {
                messageChannel.WriteLine("RUGGED MOUNTAINS");
                if (_random.NextDouble() <= 0.1)
                {
                    messageChannel.WriteLine("YOU GOT LOST---LOSE VALUABLE TIME TRYING TO FIND TRAIL!");
                    gameState.Mileage_M -= 60;
                }
                else if (_random.NextDouble() <= 0.11)
                {
                    messageChannel.WriteLine("WAGON DAMAGED!---LOSE TIME AND SUPPLIES");
                    gameState.MiscSupplies_M1 -= 5;
                    gameState.Bullets_B -= 200;
                    gameState.Mileage_M -= 20 + _random.Next(31);
                }
                else
                {
                    messageChannel.WriteLine("THE GOING GETS SLOW");
                    gameState.Mileage_M -= 15 + _random.Next(51);
                }
            }

            if (!gameState.SouthPassCleared_F1)
            {
                gameState.SouthPassCleared_F1 = true;
                if (_random.NextDouble() <= 0.7)
                {
                    messageChannel.WriteLine("BLIZZARD IN MOUNTAIN PASS--TIME AND SUPPLIES LOST");
                    gameState.Food_F -= 25;
                    gameState.MiscSupplies_M1 -= 10;
                    gameState.Bullets_B -= 300;
                    gameState.Mileage_M -= 30 + _random.Next(41);

                    if (gameState.Clothing_C < 18 + _random.Next(3))
                        Illness(gameState, messageChannel);
                }
            }

            if (gameState.Mileage_M <= 950)
                gameState.ClearingSouthPassSettingMileage_M9 = true;
        }

        private void Illness(GameState gameState, IMessageChannel messageChannel)
        {
            if (10 + 35 * (gameState.ChoiceOfEating_E - 1) > _random.Next(101))
            {
                messageChannel.WriteLine("WILD ILLNESS---MEDICINE USED");
                gameState.Mileage_M -= 5;
                gameState.MiscSupplies_M1 -= 2;
            }
            else if (100 - (40 / Math.Pow(4, gameState.ChoiceOfEating_E - 1)) > _random.Next(101))
            {
                messageChannel.WriteLine("BAD ILLNESS---MEDICINE USED");
                gameState.Mileage_M -= 5;
                gameState.MiscSupplies_M1 -= 5;
            }
            else
            {
                messageChannel.WriteLine(
@"SERIOUS ILLNESS---
YOU MUST STOP FOR MEDICAL ATTENTION");
                gameState.MiscSupplies_M1 -= 10;
                gameState.Illness_S4 = true;
            }

            if (gameState.MiscSupplies_M1 < 0)
                gameState.GameOver = true;
        }
    }
}
