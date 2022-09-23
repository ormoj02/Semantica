/* Orta Moreno Jair */
using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                /* Byte x = 255;
                Console.WriteLine("valor viejo de x: "+x);
                x++;
                Console.WriteLine("valor nuevo de x: "+x);
                x++;
                Console.WriteLine("valor nuevo de x: "+x); */

                Lenguaje a = new Lenguaje();

                a.Programa();

                a.cerrar();

                

            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
        }
    }
}