using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 6. heti labor feladat - Tesztek: 06_SzotarTesztek.cs
    public class SzotarElem<K, T>
    {
        // mezők
        /// <summary>
        /// Az eltárolandó adathoz tartozó kuécs
        /// </summary>
        public K kulcs;

        /// <summary>
        /// Az eltárolandó adat
        /// </summary>
        public T tart;

        // konstruktorok
        public SzotarElem(K kulcs, T tart)
        {
            this.kulcs = kulcs;
            this.tart = tart;
        }
    }

    public class HasitoSzotarTulcsordulasiTerulettel<K, T> : Szotar<K, T>
    {
        // mezők
        /// <summary>
        /// Szótárelemeket tároló tömb
        /// </summary>
        private SzotarElem<K, T>[] E;

        /// <summary>
        /// Hasítófüggvény
        /// </summary>
        private Func<K, int> h;

        /// <summary>
        /// Túlcsordulási terület
        /// </summary>
        private LancoltLista<SzotarElem<K, T>> U;

        // konstruktor
        public HasitoSzotarTulcsordulasiTerulettel(int meret, Func<K, int> hasitoFv)
        {
            this.E = new SzotarElem<K, T>[meret];
            this.h = H;

            int H(K arg)
            {
                return Math.Abs(hasitoFv(arg) % E.Length);
            }

            // this.h = x => Math.Abs(hasitoFv(x) % E.Length);

            this.U = new LancoltLista<SzotarElem<K, T>>();
        }

        public HasitoSzotarTulcsordulasiTerulettel(int meret)
            : this(meret, x => x.GetHashCode()) { }

        /// <summary>
        /// Visszaadja a szótárban a megadott kulcshoz rendelt SzotarElem-et
        /// </summary>
        /// <param name="kulcs">A keresni kívánt elem kulcsa</param>
        /// <returns></returns>
        private SzotarElem<K, T> KulcsKeres(K kulcs)
        {
            SzotarElem<K, T> keresettElem = E[h(kulcs)];

            if (keresettElem != null! && keresettElem.kulcs!.Equals(kulcs))
                return keresettElem;

            keresettElem = null!;
            U.Bejar(elem =>
            {
                if (elem.kulcs!.Equals(kulcs))
                {
                    keresettElem = elem;
                }
            });

            return keresettElem;
        }

        /// <summary>
        /// Beírja a szótárba a megadott értéket, kulcs alapján
        /// </summary>
        /// <param name="kulcs">A beirandó érték kulcsa</param>
        /// <param name="ertek">A beirandó érték</param>
        public void Beir(K kulcs, T ertek)
        {
            SzotarElem<K, T> keresettElem = KulcsKeres(kulcs);

            if (keresettElem != null!)
            {
                keresettElem.tart = ertek;
                return;
            }

            SzotarElem<K, T> ujElem = new SzotarElem<K, T>(kulcs, ertek);

            if (E[h(kulcs)] == null!)
            {
                E[h(kulcs)] = ujElem;
            }
            else
            {
                U.Hozzafuz(ujElem);
            }
        }

        /// <summary>
        /// Visszaadja a kulcs alapján eltárolt értéket
        /// </summary>
        /// <param name="kulcs">A kikeresni kívánt érték kulcsa</param>
        /// <returns>Tárolt érték</returns>
        /// <exception cref="HibasKulcsKivetel">Kivétel, ha érvénytelen a kulcs</exception>
        public T Kiolvas(K kulcs)
        {
            SzotarElem<K, T> keresettElem = KulcsKeres(kulcs);

            if (keresettElem == null)
                throw new HibasKulcsKivetel();

            return keresettElem.tart;
        }

        /// <summary>
        /// Törli az eltárolt elemet a megadott kulcs alapján
        /// </summary>
        /// <param name="kulcs">Törölni kívánt elem kulcsa</param>
        /// <exception cref="HibasKulcsKivetel">Kivétel, ha érvénytelen a kulcs</exception>
        public void Torol(K kulcs)
        {
            SzotarElem<K, T> keresettElem = E[h(kulcs)];

            if (keresettElem != null! && keresettElem.kulcs!.Equals(kulcs))
            {
                E[h(kulcs)] = null!;
                return;
            }

            keresettElem = null!;
            U.Bejar(elem =>
            {
                if (elem.kulcs!.Equals(kulcs))
                {
                    keresettElem = elem;
                }
            });

            if (keresettElem == null)
                throw new HibasKulcsKivetel();

            U.Torol(keresettElem);
        }
    }
}
