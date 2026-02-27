using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NyilvantartoApp.Models
{
    public class Termek
    {
        public Termek(int id, string nev, int ar, int akcios_ar, int keszlet) {
            Id = id;
            Nev = nev;
            Ar = ar;
            Akcios_ar = akcios_ar;
            Keszlet = keszlet;
        }

        public int Id { get; private set; }
        public string Nev { get; private set; }
        public int Ar { get; private set; }
        public int Akcios_ar { get; private set; }
        public int Keszlet { get; private set; }
    }
}