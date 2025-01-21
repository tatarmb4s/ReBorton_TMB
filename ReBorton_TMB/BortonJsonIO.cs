// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
using System.Text.Json;
using Borton_Lib.Classes;


namespace ReBorton_TMB.IO
{
    /// <summary>
    /// A JSON beolvasásra és mentésre szolgáló segédosztály.
    /// </summary>
    public static class BortonJsonIO
    {
        /// <summary>
        /// Segéd struktúra a JSON adatok deszerializálására
        /// (például 1 tulajdonos + a hozzátartozó cellák, rabok, őrök paraméterei).
        /// </summary>
        public class InitialData
        {
            public Tulajdonos Tulajdonos { get; set; }
            // Egyéb mezők is lehetnek, pl. Börtönőrök, Rabok, Cellák, ha úgy akarjuk,
            // de a Tulajdonos konstruktora maga is létrehozza a Borton-t.
        }

        /// <summary>
        /// Betölti a kezdeti adatokat a megadott JSON fájlból.
        /// </summary>
        /// <param name="filePath">A JSON fájl elérési útvonala.</param>
        /// <returns>Az InitialData objektum.</returns>
        public static InitialData BetolKezdetiAdatok(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    // Ha a fájl nem létezik, visszaadunk egy defaultot, vagy dobunk kivételt.
                    Console.WriteLine("[FIGYELMEZTETÉS] A JSON fájl nem található, üres adatokat hozunk létre!");
                    var dummyBorton = new Borton("AlapBorton", null);

                    return new InitialData
                    {
                        Tulajdonos = new Tulajdonos(1, "AlapTulaj", Borton_Lib.Enums.Neme.Ferfi, dummyBorton)
                    };
                }

                string jsonContent = File.ReadAllText(filePath);
                var initialData = JsonSerializer.Deserialize<InitialData>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return initialData;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HIBA] Nem sikerült betölteni a kezdeti adatokat: " + ex.Message);
                // Ha hiba van, generáljunk valami alapértelmezettet
                var dummyBorton = new Borton("AlapBorton", null);

                return new InitialData
                {
                    Tulajdonos = new Tulajdonos(1, "AlapTulaj", Borton_Lib.Enums.Neme.Ferfi, dummyBorton)
                };
            }
        }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/