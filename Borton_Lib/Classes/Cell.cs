using Borton_Lib.Exceptions;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// A börtön cellája, maximum 2 rabot tartalmazhat
    /// </summary>
    public class Cell
    {
        private Rab? rab1;
        private Rab? rab2;

        /// <summary>
        /// A cella azonosítója
        /// </summary>
        public string CellID { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="cellID">Azonosító</param>
        public Cell(string cellID)
        {
            CellID = cellID;
            rab1 = null;
            rab2 = null;
        }

        /// <summary>
        /// Lekérdezés, mely rabok vannak jelen a cellában
        /// </summary>
        /// <returns>A cellában lévő rabok listája</returns>
        public IReadOnlyList<Rab> GetRabot()
        {
            var list = new List<Rab>();
            if (rab1 != null) list.Add(rab1);
            if (rab2 != null) list.Add(rab2);
            return list;
        }

        /// <summary>
        /// Megnézzük, tele van-e a cella
        /// </summary>
        /// <returns>true, ha tele van (2 rab), false egyébként</returns>
        public bool IsFull()
        {
            return (rab1 != null && rab2 != null);
        }

        /// <summary>
        /// Megnézzük, hogy az adott rab illik-e a cellába
        /// Neme és a büntetés típusa azonos legyen a bent lévőkkel
        /// (ha már van bennük rab).
        /// </summary>
        /// <param name="rab">Az új rab</param>
        /// <returns>Igaz, ha illik, hamis, ha nem</returns>
        public bool SameConditions(Rab rab)
        {
            var bentLevoRabok = GetRabot();
            if (bentLevoRabok.Count == 0)
            {
                // Ha még üres, bármilyen rab befér
                return true;
            }
            else
            {
                // Összehasonlítjuk a bent lévők nemeit és büntetését
                return bentLevoRabok.All(r =>
                    r.Neme == rab.Neme &&
                    r.Buntetes == rab.Buntetes);
            }
        }

        /// <summary>
        /// Betesz egy rabot a cellába (ha van üres hely)
        /// </summary>
        /// <param name="rab">A beszállítandó rab</param>
        public void AddRab(Rab rab)
        {
            if (IsFull())
            {
                throw new BortonException($"A(z) {CellID} cella már tele van!");
            }

            if (!SameConditions(rab))
            {
                throw new BortonException($"A rab({rab.Nev}) neme vagy büntetése nem egyezik a cellában lévővel!");
            }

            // Keressük az üres helyet
            if (rab1 == null)
            {
                rab1 = rab;
            }
            else if (rab2 == null)
            {
                rab2 = rab;
            }

            // Állítsuk a rab Cell property-jét is, és mivel még nem használtam reflectiont, itt az ideje elkezdeni:
            rab.GetType().GetProperty("Cell")?.SetValue(rab, this);

        }

        /// <summary>
        /// Kivesz egy rabot a cellából
        /// </summary>
        /// <param name="rab">A rab, akit el akarunk távolítani</param>
        public void RemoveRab(Rab rab)
        {
            if (rab1 != null && rab1.ID == rab.ID)
            {
                rab1 = null;
            }
            else if (rab2 != null && rab2.ID == rab.ID)
            {
                rab2 = null;
            }
            // A rab Cell property-jét is nullá tehetnénk, de
            // a rab halott, ezzel jelképesen "bent se" van, illetve így megmarad hogy melyik cellában volt annó
            rab.GetType().GetProperty("Cell")?.SetValue(rab, null);
        }
    }
}
