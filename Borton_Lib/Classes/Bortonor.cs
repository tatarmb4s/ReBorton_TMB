// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
using Borton_Lib.Enums;
using Borton_Lib.Interfaces;
using Borton_Lib.Exceptions;
using System;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// A börtönőr osztály, örökli a Person-t, és tartozik egy börtönhöz
    /// </summary>
    public class Bortonor : Person, IHasBorton
    {
        /// <summary>
        /// Milyen beosztásban dolgozik a börtönőr
        /// </summary>
        public Beosztas Beosztas { get; private set; }

        /// <summary>
        /// Melyik börtönhöz tartozik
        /// </summary>
        public Borton Borton { get; private set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="id">Azonosító</param>
        /// <param name="nev">Név</param>
        /// <param name="neme">Neme</param>
        /// <param name="beosztas">Börtönőr beosztása</param>
        /// <param name="borton">Börtön referencia</param>
        public Bortonor(int id, string nev, Neme neme,
                        Beosztas beosztas,
                        Borton borton)
            : base(id, nev, neme)
        {
            Beosztas = beosztas;
            Borton = borton;
        }

        /// <summary>
        /// Megveri a megadott rabot,
        /// ha él és nem halálbüntetéses,
        /// és csak akkor, ha ez a börtönőr cellaőr.
        /// </summary>
        /// <param name="rab">A rab, akit megver</param>
        public void MegverRab(Rab rab)
        {
            if (Beosztas != Beosztas.CellaOr)
                throw new BortonException("Csak a cellaőr verhet meg rabot!");

            if (rab.Allapot == Allapot.Halott)
                throw new BortonException($"Nem lehet megverni a halott rabot: {rab.Nev}");

            if (rab.Buntetes == BuntetesTipus.Halalbuntetes)
                throw new BortonException($"Nem lehet megverni a halálbüntetéses rabot: {rab.Nev}");

            // Minden feltétel oké, megverjük
            rab.MegveresNoveles();
        }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/