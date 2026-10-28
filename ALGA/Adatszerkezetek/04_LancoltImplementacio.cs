using System;
using System.Collections;
using System.Collections.Generic;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace OE.ALGA.Adatszerkezetek
{
    // 4. heti labor feladat - Tesztek: 04_LancoltImplementacioTesztek.cs

    public class LancElem<T>
    {
        // mezők
        public T tart;
        public LancElem<T> kov;

        // konstruktorok
        public LancElem(T tart, LancElem<T> kov)
        {
            this.tart = tart;
            this.kov = kov;
        }
    }

    public class LancoltVerem<T> : Verem<T>
    {
        // mezők
        private LancElem<T> fej;

        // tulajdonságok
        public bool Ures => fej == null!;

        // metódusok
        /// <summary>
        /// Elhelyezi a veremben a megadott értéket
        /// </summary>
        /// <param name="ertek">Az elhelyezni kívánt érték</param>
        public void Verembe(T ertek)
        {
            // a verem szerekezetéből adódóan mindig az elejére helyezzük el az elemeket
            // tehát: létrehozunk egy új LáncElem-et, ami a fej-re mutat
            // (mindegy hogy a verem üres vagy nem)
            this.fej = new LancElem<T>(ertek, fej);
        }

        /// <summary>
        /// Kiveszi a veremből az első elemet
        /// </summary>
        /// <returns>A verem első elemének értéke</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha a verem üres</exception>
        public T Verembol()
        {
            if (Ures)
                throw new NincsElemKivetel();

            T tartalom = fej.tart;

            // a fej referenciáját felülírjuk az Ő általa mutatott következő elem referenciájára
            // így megtörténik a kiláncolás
            fej = fej.kov;

            return tartalom;
        }

        /// <summary>
        /// Megmutatja a verem első elemének értékét
        /// </summary>
        /// <returns>A verem első elemének értékét</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha a verem üres</exception>
        public T Felso()
        {
            if (Ures)
                throw new NincsElemKivetel();

            return fej.tart;
        }
    }

    public class LancoltSor<T> : Sor<T>
    {
        // mezők
        /// <summary>
        /// A sorban lévő első elemre mutat
        /// </summary>
        private LancElem<T> fej;

        /// <summary>
        /// A sorban lévő utolsó elemre mutat
        /// </summary>
        private LancElem<T> vege;

        // tulajdonságok
        public bool Ures => fej == null;

        /// <summary>
        /// Elhelyezi a sorban a megadott értéket
        /// </summary>
        /// <param name="ertek">Az elhelyezni kívánt érték</param>
        public void Sorba(T ertek)
        {
            LancElem<T> uj = new LancElem<T>(ertek, null!);

            if (Ures)
            {
                this.fej = uj;
            }
            else
            {
                this.vege.kov = uj;
            }

            vege = uj;
        }

        /// <summary>
        /// Visszaadja és eltávolítja a sorban lévő első elemet
        /// </summary>
        /// <returns>A sorban lévő első elem értéke</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha a sor üres</exception>
        public T Sorbol()
        {
            if (Ures)
                throw new NincsElemKivetel();

            T tartalom = fej.tart;

            fej = fej.kov;

            // ha a fej-et lépteteve null értéket kapunk
            // azaz 1 darab elem volt a sorban és azt kitöröltük
            // akkor gondoskodni kell a vége mutatóról is
            if (fej == null)
                vege = null!;

            return tartalom;
        }

        /// <summary>
        /// Megmutatja a sorban szereplő első elemet
        /// </summary>
        /// <returns>A sorban szereplő első elem értéke</returns>
        /// <exception cref="NincsElemKivetel">Kivétel, ha a sor üres</exception>
        public T Elso()
        {
            if (Ures)
                throw new NincsElemKivetel();

            return fej.tart;
        }
    }

    public class LancoltLista<T> : Lista<T>, IEnumerable<T>
    {
        // mezők
        private LancElem<T> fej;

        /// <summary>
        /// A listában lévő elemek száma
        /// </summary>
        protected int n;

        // tulajdonságok
        public int Elemszam => n;
        public bool Ures => n == 0;

        // metódusok
        /// <summary>
        /// Visszaadja a megadott indexen lévő LáncElemet
        /// </summary>
        /// <param name="index">Az elérni kívánt LáncElem indexe</param>
        /// <returns>A megadott indexen lévő LáncElem</returns>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha helytelen az index</exception>
        private LancElem<T> Keres(int index)
        {
            // index ellenőrzése
            if (index < 0 || index >= Elemszam)
                throw new HibasIndexKivetel();

            int i = 0;

            LancElem<T> atmeneti = fej;

            while (i < index && atmeneti != null)
            {
                atmeneti = atmeneti.kov;
                i++;
            }

            return atmeneti!;
        }

        /// <summary>
        /// Visszaadja az adott indexen lévő elem tárolt értékét
        /// </summary>
        /// <param name="index">A kiolvasni kívánt elem indexe</param>
        /// <returns>Az elem tárolt értéke</returns>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha helytelen az index</exception>
        public T Kiolvas(int index)
        {
            if (index < 0 || index >= Elemszam)
                throw new HibasIndexKivetel();

            LancElem<T> keresett = Keres(index);
            return keresett.tart;
        }

        /// <summary>
        /// Módosítja a megadott indexen lévő elem tárolt értéket egy új értékre
        /// </summary>
        /// <param name="index">A módosítandó elem indexe</param>
        /// <param name="ertek">Az új, módosított érték</param>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha az index helytelen</exception>
        public void Modosit(int index, T ertek)
        {
            if (index < 0 || index >= Elemszam)
                throw new HibasIndexKivetel();

            LancElem<T> modositando = Keres(index);
            modositando.tart = ertek;
        }

        /// <summary>
        /// Hozzáadja a megadott értéket a listához
        /// </summary>
        /// <param name="ertek">A felvenni kívánt új érték</param>
        public virtual void Hozzafuz(T ertek)
        {
            LancElem<T> ujElem = new LancElem<T>(ertek, null!);

            if (Ures)
            {
                fej = ujElem;
            }
            else
            {
                LancElem<T> atmeneti = fej;

                while (atmeneti.kov != null)
                {
                    atmeneti = atmeneti.kov;
                }

                atmeneti.kov = ujElem;
            }

            n++;
        }

        /// <summary>
        /// Beszúrja a megadott indexre az új értéket
        /// </summary>
        /// <param name="index">A beszurandó elem indexe</param>
        /// <param name="ertek">A beszurandó érték</param>
        /// <exception cref="HibasIndexKivetel">Kivétel, ha helytelen az index</exception>
        public virtual void Beszur(int index, T ertek)
        {
            if (index < 0 || index > Elemszam)
                throw new HibasIndexKivetel();

            // először is ellenőrizzük hogy a lista üres-e vagy az index == 0
            // tehát a fej mutató elé szeretnénk beszúrni az elemet
            if (Ures || index == 0)
            {
                LancElem<T> uj = new LancElem<T>(ertek, fej);
                fej = uj;
            }
            else
            {
                int i = 1;
                LancElem<T> atmeneti = fej;

                // addig iterálunk ameddig van következő elem
                // és az i értéke kisebb mint az index
                while (i < index && atmeneti.kov != null)
                {
                    atmeneti = atmeneti.kov;
                    i++;
                }

                // beláncoljuk az új elemet az index. helyre
                LancElem<T> uj = new LancElem<T>(ertek, atmeneti.kov!);
                atmeneti.kov = uj;
            }

            n++;
        }

        /// <summary>
        /// Törli az összes olyan elemet amely értéke megegyezik a megadott értékkel
        /// </summary>
        /// <param name="ertek">A törölni kívánt érték</param>
        public virtual void Torol(T ertek)
        {
            LancElem<T> atmeneti = fej;
            LancElem<T> elozo = null!;

            do
            {
                while (atmeneti != null && !atmeneti.tart.Equals(ertek))
                {
                    elozo = atmeneti;
                    atmeneti = atmeneti.kov;
                }

                // ha az atmeneti nem null akkor találtunk törlendő elemet
                if (atmeneti != null)
                {
                    // a fej-et kell törölni
                    if (elozo == null)
                    {
                        fej = fej.kov;
                    }
                    else
                    {
                        // kiláncoljuk a törlendő elemet
                        elozo.kov = atmeneti.kov;
                    }

                    // ezzel léptetjük a ciklust a következő elemre
                    atmeneti = atmeneti.kov;

                    n--;
                }
            } while (atmeneti != null);
        }

        /// <summary>
        /// Végrehajtja a megadott műveletet minden egyes elemen
        /// </summary>
        /// <param name="muvelet">A végrehajtandó művelet</param>
        public void Bejar(Action<T> muvelet)
        {
            LancElem<T> atmeneti = fej;

            while (atmeneti != null)
            {
                muvelet(atmeneti.tart);
                atmeneti = atmeneti.kov;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LancoltListaBejaro<T>(fej);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class KorlatosLancoltLista<T> : LancoltLista<T>
    {
        // mezők
        private int elemszam = 0;

        // tulajdonságok
        public int ElemSzam => elemszam;
        public int Kapacitas { get; private set; }

        public KorlatosLancoltLista(int kapacitas)
        {
            Kapacitas = kapacitas;
        }

        // metódusok
        public override void Beszur(int index, T ertek)
        {
            if (ElemSzam == Kapacitas)
                throw new NincsHelyKivetel();

            base.Beszur(index, ertek);

            // ha idáig lefut a metódus, akkor sikerült az elem hozzáadása
            elemszam++;
        }

        public override void Hozzafuz(T ertek)
        {
            if (ElemSzam == Kapacitas)
                throw new NincsHelyKivetel();

            base.Hozzafuz(ertek);

            // ha idáig lefut a metódus, akkor sikerült az elem hozzáadása
            elemszam++;
        }

        public override void Torol(T ertek)
        {
            int torlesElott = n;

            base.Torol(ertek);

            int torlesUtan = n;
            int toroldDb = torlesElott - torlesUtan;
            elemszam -= toroldDb;
        }
    }

    public class LancoltListaBejaro<T> : IEnumerator<T>
    {
        // fields
        private LancElem<T> fej;
        private LancElem<T> aktualisElem;

        // properties
        public T Current => aktualisElem.tart;

        object? IEnumerator.Current => Current;

        // constructors
        public LancoltListaBejaro(LancElem<T> fej)
        {
            this.fej = fej;
        }

        // methods
        public bool MoveNext()
        {
            if (aktualisElem == null)
            {
                aktualisElem = fej;
            }
            else
            {
                aktualisElem = aktualisElem.kov;
            }

            return aktualisElem != null;
        }

        public void Reset()
        {
            aktualisElem = null!;
        }

        public void Dispose() { }
    }
}
