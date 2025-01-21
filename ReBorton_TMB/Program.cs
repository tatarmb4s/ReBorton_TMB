// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/
using Borton_Lib.Classes;
using ReBorton_TMB;

Console.Title = "Börtönkezelő TMB (KonZol)";

// Például a futtatható mappában lévő data.json
string dataFilePath = "data.json";

// Létrehozzuk a börtönkezelőt
BortonKezelo kezelo = new BortonKezelo();

// Betöltjük, ha van
kezelo.BetoltesJsonbol(dataFilePath);

// Elindítjuk a menüt
var menu = new BortonMenu(kezelo, dataFilePath);
menu.Run();

// A program sose fusson hibára, mindent try-catch-csel kezelünk a menü oldalon
// Tehát kilépés
Console.WriteLine("Kilépés...");
// Copyright: 2025 Tatár Mátyás Bence - https://tatarmb.hu/