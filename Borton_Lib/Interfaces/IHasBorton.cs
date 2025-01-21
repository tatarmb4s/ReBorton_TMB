// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
namespace Borton_Lib.Interfaces
{
    /// <summary>
    /// Interfész azok számára, akik "tartoznak" egy börtönhöz
    /// </summary>
    public interface IHasBorton
    {
        /// <summary>
        /// A börtön referencia
        /// </summary>
        Borton_Lib.Classes.Borton Borton { get; }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/