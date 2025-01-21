// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
namespace Borton_Lib.Classes
{
    /// <summary>
    /// Egy börtön, aminek van neve, tulajdonosa,
    /// cellái, és börtönőrei.
    /// </summary>
    public class Borton
    {
        private string nev;
        private Tulajdonos tulajdonos;
        private List<Cell> cellak;
        private List<Bortonor> bortonorok;

        /// <summary>
        /// Börtön konstruktor
        /// </summary>
        /// <param name="nev">A börtön neve</param>
        /// <param name="tulajdonos">Tulajdonos</param>
        public Borton(string nev, Tulajdonos tulajdonos)
        {
            this.nev = nev;
            this.tulajdonos = tulajdonos;
            cellak = new List<Cell>();
            bortonorok = new List<Bortonor>();
        }

        /// <summary>
        /// A börtön neve
        /// </summary>
        public string Nev => nev;

        /// <summary>
        /// Visszaadja a tulajdonost
        /// </summary>
        /// <returns>Tulajdonos</returns>
        public Tulajdonos GetTulajdonos()
        {
            return tulajdonos;
        }

        /// <summary>
        /// Visszaadja a börtön celláit
        /// </summary>
        /// <returns>Cella lista (csak olvasható)</returns>
        public IReadOnlyList<Cell> GetCellak()
        {
            return cellak;
        }

        /// <summary>
        /// Visszaadja a börtönőreit
        /// </summary>
        /// <returns>Börtönőr lista (csak olvasható)</returns>
        public IReadOnlyList<Bortonor> GetOrok()
        {
            return bortonorok;
        }

        /// <summary>
        /// Lekérhetjük a börtönben lévő összes rabot 
        /// (a cellákon átnézve).
        /// </summary>
        /// <returns>Az összes rab</returns>
        public IEnumerable<Rab> GetAllRabok()
        {
            foreach (var cella in cellak)
            {
                var rabok = cella.GetRabot();
                foreach (var rab in rabok)
                {
                    yield return rab;
                }
            }
        }

        /// <summary>
        /// Hozzáadunk egy cellát a börtönhöz
        /// </summary>
        /// <param name="cell">Az új cella</param>
        public void AddCell(Cell cell)
        {
            cellak.Add(cell);
        }

        /// <summary>
        /// Hozzáad egy börtönőrt
        /// </summary>
        /// <param name="bortonor">Az új börtönőr</param>
        public void AddBortonor(Bortonor bortonor)
        {
            bortonorok.Add(bortonor);
        }
    }
}
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/