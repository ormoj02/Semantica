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
//  a) Considerar las variables y los casteos en las expresiones matematicas en ensamblador
//  b) Considerar el residuo de la division en assembler
//  c) Programar el printf y scanf en assembler
// Requerimiento 4:
//  x a) Programar el else en assembler
//  b) Programar el for en assembler
// Requerimiento 5:
//  a) Programar el while en assembler
//  b) Programar el do while en assembler
namespace Semantica
{
    public class Lenguaje : Sintaxis/* , IDisposable  */
    {
        /* public void Dispose()
        {
            Console.WriteLine("\nDestructor");
        }  */


        List<Variable> variables = new List<Variable>();
        Stack<float> stack = new Stack<float>();

        Variable.TipoDato dominante;
        int cIf;
        int cFor;

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
            BloqueInstrucciones(true);
        }
        //Bloque de instrucciones -> {listaIntrucciones?}
        private void BloqueInstrucciones(bool evaluacion)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "}")
            {
                ListaInstrucciones(evaluacion);
            }
        }

        //ListaInstruccionesCase -> Instruccion ListaInstruccionesCase?
        private void ListaInstruccionesCase(bool evaluacion)
        {
            Instruccion(evaluacion);
            if (getContenido() != "case" && getContenido() != "break" && getContenido() != "default" && getContenido() != "}")
            {
                ListaInstruccionesCase(evaluacion);
            }
        }

        //Instruccion -> Printf | Scanf | If | While | do while | For | Switch | Asignacion
        private void Instruccion(bool evaluacion)
        {
            if (getContenido() == "printf")
            {
                Printf(evaluacion);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(evaluacion);
            }
            else if (getContenido() == "if")
            {
                If(evaluacion);
            }
            else if (getContenido() == "while")
            {
                While(evaluacion);
            }
            else if (getContenido() == "do")
            {
                Do(evaluacion);
            }
            else if (getContenido() == "for")
            {
                For(evaluacion);
            }
            else if (getContenido() == "switch")
            {
                Switch(evaluacion);
            }
            else
            {
                Asignacion(evaluacion);
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
        private void Asignacion(bool evaluacion)
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
                modVariable(nombre, Incremento(nombre, evaluacion));

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
                Expresion();
                match(";");

                //hacemos el pop     
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
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
                asm.WriteLine("MOV " + nombre + ", AX");
            }
        }

        //While -> while(Condicion) bloque de instrucciones | instruccion
        private void While(bool evaluacion)
        {
            match("while");

            match("(");
            bool validarWhile;
            int posicionWhile = posicion;
            int lineaWhile = linea;
            string variable = getContenido();
            do
            {
                validarWhile = Condicion("");
                if (!evaluacion)
                {
                    validarWhile = false;
                }

                match(")");


                if (getContenido() == "{")
                {
                    if (validarWhile)
                    {
                        BloqueInstrucciones(evaluacion);
                    }
                    else
                    {
                        BloqueInstrucciones(false);
                    }
                }
                else
                {
                    if (validarWhile)
                    {
                        Instruccion(evaluacion);
                    }
                    else
                    {
                        Instruccion(false);
                    }
                }
                if (validarWhile)
                {
                    posicion = posicionWhile - variable.Length;
                    linea = lineaWhile;
                    setPosicion(posicion);
                    NextToken();

                }
            } while (validarWhile);


        }

        //Do -> do bloque de instrucciones | intruccion while(Condicion)
        private void Do(bool evaluacion)
        {
            bool validarDo = evaluacion;
            string variable;
            if (!evaluacion)
            {
                validarDo = false;
            }
            match("do");
            int posicionDo = posicion;
            int lineaDo = linea;
            do
            {
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
                match("while");
                match("(");
                variable = getContenido();
                //Requerimiento 4.- Si la condicion no es booleana levanta la excepcion
                validarDo = Condicion("");
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
            } while (validarDo);


            match(")");
            match(";");
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstruccones | Intruccion 
        private void For(bool evaluacion)
        {
            string etiquetaInicioFor = "inicioFor" + cFor;
            string etiquetaFinFor = "finFor" + cFor++;
            asm.WriteLine(etiquetaInicioFor + ":");
            match("for");
            match("(");
            Asignacion(evaluacion);
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
                validarFor = Condicion("");
                if (!evaluacion)
                {
                    validarFor = false;
                }
                match(";");

                //requerimiento 1.d
                match(Tipos.Identificador);
                valor = Incremento(variable, evaluacion);


                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(validarFor);
                }
                else
                {
                    Instruccion(validarFor);
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
            } while (validarFor);
            asm.WriteLine(etiquetaFinFor + ":");
            //c)Regresar a la posicion de lectura del archivo de texto
            //d) sacar otro tokencon el metodo nextToken(
        }
        private void setPosicion(int pos)
        {
            archivo.DiscardBufferedData();
            archivo.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        //Incremento -> Identificador ++ | --
        /* private void Incremento(bool evaluacion)
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
                if (evaluacion)
                {

                    modVariable(variable, getValor(variable) + 1);

                }
            }
            else
            {
                match("--");
                //si la variable evaluacion es verdadera entonces se modifica el valor de la variable
                if (evaluacion)
                {

                    modVariable(variable, getValor(variable) - 1);
                }

            }
        } */
        private float Incremento(string variable, bool evaluacion)
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
                    nuevoValor = getValor(variable) - 1;
                }

            }
            else if (getContenido() == "+=")
            {
                match("+=");
                Expresion();
                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) + incremento;
                }

            }
            else if (getContenido() == "-=")
            {
                match("-=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) - incremento;
                }

            }
            else if (getContenido() == "*=")
            {
                match("*=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) * incremento;
                }

            }
            else if (getContenido() == "/=")
            {
                match("/=");
                Expresion();

                incremento = stack.Pop();

                if (evaluacion)
                {
                    nuevoValor = getValor(variable) / incremento;
                }

            }
            else if (getContenido() == "%=")
            {
                match("%=");
                Expresion();

                incremento = stack.Pop();

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


            /* switch (getContenido())
            {
                case "++":
                    match("++");
                    if (evaluacion)
                    {
                        nuevoValor = getValor(variable) + incremento;
                        if (tipoDato == Variable.TipoDato.Char && nuevoValor > 255)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Int && nuevoValor > 65535)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Float)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else
                        {
                            return nuevoValor;
                        }
                    }
                    else
                    {
                        return getValor(variable);
                    }
                case "--":
                    match("--");
                    if (evaluacion)
                    {
                        nuevoValor = getValor(variable) - incremento;
                        if (tipoDato == Variable.TipoDato.Char && nuevoValor > 255)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Int && nuevoValor > 65535)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Float)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else
                        {
                            return nuevoValor;
                        }
                    }
                    else
                    {
                        return getValor(variable);
                    }

                case "+=":
                    match("+=");
                    //guardamos el valor del incremento
                    incrementoValor = getContenido();
                    match(Tipos.Numero);
                    //lo convertimos a float
                    incremento = float.Parse(incrementoValor);
                    //return getValor(variable)+incrementoValor;
                    if (evaluacion)
                    {
                        nuevoValor = getValor(variable) + incremento;
                        if (tipoDato == Variable.TipoDato.Char && nuevoValor > 255)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Int && nuevoValor > 65535)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Float)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else
                        {
                            return nuevoValor;
                        }
                    }
                    else
                    {
                        return getValor(variable);
                    }
                case "-=":
                    match("-=");
                    //guardamos el valor del incremento
                    incrementoValor = getContenido();
                    match(Tipos.Numero);
                    //lo convertimos a float
                    incremento = float.Parse(incrementoValor);
                    //return getValor(variable)+incrementoValor;
                    if (evaluacion)
                    {
                        nuevoValor = getValor(variable) - incremento;
                        if (tipoDato == Variable.TipoDato.Char && nuevoValor > 255)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Int && nuevoValor > 65535)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else if (tipoDato == Variable.TipoDato.Float)
                        {
                            throw new Error("Error de semantica, no podemos hacer ese incremento en una variable de tipo: " + tipoDato, log);
                        }
                        else
                        {
                            return nuevoValor;
                        }
                    }
                    else
                    {
                        return getValor(variable);
                    }
                default:
                    throw new Error("Error: Se esperaba ++ o -- en la linea: " + linea, log);
            } */
        }

        //Switch -> switch (Expresion) {Lista de casos} | (default: )
        private void Switch(bool evaluacion)
        {
            match("switch");
            match("(");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(")");
            match("{");
            ListaDeCasos(evaluacion);
            if (getContenido() == "default")
            {
                match("default");
                match(":");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(evaluacion);
                }
                else
                {
                    Instruccion(evaluacion);
                }
            }
            match("}");
        }

        //ListaDeCasos -> case Expresion: listaInstruccionesCase (break;)? (ListaDeCasos)?
        private void ListaDeCasos(bool evaluacion)
        {
            match("case");
            Expresion();
            stack.Pop();
            asm.WriteLine("POP AX");
            match(":");
            ListaInstruccionesCase(evaluacion);
            if (getContenido() == "break")
            {
                match("break");
                match(";");
            }
            if (getContenido() == "case")
            {
                ListaDeCasos(evaluacion);
            }
        }

        //Condicion -> Expresion operador relacional Expresion
        private bool Condicion(string etiqueta)
        {
            Expresion();
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion();
            float e2 = stack.Pop();
            asm.WriteLine("POP BX");
            float e1 = stack.Pop();
            asm.WriteLine("POP AX");
            asm.WriteLine("CMP AX, BX");
            switch (operador)
            {
                case ">":
                    asm.WriteLine("JLE " + etiqueta);
                    return e1 > e2;
                case "<":
                    asm.WriteLine("JGE " + etiqueta);
                    return e1 < e2;
                case ">=":
                    asm.WriteLine("JL " + etiqueta);
                    return e1 >= e2;
                case "<=":
                    asm.WriteLine("JG " + etiqueta);
                    return e1 <= e2;
                case "==":
                    asm.WriteLine("JNE " + etiqueta);
                    return e1 == e2;
                default:
                    asm.WriteLine("JE " + etiqueta);
                    return e1 != e2;
            }
        }

        //If -> if(Condicion) bloque de instrucciones (else bloque de instrucciones)?
        private void If(bool evaluacion)
        {
            string etiquetaIf = "if" + ++cIf;
            string etiquetaElse = "else" + cIf;
            match("if");
            match("(");
            //Requerimiento 4
            bool validarIf = Condicion(etiquetaIf);
            if (!evaluacion)
            {
                validarIf = false;
            }

            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(validarIf);
            }
            else
            {
                Instruccion(validarIf);
            }
            asm.WriteLine("JMP " + etiquetaElse);
            asm.WriteLine(etiquetaIf + ":");
            if (getContenido() == "else")
            {
                match("else");
                //Requerimiento 4
                if (getContenido() == "{")
                {
                    if (evaluacion)
                    {
                        BloqueInstrucciones(!validarIf);
                    }
                    else
                    {
                        BloqueInstrucciones(false);
                    }

                }
                else
                {
                    if (evaluacion)
                    {
                        Instruccion(!validarIf);
                    }
                    else
                    {
                        Instruccion(false);
                    }
                }

            }
            asm.WriteLine(etiquetaElse + ":");
        }

        //Printf -> printf(cadena|expresion);
        private void Printf(bool evaluacion)
        {

            match("printf");
            match("(");

            //revisamos si es una cadena
            if (getClasificacion() == Tipos.Cadena)
            {
                if (evaluacion)
                {
                    //cambiamos las comillas por los datos correctos
                    setContenido(getContenido().Replace("\\t", "     "));
                    setContenido(getContenido().Replace("\\n", "\n"));
                    setContenido(getContenido().Replace("\"", string.Empty));
                    //escribe contenido
                    Console.Write(getContenido());
                }
                asm.WriteLine("PRINTN \"" + getContenido() + "\"");

                match(Tipos.Cadena);
            }
            else
            {
                Expresion();
                float resultado = stack.Pop();
                asm.WriteLine("POP AX");
                if (evaluacion)
                {
                    Console.Write(resultado);
                    //Codigo assembler para imprimir una variable
                }
            }


            match(")");
            match(";");
        }

        //Scanf -> scanf(cadena, &identificador);
        private void Scanf(bool evaluacion)
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

                asm.WriteLine("CALL SCAN_NUM");
                asm.WriteLine("MOV " + getContenido() + ", CX");
                //modVariable(getContenido(), float.Parse(val));

            }

            match(Tipos.Identificador);
            match(")");
            match(";");
        }

        //Expresion -> Termino MasTermino
        private void Expresion()
        {
            Termino();
            MasTermino();
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino()
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");
                switch (operador)
                {
                    case "+":
                        stack.Push(n2 + n1);
                        asm.WriteLine("ADD AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                    case "-":
                        stack.Push(n2 - n1);
                        asm.WriteLine("SUB AX, BX");
                        asm.WriteLine("PUSH AX");
                        break;
                }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino()
        {
            Factor();
            PorFactor();
        }
        //PorFactor -> (OperadorFactor Factor)? 
        private void PorFactor()
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor();
                log.Write(operador + " ");
                float n1 = stack.Pop();
                asm.WriteLine("POP BX");
                float n2 = stack.Pop();
                asm.WriteLine("POP AX");



                //Requerimiento 1.a
                switch (operador)
                {
                    case "*":
                        stack.Push(n2 * n1);
                        asm.WriteLine("MUL BX");
                        //en AX se guarda el resultado de multiplicar AL y el otro operador
                        //en este caso lo guardamos en BX
                        asm.WriteLine("PUSH AX");
                        break;
                    case "/":
                        stack.Push(n2 / n1);
                        asm.WriteLine("DIV BX");
                        //Se guarda el resultado de la division en AL y lo metemos al stack
                        asm.WriteLine("PUSH AX");
                        break;
                    case "%":
                        stack.Push(n2 % n1);
                        asm.WriteLine("DIV BX");
                        //Se guarda el resto de la division en AH y lo metemos al stack
                        asm.WriteLine("PUSH DX");
                        break;
                }
            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor()
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(getContenido() + " ");
                if (dominante < evaluaNumero(float.Parse(getContenido())))
                {
                    dominante = evaluaNumero(float.Parse(getContenido()));
                }
                stack.Push(float.Parse(getContenido()));
                asm.WriteLine("MOV AX, " + getContenido());
                asm.WriteLine("PUSH AX");
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

                //metemos la variable dentro del stack para hacer operaciones 
                stack.Push(getValor(getContenido()));
                // Requerimiento 3.a 
                //pasamos al siguiente token
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
                Expresion();
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
                    asm.WriteLine("POP AX");
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