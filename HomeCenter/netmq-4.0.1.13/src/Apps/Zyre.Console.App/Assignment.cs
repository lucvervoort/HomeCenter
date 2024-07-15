// using Assignment.Repository.Context;

namespace Assignment.Console.App
{
    public class Assignment: IAssignment
    {
        /*
        private readonly HotelManagementContext _dbContext;

        public Assignment(HotelManagementContext dbContext)
        {
            _dbContext = dbContext;
        }
        */

        public void Run()
        {
            /* 
               1. Maak de databank aan in SQLServer onder Docker en pas de connection string in appsettings.json aan; voor de te gebruiken DDL, zie bestand Database/HotelManagement.sql 
             */

            /* 
               2. Voer een scaffolding uit: overerven van de gegenereerde DbContext klasse is niet nodig; voor de te gebruiken commandolijn-opdrachten, zie bestand Database/Commands.txt 
             */

            /* 
               3. Voer de migratie met naam "InitialCreate" uit; voor de te gebruiken commandolijn-opdrachten, zie bestand Database/Commands.txt 
             */

            /* 
               4. Voeg via EF Core coding toe:

               -- 1 hotel.
               -- 1 departement.
               -- 2 werknemers (employees).
               -- 2 discount types: "No discount" (DiscountRate: 0), "Valued customer" (DiscountRate: 25).
               -- 2 gasten (guests), 1 gast met status "Deceased", 1 gast met status "minor".
               -- 2 bookings, waarvan eentje als "Valued customer" en de andere als "No discount".

               De waarde van niet opgegeven gegevens mag je zelf kiezen.
             */

            /* Toon op de console de gegevens van volgende queries (gebruik LINQ): */

            System.Console.WriteLine("5. De voor- en achternaam van alle werknemers, gevolgd door hun emailadres, alfabetisch gesorteerd volgens hun achternaam:");
            System.Console.WriteLine("6. De voor- en achternaam van alle gasten die als status 'Deceased' hebben, gevolgd door hun emailadres en stad van herkomst, gesorteerd volgens hun achternaam:");
            System.Console.WriteLine("7. Een overzicht van alle bookings waarbij enkel getoond worden: CheckInDate, CheckOutDate, voor- en achternaam van de gast, de DiscountDescription en de status van de gast, gesorteerd volgens de CheckInDate:");

            /* 
               8. Voeg via Server Explorer of SSMS toe:

               -- 2 kamertypes met als namen "Standard" en "Luxe" en als respectievelijke prijzen 150 en 300 (de waarden van de overige velden mag je zelf kiezen)
               -- 4 kamers met als kamernummers 34 (type: "Standard"), 78 (type: "Standard"), 102 (type "Luxe"), 235 (type "Luxe") die alle "true" hebben als waarde in de kolom Available
            */

            /* 
               9. Pas via EF Core coding de prijzen van de kamertypes aan: "Standard" wordt 200 en "Luxe" wordt 400. 
             */

            /* 
               10. Voer volgende "code first" migraties uit:

               - Voeg de kolom "PhoneNumber" (string, null) toe aan de tabel Room.
               - Maak van het veld Available in tabel Room een boolean in plaats van een string.
            */

            /* 
               11. BONUS: maak een RoomRepository aan door over te erven van GenericRepository.
             */

            /* 
               12. BONUS: maak gebruik van RoomRepository om de kamers op te halen met nummers 78 en 235 en alle gegevens van deze kamers af te drukken op de console, 
                   waarbij je ervoor zorgt dat EF Core deze gegevens niet "tracks".
            */

            /* 
               13. BONUS: implementeer soft delete (zie Chamilo); vergeet niet via Server Explorer of SSMS alle tabellen van een geschikte IsDeleted kolom te voorzien .
             */

            /*
               14. BONUS: zorg voor gedetailleerde foutmeldingen van EF Core. Tip: override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) van je DbContext klasse.
             */
        }
    }
}
