// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
namespace Borton_Lib.Exceptions
{
    /// <summary>
    /// A börtön logikában fellépő kivételekhez használt egyedi kivétel
    /// </summary>
    public class BortonException : Exception
    {
        /// <summary>
        /// Alapértelmezett konstruktor
        /// </summary>
        public BortonException() { }

        /// <summary>
        /// Üzenettel ellátott kivétel
        /// </summary>
        /// <param name="message">Hibaüzenet</param>
        public BortonException(string message) : base(message) { }

        /// <summary>
        /// Üzenettel és belső kivétellel ellátott konstruktor
        /// </summary>
        /// <param name="message">Hibaüzenet</param>
        /// <param name="innerException">Belső kivétel</param>
        public BortonException(string message, Exception innerException) : base(message, innerException) { }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/