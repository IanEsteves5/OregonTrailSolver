using OregonTrail.Shared;

namespace OregonTrail.Game
{
    public class InitialGameStateProvider
    {
        public GameState GetInitialGameState(IMessageChannel messageChannel)
        {
            messageChannel.WriteLine(string.Empty);
            messageChannel.WriteLine(string.Empty);

            var shootingExpertise = GetShootingExpertise(messageChannel);

            int amountSpentAnimals;
            int amountSpentFood;
            int amountSpentAmmunition;
            int amountSpentClothing;
            int amountSpentMiscellaneous;
            int amountUnspent;

            do
            {
                amountSpentAnimals = GetAmoutSpentAnimals(messageChannel);
                amountSpentFood = GetAmoutSpent(messageChannel, "HOW MUCH DO YOU WANT TO SPEND ON FOOD");
                amountSpentAmmunition = GetAmoutSpent(messageChannel, "HOW MUCH DO YOU WANT TO SPEND ON AMMUNITION");
                amountSpentClothing = GetAmoutSpent(messageChannel, "HOW MUCH DO YOU WANT TO SPEND ON CLOTHING");
                amountSpentMiscellaneous = GetAmoutSpent(messageChannel, "HOW MUCH DO YOU WANT TO SPEND ON MISCELLANEOUS SUPPLIES");

                amountUnspent = 700 - amountSpentAnimals - amountSpentFood - amountSpentAmmunition - amountSpentClothing - amountSpentMiscellaneous;

                if (amountUnspent < 0)
                    messageChannel.WriteLine("YOU OVERSPENT--YOU ONLY HAD $700 TO SPEND.  BUY AGAIN");
            }
            while (amountUnspent < 0);

            messageChannel.WriteLine("AFTER ALL YOUR PURCHASES. YOU NOW HAVE {0} DOLLARS LEFT", amountUnspent);

            return new GameState(shootingExpertise, amountSpentAnimals, amountSpentFood, amountSpentAmmunition * 50,
                amountSpentClothing, amountSpentMiscellaneous, amountUnspent);
        }

        private int GetShootingExpertise(IMessageChannel messageChannel)
        {
            int shootingExpertise;

            do
            {
                messageChannel.WriteLine(
@"HOW GOOD A SHOT ARE YOU WITH YOUR RIFLE?
    (1) ACE MARKSMAN,  (2) GOOD SHOT,  (3) FAIR TO MIDDLIN'
            (4) NEED MORE PRACTICE,  (5) SHAKY KNEES
ENTER ONE OF THE ABOVE -- THE BETTER YOU CLAIM YOU ARE, THE
FASTER YOU'LL HAVE TO BE WITH YOUR GUN TO BE SUCCESSFUL.");

                if (!messageChannel.TryReadInt(out shootingExpertise))
                    return shootingExpertise = -1;
            }
            while (shootingExpertise < 1 || shootingExpertise > 5);

            return shootingExpertise;
        }

        private int GetAmoutSpentAnimals(IMessageChannel messageChannel)
        {
            int amountSpentAnimals = 0;

            do
            {
                messageChannel.WriteLine("HOW MUCH DO YOU WANT TO SPEND ON YOUR OXEN TEAM");
                if (!messageChannel.TryReadInt(out amountSpentAnimals))
                {
                    amountSpentAnimals = 0;
                    continue;
                }

                if (amountSpentAnimals < 200)
                    messageChannel.WriteLine("NOT ENOUGH");
                else if (amountSpentAnimals > 300)
                    messageChannel.WriteLine("TOO MUCH");
            }
            while (amountSpentAnimals < 200 || amountSpentAnimals > 300);

            return amountSpentAnimals;
        }

        private int GetAmoutSpent(IMessageChannel messageChannel, string message)
        {
            int amountSpent = 0;

            do
            {
                messageChannel.WriteLine(message);
                if (!messageChannel.TryReadInt(out amountSpent))
                    amountSpent = -1;

                if (amountSpent < 0)
                    messageChannel.WriteLine("IMPOSSIBLE");
            }
            while (amountSpent < 0);

            return amountSpent;
        }
    }
}
