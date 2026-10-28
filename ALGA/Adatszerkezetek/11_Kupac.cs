using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 11. heti labor feladat - Tesztek: 11_KupacTesztek.cs
    public class Kupac<T>
    {
        /// <summary>
        /// A kupac elemeit tárolja
        /// </summary>
        protected T[] E;

        /// <summary>
        /// A kupacban lévő aktuális elemek száma
        /// </summary>
        protected int n;

        /// <summary>
        /// Eldönti kettő elemről (a, b), hogy melyiknek nagyobb a prioritása
        /// Igaz -> ha a > b
        /// </summary>
        protected Func<T, T, bool> nagyobb;

        public Kupac(T[] E, int n, Func<T, T, bool> nagyobb)
        {
            this.E = E;
            this.n = n;
            this.nagyobb = nagyobb;
            KupacotEpit();
        }

        /// <summary>
        /// Megadja paraméterként átadott i. elem bal gyerekének helyét
        /// </summary>
        /// <param name="i">i. elem helye (index)</param>
        /// <returns>i. elem bal gyerekének helye (index)</returns>
        public static int Bal(int i)
        {
            return 2 * (i + 1) - 1;
        }

        /// <summary>
        /// Megadja paraméterként átadott i. elem jobb gyerekének helyét
        /// </summary>
        /// <param name="i">i. elem helye (index)</param>
        /// <returns>i. elem jobb gyerekének helye (index)</returns>
        public static int Jobb(int i)
        {
            return 2 * (i + 1);
        }

        /// <summary>
        /// Megadja paraméterként átadott i. elem szülőjének helyét
        /// </summary>
        /// <param name="i">i. elem helye (index)</param>
        /// <returns>i. elem szülőjének gyerekének helye (index)</returns>
        public static int Szulo(int i)
        {
            // ha páros az i akkor le kell vonni belőle egyet
            int place = i / 2;
            return i % 2 == 0 ? place - 1 : place;
        }

        /// <summary>
        /// Egy gyökérelem miatt "sérült" kupacot alakít át teljes kupaccá
        /// </summary>
        /// <param name="i">Gyökér elem indexe</param>
        protected void Kupacol(int i)
        {
            // bal és jobb oldali elemek indexének meghatározása
            int b = Bal(i);
            int j = Jobb(i);

            // szülő, bal, jobb oldali elem közöl kiválasztjuk a legnagyobbat

            int max;
            // ha a bal oldali elem indexe a tömbön belül van
            // és nagyobb mint a szülő
            if (b < n && nagyobb(E[b], E[i]))
            {
                // akkor a maximális elemnek a b-t állítjuk be (de ez még változhat)
                max = b;
            }
            else
            {
                max = i;
            }

            // ha a jobb oldali elem indexe a tömbön belül van
            // és nagyobb mint az eddigi max érték
            if (j < n && nagyobb(E[j], E[max]))
            {
                max = j;
            }

            // ha a szülő(i) valamelyik gyereke nagyobb mint saját maga
            // akkor megcseréljük őket, majd meghívjuk a cserélt elem
            // indexére rekurzívan a metódust
            if (max != i)
            {
                T atmeneti = E[i];
                E[i] = E[max];
                E[max] = atmeneti;

                Kupacol(max);
            }
        }

        protected void KupacotEpit()
        {
            int kezdoIndex = n / 2 - 1;

            for (int i = kezdoIndex; i >= 0; i--)
            {
                Kupacol(i);
            }
        }
    }

    // --- 2. Feladat: KupacRendezes ---

    // Itt kötelező a T: IComparable, mert a rendezéshez az alapértelmezett összehasonlítást használjuk
    public class KupacRendezes<T> : Kupac<T>
        where T : IComparable
    {
        public KupacRendezes(T[] A)
            // Az ősnek átadjuk a tömböt, a méretet, és egy lambda kifejezést,
            // ami az IComparable CompareTo metódusát használja.
            : base(A, A.Length, (x, y) => x.CompareTo(y) > 0) { }

        public void Rendezes()
        {
            // A kupacméretet (n) csökkentjük, miközben a legnagyobb elemet (gyökér)
            // kicseréljük az aktuális utolsó elemmel.
            // Mivel az 'n' mező protected, közvetlenül módosíthatjuk.

            // Fontos: Elmentjük az eredeti méretet, ha vissza akarnánk állítani,
            // de a feladat szerint a tömb rendezése a cél.
            int originalSize = n;

            for (int i = n - 1; i > 0; i--)
            {
                // 1. Csere: Gyökér (legnagyobb) <-> Utolsó elem
                T temp = E[0];
                E[0] = E[i];
                E[i] = temp;

                // 2. Méret csökkentése (a rendezett elem már nem része a kupacnak)
                n--;

                // 3. Helyreállítás a gyökértől
                Kupacol(0);
            }

            // Opcionális: visszaállítjuk az n-t, bár a kupac tulajdonság elveszett
            n = originalSize;
        }
    }

    // --- 3. Feladat: KupacPrioritasosSor ---

    public class KupacPrioritasosSor<T> : Kupac<T>, PrioritasosSor<T>
    {
        // Az Üres tulajdonság akkor igaz, ha n == 0
        public bool Ures => n == 0;

        public KupacPrioritasosSor(int meret, Func<T, T, bool> nagyobbPrioritas)
            // Kezdetben 0 elemünk van (n=0), de a tömböt lefoglaljuk a maximális méretre
            : base(new T[meret], 0, nagyobbPrioritas) { }

        // Segédfüggvény: felfelé mozgatja az elemet, amíg a szülője kisebb prioritású
        private void KulcsotFelvisz(int i)
        {
            int szulo = Szulo(i);
            // Amíg nem vagyunk a gyökérben ÉS a gyerek prioritása nagyobb mint a szülőé
            while (i > 0 && nagyobb(E[i], E[szulo]))
            {
                // Csere
                T temp = E[i];
                E[i] = E[szulo];
                E[szulo] = temp;

                // Lépés felfelé
                i = szulo;
                szulo = Szulo(i);
            }
        }

        public void Sorba(T ertek)
        {
            if (n >= E.Length)
                throw new NincsHelyKivetel();

            // Új elem az utolsó helyre
            E[n] = ertek;
            n++;

            // Helyreállítás felfelé
            KulcsotFelvisz(n - 1);
        }

        public T Sorbol()
        {
            if (Ures)
                throw new NincsElemKivetel();

            // A legnagyobb elem (gyökér) kimentése
            T max = E[0];

            // Az utolsó elemet felhozzuk a gyökér helyére
            E[0] = E[n - 1];
            n--;

            // Ha maradt még elem, helyreállítjuk a kupacot lefelé
            if (n > 0)
            {
                Kupacol(0);
            }

            return max;
        }

        public T Elso()
        {
            if (Ures)
                throw new NincsElemKivetel();

            return E[0];
        }

        public void Frissit(T ertek)
        {
            // Megkeressük az elem indexét (lineáris keresés)
            int index = -1;
            for (int i = 0; i < n; i++)
            {
                if (E[i].Equals(ertek))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                throw new NincsElemKivetel(); // Vagy érdemes lenne jelezni, hogy nincs ilyen

            // A PDF szerint hívjuk meg mindkettőt, mert nem tudjuk,
            // nőtt vagy csökkent-e a prioritás.
            // 1. Megpróbáljuk felvinni (ha nőtt a prioritás)
            KulcsotFelvisz(index);
            // 2. Megpróbáljuk levinni (ha csökkent a prioritás)
            // Megjegyzés: a KulcsotFelvisz után az index megváltozhatott volna,
            // de mivel a két művelet közül egyszerre csak az egyik fog ténylegesen mozogni
            // (vagy egyik sem), és a Kupacol rekurzív, elméletileg a 'index' változó
            // itt már nem pontos, de a specifikáció szerint "erre a pontra" hívjuk meg.
            // A helyes implementációban tudnunk kellene az új indexet a Felvisz után.
            // De egyszerűsítve, hívjuk meg az aktuális pozícióra a lefelé mozgatást is.
            Kupacol(index);

            // Profibb megoldás: Ha a KulcsotFelvisz mozgatta, akkor feljebb került,
            // onnan már nem kell Kupacolni (mert a gyerekei biztos kisebbek voltak a réginél is).
            // Ha nem mozgatta, akkor lehet, hogy Kupacolni kell.
        }
    }
}
