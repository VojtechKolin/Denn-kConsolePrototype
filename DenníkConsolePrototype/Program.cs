using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DenníkConsolePrototype
{
    internal class Program
    {
        class Zaznam
        {
            public DateTime Datum { get; set; }
            public string Text { get; set; }
        }

        static LinkedList<Zaznam> Denik = new LinkedList<Zaznam>();
        static LinkedListNode<Zaznam> AktualniZaznam;

        static bool ZmenyVDeniku = false;

        static void Main()
        {
            NactiDenikZeSouboru(); // Načtení deníku při spuštění programu

            if (Denik.Count > 0)
            {
                AktualniZaznam = Denik.First;
                VypisAktualniZaznam();
            }

            while (true)
            {
                VypisMenu();
                string volba = Console.ReadLine().ToLower();

                switch (volba)
                {
                    case "novy":
                        PridatNovyZaznam();
                        ZmenyVDeniku = true; // Nastavení příznaku změn v deníku
                        break;
                    case "predchozi":
                        ZobrazPredchoziZaznam();
                        break;
                    case "dalsi":
                        ZobrazNasledujiciZaznam();
                        break;
                    case "uloz":
                        UlozitDenikDoSouboru();
                        break;
                    case "smaz":
                        SmazatZaznam();
                        ZmenyVDeniku = true; // Nastavení příznaku změn v deníku
                        break;
                    case "zavri":
                        UlozDenikDoSouboru(); // Uložení deníku při ukončení programu
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Neplatný příkaz, zkuste to znovu.");
                        break;
                }
            }
        }

        static void VypisMenu()
        {
            Console.WriteLine("###############################################");
            Console.WriteLine("Seznam příkazů:");
            Console.WriteLine(" - predchozi");
            Console.WriteLine(" - dalsi");
            Console.WriteLine(" - novy");
            Console.WriteLine(" - uloz");
            Console.WriteLine(" - smaz");
            Console.WriteLine(" - zavri");
            Console.WriteLine($"Počet záznamů v deníku: {Denik.Count}");
            Console.WriteLine("###############################################");
        }

        static void PridatNovyZaznam()
        {
            Zaznam novyZaznam = new Zaznam();

            string inputDate;
            DateTime parsedDate;

            do
            {
                Console.WriteLine("Zadejte datum (formát yyyy-mm-dd):");
                inputDate = Console.ReadLine();

                if (DateTime.TryParse(inputDate, out parsedDate))
                {
                    novyZaznam.Datum = parsedDate;
                }
                else
                {
                    Console.WriteLine("Nesprávný formát data, zkuste to znovu:");
                }
            } while (novyZaznam.Datum == default);

            Console.WriteLine("Zadejte text záznamu:");
            novyZaznam.Text = Console.ReadLine();

            Denik.AddLast(novyZaznam);
            AktualniZaznam = Denik.Last;

            Console.WriteLine("Nový záznam byl přidán:");
            VypisAktualniZaznam();
        }

        static void ZobrazPredchoziZaznam()
        {
            if (AktualniZaznam?.Previous != null)
            {
                AktualniZaznam = AktualniZaznam.Previous;
                VypisAktualniZaznam();
            }
            else
            {
                //Console.WriteLine("Jste na začátku deníku.");
                Console.WriteLine(" ");
            }
        }

        static void ZobrazNasledujiciZaznam()
        {
            if (AktualniZaznam?.Next != null)
            {
                AktualniZaznam = AktualniZaznam.Next;
                VypisAktualniZaznam();
            }
            else
            {
                //Console.WriteLine("Jste na konci deníku.");
                Console.WriteLine(" ");
            }
        }

        static void UlozitDenikDoSouboru()
        {
            if (ZmenyVDeniku)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter("denik.csv"))
                    {
                        foreach (var zaznam in Denik)
                        {
                            sw.WriteLine($"{zaznam.Datum.ToString("yyyy-MM-dd")},{zaznam.Text}");
                        }
                    }
                    ZmenyVDeniku = false; // Resetování příznaku změn po uložení
                    Console.WriteLine("Deník byl uložen do souboru.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Chyba při ukládání deníku do souboru: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Žádné změny k uložení.");
            }
        }

        static void SmazatZaznam()
        {
            if (AktualniZaznam != null)
            {
                Console.WriteLine("Odstraňovaný záznam:");
                VypisAktualniZaznam();
                Console.WriteLine("Potvrdit smazání? (ano/ne)");

                if (Console.ReadLine().ToLower() == "ano")
                {
                    LinkedListNode<Zaznam> predchozi = AktualniZaznam.Previous;
                    Denik.Remove(AktualniZaznam);
                    AktualniZaznam = predchozi;

                    Console.WriteLine("Záznam byl smazán.");
                    if (AktualniZaznam != null)
                    {
                        VypisAktualniZaznam();
                    }
                    else
                    {
                        Console.WriteLine("Deník je prázdný.");
                    }
                }
                else
                {
                    Console.WriteLine("Smazání záznamu bylo zrušeno.");
                }
            }
            else
            {
                Console.WriteLine("Není žádný záznam k odstranění.");
            }
        }

        static void VypisAktualniZaznam()
        {
            Console.WriteLine($"Datum: {AktualniZaznam.Value.Datum.ToString("yyyy-MM-dd")}");
            Console.WriteLine($"Text: {AktualniZaznam.Value.Text}");
        }

        static void NactiDenikZeSouboru()
        {
            try
            {
                if (File.Exists("denik.csv"))
                {
                    using (StreamReader sr = new StreamReader("denik.csv"))
                    {
                        while (!sr.EndOfStream)
                        {
                            string[] line = sr.ReadLine().Split(',');
                            if (line.Length == 2 && DateTime.TryParse(line[0], out DateTime datum))
                            {
                                Denik.AddLast(new Zaznam { Datum = datum, Text = line[1] });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při načítání deníku ze souboru: {ex.Message}");
            }
        }
        static void UlozDenikDoSouboru()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter("denik.csv"))
                {
                    foreach (var zaznam in Denik)
                    {
                        sw.WriteLine($"{zaznam.Datum.ToString("yyyy-MM-dd")},{zaznam.Text}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba při ukládání deníku do souboru: {ex.Message}");
            }
        }
    }
}
