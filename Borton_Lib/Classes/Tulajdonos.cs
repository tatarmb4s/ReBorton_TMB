// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
using Borton_Lib.Enums;
using Borton_Lib.Interfaces;
using Borton_Lib.Exceptions;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// A börtön tulajdonosa
    /// </summary>
    public class Tulajdonos : Person, IHasBorton
    {
        /// <summary>
        /// Melyik börtön tulajdonosa
        /// (Most a feladat szerint 1-1, de kibővíthető többesre is)
        /// </summary>
        public Borton Borton { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Azonosító</param>
        /// <param name="nev">Tulajdonos neve</param>
        /// <param name="neme">Neme</param>
        /// <param name="borton">Ehhez a börtönhöz tartozik</param>
        public Tulajdonos(int id, string nev, Neme neme, Borton borton)
            : base(id, nev, neme)
        {
            Borton = borton;
        }

        /// <summary>
        /// Rab felvétele a börtönbe
        /// Csak akkor, ha van legalább 1 börtönőr,
        /// és a rab még nincs benne,
        /// és találunk megfelelő cellát.
        /// </summary>
        /// <param name="rab">Felvenni kívánt rab</param>
        public void FelveszRab(Rab rab)
        {
            // 1) Van-e legalább 1 börtönőr?
            if (Borton.GetOrok().Count == 0)
            {
                throw new BortonException("Nincs egyetlen börtönőr sem, nem vehető fel rab!");
            }

            // 2) Nincs-e már a börtönben? (keressük a cellákban)
            var allRabok = Borton.GetAllRabok();
            if (allRabok.Any(r => r.ID == rab.ID))
            {
                throw new BortonException("Ez a rab már a börtönben van!");
            }

            // 3) Keresünk egy megfelelő cellát
            var cellak = Borton.GetCellak();
            foreach (var cella in cellak)
            {
                if (!cella.IsFull() && cella.SameConditions(rab))
                {
                    // Betesszük a cellába
                    cella.AddRab(rab);
                    return;
                }
            }

            // Ha idáig jutottunk, nem találtunk megfelelő cellát
            throw new BortonException("Nincs megfelelő cella, ahova a rabot el lehet helyezni!");
        }

        /// <summary>
        /// Börtönőr felvétele a börtönbe
        /// </summary>
        /// <param name="bortonor">A felvenni kívánt börtönőr</param>
        public void FelveszBortonor(Bortonor bortonor)
        {
            Borton.AddBortonor(bortonor);
        }

        /// <summary>
        /// Új cella építtetése a börtönbe
        /// </summary>
        /// <param name="cellID">Az új cella azonosítója</param>
        public void EpittetCellat(string cellID)
        {
            var cell = new Cell(cellID);
            Borton.AddCell(cell);
        }

        /// <summary>
        /// Engedély a rab kivégzésére,
        /// csak ha halálbüntetéses és még él.
        /// Ha a rab már halott, dobjunk figyelmeztetést (hibát).
        /// </summary>
        /// <param name="rab">A rab, akit ki kell végezni</param>
        public void EngedelyKivegzes(Rab rab)
        {
            if (rab.Buntetes != BuntetesTipus.Halalbuntetes)
            {
                throw new BortonException("Csak halálbüntetéses rab kivégzésére adható engedély!");
            }

            if (rab.Allapot == Allapot.Halott)
            {
                // Kérés szerint dobjunk "figyelmeztetést", de sose fusson a program
                // "váratlan" kivételre. Ezért BortonException-nel megfogjuk:
                throw new BortonException($"A rab ({rab.Nev}) már halott, nem lehet újra kivégezni!");
            }

            // Kivégzés
            rab.Meghal();
            // Kivesszük a cellájából
            rab.Cell.RemoveRab(rab);
        }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/