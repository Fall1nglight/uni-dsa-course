using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 10. heti labor feladat - Tesztek: 10_SulyozatlanGrafTesztek.cs

    public class EgeszGrafEl : GrafEl<int>, IComparable
    {
        /// <summary>
        /// Melyik csúcsból
        /// </summary>
        public int Honnan { get; }

        /// <summary>
        /// Melyik csúcsba
        /// </summary>
        public int Hova { get; }

        public EgeszGrafEl(int honnan, int hova)
        {
            Honnan = honnan;
            Hova = hova;
        }

        // FaHalmaz-ban való tárolás miatt elengedhetetlen
        // az IComparable interfész megvalósítása
        public virtual int CompareTo(object? obj)
        {
            if (ReferenceEquals(obj, null))
                throw new ArgumentNullException();

            if (ReferenceEquals(this, obj))
                return 0;

            if (obj is not EgeszGrafEl b)
                throw new ArgumentException();

            // szabályok:
            // 1. ha this.Honnan != b.Honnan
            // => az az él a kisebb, amelyiknek kisebb a "Honnan" értéke
            // 2. ha this.Honnan == b.Honnan
            // => az az él a kisebb, amelyiknek kisebb a "Hova" értéke
            // 3. ha mindkét tulajdonság értéke ugyanaz
            // => a két gráf egyenlő

            // összehasonlítjuk a 'Honnan' értékeket
            int honnanOsszehasonlitas = this.Honnan.CompareTo(b.Honnan);

            // ha NEM egyenlőek, akkor ez dönts
            if (honnanOsszehasonlitas != 0)
            {
                return honnanOsszehasonlitas;
            }

            // ha egyenlőek voltak
            // akkor a 'Hova' dönt
            return this.Hova.CompareTo(b.Hova);
        }
    }

    public class CsucsmatrixSulyozatlanEgeszGraf : SulyozatlanGraf<int, EgeszGrafEl>
    {
        // mezők
        /// <summary>
        /// Csúcsok száma
        /// </summary>
        private int n;

        /// <summary>
        /// Csúcsmátrix, ahol <br />
        /// i => melyik csúcsból, j => melyik csúcsba | fut él
        /// </summary>
        private bool[,] M;

        // tulajdonságok
        /// <summary>
        /// Megadja a csúcsok számát
        /// </summary>
        public int CsucsokSzama => n;

        /// <summary>
        /// Megadja az élek (M tömbben található igaz értékek) számát
        /// </summary>
        public int ElekSzama => GetElekSzama();

        /// <summary>
        /// Megadja a csúcsok azonsítóit [0, n-1]
        /// </summary>
        public Halmaz<int> Csucsok => GetCsucsokSzama();

        /// <summary>
        /// Megadja a gráfban található élek halmazát
        /// </summary>
        public Halmaz<EgeszGrafEl> Elek => GetElek();

        // konstruktorok
        public CsucsmatrixSulyozatlanEgeszGraf(int csucsokSzama)
        {
            this.n = csucsokSzama;
            this.M = new bool[this.n, this.n];
        }

        // metódusok
        /// <summary>
        /// Megadja az élek számát
        /// </summary>
        /// <returns>Az M tömbben található igaz értékek száma)</returns>
        private int GetElekSzama()
        {
            int db = 0;

            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    if (M[i, j])
                        db++;
                }
            }

            return db;
        }

        /// <summary>
        /// Megadja a csúcsok azonosítóit
        /// </summary>
        /// <returns>A csúcsok azonosítói [0, n-1]</returns>
        private FaHalmaz<int> GetCsucsokSzama()
        {
            FaHalmaz<int> csucsok = new FaHalmaz<int>();

            for (int i = 0; i < n; i++)
            {
                csucsok.Beszur(i);
            }

            return csucsok;
        }

        /// <summary>
        /// Megadja a gráfban található éleket
        /// </summary>
        /// <returns>Gráfban található élek halmaza</returns>
        private FaHalmaz<EgeszGrafEl> GetElek()
        {
            FaHalmaz<EgeszGrafEl> elek = new FaHalmaz<EgeszGrafEl>();

            for (int i = 0; i < M.GetLength(0); i++)
            {
                for (int j = 0; j < M.GetLength(1); j++)
                {
                    if (M[i, j])
                    {
                        EgeszGrafEl el = new EgeszGrafEl(i, j);
                        elek.Beszur(el);
                    }
                }
            }

            return elek;
        }

        /// <summary>
        /// Megadja hogy az egyik csúcsból vezet-e él másik megadott csúcsba
        /// </summary>
        /// <param name="honnan">Melyik csúcsból indul az él</param>
        /// <param name="hova">Melyik csúcsba fut be az él</param>
        public bool VezetEl(int honnan, int hova)
        {
            return M[honnan, hova];
        }

        /// <summary>
        /// Megadja azokat a csúcsazonosítókat, amelyekbe a megadott csúcsból vezet él
        /// </summary>
        /// <param name="csucs">A vizsgálni kívánt csúcs azonosítója</param>
        public Halmaz<int> Szomszedai(int csucs)
        {
            FaHalmaz<int> szomszedok = new FaHalmaz<int>();

            for (int j = 0; j < M.GetLength(1); j++)
            {
                if (M[csucs, j])
                {
                    szomszedok.Beszur(j);
                }
            }

            return szomszedok;
        }

        /// <summary>
        /// Beír egy új élt a gráfba
        /// </summary>
        /// <param name="honnan">Melyik csúcsból indul az él</param>
        /// <param name="hova">Melyik csúcsba fut be az él</param>
        public void UjEl(int honnan, int hova)
        {
            M[honnan, hova] = true;
        }
    }

    public class GrafBejarasok
    {
        public static Halmaz<V> SzelessegiBejaras<V, E>(Graf<V, E> graf, V start, Action<V> muvelet)
            where V : IComparable
        {
            // feldolgozásra váró elemek
            Sor<V> sor = new TombSor<V>(graf.CsucsokSzama);

            // feldolgozott / elért elemek
            Halmaz<V> elertElemek = new FaHalmaz<V>();

            sor.Sorba(start);
            elertElemek.Beszur(start);

            while (!sor.Ures)
            {
                V vizsgaltCsucs = sor.Sorbol();

                // feldolgozzuk a csúcsot
                muvelet(vizsgaltCsucs);

                // bejárjuk a csúcs szomszédait
                graf.Szomszedai(vizsgaltCsucs)
                    .Bejar(szomszed =>
                    {
                        // ha még nem értük el az adott szomszédot
                        if (!elertElemek.Eleme(szomszed))
                        {
                            // elhelyezzük a sorba, későbbi feldolgozásra
                            sor.Sorba(szomszed);

                            // megjegyezzük, hogy többször ne dolgozzuk fel
                            elertElemek.Beszur(szomszed);
                        }
                    });
            }

            return elertElemek;
        }

        public static Halmaz<V> MelysegiBejaras<V, E>(Graf<V, E> graf, V start, Action<V> muvelet)
            where V : IComparable
        {
            FaHalmaz<V> elertElemek = new FaHalmaz<V>();

            MelysegiBejarasRekurzio(graf, start, elertElemek, muvelet);

            return elertElemek;
        }

        private static void MelysegiBejarasRekurzio<V, E>(
            Graf<V, E> graf,
            V vizsgaltCsucs,
            Halmaz<V> elertElemek,
            Action<V> muvelet
        )
        {
            muvelet(vizsgaltCsucs);
            elertElemek.Beszur(vizsgaltCsucs);

            graf.Szomszedai(vizsgaltCsucs)
                .Bejar(szomszed =>
                {
                    if (!elertElemek.Eleme(szomszed))
                    {
                        MelysegiBejarasRekurzio(graf, szomszed, elertElemek, muvelet);
                    }
                });
        }
    }
}
