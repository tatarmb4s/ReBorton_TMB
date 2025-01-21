using Borton_Lib.Enums;
using Borton_Lib.Interfaces;
using Borton_Lib.Exceptions;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// A rab osztály, amely örökli a Person-t és megvalósítja az IHasBorton, IHasCell interfészeket
    /// </summary>
    public class Rab : Person, IHasBorton, IHasCell
    {
        private BuntetesTipus _buntetes;
        private Allapot _allapot;
        private int _megveresekSzama;

        /// <summary>
        /// A börtön, ahol a rab tartózkodik
        /// </summary>
        public Borton Borton { get; private set; }

        /// <summary>
        /// A cella, ahol a rab tartózkodik
        /// </summary>
        public Cell Cell { get; private set; }

        /// <summary>
        /// A rab büntetésének típusa
        /// </summary>
        public BuntetesTipus Buntetes
        {
            get { return _buntetes; }
            private set { _buntetes = value; }
        }

        /// <summary>
        /// A rab állapota (Élő vagy Halott)
        /// </summary>
        public Allapot Allapot
        {
            get { return _allapot; }
            private set { _allapot = value; }
        }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">A rab azonosítója</param>
        /// <param name="nev">A rab neve</param>
        /// <param name="neme">Neme</param>
        /// <param name="buntetes">Büntetés típusa</param>
        /// <param name="allapot">Kezdeti állapot</param>
        /// <param name="borton">Börtön referencia</param>
        /// <param name="cell">Cell referencia</param>
        public Rab(int id, string nev, Neme neme,
                   BuntetesTipus buntetes,
                   Allapot allapot,
                   Borton borton,
                   Cell cell)
            : base(id, nev, neme)
        {
            Buntetes = buntetes;
            Allapot = allapot;
            Borton = borton;
            Cell = cell;
            _megveresekSzama = 0;
        }

        /// <summary>
        /// Megmondja, hányszor verték meg a rabot
        /// </summary>
        /// <returns>A verések száma</returns>
        public int HanyszorMegverve()
        {
            return _megveresekSzama;
        }

        /// <summary>
        /// Növeli a verések számát eggyel
        /// </summary>
        internal void MegveresNoveles()
        {
            _megveresekSzama++;
        }

        /// <summary>
        /// Belső metódus betöltéshez: beállítja a megverések számát
        /// </summary>
        /// <param name="count">A JSON-ből beolvasott érték</param>
        internal void SetMegveresekSzama(int count)
        {
            _megveresekSzama = count;
        }

        /// <summary>
        /// A rab leszúrhatja a cellatársát, ha van neki (és él)
        /// </summary>
        /// <remarks>
        /// Ha a cellatárs meghal, felszabadul a cellából.
        /// </remarks>
        public void LeszurCellatars()
        {
            if (Allapot == Allapot.Halott)
            {
                // Ha ez a rab halott, nem tud akciót végrehajtani; dobhatsz kivételt vagy figyelmeztetést
                throw new BortonException("A halott rab nem tud leszúrni senkit!");
            }

            // Keressük a cellatársat:
            var rabok = Cell.GetRabot();
            if (rabok.Count == 2)
            {
                // Van cellatárs
                foreach (var cellatars in rabok)
                {
                    if (cellatars.ID != this.ID && cellatars.Allapot == Allapot.Elo)
                    {
                        // Leszúrjuk
                        cellatars.Meghal();
                        // Kivesszük a cellából
                        Cell.RemoveRab(cellatars);
                        return;
                    }
                }
                throw new BortonException("Nincs élő cellatárs, akit leszúrhatna.");
            }
            else
            {
                throw new BortonException("A cellában nincs cellatárs, akit leszúrhatna.");
            }
        }

        /// <summary>
        /// Belső metódus a rab megöléséhez
        /// </summary>
        internal void Meghal()
        {
            Allapot = Allapot.Halott;
        }
    }
}
