// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
namespace Borton_Lib.Interfaces
{
    /// <summary>
    /// Interfész azok számára, akik egy cellában tartózkodnak
    /// </summary>
    public interface IHasCell
    {
        /// <summary>
        /// A cella referencia
        /// </summary>
        Borton_Lib.Classes.Cell Cell { get; }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/