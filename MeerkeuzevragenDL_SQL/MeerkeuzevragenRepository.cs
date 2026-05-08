using System.Data;
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
        SELECT r.Id AS ResultaatId, r.IngeleverdeAntwoorden, r.Score,
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
                            string antwoorden = (string)reader["IngeleverdeAntwoorden"];
                            int score = (int)reader["Score"];

                            int leerlingId = (int)reader["LeerlingId"];
                            string leerlingNaam = (string)reader["LeerlingNaam"];

                            int toetsId = (int)reader["ToetsId"];
                            string onderwerpNaam = (string)reader["OnderwerpNaam"];

                            Onderwerp objOnderwerp = new Onderwerp(onderwerpNaam);
                            Toets objToets = new Toets(toetsId, objOnderwerp, DateTime.Now);

                            Leerling objLeerling = new Leerling(leerlingId, leerlingNaam, null);

                            Resultaat nieuwResultaat = new Resultaat(resultaatId, objToets, objLeerling, antwoorden, score);

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
                    string sqlVraag = @"INSERT INTO Vragen (VraagTekst, Moeilijkheid, IsActief, OnderwerpId) 
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

                    string sqlAntwoord = @"INSERT INTO Antwoorden (VraagId, AntwoordTekst, IsCorrect) 
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
    }
}
