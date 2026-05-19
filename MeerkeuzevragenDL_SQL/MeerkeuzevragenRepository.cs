using System.Data;
using MeerkeuzevragenBL.Enum;
using MeerkeuzevragenBL.Exceptions;
using MeerkeuzevragenBL.Gebruikers;
using MeerkeuzevragenBL.Interfaces;
using MeerkeuzevragenBL.Model;
using Microsoft.Data.SqlClient;

namespace MeerkeuzevragenDL_SQL {
    public class MeerkeuzevragenRepository : IMeerkeuzevragenRepository {
        private readonly string _connectionString;

        public MeerkeuzevragenRepository(string connectionString) {
            _connectionString = connectionString;
        }
        public List<Resultaat> HaalAlleResultatenOp() {
            List<Resultaat> resultatenLijst = new List<Resultaat>();

            string sql = @"
        SELECT r.Id AS ResultaatId, r.IngeleverdeAntwoorden, r.Score, r.MaxScore,
               l.Id AS LeerlingId, l.Naam AS LeerlingNaam,
               t.Id AS ToetsId, o.Naam AS OnderwerpNaam
        FROM Resultaat r
        INNER JOIN Gebruiker l ON r.GebruikerId = l.Id
        INNER JOIN Toets t ON r.ToetsId = t.Id
        INNER JOIN Onderwerp o ON t.OnderwerpId = o.Id";

            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                using (SqlCommand cmd = conn.CreateCommand()) {
                    cmd.CommandText = sql;

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            int resultaatId = (int)reader["ResultaatId"];
                            string antwoorden = reader["IngeleverdeAntwoorden"] == DBNull.Value ? "" : (string)reader["IngeleverdeAntwoorden"];
                            int score = (int)reader["Score"];
                            int maxScore = (int)reader["maxScore"];

                            int leerlingId = (int)reader["LeerlingId"];
                            string leerlingNaam = (string)reader["LeerlingNaam"];

                            int toetsId = (int)reader["ToetsId"];
                            string onderwerpNaam = (string)reader["OnderwerpNaam"];

                            Onderwerp objOnderwerp = new Onderwerp(onderwerpNaam);
                            Toets objToets = new Toets(toetsId, objOnderwerp, DateTime.Now);

                            Leerling objLeerling = new Leerling(leerlingId, leerlingNaam, null);

                            Resultaat nieuwResultaat = new Resultaat(resultaatId, objToets, objLeerling, antwoorden, score, maxScore);

                            resultatenLijst.Add(nieuwResultaat);
                        }

                        return resultatenLijst;
                    }
                }
            }
        }

        public void VoegVraagToe(Vraag vraag) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction();

                try {
                    string sqlVraag = @"INSERT INTO Vraag (VraagTekst, Moeilijkheidsgraad, IsActief, OnderwerpId) 
                                        OUTPUT INSERTED.ID 
                                        VALUES (@vraagTekst, @moeilijkheid, @isActief, @onderwerpId)";

                    using (SqlCommand cmdVraag = new SqlCommand(sqlVraag, conn, transaction)) {
                        cmdVraag.Parameters.AddWithValue("@vraagTekst", vraag.VraagTekst);
                        cmdVraag.Parameters.AddWithValue("@moeilijkheid", (int)vraag.Moeilijkheidsgraad);
                        cmdVraag.Parameters.AddWithValue("@isActief", vraag.IsActief);
                        cmdVraag.Parameters.AddWithValue("@onderwerpId", vraag.Onderwerp.Id);

                        int nieuweVraagId = (int)cmdVraag.ExecuteScalar();
                        vraag.Id = nieuweVraagId;
                    }

                    string sqlAntwoord = @"INSERT INTO Antwoord (VraagId, Tekst, IsCorrect) 
                                           VALUES (@vraagId, @antwoordTekst, @isCorrect)";

                    using (SqlCommand cmdAntwoord = new SqlCommand(sqlAntwoord, conn, transaction)) {
                        cmdAntwoord.Parameters.Add("@vraagId", System.Data.SqlDbType.Int);
                        cmdAntwoord.Parameters.Add("@antwoordTekst", System.Data.SqlDbType.NVarChar);
                        cmdAntwoord.Parameters.Add("@isCorrect", System.Data.SqlDbType.Bit);

                        foreach (Antwoord antwoord in vraag.Antwoorden) {
                            cmdAntwoord.Parameters["@vraagId"].Value = vraag.Id;
                            cmdAntwoord.Parameters["@antwoordTekst"].Value = antwoord.Tekst;
                            cmdAntwoord.Parameters["@isCorrect"].Value = antwoord.IsCorrect;

                            cmdAntwoord.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                }
                catch (Exception ex) {
                    transaction.Rollback();

                    throw new Exception("Fout bij het wegschrijven van de vraag en antwoorden naar de database.", ex);
                }
            }
        }

        public void WisAlleData() {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();

                // Let op de volgorde! Eerst de afhankelijke tabellen, dan pas de basistabellen.
                // DBCC CHECKIDENT zet de teller voor de auto-increment (ID) weer mooi op 0.
                string sql = @"
            DELETE FROM Resultaat;
            DELETE FROM ToetsVraag;
            DELETE FROM Toets;
            DELETE FROM Antwoord;
            DELETE FROM Vraag;
            DELETE FROM Gebruiker;
            DELETE FROM Klas;
            DELETE FROM Onderwerp;

            DBCC CHECKIDENT ('Resultaat', RESEED, 0);
            DBCC CHECKIDENT ('Toets', RESEED, 0);
            DBCC CHECKIDENT ('Antwoord', RESEED, 0);
            DBCC CHECKIDENT ('Vraag', RESEED, 0);
            DBCC CHECKIDENT ('Gebruiker', RESEED, 0);
            DBCC CHECKIDENT ('Klas', RESEED, 0);
            DBCC CHECKIDENT ('Onderwerp', RESEED, 0);
        ";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void VoegOnderwerpToe(Onderwerp onderwerp) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
                // We slaan de naam op en vragen meteen de nieuwe ID terug
                string sql = @"INSERT INTO Onderwerp (Naam) 
                       OUTPUT INSERTED.ID 
                       VALUES (@naam)";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@naam", onderwerp.Naam);

                    // Haal de nieuwe ID op en wijs hem toe aan je object!
                    int nieuwId = (int)cmd.ExecuteScalar();
                    onderwerp.Id = nieuwId;
                }
            }
        }

        public List<Onderwerp> HaalAlleOnderwerpenOp() {
            List<Onderwerp> onderwerpen = new List<Onderwerp>();

            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                string sql = "SELECT Id, Naam FROM Onderwerp ORDER BY Naam";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            int id = (int)reader["Id"];
                            string naam = (string)reader["Naam"];

                            onderwerpen.Add(new Onderwerp(id, naam));
                        }
                    }
                }
            }
            return onderwerpen;
        }

        public List<Vraag> HaalVragenOpPerOnderwerp(int onderwerpId) {
            List<Vraag> vragenlijst = new List<Vraag>();
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                // We halen de basisgegevens op die we in de lijst willen tonen
                string sql = "SELECT Id, VraagTekst, Moeilijkheidsgraad, IsActief FROM Vraag WHERE OnderwerpId = @onderwerpId";
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@onderwerpId", onderwerpId);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read()) {
                            int id = (int)reader["Id"];
                            string tekst = (string)reader["VraagTekst"];
                            Moeilijkheid moeilijkheid = (Moeilijkheid)(int)reader["Moeilijkheidsgraad"];
                            bool isActief = (bool)reader["IsActief"];

                            // We maken de vraag aan (Onderwerp mag hier even null zijn, we hebben enkel de tekst en status nodig voor de UI)
                            Vraag v = new Vraag(id, tekst, moeilijkheid, null, isActief);
                            vragenlijst.Add(v);
                        }
                    }
                }
            }
            return vragenlijst;
        }

        public void UpdateVraagActiefStaat(int vraagId, bool isActief) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                string sql = "UPDATE Vraag SET IsActief = @isActief WHERE Id = @vraagId";
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@isActief", isActief);
                    cmd.Parameters.AddWithValue("@vraagId", vraagId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Vraag> HaalWillekeurigeVolledigeVragenOp(int onderwerpId, int aantalVragen) {
            List<Vraag> vragenlijst = new List<Vraag>();
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();

                string sqlVragen = @"
            SELECT TOP (@aantal) Id, VraagTekst, Moeilijkheidsgraad 
            FROM Vraag 
            WHERE OnderwerpId = @onderwerpId AND IsActief = 1 
            ORDER BY NEWID()";

                using (SqlCommand cmdVraag = new SqlCommand(sqlVragen, conn)) {
                    cmdVraag.Parameters.AddWithValue("@aantal", aantalVragen);
                    cmdVraag.Parameters.AddWithValue("@onderwerpId", onderwerpId);

                    using (SqlDataReader reader = cmdVraag.ExecuteReader()) {
                        while (reader.Read()) {
                            int id = (int)reader["Id"];
                            string tekst = (string)reader["VraagTekst"];
                            Moeilijkheid moeilijkheid = (Moeilijkheid)(int)reader["Moeilijkheidsgraad"];

                            vragenlijst.Add(new Vraag(id, tekst, moeilijkheid, null, true));
                        }
                    }
                }

                foreach (Vraag v in vragenlijst) {
                    string sqlAntwoorden = "SELECT Id, Tekst, IsCorrect FROM Antwoord WHERE VraagId = @vraagId";
                    using (SqlCommand cmdAntwoord = new SqlCommand(sqlAntwoorden, conn)) {
                        cmdAntwoord.Parameters.AddWithValue("@vraagId", v.Id);
                        using (SqlDataReader readerA = cmdAntwoord.ExecuteReader()) {
                            while (readerA.Read()) {
                                int aId = (int)readerA["Id"];
                                string aTekst = (string)readerA["Tekst"];
                                bool isCorrect = (bool)readerA["IsCorrect"];

                                v.VoegAntwoordToe(new Antwoord(aId, aTekst, isCorrect));
                            }
                        }
                    }
                }
            }
            return vragenlijst;
        }
        public Gebruiker ZoekGebruiker(string naam) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                // We zoeken de gebruiker op naam en kijken naar de kolom 'Rol'
                string sql = "SELECT Id, Naam, Rol FROM Gebruiker WHERE Naam = @naam";
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@naam", naam);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            int id = (int)reader["Id"];
                            string n = (string)reader["Naam"];
                            string rol = (string)reader["Rol"];

                            if (rol.Equals("Leerkracht", StringComparison.OrdinalIgnoreCase))
                                return new Leerkracht(id, n);
                            else
                                return new Leerling(id, n, null); // Klas laten we even voor wat het is
                        }
                    }
                }
            }
            return null; // Gebruiker niet gevonden
        }

        public void BewaarResultaat(Resultaat resultaat) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                string sql = @"INSERT INTO Resultaat (ToetsId, GebruikerId, Score, MaxScore, IngeleverdeAntwoorden) 
                       VALUES (@toetsId, @gebruikerId, @score, @maxScore, @antwoorden)";
                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@toetsId", resultaat.AfgelegdeToets.Id);
                    cmd.Parameters.AddWithValue("@gebruikerId", resultaat.Eigenaar.Id);
                    cmd.Parameters.AddWithValue("@score", resultaat.Score);
                    cmd.Parameters.AddWithValue("@maxScore", resultaat.MaxScore);
                    cmd.Parameters.AddWithValue("@antwoorden", resultaat.IngeleverdeAntwoorden ?? "");
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void VoegToetsToe(Toets toets) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();

                string sql = @"INSERT INTO Toets (OnderwerpId, AanmaakDatum) 
                       OUTPUT INSERTED.ID 
                       VALUES (@onderwerpId, @datum)";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    cmd.Parameters.AddWithValue("@onderwerpId", toets.Onderwerp.Id);
                    cmd.Parameters.AddWithValue("@datum", toets.AanmaakDatum);

                    int nieuwId = (int)cmd.ExecuteScalar();
                    toets.Id = nieuwId;
                }
            }
        }

        public void WisAlleResultaten() {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                // Maak de tabel leeg en reset de teller
                string sql = @"
            DELETE FROM Resultaat;
            DBCC CHECKIDENT ('Resultaat', RESEED, 0);";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void VerwijderOnderwerp(int onderwerpId) {
            using (SqlConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction()) {
                    try {
                        // 1. Verwijder resultaten gekoppeld aan toetsen van dit onderwerp
                        string sqlResultaten = @"DELETE FROM Resultaat WHERE ToetsId IN 
                                         (SELECT Id FROM Toets WHERE OnderwerpId = @id)";
                        using (SqlCommand cmd = new SqlCommand(sqlResultaten, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        // 2. Verwijder ToetsVragen gekoppeld aan toetsen of vragen van dit onderwerp
                        string sqlToetsVragen = @"DELETE FROM ToetsVraag WHERE ToetsId IN (SELECT Id FROM Toets WHERE OnderwerpId = @id)
                                          OR VraagId IN (SELECT Id FROM Vraag WHERE OnderwerpId = @id)";
                        using (SqlCommand cmd = new SqlCommand(sqlToetsVragen, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        // 3. Verwijder Toetsen van dit onderwerp
                        string sqlToetsen = "DELETE FROM Toets WHERE OnderwerpId = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlToetsen, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        // 4. Verwijder Antwoorden van vragen van dit onderwerp
                        string sqlAntwoorden = @"DELETE FROM Antwoord WHERE VraagId IN 
                                         (SELECT Id FROM Vraag WHERE OnderwerpId = @id)";
                        using (SqlCommand cmd = new SqlCommand(sqlAntwoorden, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        // 5. Verwijder Vragen van dit onderwerp
                        string sqlVragen = "DELETE FROM Vraag WHERE OnderwerpId = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlVragen, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        // 6. Verwijder tot slot het Onderwerp zelf
                        string sqlOnderwerp = "DELETE FROM Onderwerp WHERE Id = @id";
                        using (SqlCommand cmd = new SqlCommand(sqlOnderwerp, conn, transaction)) {
                            cmd.Parameters.AddWithValue("@id", onderwerpId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex) {
                        transaction.Rollback();
                        throw new Exception("Fout bij het verwijderen van het onderwerp en de bijbehorende data uit de database.", ex);
                    }
                }
            }
        }
    }
}
