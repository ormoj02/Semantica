/* Orta Moreno Jair */
using System;
using System.Collections.Generic;
//Unidad 3
//Requerimiento 1: 
//  x a) Agregar el residuo de la division en el PorFactor()
//  x b) Agregar en instruccion los incrementos de termino() y los incrementos de factor()
//     a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
//     en donde el 1 puede ser una expresion
//  x c) Programar el destructor 
//        para ejecutar el metodo cerrarArchivo()
//        #libreria especial? contenedor?
//        #en la clase lexico
//Requerimiento 2:
//  x a) Marcar errores semanticos cuando los incrementos de termino() o incrementos de factor() superen el limite de la variable
//  x b) Considerar el inciso b y c para el for
//  x c) Correcto funcionamiento del ciclo while y do while
//Requerimiento 3:
//  x a) Considerar las variables y los casteos en las expresiones matematicas en ensamblador
//  x b) Considerar el residuo de la division en assembler
//  x c) Programar el printf y scanf en assembler
//Requerimiento 4:
//  x a) Programar el else en assembler
//  x b) Programar el for en assembler
//Requerimiento 5:
//  x a) Programar el while en assembler
//  x b) Programar el do while en assembler
namespace Semantica
{
    public class Lenguaje : Sintaxis, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("\nFinalizador");
        }


        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();

        Variable.TipoDato dominante;
        int cIf;
        int cFor;
        int cWhile;
        int cDoWhile;
        string incrementosASM = "";

        public Lenguaje()
        {
            cIf = cFor = 0;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            cIf = cFor = 0;
        }
        private void addVariable(String nombre, Variable.TipoDato tipo)
        {
            variables.Add(new Variable(nombre, tipo));
        }

        private void displayVariables()
        {
            log.WriteLine();
            log.WriteLine("variables: ");
            foreach (Variable v in variables)
            {
                log.WriteLine(v.getNombre() + " " + v.getTipo() + " " + v.getValor());
            }
        }

        private void variablesASM()
        {

            asm.WriteLine(";Variables: ");
            foreach (Variable v in variables)
            {
                asm.WriteLine("\t " + v.getNombre() + " DW " + v.getValor());
            }
        }

        private bool existeVariable(string nombre)
        {
            foreach (Variable v in variables)
            {
                //si encuentra una variable con el mismo nombre en el List 
                if (v.getNombre().Equals(nombre))
                {
                    return true;
                }
            }

            return false;
        }

        private void modVariable(string nombre, float nuevoValor)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombreVariable)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombreVariable))
                {
                    return v.getValor();
                }
            }

            return 0;
        }


        private Variable.TipoDato getTipo(string nombre)
        {
            foreach (Variable v in variables)
            {
                if (v.getNombre().Equals(nombre))
                {
                    return v.getTipo();
                }
            }

            return Variable.TipoDato.Char;
        }


        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("#make_COM#");
            asm.WriteLine("include emu8086.inc");
            asm.WriteLine("ORG 100h");
            Libreria();
            Variables();
            variablesASM();
            Main();
            displayVariables();
            asm.WriteLine("RET");
            asm.WriteLine("DEFINE_PRINT_NUM");
            asm.WriteLine("DEFINE_PRINT_NUM_UNS");
            asm.WriteLine("DEFINE_SCAN_NUM");
        }

        //Librerias -> #include<identificador(.h)?> Librerias?
        private void Libreria()
        {
            if (getContenido() == "#")
            {
                match("#");
                match("include");
                match("<");
                match(Tipos.Identificador);
                if (getContenido() == ".")
                {
                    match(".");
                    match("h");
                }
                match(">");
                Libreria();
            }
        }

        //Variables -> tipo_dato Lista_identificadores; Variables?
        private void Variables()
        {
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variable.TipoDato tipo = Variable.TipoDato.Char;
                switch (getContenido())
                {
                    case "int": tipo = Variable.TipoDato.Int; break;
                    case "float": tipo = Variable.TipoDato.Float; break;
                }
                match(Tipos.TipoDato);
                Lista_identificadores(tipo);
                match(Tipos.FinSentencia);
                Variables();
            }
        }

        //Lista_identificadores -> identificador (,Lista_identificadores)?
        private void Lista_identificadores(Variable.TipoDato tipo)
        {
            if (getClasificacion() == Tipos.Identificador)
            {
                if (!existeVariable(getContenido()))
                {
                    addVariable(getContenido(), tipo);
                }
                else
                {
                    throw new Error("Error de sintaxis, variable duplicada <" + getContenido() + "> en linea: " + linea, log);
                }
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                Lista_identificadores(tipo);
            }
        }

        //Main      -> void main() Bloque de instrucciones
        private void Main()
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(true, true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion, bool imprimir)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprimir);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion, bool imprimir)
        {
            Instruccion(evaluacion, imprimir);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion, imprimir);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion, bool imprimir)
        {
            Instruccion(evaluacion, imprimir);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion, imprimir);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion, bool imprimir)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion, imprimir);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion, imprimir);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion, imprimir);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion, imprimir);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion, imprimir);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion, imprimir);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion, imprimir);
            }
            else
            {
                Asignacion(evaluacion, imprimir);
            }
        }

        private Variable.TipoDato evaluaNumero(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TipoDato.Float;
            }

            if (resultado <= 255)
            {
                return Variable.TipoDato.Char;
            }
            else if (resultado <= 65535)
            {
                return Variable.TipoDato.Int;
            }
            return Variable.TipoDato.Float;
        }
        private bool evaluaSemantica(string variable, float resultado)
        {
            Variable.TipoDato tipoDato = getTipo(variable);
            return false;
        }

        //Asignacion -> identificador = cadena | Expresion;
        private void Asignacion(bool evaluacion, bool imprimir)
        {
            //guardamos el nombre de la variable
            string nombre = getContenido();

            if (!existeVariable(nombre))
            {
                throw new Error("Error: Variable inexistente " + getContenido() + " en la linea: " + linea, log);
            }

            match(Tipos.Identificador);

            if (getClasificacion() == Tipos.IncrementoTermino || getClasificacion() == Tipos.IncrementoFactor)
            {
                Variable.TipoDato tipoDato = getTipo(nombre);
                float nuevoValor = getValor(nombre);
                dominante = Variable.TipoDato.Char;
                //Requerimiento 1.b
                //Agregamos los incrementos a++, a--, a+=1, a-=1, a*=1, a/=1, a%=1
                modVariable(nombre, Incremento(nombre, evaluacion, imprimir));
                if (imprimir)
                {
                    asm.WriteLine(incrementosASM);
                }
                match(";");
                //Req 1.c
            }
            else
            {
                log.WriteLine();
                log.Write(getContenido() + " = ");

                match(Tipos.Asignacion);
                dominante = Variable.TipoDato.Char;
                //Console.WriteLine("Dominante: " + dominante);
                Expresion(imprimir);
                match(";");

                //hacemos el pop     
                float resultado = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP AX");
                }

                log.Write("= " + resultado);
                log.WriteLine();
                //Console.WriteLine("Evalua Numero: " + evaluaNumero(resultado));
                if (dominante < evaluaNumero(resultado))
                {
                    Console.WriteLine("Dominante ahora cambiara de valor al mayor");
                    dominante = evaluaNumero(resultado);
                }

                if (dominante <= getTipo(nombre))
                {
                    if (evaluacion)
                    {
                        modVariable(nombre, resultado);
                    }
                }
                else
                {
                    throw new Error("Error de semantica: no podemos asignar un valor de tipo <" + dominante + "> a una variable de tipo <" + getTipo(nombre) + "> en la linea: " + linea, log);
                }
                if (getTipo(nombre) == Variable.TipoDato.Char)
                {
                    if (imprimir)
                    {
                        asm.WriteLine("MOV AH, 0");
                    }

                }
                if (imprimir)
                {
                    asm.WriteLine("MOV " + nombre + ", AX");
                }

            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion, bool imprimir)
        {
            
            if(imprimir)
            {
                cWhile++;
            }
            string etiquetaWhileInicio = "whileInicio" + cWhile+":";
            string etiquetaWhileFin = "whileFin" + cWhile+":";
            match("while");
            match("(");

            bool validarWhile;
            int posicionWhile = posicion;
            int lineaWhile = linea;
            string variable = getContenido();
            do
            {
                if (imprimir)
                {
                    asm.WriteLine(etiquetaWhileInicio);
                }

                validarWhile = Condicion(etiquetaWhileFin, imprimir);
                if (!evaluacion)
                {
                    validarWhile = false;
                }

                match(")");


                if (getContenido() == "{")
                {
                    if (validarWhile)
                    {
                        BloqueInstrucciones(evaluacion, imprimir);
                    }
                    else
                    {
                        BloqueInstrucciones(false, imprimir);
                    }
                }
                else
                {
                    if (validarWhile)
                    {
                        Instruccion(evaluacion, imprimir);
                    }
                    else
                    {
                        Instruccion(false, imprimir);
                    }
                }
                if (validarWhile)
                {
                    posicion = posicionWhile - variable.Length;
                    linea = lineaWhile;
                    setPosicion(posicion);
                    NextToken();

                }
                if(imprimir)
                {
                    asm.WriteLine("JMP " + etiquetaWhileInicio);
                    asm.WriteLine(etiquetaWhileFin);
                }
                imprimir = false;
            } while (validarWhile);


        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion, bool imprimir)
        {
            if(imprimir)
            {
                cDoWhile++;
            }
            string etiquetaDoWhileInicio = "doWhileInicio" + cDoWhile + ":";
            string etiquetaDoWhileFin = "doWhileFin" + cDoWhile + ":";
            bool validarDo = evaluacion;
            //string variable;
            if (!evaluacion)
            {
                validarDo = false;
            }
            match("do");
            int posicionDo = posicion;
            int lineaDo = linea;
            do
            {
                if (imprimir)
                {
                    asm.WriteLine(etiquetaDoWhileInicio);
                }
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, imprimir);
                }
                else
                {
                    Instruccion(evaluacion, imprimir);
                }
                match("while");
                match("(");
                //variable = getContenido();
                //Requerimiento 4.- Si la condicion no es booleana levanta la excepcion
                validarDo = Condicion(etiquetaDoWhileFin, imprimir);
                if (!evaluacion)
                {
                    validarDo = false;
                }
                else if (validarDo)
                {
                    posicion = posicionDo - 1;
                    linea = lineaDo;
                    setPosicion(posicion);
                    NextToken();
                }
                if (imprimir)
                {
                    asm.WriteLine("JMP " + etiquetaDoWhileInicio);
                    asm.WriteLine(etiquetaDoWhileFin);
                }
                imprimir = false;

            } while (validarDo);


            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion, bool imprimir)
        {
            if (imprimir)
            {
                cFor++;
            }
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor;

            match("for");
            match("(");
            Asignacion(evaluacion, imprimir);
            //Requerimiento 4.- Si la condicion no es booleana levanta la excepcion
            float valor = 0;
            bool validarFor;
            int pos = posicion;
            int lineaGuardada = linea;
            int tam = getContenido().Length;
            string variable = getContenido();
            //b) Agregar un ciclo while despues de validar el for, que se ejecute mientras la condicion sea verdadera
            do
            {
                if (imprimir)
                {
                    asm.WriteLine(etiquetaInicioFor + ":");
                }
                validarFor = Condicion(etiquetaFinFor, imprimir);
                if (!evaluacion)
                {
                    validarFor = false;
                }
                match(";");

                //requerimiento 1.d
                match(Tipos.Identificador);
                valor = Incremento(variable, evaluacion, imprimir);


                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor, imprimir);
                }
                else
                {
                    Instruccion(validarFor, imprimir);
                }
                if (validarFor)
                {
                    posicion = pos - tam;
                    linea = lineaGuardada;
                    setPosicion(posicion);
                    NextToken();
                    //al finalizar el for se ejecuta el incremento
                    modVariable(getContenido(), valor);
                }
                if (imprimir)
                {
                    asm.WriteLine(incrementosASM);
                    asm.WriteLine("JMP " + etiquetaInicioFor);
                    asm.WriteLine(etiquetaFinFor + ":");
                }
                imprimir = false;
            } while (validarFor);
            //c)Regresar a la posicion de lectura del archivo de texto
            //d) sacar otro tokencon el metodo nextToken(
        }
        private void setPosicion(int pos)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        //Incremento -> Identificador ++ | --
        /* private void Incremento(bool evaluacion, bool imprimir)
        {

            string variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (existeVariable(variable) == false)
            {
                throw new Error("Error: Variable inexistente " + getContenido() + " en la linea: " + linea, log);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");

                //si la variable evaluacion es verdadera entonces se modifica el valor de la variable
                if (evaluacion, imprimir)
                {

                    modVariable(variable, getValor(variable) + 1);

                }
            }
            else
            {
                match("--");
                //si la variable evaluacion es verdadera entonces se modifica el valor de la variable
                if (evaluacion, imprimir)
                {

                    modVariable(variable, getValor(variable) - 1);
                }

            }
        } */
        private float Incremento(string variable, bool evaluacion, bool imprimir)
        {
            //variable = getContenido();
            //Requerimiento 2.- Si no existe la variable levanta la excepcion
            if (existeVariable(variable) == false)
            {
                throw new Error("Error: Variable inexistente " + getContenido() + " en la linea: " + linea, log);
            }
            //match(Tipos.Identificador);

            //string incrementoValor;
            float incremento = 0;
            float nuevoValor = getValor(variable);
            Variable.TipoDato tipoDato = getTipo(variable);
            dominante = Variable.TipoDato.Char;


            if (getContenido() == "++")
            {
                match("++");
                if (imprimir)
                {
                    incrementosASM = "INC " + variable;
                }
                if (evaluacion)
                {
                    nuevoValor = getValor(variable) + 1;
                }

            }
            else if (getContenido() == "--")
            {
                match("--");
                if (evaluacion)
                {
                    if (imprimir)
                    {
                        incrementosASM = "DEC " + variable;
                    }
                    nuevoValor = getValor(variable) - 1;
                }

            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion(imprimir);
                incremento = stack.Pop();

                if (evaluacion)
                {
                    if (imprimir)
                    {
                        incrementosASM = "POP AX";
                        incrementosASM += "\nADD " + variable + ", AX";
                    }
                    nuevoValor = getValor(variable) + incremento;
                }

            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion(imprimir);

                incremento = stack.Pop();

                if (evaluacion)
                {
                    if (imprimir)
                    {
                        incrementosASM = "POP AX";
                        incrementosASM += "\nSUB " + variable + ", AX";
                    }
                    nuevoValor = getValor(variable) - incremento;
                }

            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion(imprimir);

                incremento = stack.Pop();

                if (imprimir)
                {
                    incrementosASM = "POP AX";
                    incrementosASM += "\nMOV BX, " + variable;
                    incrementosASM += "\nMUL BX";
                    incrementosASM += "\nMOV " + variable + ", AX";

                }
                if (evaluacion)
                {

                    nuevoValor = getValor(variable) * incremento;
                }

            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion(imprimir);

                incremento = stack.Pop();

                if (imprimir)
                {
                    incrementosASM = "POP BX";
                    incrementosASM += "\nMOV AX, " + variable;
                    incrementosASM += "\nDIV BX ";
                    incrementosASM += "\nMOV " + variable + ", AX";
                }

                if (evaluacion)
                {

                    nuevoValor = getValor(variable) / incremento;
                }

            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion(imprimir);

                incremento = stack.Pop();

                if (imprimir)
                {
                    incrementosASM = "POP BX";
                    incrementosASM += "\nMOV AX, " + variable;
                    incrementosASM += "\nDIV BX ";
                    incrementosASM += "\nMOV " + variable + ", DX";
                }
                if (evaluacion)
                {

                    nuevoValor = getValor(variable) % incremento;
                }

            }
            if (getTipo(variable) == Variable.TipoDato.Char && nuevoValor > 255)
            {
                throw new Error("Error: El valor de la variable " + variable + " excede el rango de un char", log);
            }
            else if (getTipo(variable) == Variable.TipoDato.Int && nuevoValor > 65535)
            {
                throw new Error("Error: El valor de la variable " + variable + " excede el rango de un int", log);
            }
            return nuevoValor;
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion, bool imprimir)
        {
            match("switch");
            match("(");
            Expresion(imprimir);
            stack.Pop();
            if (imprimir)
            {
                asm.WriteLine("POP AX");
            }
            match(")");
            match("{");
            ListaDeCasos(evaluacion, imprimir);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion, imprimir);
                }
                else
                {
                    Instruccion(evaluacion, imprimir);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion, bool imprimir)
        {
            match("case");
            Expresion(imprimir);
            stack.Pop();
            if (imprimir)
            {
                asm.WriteLine("POP AX");
            }
            match(":");
            ListaInstruccionesCase(evaluacion, imprimir);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion, imprimir);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta, bool imprimir)
        {
            Expresion(imprimir);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(imprimir);
            float e2 = stack.Pop();
            float e1 = stack.Pop();
            if (imprimir)
            {
                asm.WriteLine("POP BX");
                asm.WriteLine("POP AX");
                asm.WriteLine("CMP AX, BX");
            }

            switch (operador)
            {
                case ">":
                    if (imprimir)
                    {
                        asm.WriteLine("JLE " + etiqueta);
                    }
                    return e1 > e2;
                case "<":
                    if (imprimir)
                    {
                        asm.WriteLine("JGE " + etiqueta);
                    }
                    return e1 < e2;
                case ">=":
                    if (imprimir)
                    {
                        asm.WriteLine("JL " + etiqueta);
                    }
                    return e1 >= e2;
                case "<=":
                    if (imprimir)
                    {
                        asm.WriteLine("JG " + etiqueta);
                    }
                    return e1 <= e2;
                case "==":
                    if (imprimir)
                    {
                        asm.WriteLine("JNE " + etiqueta);
                    }
                    return e1 == e2;
                default:
                    if (imprimir)
                    {
                        asm.WriteLine("JE " + etiqueta);
                    }
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion, bool imprimir)
        {
            if (imprimir)
            {
                cIf++;
            }
            string etiquetaIf = "if" + cIf;
            string etiquetaElse = "else" + cIf;
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion(etiquetaIf, imprimir);
            if (!evaluacion)
            {
                validarIf = false;
            }

            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf, imprimir);
            }
            else
            {
                Instruccion(validarIf, imprimir);
            }
            if (imprimir)
            {
                asm.WriteLine("JMP " + etiquetaElse);
                asm.WriteLine(etiquetaIf + ":");
            }
            if (getContenido() == "else")
            {

                match("else");
                //Requerimiento 4
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf, imprimir);
                    }
                    else
                    {
                        BloqueInstrucciones(false, imprimir);
                    }

                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf, imprimir);
                    }
                    else
                    {
                        Instruccion(false, imprimir);
                    }
                }

            }
            if (imprimir)
            {
                asm.WriteLine(etiquetaElse + ":");
            }
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion, bool imprimir)
        {

            match("printf");
            match("(");

            //revisamos si es una cadena
            if (getClasificacion() == Tipos.Cadena)
            {
                string cadena = getContenido();

                if (evaluacion)
                {
                    //cambiamos las comillas por los datos correctos
                    setContenido(getContenido().Replace("\"", string.Empty));
                    setContenido(getContenido().Replace("\\t", "     "));
                    cadena = getContenido();
                    setContenido(getContenido().Replace("\\n", "\n"));
                    
                    //escribe contenido
                    Console.Write(getContenido());
                }
                if (imprimir)
                {
                    if(cadena.Contains("\\n"))
                    {
                        string [] subCadena = cadena.Split("\\n" );
                        for (int i = 0; i < subCadena.Length; i++)
                        {
                            if (i == subCadena.Length - 1)
                            {
                                asm.WriteLine("PRINT \"" + subCadena[i]+"\"");
                            }
                            else
                            {
                                asm.WriteLine("PRINTN \"" + subCadena[i]+"\"");
                            }
                        }
                    }
                    else
                    {
                        asm.WriteLine("PRINT \"" + cadena + "\"");
                    }
                }
                match(Tipos.Cadena);
            }
            else
            {
                Expresion(imprimir);
                float resultado = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP AX");
                }
                if (evaluacion)
                {
                    Console.Write(resultado);
                    //Codigo assembler para imprimir una variable
                    //asm.WriteLine("POP AX");
                    if (imprimir)
                    {
                        asm.WriteLine("CALL PRINT_NUM");

                    }
                }
            }


            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &identificador);
        private void Scanf(bool evaluacion, bool imprimir)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            //revisamos si es un identificador
            if (getClasificacion() == Tipos.Identificador)
            {
                //revisamos si la variable existe
                if (!existeVariable(getContenido()))
                {
                    throw new Error("Error: Variable inexistente " + getContenido() + " en la linea: " + linea, log);
                }
            }

            if (evaluacion)
            {

                string val = "" + Console.ReadLine();
                if (float.TryParse(val, out float numero))
                {
                    modVariable(getContenido(), numero);
                }
                else
                {
                    //modVariable(getContenido(), 0);
                    throw new Error("Error: No se puede asignar un valor de tipo cadena a una variable de tipo numerico " + getContenido() + " en la linea: " + linea, log);
                }
                if (imprimir)
                {
                    asm.WriteLine("CALL SCAN_NUM");
                    asm.WriteLine("MOV " + getContenido() + ", CX");

                }
                //modVariable(getContenido(), float.Parse(val));

            }

            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Expresion -> Termino MasTermino
        private void Expresion(bool imprimir)
        {
            Termino(imprimir);
            MasTermino(imprimir);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool imprimir)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(imprimir);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP AX");
                }
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        if (imprimir)
                        {
                            asm.WriteLine("ADD AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        if (imprimir)
                        {
                            asm.WriteLine("SUB AX, BX");
                            asm.WriteLine("PUSH AX");
                        }
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool imprimir)
        {
            Factor(imprimir);
            PorFactor(imprimir);
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor(bool imprimir)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(imprimir);
                log.Write(operador + " ");
                float n1 = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP BX");
                }
                float n2 = stack.Pop();
                if (imprimir)
                {
                    asm.WriteLine("POP AX");
                }



                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        if (imprimir)
                        {
                            asm.WriteLine("MUL BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //en AX se guarda el resultado de multiplicar AL y el otro operador
                        //en este caso lo guardamos en BX
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        if (imprimir)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH AX");
                        }
                        //Se guarda el resultado de la division en AL y lo metemos al stack
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        if (imprimir)
                        {
                            asm.WriteLine("DIV BX");
                            asm.WriteLine("PUSH DX");
                        }
                        //Se guarda el resto de la division en AH y lo metemos al stack
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool imprimir)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                if (imprimir)
                {
                    asm.WriteLine("MOV AX, " + getContenido());
                    asm.WriteLine("PUSH AX");

                }
                match(Tipos.Numero);
            }
            else if (getClasificacion() == Tipos.Identificador)
            {
                //si no existe la variable, mandamos un error
                if (!existeVariable(getContenido()))
                {
                    throw new Error("Error de sintaxis, variable solicitada:  <" + getContenido() + "> es inexistente, en linea: " + linea, log);
                }

                log.Write(getContenido() + " ");
                if (dominante < getTipo(getContenido()))
                {
                    dominante = getTipo(getContenido());
                }
                if (imprimir)
                {
                    asm.WriteLine("MOV AX, " + getContenido());

                }
                //metemos la variable dentro del stack para hacer operaciones 
                stack.Push(getValor(getContenido()));
                // Requerimiento 3.a 
                //pasamos al siguiente token
                if (imprimir)
                {
                    asm.WriteLine("PUSH AX");

                }
                match(Tipos.Identificador);
            }
            else
            {
                match("(");
                bool huboCasteo = false;
                Variable.TipoDato casteo = Variable.TipoDato.Char;
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCasteo = true;
                    switch (getContenido())
                    {
                        case "int":
                            casteo = Variable.TipoDato.Int;
                            break;
                        case "float":
                            casteo = Variable.TipoDato.Float;
                            break;
                        case "char":
                            casteo = Variable.TipoDato.Char;
                            break;
                    }

                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(imprimir);
                match(")");
                if (huboCasteo)
                {
                    //req 2 unidad 2
                    //saco un elemento del stack
                    //convierto ese valor al equivalente en casteo

                    //req 3 unidad 2
                    //ejemplo: si el casteo es char y el pop regresa un 256, 
                    //          el valor equivalente en casteo es un 0
                    //llamamos al metodo convertir
                    float valor = stack.Pop();
                    //asm.WriteLine("POP AX xd");
                    stack.Push(convertir(valor, casteo));
                    dominante = casteo;
                }
            }

            float convertir(float valor, Variable.TipoDato casteo)
            {
                switch (casteo)
                {
                    case Variable.TipoDato.Char:
                        return valor % 256;
                    case Variable.TipoDato.Int:
                        return valor % 65536;
                }
                return valor;
            }
        }
    }
}