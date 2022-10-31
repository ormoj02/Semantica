/* Orta Moreno Jair */
using System;
using System.IO;
using System.Collections.Generic;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            
            using (Lenguaje a = new Lenguaje())
            {
                try
                {
                    //Lenguaje a = new Lenguaje();

                    a.Programa();

                    //a.cerrar();

                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }



        }

    }
}