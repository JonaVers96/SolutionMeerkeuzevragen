using System;
using MeerkeuzevragenBL.Managers;
using MeerkeuzevragenBL.Model;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenFactory;

namespace ConsoleTester {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("--- START TEST OMGEVING (ZONDER CONFIG BUILDER) ---");

            try {
                // 1. Hardcode je connectiestring direct in de code
                // VERGEET NIET: Pas JOUW_SERVER_NAAM aan naar de juiste server!
                string connString = "Server=JOUW_SERVER_NAAM;Database=MeerkeuzevragenDB;Trusted_Connection=True;TrustServerCertificate=True;";
                Console.WriteLine("Connectiestring ingesteld.");

                // 2. Manager aanmaken via jouw Factory
                Console.WriteLine("\nManager aanmaken via Factory...");
                MeerkeuzevragenManager manager = ManagerFactory.CreateManager(connString);
                Console.WriteLine("Manager succesvol aangemaakt!");

                // 3. Test de functionaliteit: Importeer Geo2.txt
                Console.WriteLine("\nTest 1: Bestand inlezen en naar database schrijven...");

                Onderwerp testOnderwerp = new Onderwerp("Aardrijkskunde");

                // VERGEET NIET: Pas dit pad aan naar waar jouw Geo2.txt staat!
                string padNaarBestand = @"C:\Temp\Geo2.txt";
                 
                // Roep de manager aan
                manager.ImporteerVragenUitBestand(padNaarBestand, testOnderwerp, Moeilijkheid.Gemiddeld);

                Console.WriteLine("\nSUCCES! De test is geslaagd. De vragen zitten in de database.");
            }
            catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nERROR: Er is een fout opgetreden!");
                Console.WriteLine($"Bericht: {ex.Message}");

                if (ex.InnerException != null) {
                    Console.WriteLine($"Details (Inner Exception): {ex.InnerException.Message}");
                }
                Console.ResetColor();
            }

            Console.WriteLine("\nDruk op Enter om af te sluiten.");
            Console.ReadLine();
        }
    }
}