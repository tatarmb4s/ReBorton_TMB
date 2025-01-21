namespace Borton_Lib.Classes
{
    /// <summary>
    /// Egyszerű statikus ID-generátor.
    /// A GetNextId hívásával mindig egyedi, növekvő számot kapunk.
    /// </summary>
    public static class IdGenerator
    {
        private static int _lastId = 0;

        /// <summary>
        /// Visszaad egy új, egyedi azonosítót növekvő sorrendben.
        /// </summary>
        /// <returns>A generált ID</returns>
        public static int GetNextId()
        {
            return Interlocked.Increment(ref _lastId);
        }

        /// <summary>
        /// Beállítja a kezdőértéket. Ha a beérkező érték nagyobb, mint a mostani _lastId,
        /// akkor onnan folytatjuk a számozást, különben nem változtatjuk.
        /// </summary>
        /// <param name="startValue">A kezdő (vagy minimum) ID</param>
        public static void InitIfHigher(int startValue)
        {
            // Szálbiztosan kicseréljük, csak ha startValue nagyobb, mint a jelenlegi
            int current = _lastId;
            if (startValue > current)
            {
                Interlocked.Exchange(ref _lastId, startValue);
            }
        }
    }
}
