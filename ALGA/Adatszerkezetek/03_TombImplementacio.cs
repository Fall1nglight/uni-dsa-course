using System;
using System.Collections;
using System.Collections.Generic;

namespace OE.ALGA.Adatszerkezetek
{
    // 3. heti labor feladat - Tesztek: 03_TombImplementacioTesztek.cs

    public class TombVerem<T> : Verem<T>
    {
        // mezők
        /// <summary>
        /// A verem elemeit tároló tömb
        /// </summary>
        private T[] E;

        /// <summary>
        /// A veremben lévő elemek száma
        /// </summary>
        private int n;

        // tulajdonságok
        /// <summary>
        /// Megmutatja, hogy üres-e a verem
        /// </summary>
        public bool Ures => n == 0;

        /// <summary>
        /// Megmutatja, hogy tele van-e a verem
        /// </summary>
        public bool Teli => n == E.Length;

        // konstruktorok
        public TombVerem(int meret)
        {
            E = new T[meret];
        }

        // metódusok
        /// <summary>
        /// Elhelyezi a veremben a kapott értéket
        /// </summary>
        /// <param name="ertek">Elhelyezendő érték</param>
        /// <exception cref="NincsHelyKivetel">Kivétel, ha tele van a verem</exception>
        public void Verembe(T ertek)
        {
            if (Teli)
                throw new NincsHelyKivetel();

            E[n] = ertek;
            n++;
        }

        /// <summary>
        /// Visszaadja és eltávolítja a verem tetején lévő elemet
        /// </summary>
        /// <returns>A verem tetején lévő elem értéke</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha üres a verem</exception>
        public T Verembol()
        {
            if (Ures)
                throw new NincsElemKivetel();

            T elem = E[n - 1];
            n--;

            return elem;
        }

        /// <summary>
        /// Viszaadja a verem tetején lévő értéket
        /// </summary>
        /// <returns>A verem tetején lávő érték</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha üres a verem</exception>
        public T Felso()
        {
            if (Ures)
                throw new NincsElemKivetel();

            return E[n - 1];
        }

        /// <summary>
        /// Felszabadítja a veremben lévő elemeket
        /// </summary>
        public void Felszabadit()
        {
            // Nem kell implementálni, mivel a Garbage Collector autómatikusan elintézi ezt nekünk
        }
    }

    public class TombSor<T> : Sor<T>
    {
        // mezők
        /// <summary>
        /// A sorban lévő elemeket tároló tömb
        /// </summary>
        private T[] E;

        /// <summary>
        /// Az első kivehető elem indexe
        /// </summary>
        private int e;

        /// <summary>
        /// A következő elem indexe, ahová az új elem kerül
        /// </summary>
        private int u;

        /// <summary>
        /// A veremben lévő elemek száma
        /// </summary>
        private int n;

        // tulajdonságok
        /// <summary>
        /// Megmutatja, hogy a verem üres-e
        /// </summary>
        public bool Ures => n == 0;

        /// <summary>
        /// Megmutatja, hogy tele van-e a verem
        /// </summary>
        public bool Teli => n == E.Length;

        // konstruktorok
        public TombSor(int meret)
        {
            E = new T[meret];
        }

        // metódusok
        /// <summary>
        /// Elhelyezi a sorba a kapott értéket
        /// </summary>
        /// <param name="ertek">Az elhelyezendő érték</param>
        /// <exception cref="NincsHelyKivetel">Kivétel, ha a sor tele van</exception>
        public void Sorba(T ertek)
        {
            if (Teli)
                throw new NincsHelyKivetel();

            // 0<=u<E.Length intervallumba esik, ezért hivatkozhatunk rá
            E[u] = ertek;

            // növeljük az értékét 1-gyel, majd maradékos osztással biztosítjuk
            // hogy ne legyen túlcímezve az E tömb
            u = (u + 1) % E.Length;
            n++;
        }

        /// <summary>
        /// Visszaadja és eltávolítja a sorban lévő első elemet
        /// </summary>
        /// <returns>A sorban lévő első elem értéke</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha üres a sor</exception>
        public T Sorbol()
        {
            if (Ures)
                throw new NincsElemKivetel();

            // 0<=e<E.Length intervallumba esik
            T elem = E[e];
            e = (e + 1) % E.Length;
            n--;

            return elem;
        }

        /// <summary>
        /// Visszaadja a sorban lévő első elemet
        /// </summary>
        /// <returns>A sorban lévő első elem értéke</returns>
        /// <exception cref="NincsElemKivetel"></exception>
        public T Elso()
        {
            if (Ures)
                throw new NincsElemKivetel();

            // "e" a megszabott intervallumba esik ezért nem kell maradékos osztást végezni itt
            return E[e];
        }

        /// <summary>
        /// Felszabadítja a sorban lévő elemeket
        /// </summary>
        public void Felszabadit()
        {
            // Nem kell implementálni a Garbage Collector miatt
        }
    }

    public class TombLista<T> : Lista<T>, IEnumerable<T>
    {
        // mezők
        /// <summary>
        /// A listában lévő elemeket tároló tömb
        /// </summary>
        private T[] E;

        /// <summary>
        /// Listában lévő elemek száma
        /// </summary>
        private int n;

        // tulajdonságok
        /// <summary>
        /// Listában lévő elemek száma
        /// </summary>
        public int Elemszam => n;

        // konstruktorok
        public TombLista(int meret = 32)
        {
            E = new T[meret];
        }

        // metódusok
        public IEnumerator<T> GetEnumerator() => new TombListaBejaro<T>(E, n);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Kétszeresére növeli a lista elemeit tároló tömb méretét <br />
        /// Ezzel lehetővé téve a lista dinamikus működését
        /// </summary>
        private void MeretNovel()
        {
            // eltároljuk az eredeti tömb referenciáját
            T[] atmeneti = E;

            // felülírjuk az eredeti E tömb referenciáját
            // egy nála kétszer nagyobb tömbbel
            E = new T[E.Length * 2];

            // bejárjuk a régi E tömböt
            // és átmosuljuk az elemeket az új, nagyobb tömbbe
            for (int i = 0; i < n; i++)
            {
                E[i] = atmeneti[i];
            }
        }

        /// <summary>
        /// Ellenőrzi, hogy a megadott index helyes-e <br />
        /// </summary>
        /// <param name="index">Az ellenőrizendő index</param>
        /// <returns></returns>
        private bool HelyesIndex(int index) => index >= 0 && index < Elemszam;

        /// <summary>
        /// Visszaadja a megadott indexen lévő elem értékét
        /// </summary>
        /// <param name="index">A kiolvasni kívánt elem indexe</param>
        /// <returns>A kiolvasott elem értéke</returns>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha az index helytelen</exception>
        public T Kiolvas(int index)
        {
            if (!HelyesIndex(index))
                throw new HibasIndexKivetel();

            return E[index];
        }

        /// <summary>
        /// Módosítja a megadott indexen lévő elem értékét a megadott értékre
        /// </summary>
        /// <param name="index">A módosítani kívánt elem indexe</param>
        /// <param name="ertek">Az új, módosított érték</param>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha az index helytelen</exception>
        public void Modosit(int index, T ertek)
        {
            if (!HelyesIndex(index))
                throw new HibasIndexKivetel();

            E[index] = ertek;
        }

        /// <summary>
        /// Hozzáfűzi a megadott értéket a lista végére
        /// </summary>
        /// <param name="ertek">A hozzáfűzni kívánt érték</param>
        public void Hozzafuz(T ertek)
        {
            if (Elemszam == E.Length)
                MeretNovel();

            E[n++] = ertek;
        }

        /// <summary>
        /// Beszurja a megadott indexre a megadott értéket
        /// </summary>
        /// <param name="index">A beszúrandó érték indexe</param>
        /// <param name="ertek">A beszurandó érték</param>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha az index helytelen</exception>
        public void Beszur(int index, T ertek)
        {
            // ellenőrizzük, hogy helyes-e az index
            // fontos: az index értéke lehet "n" is ebben az esetben
            // hiszen a lista egy dinamikus adatszerkezet és, ha az utolsó
            // elem után szeretnénk beszúrni elemet azt is lehetővé kell tennünk
            if (index < 0 || index > Elemszam)
                throw new HibasIndexKivetel();

            // ha megtelt a tömb, növeljük a méretét
            if (Elemszam == E.Length)
                MeretNovel();

            n++;

            // n-től index-ig visszafelé haladva
            // eltoljuk az elemeket egyel jobbra
            for (int i = n; i > index; i--)
            {
                E[i] = E[i - 1];
            }

            E[index] = ertek;
        }

        /// <summary>
        /// Törli az összes értékkel megegyező lista-beli elemet
        /// </summary>
        /// <param name="ertek">A törölni kívánt érték</param>
        public void Torol(T ertek)
        {
            // törölt elemek száma
            int db = 0;

            // bejárjuk a lista elemeit
            for (int i = 0; i < n; i++)
            {
                // ha az aktuális elem megegyezik a törlendő értékkel növeljük a "db" értékét
                if (E[i]!.Equals(ertek))
                {
                    db++;
                }
                // ha nincs egyezés akkor mozgatjuk az elemet
                // annyival, amennyit eddig töröltünk
                // ezáltal felülírjuk a törlendő elemeket
                else
                {
                    E[i - db] = E[i];
                }
            }

            // csökkentjük az elemszámot a talált elemek értékével
            n -= db;
        }

        /// <summary>
        /// Végrehatjuk a lista összes elemén a megadott műveletet
        /// </summary>
        /// <param name="muvelet">A végrehajtandó művelet</param>
        public void Bejar(Action<T> muvelet)
        {
            // bejárjuk a lista elemeit
            for (int i = 0; i < n; i++)
            {
                // az aktuális elemen végrehatjuk a megadott műveletet
                muvelet(E[i]);
            }
        }

        /// <summary>
        /// Felszabadítja a lista elemeit
        /// </summary>
        public void Felszabadit()
        {
            // Nem kell implementálni, a Garbage Collector miatt
        }
    }

    public class TombListaBejaro<T> : IEnumerator<T>
    {
        // mezők
        private T[] E;
        private int n;
        private int aktualisIndex = -1;

        // tulajdonságok
        public T Current => E[aktualisIndex];

        object? IEnumerator.Current => Current;

        // konstruktorok
        public TombListaBejaro(T[] E, int n)
        {
            this.E = E;
            this.n = n;
        }

        // metódusok
        public bool MoveNext()
        {
            // növeljük az indexet 1-gyel
            aktualisIndex++;

            // ha az aktIndex kisebb mint n, akkor vannak még elemek a listában
            // ellenkező esetben elértünk a lista végére
            return aktualisIndex < n;
        }

        public void Reset()
        {
            aktualisIndex = -1;
        }

        public void Dispose() { }
    }
}
