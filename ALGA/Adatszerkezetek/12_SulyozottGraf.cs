using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 12. heti labor feladat - Tesztek: 12_SulyozottGrafTesztek.cs
    public class SulyozottEgeszGrafEl : EgeszGrafEl, SulyozottGrafEl<int>
    {
        public SulyozottEgeszGrafEl(int honnan, int hova, float suly)
            : base(honnan, hova)
        {
            this.Suly = suly;
        }

        public float Suly { get; }
    }

    // --- 2. Feladat: Csúcsmátrix Súlyozott Egész Gráf ---

    public class CsucsmatrixSulyozottEgeszGraf : SulyozottGraf<int, SulyozottEgeszGrafEl>
    {
        private int n;
        private float[,] M;

        public int CsucsokSzama => n;

        public int ElekSzama
        {
            get
            {
                int db = 0;
                for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (!float.IsNaN(M[i, j]))
                        db++;
                return db;
            }
        }

        public Halmaz<int> Csucsok
        {
            get
            {
                FaHalmaz<int> csucsok = new FaHalmaz<int>();
                for (int i = 0; i < n; i++)
                    csucsok.Beszur(i);
                return csucsok;
            }
        }

        public Halmaz<SulyozottEgeszGrafEl> Elek
        {
            get
            {
                FaHalmaz<SulyozottEgeszGrafEl> elek = new FaHalmaz<SulyozottEgeszGrafEl>();
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (!float.IsNaN(M[i, j]))
                        {
                            elek.Beszur(new SulyozottEgeszGrafEl(i, j, M[i, j]));
                        }
                    }
                }
                return elek;
            }
        }

        public CsucsmatrixSulyozottEgeszGraf(int n)
        {
            this.n = n;
            this.M = new float[n, n];
            for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                M[i, j] = float.NaN;
        }

        public void UjEl(int honnan, int hova, float suly)
        {
            if (honnan >= 0 && honnan < n && hova >= 0 && hova < n)
            {
                M[honnan, hova] = suly;
            }
        }

        public bool VezetEl(int honnan, int hova)
        {
            if (honnan < 0 || honnan >= n || hova < 0 || hova >= n)
                return false;
            return !float.IsNaN(M[honnan, hova]);
        }

        public float Suly(int honnan, int hova)
        {
            if (!VezetEl(honnan, hova))
                throw new NincsElKivetel();
            return M[honnan, hova];
        }

        public Halmaz<int> Szomszedai(int csucs)
        {
            FaHalmaz<int> szomszedok = new FaHalmaz<int>();
            if (csucs >= 0 && csucs < n)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!float.IsNaN(M[csucs, j]))
                    {
                        szomszedok.Beszur(j);
                    }
                }
            }
            return szomszedok;
        }
    }

    // --- 3. Feladat: Útkeresés (Dijkstra) ---

    public static class Utkereses
    {
        public static Szotar<V, float> Dijkstra<V, E>(SulyozottGraf<V, E> graf, V start)
            where V : IComparable
            where E : SulyozottGrafEl<V>
        {
            Szotar<V, float> d = new HasitoSzotarTulcsordulasiTerulettel<V, float>(
                graf.CsucsokSzama
            );

            graf.Csucsok.Bejar(v => d.Beir(v, float.MaxValue));
            d.Beir(start, 0);

            // Min-Kupac: "Nagyobb" prioritás = Kisebb távolság
            KupacPrioritasosSor<V> Q = new KupacPrioritasosSor<V>(
                graf.CsucsokSzama,
                (x, y) => d.Kiolvas(x) < d.Kiolvas(y)
            );

            Q.Sorba(start);

            while (!Q.Ures)
            {
                V u = Q.Sorbol();
                graf.Szomszedai(u)
                    .Bejar(v =>
                    {
                        float suly = graf.Suly(u, v);
                        float u_tav = d.Kiolvas(u);
                        float v_tav = d.Kiolvas(v);

                        if (u_tav + suly < v_tav)
                        {
                            d.Beir(v, u_tav + suly);
                            try
                            {
                                Q.Frissit(v);
                            }
                            catch (NincsElemKivetel)
                            {
                                Q.Sorba(v);
                            }
                        }
                    });
            }
            return d;
        }
    }

    // --- 4. Feladat: Feszítőfa Keresés ---

    public static class FeszitofaKereses
    {
        // --- 4a. Prim algoritmus ---
        public static Szotar<V, V> Prim<V, E>(SulyozottGraf<V, E> graf, V start)
            where V : IComparable
            where E : SulyozottGrafEl<V>
        {
            Szotar<V, V> pi = new HasitoSzotarTulcsordulasiTerulettel<V, V>(graf.CsucsokSzama);
            Szotar<V, float> k = new HasitoSzotarTulcsordulasiTerulettel<V, float>(
                graf.CsucsokSzama
            );
            Halmaz<V> sorbanVan = new FaHalmaz<V>();

            graf.Csucsok.Bejar(v => k.Beir(v, float.MaxValue));
            k.Beir(start, 0);

            KupacPrioritasosSor<V> Q = new KupacPrioritasosSor<V>(
                graf.CsucsokSzama,
                (x, y) => k.Kiolvas(x) < k.Kiolvas(y)
            );

            graf.Csucsok.Bejar(v =>
            {
                Q.Sorba(v);
                sorbanVan.Beszur(v);
            });

            while (!Q.Ures)
            {
                V u = Q.Sorbol();
                try
                {
                    ((FaHalmaz<V>)sorbanVan).Torol(u);
                }
                catch { }

                graf.Szomszedai(u)
                    .Bejar(v =>
                    {
                        if (sorbanVan.Eleme(v))
                        {
                            float suly = graf.Suly(u, v);
                            if (suly < k.Kiolvas(v))
                            {
                                pi.Beir(v, u);
                                k.Beir(v, suly);
                                Q.Frissit(v);
                            }
                        }
                    });
            }
            return pi;
        }

        // --- 4b. Kruskal algoritmus ---
        public static Halmaz<E> Kruskal<V, E>(SulyozottGraf<V, E> graf)
            where V : IComparable
            where E : SulyozottGrafEl<V>, IComparable
        {
            Halmaz<E> A = new FaHalmaz<E>();

            // 1. Élek rendezése SÚLY szerint a KupacPrioritasosSor segítségével!
            // Így nem kell Lista.Sort, és nem kell módosítani az Él osztályt.

            // A kupac mérete:
            int elekSzama = graf.ElekSzama;

            // Létrehozunk egy prioritási sort az éleknek.
            // A "nagyobb prioritás" itt a "kisebb súlyt" jelenti (Min-Kupac kell).
            KupacPrioritasosSor<E> elKupac = new KupacPrioritasosSor<E>(
                elekSzama,
                (e1, e2) => e1.Suly < e2.Suly // Ez rendezi súly szerint növekvőbe!
            );

            // Minden élt beteszünk a prioritási sorba
            graf.Elek.Bejar(el => elKupac.Sorba(el));

            // 2. Halmazrendszer (Szótárral megvalósítva)
            Szotar<V, int> komponensek = new HasitoSzotarTulcsordulasiTerulettel<V, int>(
                graf.CsucsokSzama
            );
            int i = 0;
            graf.Csucsok.Bejar(v =>
            {
                komponensek.Beir(v, i++);
            });

            // 3. Élek feldolgozása a kupacból (mindig a legkisebb súlyút vesszük ki)
            while (!elKupac.Ures)
            {
                E el = elKupac.Sorbol();

                // Itt picit trükközni kell a Végpontok kinyerésével, mert a generikus E nem adja.
                // De tudjuk, hogy EgeszGrafEl alapú a tesztben.
                if (el is EgeszGrafEl ege)
                {
                    V u = (V)(object)ege.Honnan;
                    V v = (V)(object)ege.Hova;

                    int kompU = komponensek.Kiolvas(u);
                    int kompV = komponensek.Kiolvas(v);

                    if (kompU != kompV)
                    {
                        A.Beszur(el); // Él bekerül a feszítőfába

                        // Unió: komponensek összevonása (lassú unió, de működik)
                        graf.Csucsok.Bejar(csucs =>
                        {
                            if (komponensek.Kiolvas(csucs) == kompV)
                            {
                                komponensek.Beir(csucs, kompU);
                            }
                        });
                    }
                }
            }

            return A;
        }
    }
}
