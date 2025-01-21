using Borton_Lib.Enums;
using Borton_Lib.Interfaces;

namespace Borton_Lib.Classes
{
    /// <summary>
    /// Absztrakt ősosztály minden személy számára (Rab, Börtönőr, Tulajdonos)
    /// </summary>
    public abstract class Person : IPerson
    {
        /// <summary>
        /// Azonosító
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Név
        /// </summary>
        public string Nev { get; private set; }

        /// <summary>
        /// Neme (Férfi vagy Nő)
        /// </summary>
        public Neme Neme { get; private set; }

        /// <summary>
        /// Kizárólag az ősosztály gyermekei hozhatják létre a konstruktort
        /// </summary>
        /// <param name="id">Azonosító</param>
        /// <param name="nev">Név</param>
        /// <param name="neme">Neme</param>
        protected Person(int id, string nev, Neme neme)
        {
            ID = id;
            Nev = nev;
            Neme = neme;
        }
    }
}
