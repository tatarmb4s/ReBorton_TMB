using Borton_Lib.Enums;

namespace Borton_Lib.Interfaces
{
    /// <summary>
    /// Alap interfész minden személy számára
    /// </summary>
    public interface IPerson
    {
        /// <summary>
        /// Azonosító
        /// </summary>
        int ID { get; }

        /// <summary>
        /// Név
        /// </summary>
        string Nev { get; }

        /// <summary>
        /// Neme
        /// </summary>
        Neme Neme { get; }
    }
}
