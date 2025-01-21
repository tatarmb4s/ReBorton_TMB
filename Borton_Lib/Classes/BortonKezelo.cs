// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
using System.Text.Json;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// Több börtön kezeléséért felelős osztály
    /// </summary>
    public class BortonKezelo
    {
        private List<Borton> bortonLista;

        /// <summary>
        /// A kezelt börtönök listája
        /// </summary>
        public IReadOnlyList<Borton> BortonLista => bortonLista;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public BortonKezelo()
        {
            bortonLista = new List<Borton>();
        }

        /// <summary>
        /// Hozzáad egy börtönt a listához
        /// </summary>
        /// <param name="b">Az új börtön</param>
        public void HozzaadBorton(Borton b)
        {
            bortonLista.Add(b);
        }

        /// <summary>
        /// Megpróbál törölni egy börtönt a listából
        /// </summary>
        /// <param name="b">Az eltávolítandó börtön</param>
        /// <returns>Igaz, ha sikerült törölni</returns>
        public bool TorolBorton(Borton b)
        {
            return bortonLista.Remove(b);
        }

        /// <summary>
        /// Keres egy börtönt név alapján
        /// </summary>
        /// <param name="nev">Börtön neve</param>
        /// <returns>Borton vagy null, ha nincs</returns>
        public Borton KeresBorton(string nev)
        {
            return bortonLista.FirstOrDefault(x => x.Nev == nev)!;
        }

        /// <summary>
        /// Betölti a börtön adatokat egy JSON fájlból
        /// és frissíti az ID-generátor induló értékét,
        /// hogy ne ütközzön a már létező ID-kkel.
        /// </summary>
        /// <param name="filePath">Fájlelérési út</param>
        public void BetoltesJsonbol(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string jsonString = File.ReadAllText(filePath);
            try
            {
                var adat = JsonSerializer.Deserialize<BortonAdat>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (adat != null && adat.Bortonok != null)
                {
                    bortonLista.Clear();
                    int maxIdFound = 0;

                    foreach (var bAdat in adat.Bortonok)
                    {
                        // Tulajdonos
                        maxIdFound = System.Math.Max(maxIdFound, bAdat.Tulajdonos!.ID);
                        var tulaj = new Tulajdonos(
                            bAdat.Tulajdonos.ID,
                            bAdat.Tulajdonos.Nev!,
                            bAdat.Tulajdonos.Neme,
                            null!
                        );
                        var bObj = new Borton(bAdat.Nev!, tulaj);
                        typeof(Tulajdonos).GetProperty("Borton")
                            ?.SetValue(tulaj, bObj);

                        // Cellák
                        foreach (var cAdat in bAdat.Cellak!)
                        {
                            var cellObj = new Cell(cAdat.CellID!);
                            bObj.AddCell(cellObj);

                            // Rabok
                            foreach (var rAdat in cAdat.Rabok!)
                            {
                                maxIdFound = System.Math.Max(maxIdFound, rAdat.ID);

                                var rabObj = new Rab(
                                    rAdat.ID,
                                    rAdat.Nev!,
                                    rAdat.Neme,
                                    rAdat.Buntetes,
                                    rAdat.Allapot,
                                    bObj,
                                    cellObj
                                );
                                // **Itt állítjuk be a megverés-számot**
                                rabObj.SetMegveresekSzama(rAdat.MegveresekSzama);

                                cellObj.AddRab(rabObj);
                            }
                        }

                        // Börtönőrök
                        foreach (var oAdat in bAdat.Bortonorok!)
                        {
                            maxIdFound = System.Math.Max(maxIdFound, oAdat.ID);

                            var orObj = new Bortonor(
                                oAdat.ID,
                                oAdat.Nev!,
                                oAdat.Neme,
                                oAdat.Beosztas,
                                bObj
                            );
                            bObj.AddBortonor(orObj);
                        }

                        // Hozzáadjuk
                        bortonLista.Add(bObj);
                    }

                    // ID-generátor frissítése
                    Borton_Lib.Classes.IdGenerator.InitIfHigher(maxIdFound + 1);
                }
            }
            catch
            {
                // Rossz JSON formátum: ne omoljon össze a program
            }
        }

        /// <summary>
        /// Mentés JSON-be
        /// </summary>
        /// <param name="filePath">Fájlelérési út</param>
        public void MentesJsonba(string filePath)
        {
            var adat = new BortonAdat();
            adat.Bortonok = new List<BortonDTO>();

            foreach (var b in bortonLista)
            {
                var bDto = new BortonDTO();
                bDto.Nev = b.Nev;

                var t = b.GetTulajdonos();
                bDto.Tulajdonos = new TulajdonosDTO
                {
                    ID = t.ID,
                    Nev = t.Nev,
                    Neme = t.Neme
                };

                bDto.Cellak = new List<CellDTO>();
                var cellak = b.GetCellak();
                foreach (var c in cellak)
                {
                    var cDto = new CellDTO();
                    cDto.CellID = c.CellID;
                    cDto.Rabok = new List<RabDTO>();

                    var rabok = c.GetRabot();
                    foreach (var r in rabok)
                    {
                        var rDto = new RabDTO
                        {
                            ID = r.ID,
                            Nev = r.Nev,
                            Neme = r.Neme,
                            Buntetes = r.Buntetes,
                            Allapot = r.Allapot,
                            MegveresekSzama = r.HanyszorMegverve()
                        };
                        cDto.Rabok.Add(rDto);
                    }

                    bDto.Cellak.Add(cDto);
                }

                bDto.Bortonorok = new List<BortonorDTO>();
                var orok = b.GetOrok();
                foreach (var o in orok)
                {
                    var oDto = new BortonorDTO
                    {
                        ID = o.ID,
                        Nev = o.Nev,
                        Neme = o.Neme,
                        Beosztas = o.Beosztas
                    };
                    bDto.Bortonorok.Add(oDto);
                }

                adat.Bortonok.Add(bDto);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(adat, options);
            File.WriteAllText(filePath, jsonString);
        }
    }

    #region Helper DTO-k (Data Transfer Object)

    internal class BortonAdat
    {
        public List<BortonDTO>? Bortonok { get; set; }
    }

    internal class BortonDTO
    {
        public string? Nev { get; set; }
        public TulajdonosDTO? Tulajdonos { get; set; }
        public List<CellDTO>? Cellak { get; set; }
        public List<BortonorDTO>? Bortonorok { get; set; }
    }

    internal class TulajdonosDTO
    {
        public int ID { get; set; }
        public string? Nev { get; set; }
        public Borton_Lib.Enums.Neme Neme { get; set; }
    }

    internal class CellDTO
    {
        public string? CellID { get; set; }
        public List<RabDTO>? Rabok { get; set; }
    }

    internal class RabDTO
    {
        public int ID { get; set; }
        public string? Nev { get; set; }
        public Borton_Lib.Enums.Neme Neme { get; set; }
        public Borton_Lib.Enums.BuntetesTipus Buntetes { get; set; }
        public Borton_Lib.Enums.Allapot Allapot { get; set; }
        public int MegveresekSzama { get; set; }
    }

    internal class BortonorDTO
    {
        public int ID { get; set; }
        public string? Nev { get; set; }
        public Borton_Lib.Enums.Neme Neme { get; set; }
        public Borton_Lib.Enums.Beosztas Beosztas { get; set; }
    }

    #endregion
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/