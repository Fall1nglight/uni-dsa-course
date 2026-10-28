using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 5. heti labor feladat - Tesztek: 05_BinarisKeresoFaTesztek.cs

    public class FaElem<T>
        where T : IComparable
    {
        // mezők
        /// <summary>
        /// Tárolt érték
        /// </summary>
        public T tart;

        /// <summary>
        /// Hivatkozás a bal oldali részfára
        /// </summary>
        public FaElem<T> bal;

        /// <summary>
        /// Hivatkozás a jobb oldali részfára
        /// </summary>
        public FaElem<T> jobb;

        // konstruktorok
        public FaElem(T tart, FaElem<T> bal, FaElem<T> jobb)
        {
            this.tart = tart;
            this.bal = bal;
            this.jobb = jobb;
        }
    }

    public class FaHalmaz<T> : Halmaz<T>
        where T : IComparable
    {
        // mezők
        protected FaElem<T> gyoker;

        // metódusok

        /// <summary>
        /// Beszúrja a megadott értéket a fába
        /// </summary>
        /// <param name="ertek">A beszurandó érték</param>
        public void Beszur(T ertek)
        {
            gyoker = ReszfabaSzur(gyoker, ertek);
        }

        private FaElem<T> ReszfabaSzur(FaElem<T> p, T ertek)
        {
            // ha a részfa(p) null érték, akkor megtaláltuk a beszúrás helyét
            if (p == null!)
            {
                // létrehozzuk az új elemet, és ez lesz a visszatérési érték
                // majd a rekurzió felülírja az ide vezető útvonalat
                FaElem<T> ujElem = new FaElem<T>(ertek, null!, null!);
                return ujElem;
            }

            // érték < csomópont
            // => balra kell tovább haladnunk
            if (ertek.CompareTo(p.tart) < 0)
            {
                p.bal = ReszfabaSzur(p.bal, ertek);
            }
            // érték > csomópont
            // => jobbra haladunk tovább
            else if (ertek.CompareTo(p.tart) > 0)
            {
                p.jobb = ReszfabaSzur(p.jobb, ertek);
            }

            return p;
        }

        /// <summary>
        /// Meghatározza, hogy a megadott érték eleme-e a fának
        /// </summary>
        /// <param name="ertek">A keresendő érték</param>
        /// <returns></returns>
        public bool Eleme(T ertek)
        {
            return ReszfaEleme(gyoker, ertek);
        }

        private bool ReszfaEleme(FaElem<T> p, T ertek)
        {
            // ha részfa(p) értéke null
            // akkor eddig nem találtuk meg a keresett értéket
            // és nincs is több elem => nem eleme a fának
            if (p == null!)
                return false;

            // érték < csomópont
            // => bal oldali részfában keresünk tovább
            if (ertek.CompareTo(p.tart) < 0)
                return ReszfaEleme(p.bal, ertek);

            // érték > csomópont
            // => jobb oldali részfában keresünk tovább

            if (ertek.CompareTo(p.tart) > 0)
                return ReszfaEleme(p.jobb, ertek);

            // megtaláltuk a keresett értéket
            // => eleme a fának
            return true;
        }

        /// <summary>
        /// Törli a megadott értéket a fából
        /// </summary>
        /// <param name="ertek">A törölni kívánt érték</param>
        public void Torol(T ertek)
        {
            gyoker = ReszfabolTorol(gyoker, ertek);
        }

        private FaElem<T> ReszfabolTorol(FaElem<T> p, T ertek)
        {
            // nem létező elem esetén
            if (p == null!)
                throw new NincsElemKivetel();

            if (ertek.CompareTo(p.tart) < 0)
            {
                p.bal = ReszfabolTorol(p.bal, ertek);
            }
            else if (ertek.CompareTo(p.tart) > 0)
            {
                p.jobb = ReszfabolTorol(p.jobb, ertek);
            }
            // megtaláltuk a törlendő elemet
            else
            {
                // nincs bal oldali részfája
                // => felülírjuk a jobb oldali részfával
                if (p.bal == null!)
                {
                    p = p.jobb;
                }
                // nincs jobb oldali részfája
                // => felülírjuk a bal oldai részfával
                else if (p.jobb == null!)
                {
                    p = p.bal;
                }
                // két gyereke is van
                else
                {
                    p.bal = KetGyerekesTorles(p, p.bal);
                }
            }

            return p;
        }

        private FaElem<T> KetGyerekesTorles(FaElem<T> e, FaElem<T> r)
        {
            // megkeressük a "legjobb-oldalibb" elemet
            if (r.jobb != null!)
            {
                r.jobb = KetGyerekesTorles(e, r.jobb);
                return r;
            }

            // felmásoljuk a "legjobb-oldalibb" elem tartalmát
            // a törlendő(e) elem helyére
            e.tart = r.tart;

            // majd kiláncoljuk
            r = r.bal;

            // visszaadjuk a módosított referenciát
            return r;
        }

        /// <summary>
        /// Bejárja a fát "PreOrder" szerint és végrehajtja a csomópontokon a megadott műveletet
        /// </summary>
        /// <param name="muvelet">A végrehajtandó művelet</param>
        public void Bejar(Action<T> muvelet)
        {
            ReszfaBejarasPreOrder(gyoker, muvelet);
        }

        // PreOrder: művelet, bal-oldali részfa, jobb oldali részfa
        // használat => fa teljes elmentése (akár fájlba)
        private static void ReszfaBejarasPreOrder(FaElem<T> p, Action<T> muvelet)
        {
            // PreOrder: művelet, bal-oldali részfa, jobb oldali részfa
            // használat => fa teljes elmentése (akár fájlba)

            // null érték esetén kilépünk
            if (p == null!)
                return;

            // elvégezzük a kapott műveletet p.tart-on
            muvelet(p.tart);

            // rekurzívan haladunk a bal oldali részfán tovább
            ReszfaBejarasPreOrder(p.bal, muvelet);

            // rekurzívan haladunk a jobb oldali részfán tovább
            ReszfaBejarasPreOrder(p.jobb, muvelet);
        }

        // InOrder: bal-oldali részfa, művelet, jobb oldali részfa
        // használat => fa elemeit kapjuk növekvő módon rendezve
        private static void ReszfaBejarasInOrder(FaElem<T> p, Action<T> muvelet)
        {
            // InOrder: bal-oldali részfa, művelet, jobb oldali részfa
            // használat => fa elemeit kapjuk növekvő módon rendezve

            // null érték esetén kilépünk
            if (p == null!)
                return;

            // rekurzívan haladunk a bal oldali részfán tovább
            ReszfaBejarasInOrder(p.bal, muvelet);

            // elvégezzük a kapott műveletet p.tart-on
            muvelet(p.tart);

            // rekurzívan haladunk a jobb oldali részfán tovább
            ReszfaBejarasInOrder(p.jobb, muvelet);
        }

        // PostOrder: bal-oldali részfa, művelet, jobb oldali részfa
        // használat => fa levélelemeit kapjuk meg először
        private static void ReszfaBejarasPostOrder(FaElem<T> p, Action<T> muvelet)
        {
            // PostOrder: bal-oldali részfa, művelet, jobb oldali részfa
            // használat => fa levélelemeit kapjuk meg először

            // null érték esetén kilépünk
            if (p == null!)
                return;

            // rekurzívan haladunk a bal oldali részfán tovább
            ReszfaBejarasPostOrder(p.bal, muvelet);

            // rekurzívan haladunk a jobb oldali részfán tovább
            ReszfaBejarasPostOrder(p.jobb, muvelet);

            // elvégezzük a kapott műveletet p.tart-on
            muvelet(p.tart);
        }
    }

    public class MinMaxFaHalmaz<T> : FaHalmaz<T>
        where T : IComparable
    {
        public T MinErtek => GetMinErtek();
        public T MaxErtek => GetMaxErtek();

        private T GetMinErtek()
        {
            T min = GetMinErtekRekurzio(this.gyoker);
            return min;
        }

        private T GetMinErtekRekurzio(FaElem<T> p)
        {
            if (p == null!)
                throw new NincsElemKivetel();

            if (p.bal == null!)
                return p.tart;

            return GetMinErtekRekurzio(p.bal);
        }

        private T GetMaxErtek()
        {
            T min = GetMaxErtekRekurzio(this.gyoker);
            return min;
        }

        private T GetMaxErtekRekurzio(FaElem<T> p)
        {
            if (p == null!)
                throw new NincsElemKivetel();

            if (p.jobb == null!)
                return p.tart;

            return GetMaxErtekRekurzio(p.jobb);
        }

        public void Korlatoz(T min, T max)
        {
            while (this.gyoker != null! && MaxErtek.CompareTo(max) > 0)
            {
                base.Torol(MaxErtek);
            }

            while (this.gyoker != null! && MinErtek.CompareTo(min) < 0)
            {
                base.Torol(MinErtek);
            }
        }
    }
}
