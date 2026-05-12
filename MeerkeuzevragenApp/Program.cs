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
                string connString = "Data Source=JUSTME;Initial Catalog=MeerkeuzevragenDB;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
                Console.WriteLine("Connectiestring ingesteld.");

                Console.WriteLine("\nManager aanmaken via Factory...");
                MeerkeuzevragenManager manager = ManagerFactory.CreateManager(connString);
                Console.WriteLine("Manager succesvol aangemaakt!");

                Console.WriteLine("\nDatabase leegmaken voor een schone test...");
                manager.WisAlleData();
                Console.WriteLine("Database is weer helemaal leeg!");

                Console.WriteLine("\nTest 1: Bestand inlezen en naar database schrijven...");


                string padNaarMap = @"C:\data\meerkeuzevragen";
                 
                manager.ImporteerVragenUitMap(padNaarMap, Moeilijkheid.Gemiddeld);

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