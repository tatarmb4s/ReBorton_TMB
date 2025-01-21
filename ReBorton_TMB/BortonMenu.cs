using Borton_Lib.Classes;
using Borton_Lib.Enums;
using Borton_Lib.Exceptions;


namespace ReBorton_TMB
{
    /// <summary>
    /// A konzol menü kezelője, nyilakkal és biztonságos beolvasással.
    /// </summary>
    public class BortonMenu
    {
        private BortonKezelo _bortonKezelo;
        private string _dataFilePath;

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="bortonKezelo">A BortonKezelo példány</param>
        /// <param name="dataFilePath">A JSON fájl elérési útja</param>
        public BortonMenu(BortonKezelo bortonKezelo, string dataFilePath)
        {
            _bortonKezelo = bortonKezelo;
            _dataFilePath = dataFilePath;
        }

        /// <summary>
        /// A menü futtatása
        /// </summary>
        public void Run()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== Börtönkezelő Rendszer (TMB) ===");
            Console.ResetColor();

            bool vege = false;

            while (!vege)
            {
                // Főmenü nyilas kiválasztás
                int chosenIndex = ShowArrowMenu("Főmenü", new List<string>()
                {
                    "Börtönök listázása",
                    "Új börtön létrehozása",
                    "Börtön menü (cellák, rabok, őrök)",
                    "Mentés JSON-be",
                    "Kilépés"
                });

                switch (chosenIndex)
                {
                    case 0:
                        ListazBortonok();
                        break;
                    case 1:
                        UjBortonLetrehozasa();
                        break;
                    case 2:
                        BortonMenuFunkciok();
                        break;
                    case 3:
                        MentesJsonba();
                        break;
                    case 4:
                        vege = true;
                        break;
                }
            }

            // Kilépés előtti mentés
            MentesJsonba();
            Console.WriteLine("Kilépés...");
        }

        #region Fel/Le nyilas menülogika

        /// <summary>
        /// Kijelzi a megadott opciókat fel/le nyilakkal választható formában.
        /// Enterrel választ, ESC-vel visszalépés (-1).
        /// </summary>
        /// <param name="title">Menücím</param>
        /// <param name="options">Választható elemek</param>
        /// <returns>A kiválasztott elem indexe, vagy -1 ESC esetén</returns>
        private int ShowArrowMenu(string title, List<string> options)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"-- {title} --");
            Console.ResetColor();

            int index = 0;

            while (true)
            {
                // Menü kijelzése
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == index)
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(" > " + options[i]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine("   " + options[i]);
                    }
                }

                // Gombnyomás várás
                var key = Console.ReadKey(true).Key;

                // Töröljük a konzolról a menüsort
                Console.CursorTop = Console.CursorTop - options.Count;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        index--;
                        if (index < 0) index = 0;
                        break;
                    case ConsoleKey.DownArrow:
                        index++;
                        if (index >= options.Count) index = options.Count - 1;
                        break;
                    case ConsoleKey.Enter:
                        Console.CursorTop += options.Count;
                        return index;
                    case ConsoleKey.Escape:
                        Console.CursorTop += options.Count;
                        return -1;
                }
            }
        }

        #endregion

        #region Főmenü funkciók

        private void ListazBortonok()
        {
            var lista = _bortonKezelo.BortonLista;
            Console.WriteLine("\n=== Börtönök ===");
            if (lista.Count == 0)
            {
                Console.WriteLine("  Nincs egyetlen börtön sem.");
                return;
            }
            foreach (var b in lista)
            {
                Console.WriteLine($"  - {b.Nev} (Tulaj: {b.GetTulajdonos().Nev})");
            }
        }

        private void UjBortonLetrehozasa()
        {
            Console.WriteLine("\nÚj börtön létrehozása:");
            Console.Write("Add meg a börtön nevét: ");
            string nev = SafeReadLineNonEmpty("Börtön neve nem lehet üres!");

            // Automatikusan generált ID a tulajdonosnak
            int tid = IdGenerator.GetNextId();

            Console.Write("Tulajdonos neve: ");
            string tnev = SafeReadLineNonEmpty("Tulajdonos neve nem lehet üres!");

            // Neme kiválasztás nyíllal
            int chosenNeme = ShowArrowMenu("Válassz nemet", new List<string> { "Férfi", "Nő" });
            if (chosenNeme == -1) return; // ESC => visszalépés
            Neme n = (chosenNeme == 0) ? Neme.Ferfi : Neme.No;

            var dummyBorton = new Borton(nev, null);
            var tulaj = new Tulajdonos(tid, tnev, n, dummyBorton);

            typeof(Borton).GetField("tulajdonos",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
                ?.SetValue(dummyBorton, tulaj);

            _bortonKezelo.HozzaadBorton(dummyBorton);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Börtön létrehozva!");
            Console.ResetColor();
        }

        private void BortonMenuFunkciok()
        {
            Console.Write("\nAdd meg a börtön nevét, amivel dolgozni akarsz: ");
            string bnev = SafeReadLineNonEmpty("A börtön neve nem lehet üres!");
            var b = _bortonKezelo.KeresBorton(bnev);
            if (b == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nincs ilyen börtön!");
                Console.ResetColor();
                return;
            }

            bool subVege = false;
            while (!subVege)
            {
                int chosenIndex = ShowArrowMenu($"Börtön menü: {b.Nev}", new List<string>
                {
                    "Cellák listája",
                    "Cella építtetése",
                    "Börtönőrök listája",
                    "Börtönőr felvétele",
                    "Rab felvétele",
                    "Leszúrás szimulálása (rab cselekmény)",
                    "Rab megverése (őr cselekmény)",
                    "Kivégzés engedélyezése (tulajdonos cselekmény)",
                    "Rabok listázása",
                    "Börtön teljes infó",
                    "Vissza a Főmenübe"
                });

                switch (chosenIndex)
                {
                    case 0:
                        ListazCellakat(b);
                        break;
                    case 1:
                        EpittetCellat(b);
                        break;
                    case 2:
                        ListazBortonorok(b);
                        break;
                    case 3:
                        FelveszBortonort(b);
                        break;
                    case 4:
                        FelveszRabot(b);
                        break;
                    case 5:
                        Leszuras(b);
                        break;
                    case 6:
                        Megveres(b);
                        break;
                    case 7:
                        KivegzesEngedely(b);
                        break;
                    case 8:
                        ListazRabokat(b);
                        break;
                    case 9:
                        BortonTeljesInfo(b);
                        break;
                    case 10:
                    case -1:
                        subVege = true;
                        break;
                }
            }
        }

        private void MentesJsonba()
        {
            try
            {
                _bortonKezelo.MentesJsonba(_dataFilePath);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Mentés sikeres!");
            }
            catch (BortonException ex)
            {
                Hibakiiras("Hiba a mentés közben: " + ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        #endregion

        #region Börtön menü funkciók (cellák, rabok, őrök)

        private void ListazCellakat(Borton b)
        {
            var cellak = b.GetCellak();
            Console.WriteLine("\n=== Cellák ===");
            if (cellak.Count == 0)
            {
                Console.WriteLine("  (Nincs cella)");
                return;
            }
            foreach (var c in cellak)
            {
                var rabok = c.GetRabot();
                if (rabok.Count == 0)
                {
                    Console.WriteLine($"  - {c.CellID}, rabok száma: 0");
                }
                else
                {
                    // Kiírjuk a bent lévő rabok ID-jét és nevét
                    Console.WriteLine($"  - {c.CellID}, rabok száma: {rabok.Count}");
                    foreach (var r in rabok)
                    {
                        Console.WriteLine($"    -> Rab ID: {r.ID}, Név: {r.Nev}");
                    }
                }
            }
        }

        private void EpittetCellat(Borton b)
        {
            var tulaj = b.GetTulajdonos();
            Console.Write("\nAdd meg az új cella azonosítót: ");
            string cid = SafeReadLineNonEmpty("CellaID nem lehet üres!");

            tulaj.EpittetCellat(cid);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Cella létrehozva!");
            Console.ResetColor();
        }

        private void ListazBortonorok(Borton b)
        {
            var orok = b.GetOrok();
            Console.WriteLine("\n=== Börtönőrök ===");
            if (orok.Count == 0)
            {
                Console.WriteLine("  (Nincs börtönőr)");
                return;
            }
            foreach (var o in orok)
            {
                Console.WriteLine($"  - {o.ID} | {o.Nev}, beosztás: {o.Beosztas}");
            }
        }

        private void FelveszBortonort(Borton b)
        {
            var tulaj = b.GetTulajdonos();
            Console.WriteLine("\nBörtönőr felvétele:");

            int newId = IdGenerator.GetNextId();

            Console.Write("Őr neve: ");
            string onev = SafeReadLineNonEmpty("A név nem lehet üres!");

            int chosenNeme = ShowArrowMenu("Válassz nemet", new List<string> { "Férfi", "Nő" });
            if (chosenNeme == -1) return;
            Neme n = (chosenNeme == 0) ? Neme.Ferfi : Neme.No;

            int chosenBeosztas = ShowArrowMenu("Válassz beosztást", new List<string> { "KapuOr", "CellaOr", "ToronyOr" });
            if (chosenBeosztas == -1) return;
            Beosztas beosztas = Beosztas.KapuOr;
            if (chosenBeosztas == 1) beosztas = Beosztas.CellaOr;
            else if (chosenBeosztas == 2) beosztas = Beosztas.ToronyOr;

            var bor = new Bortonor(newId, onev, n, beosztas, b);

            try
            {
                tulaj.FelveszBortonor(bor);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Börtönőr felvéve!");
            }
            catch (BortonException ex)
            {
                Hibakiiras(ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private void FelveszRabot(Borton b)
        {
            var tulaj = b.GetTulajdonos();
            Console.WriteLine("\nRab felvétele:");

            int rid = IdGenerator.GetNextId();

            Console.Write("Rab neve: ");
            string rnev = SafeReadLineNonEmpty("A rab neve nem lehet üres!");

            int chosenNeme = ShowArrowMenu("Válassz nemet", new List<string> { "Férfi", "Nő" });
            if (chosenNeme == -1) return;
            Neme n = (chosenNeme == 0) ? Neme.Ferfi : Neme.No;

            int chosenBunt = ShowArrowMenu("Válassz büntetést", new List<string> { "Halálbüntetés", "Életfogytiglan" });
            if (chosenBunt == -1) return;
            BuntetesTipus bt = (chosenBunt == 0) ? BuntetesTipus.Halalbuntetes : BuntetesTipus.Eletfogytiglan;

            var rabObj = new Rab(rid, rnev, n, bt, Allapot.Elo, b, null);

            try
            {
                tulaj.FelveszRab(rabObj);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rab felvéve a börtönbe!");
            }
            catch (BortonException ex)
            {
                Hibakiiras(ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private void Leszuras(Borton b)
        {
            Console.WriteLine("\nLeszúrás szimulálása (rab cselekmény).");
            var rabok = b.GetAllRabok().ToList();
            if (rabok.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nincs rab a börtönben!");
                Console.ResetColor();
                return;
            }

            // Megjelenítjük a büntetést is
            var rabOptions = rabok.Select(x => $"{x.ID} - {x.Nev} [{x.Buntetes}]").ToList();
            int chosenIndex = ShowArrowMenu("Válassz rabot", rabOptions);
            if (chosenIndex == -1) return;

            var rab = rabok[chosenIndex];

            try
            {
                rab.LeszurCellatars();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Leszúrás sikeresen megtörtént!");
            }
            catch (BortonException ex)
            {
                Hibakiiras(ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private void Megveres(Borton b)
        {
            Console.WriteLine("\nRab megverése (őr cselekmény).");

            var orok = b.GetOrok();
            if (orok.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nincs egyetlen börtönőr sem!");
                Console.ResetColor();
                return;
            }
            int chosenOr = ShowArrowMenu("Válassz őrt", orok.Select(o => $"{o.ID} - {o.Nev} [{o.Beosztas}]").ToList());
            if (chosenOr == -1) return;
            var orObj = orok[chosenOr];

            var rabok = b.GetAllRabok().ToList();
            if (rabok.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nincs rab a börtönben!");
                Console.ResetColor();
                return;
            }
            var rabOptions = rabok.Select(r => $"{r.ID} - {r.Nev} [{r.Buntetes}]").ToList();
            int chosenRab = ShowArrowMenu("Válassz rabot", rabOptions);
            if (chosenRab == -1) return;
            var rab = rabok[chosenRab];

            try
            {
                orObj.MegverRab(rab);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rab megverve!");
            }
            catch (BortonException ex)
            {
                Hibakiiras(ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private void KivegzesEngedely(Borton b)
        {
            Console.WriteLine("\nKivégzés engedélyezése (tulajdonos).");
            var tulaj = b.GetTulajdonos();

            var rabok = b.GetAllRabok().ToList();
            if (rabok.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nincs rab a börtönben!");
                Console.ResetColor();
                return;
            }

            // A rab nevénél mutassuk a büntetést is
            var rabOptions = rabok.Select(r => $"{r.ID} - {r.Nev} [{r.Buntetes}]").ToList();
            int chosenRab = ShowArrowMenu("Válassz kivégzendő rabot", rabOptions);
            if (chosenRab == -1) return;

            var rab = rabok[chosenRab];

            try
            {
                tulaj.EngedelyKivegzes(rab);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rab kivégzése megtörtént.");
            }
            catch (BortonException ex)
            {
                Hibakiiras(ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }

        private void ListazRabokat(Borton b)
        {
            var rabok = b.GetAllRabok().ToList();
            Console.WriteLine($"\n=== Rabok a {b.Nev}-ben ===");
            if (rabok.Count == 0)
            {
                Console.WriteLine("  (Nincs rab)");
                return;
            }
            foreach (var r in rabok)
            {
                // Megmutatjuk a cella-azonosítót is
                string cellID = (r.Cell == null) ? "nincs" : r.Cell.CellID;
                Console.WriteLine(
                    $"  - {r.ID}, {r.Nev}, Állapot: {r.Allapot}, Büntetés: {r.Buntetes}, Verések: {r.HanyszorMegverve()}, Cella: {cellID}");
            }
        }

        /// <summary>
        /// Új menüpont: a börtön teljes infójának listázása
        /// (cellák, bennük rabok, plusz az őrök, tulajdonos).
        /// </summary>
        /// <param name="b">A kiválasztott börtön</param>
        private void BortonTeljesInfo(Borton b)
        {
            Console.WriteLine($"\n=== Börtön teljes infó: {b.Nev} ===");
            Console.WriteLine($"Tulajdonos: {b.GetTulajdonos().ID} - {b.GetTulajdonos().Nev} (Neme: {b.GetTulajdonos().Neme})");

            // Börtönőrök
            var orok = b.GetOrok();
            Console.WriteLine("\n--- Börtönőrök ---");
            if (orok.Count == 0)
                Console.WriteLine("  (Nincsenek őrök)");
            else
            {
                foreach (var o in orok)
                {
                    Console.WriteLine($"  #{o.ID} - {o.Nev}, Beosztás: {o.Beosztas}, Neme: {o.Neme}");
                }
            }

            // Cellák és rabok
            var cellak = b.GetCellak();
            Console.WriteLine("\n--- Cellák és rabok ---");
            if (cellak.Count == 0)
            {
                Console.WriteLine("  (Nincsenek cellák)");
            }
            else
            {
                foreach (var c in cellak)
                {
                    Console.WriteLine($"  CellID: {c.CellID}");
                    var rabok = c.GetRabot();
                    if (rabok.Count == 0)
                    {
                        Console.WriteLine("    (Nincsenek rabok ebben a cellában)");
                    }
                    else
                    {
                        foreach (var r in rabok)
                        {
                            Console.WriteLine(
                                $"    -> Rab #{r.ID} - {r.Nev}, Büntetés: {r.Buntetes}, Állapot: {r.Allapot}, Verések: {r.HanyszorMegverve()}"
                            );
                        }
                    }
                }
            }
        }

        #endregion

        #region Segédfüggvények az inputhoz

        /// <summary>
        /// Kér egy sor bevitelt a konzolról és ellenőrzi, hogy ne legyen üres.
        /// Ha üres, figyelmeztet és újra bekéri.
        /// </summary>
        /// <param name="errorMsg">Hibaüzenet, ha üres volt</param>
        /// <returns>A beolvasott, nem üres string</returns>
        private string SafeReadLineNonEmpty(string errorMsg)
        {
            while (true)
            {
                string inp = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inp))
                {
                    return inp;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMsg);
                Console.ResetColor();
                Console.Write("> ");
            }
        }

        /// <summary>
        /// Hibakiírás pirossal, de a program sose dőljön össze.
        /// </summary>
        /// <param name="uzenet">A megjelenítendő hibaüzenet</param>
        private void Hibakiiras(string uzenet)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Hiba: {uzenet}");
            Console.ResetColor();
        }

        #endregion
    }
}
