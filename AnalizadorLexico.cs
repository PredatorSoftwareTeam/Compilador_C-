using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador
{
    class AnalizadorLexico
    {
        public List<Error> listaError;
        public List<Token> listaToken;

        private string codigoFuente;
        private int linea;

        private int[,] matrizTransicion =
        {
               //          0            1           2               3           4         5         6         7      8        9      10       11     12       13     14      15      16      17      18     19      20      21      22       23      24       25        26          27          28          29
               //      espacio  ||   enter  ||     digito  ||    palabra  ||   "	 ||   '	 ||    +	 ||   -	 ||   *	 ||   /	 ||   =	 ||   <	 ||   >	 ||   !	 ||   &	 ||   |	 ||   {	 ||   }	 ||   [	 ||   ]	 ||   ,	 ||   .	 ||   ;	 ||   :	 ||   ?	 ||   _	 ||   EOF	 ||   Unknown  ||   (    ||     )   ||
               /*0*/   {    0,          0,          3,              1,          6,        7,        9,        10,    11,      12,    16,      18,    19,      17,    20,     21,     -28,    -29,    -30,   -31,    -32,    -37,    -33,     -34,    -35,     2,        0,          -507,       -26,        -27  },
               /*1*/   {    -1,         -1,         1,              1,          -500,     -500,     -1,       -1,    -1,      -1,    -1,      -1,    -1,      -1,    -1,     -1,     -1,     -1,     -1,    -1,     -1,     -1,     -1,      -1,     -500,    -1,       -1,         -507,       -1,         -1   },
               /*2*/   {    -500,       -500,       1,              1,          -500,     -500,     -500,     -500,  -500,    -500,  -500,    -500,  -500,    -500,  -500,   -500,   -500,   -500,   -500,  -500,   -500,   -500,   -500,    -500,   -500,    -500,     -500,       -507,       -500,       -500 },
               /*3*/   {    -2,         -2,         3,              -502,       -502,     -502,     -2,       -2,    -2,      -2,    -2,      -2,    -2,      -502,  -2,     -2,     -502,   -2,     -502,  -2,     -2,     4,      -2,      -502,   -502,    -502,     -2,         -507,       -2,         -502 },
               /*4*/   {    -503,       -503,       5,              -503,       -503,     -503,     -503,     -503,  -503,    -503,  -503,    -503,  -503,    -503,  -503,   -503,   -503,   -503,   -503,  -503,   -503,   -503,   -503,    -503,   -503,    -503,     -503,       -507,       -503,       -503 },
               /*5*/   {    -3,         -3,         5,              -503,       -503,     -503,     -3,       -3,    -3,      -3,    -3,      -3,    -3,      -503,  -3,     -3,     -503,   -3,     -503,  -3,     -3,     -3,     -3,      -503,   -503,    -503,     -3,         -507,       -3,         -503 },
               /*6*/   {    6,          -504,       6,              6,          -4,       6,        6,        6,     6,       6,     6,       6,     6,       6,     6,      6,      6,      6,      6,     6,      6,      6,      6,       6,      6,       6,        -504,       -507,       6,          6    },
               /*7*/   {    8,          -505,       8,              8,          8,        -5,        8,        8,     8,       8,     8,       8,     8,       8,     8,      8,      8,      8,      8,     8,      8,      8,      8,       8,      8,       8,        -505,       -507,       8,          8    },
               /*8*/   {    8,          -505,       8,              8,          8,        -5,       8,        8,     8,       8,     8,       8,     8,       8,     8,      8,      8,      8,      8,     8,      8,      8,      8,       8,      8,       8,        -505,       -507,       8,          8    },
               /*9*/   {    -6,         -6,         -6,             -6,         -6,       -6,       -20,      -6,    -6,      -6,    -22,     -6,    -6,      -6,    -6,     -6,     -6,     -6,     -6,    -6,     -6,     -6,     -6,      -6,     -6,      -6,       -6,         -507,       -6,         -6   },
               /*10*/  {    -7,         -7,         -7,             -7,         -7,       -7,       -7,       -21,   -7,      -7,    -23,     -7,    -7,      -7,    -7,     -7,     -7,     -7,     -7,    -7,     -7,     -7,     -7,      -7,     -7,      -7,       -7,         -507,       -7,         -7   },
               /*11*/  {    -8,         -8,         -8,             -8,         -8,       -8,       -8,       -8,    -8,      -8,    -24,     -8,    -8,      -8,    -8,     -8,     -8,     -8,     -8,    -8,     -8,     -8,     -8,      -8,     -8,      -8,       -8,         -507,       -8,         -8   },
               /*12*/  {    -9,         -9,         -9,             -9,         -9,       -9,       -9,       -9,    14,      13,    -25,     -9,    -9,      -9,    -9,     -9,     -9,     -9,     -9,    -9,     -9,     -9,     -9,      -9,     -9,      -9,       -9,         -507,       -9,         -9   },
               /*13*/  {    13,         0,          13,             13,         13,       13,       13,       13,    13,      13,    13,      13,    13,      13,    13,     13,     13,     13,     13,    13,     13,     13,     13,      13,     13,      13,       0,          -507,       13,         13   },
               /*14*/  {    14,         14,         14,             14,         14,       14,       14,       14,    15,      14,    14,      14,    14,      14,    14,     14,     14,     14,     14,    14,     14,     14,     14,      14,     14,      14,       -506,       -507,       14,         14   },
               /*15*/  {    14,         0,          14,             14,         14,       14,       14,       14,    14,      0,     14,      14,    14,      14,    14,     14,     14,     14,     14,    14,     14,     14,     14,      14,     14,      14,       -506,       -507,       14,         14   },
               /*16*/  {    -36,        -36,        -36,            -36,        -36,      -36,      -36,      -36,   -36,     -36,   -11,     -36,   -36,     -36,   -36,    -36,    -36,    -36,    -36,   -36,    -36,    -36,    -36,     -36,    -36,     -36,      -36,        -507,       -36,        -36  },
               /*17*/  {    -19,        -19,        -19,            -19,        -19,      -19,      -19,      -19,   -19,     -19,   -12,     -19,   -19,     -19,   -19,    -19,    -19,    -19,    -19,   -19,    -19,    -19,    -19,     -19,    -19,     -19,      -19,        -507,       -19,        -19  },
               /*18*/  {    -13,        -13,        -13,            -13,        -13,      -13,      -13,      -13,   -13,     -13,   -15,     -13,   -13,     -13,   -13,    -13,    -13,    -13,    -13,   -13,    -13,    -13,    -13,     -13,    -13,     -13,      -13,        -507,       -13,        -13  },
               /*19*/  {    -14,        -14,        -14,            -14,        -14,      -14,      -14,      -14,   -14,     -14,   -16,     -14,   -14,     -14,   -14,    -14,    -14,    -14,    -14,   -14,    -14,    -14,    -14,     -14,    -14,     -14,      -14,        -507,       -14,        -14  },
               /*20*/  {    -501,       -501,       -501,           -501,       -501,     -501,     -501,     -501,  -501,    -501,  -501,    -501,  -501,    -501,  -17,    -501,   -501,   -501,   -501,  -501,   -501,   -501,   -501,    -501,   -501,    -501,     -501,       -507,       -501,       -501 },
               /*21*/  {    -501,       -501,       -501,           -501,       -501,     -501,     -501,     -501,  -501,    -501,  -501,    -501,  -501,    -501,  -501,   -18,    -501,   -501,   -501,  -501,   -501,   -501,   -501,    -501,   -501,    -501,     -501,       -507,       -501,       -501 },

        };

        public AnalizadorLexico(string codigoFuente)
        {
            this.codigoFuente = codigoFuente + " ";
            listaError = new List<Error>();
            listaToken = new List<Token>();
        }

        private int determinarPalabraReservada(string lexema)
        {
            switch (lexema)
            {
                case "abstract":
                    return -50;
                case "bool":
                    return -51;
                case "break":
                    return -52;
                case "byte":
                    return -53;
                case "case":
                    return -54;
                case "catch":
                    return -55;
                case "char":
                    return -56;
                case "class":
                    return -57;
                case "const":
                    return -58;
                case "decimal":
                    return -59;
                case "default":
                    return -60;
                case "do":
                    return -61;
                case "double":
                    return -62;
                case "else":
                    return -63;
                case "false":
                    return -64;
                case "float":
                    return -65;
                case "for":
                    return -66;
                case "foreach":
                    return -67;
                case "if":
                    return -68;
                case "in":
                    return -69;
                case "int":
                    return -70;
                case "is":
                    return -71;
                case "long":
                    return -72;
                case "new":
                    return -73;
                case "null":
                    return -74;
                case "object":
                    return -75;
                case "private":
                    return -76;
                case "protected":
                    return -77;
                case "public":
                    return -78;
                case "ref":
                    return -79;
                case "return":
                    return -80;
                case "sealed":
                    return -81;
                case "short":
                    return -82;
                case "sizeof":
                    return -83;
                case "static":
                    return -84;
                case "string":
                    return -85;
                case "switch":
                    return -86;
                case "this":
                    return -87;
                case "true":
                    return -88;
                case "try":
                    return -89;
                case "typeof":
                    return -90;
                case "using":
                    return -91;
                case "void":
                    return -92;
                case "while":
                    return -93;
                case "namespace":
                    return -94;
                case "system":
                    return -95;
                case "console":
                    return -97;
                case "read":
                    return -98;
                case "write":
                    return -99;
                default:
                    return -1;
            }
        }

        private char siguienteCaracter(int i)
        {
            return Convert.ToChar(codigoFuente.Substring(i,1));
        }

        private int determinarColumna(char caracter)
        {
            if (caracter.Equals(' '))
            {
                return 0;
            }
            else if (caracter.Equals('\n'))
            {
                return 1;
            }
            else if (char.IsDigit(caracter))
            {
                return 2;
            }
            else if (char.IsLetter(caracter))
            {
                return 3;
            }
            else if (caracter.Equals('"'))
            {
                return 4;
            }
            else if (caracter.Equals('\''))
            {
                return 5;
            }
            else if (caracter.Equals('+'))
            {
                return 6;
            }
            else if (caracter.Equals('-'))
            {
                return 7;
            }
            else if (caracter.Equals('*'))
            {
                return 8;
            }
            else if (caracter.Equals('/'))
            {
                return 9;
            }
            else if (caracter.Equals('='))
            {
                return 10;
            }
            else if (caracter.Equals('<'))
            {
                return 11;
            }
            else if (caracter.Equals('>'))
            {
                return 12;
            }
            else if (caracter.Equals('!'))
            {
                return 13;
            }
            else if (caracter.Equals('&'))
            {
                return 14;
            }
            else if (caracter.Equals('|'))
            {
                return 15;
            }
            else if (caracter.Equals('{'))
            {
                return 16;
            }
            else if (caracter.Equals('}'))
            {
                return 17;
            }
            else if (caracter.Equals('['))
            {
                return 18;
            }
            else if (caracter.Equals(']'))
            {
                return 19;
            }
            else if (caracter.Equals(','))
            {
                return 20;
            }
            else if (caracter.Equals('.'))
            {
                return 21;
            }
            else if (caracter.Equals(';'))
            {
                return 22;
            }
            else if (caracter.Equals(':'))
            {
                return 23;
            }
            else if (caracter.Equals('?'))
            {
                return 24;
            }
            else if (caracter.Equals('_'))
            {
                return 25;
            }
            else if (caracter.Equals('('))
            {
                return 28;
            }
            else if (caracter.Equals(')'))
            {
                return 29;
            }
            else
            {
                return 27;
            }
        }

        private TipoToken determianrTipoToken(int estadoToken)
        {
            switch (estadoToken)
            {
                case -1:
                    return TipoToken.identificador;
                case -2:
                    return TipoToken.numeroEntero;
                case -3:
                    return TipoToken.numeroDecimal;
                case -4:
                    return TipoToken.cadena;
                case -5:
                    return TipoToken.caracter;
                case -6:
                    return TipoToken.operadorAritmetico;
                case -7:
                    return TipoToken.operadorAritmetico;
                case -8:
                    return TipoToken.operadorAritmetico;
                case -9:
                    return TipoToken.operadorAritmetico;
                case -11:
                    return TipoToken.operadorRelacional;
                case -12:
                    return TipoToken.operadorRelacional;
                case -13:
                    return TipoToken.operadorRelacional;
                case -14:
                    return TipoToken.operadorRelacional;
                case -15:
                    return TipoToken.operadorRelacional;
                case -16:
                    return TipoToken.operadorRelacional;
                case -17:
                    return TipoToken.operadorLogico;
                case -18:
                    return TipoToken.operadorLogico;
                case -19:
                    return TipoToken.operadorLogico;
                case -20:
                    return TipoToken.operadorIncremento;
                case -21:
                    return TipoToken.operadorIncremento;
                case -22:
                    return TipoToken.operadorIncremento;
                case -23:
                    return TipoToken.operadorIncremento;
                case -24:
                    return TipoToken.operadorIncremento;
                case -25:
                    return TipoToken.operadorIncremento;
                case -26:
                    return TipoToken.simboloSimple;
                case -27:
                    return TipoToken.simboloSimple;
                case -28:
                    return TipoToken.simboloSimple;
                case -29:
                    return TipoToken.simboloSimple;
                case -30:
                    return TipoToken.simboloSimple;  
                case -31:
                    return TipoToken.simboloSimple;
                case -32:
                    return TipoToken.simboloSimple;
                case -33:
                    return TipoToken.simboloSimple;
                case -34:
                    return TipoToken.simboloSimple;
                case -35:
                    return TipoToken.simboloSimple;
                case -36:
                    return TipoToken.asignacion;
                case -37:
                    return TipoToken.acceso;
                case -50:
                    return TipoToken.palabraReservada;
                case -51:
                    return TipoToken.palabraReservada;
                case -52:
                    return TipoToken.palabraReservada;
                case -53:
                    return TipoToken.palabraReservada;
                case -54:
                    return TipoToken.palabraReservada;
                case -55:
                    return TipoToken.palabraReservada;
                case -56:
                    return TipoToken.palabraReservada;
                case -57:
                    return TipoToken.palabraReservada;
                case -58:
                    return TipoToken.palabraReservada;
                case -59:
                    return TipoToken.palabraReservada;
                case -60:
                    return TipoToken.palabraReservada;
                case -61:
                    return TipoToken.palabraReservada;
                case -62:
                    return TipoToken.palabraReservada;
                case -63:
                    return TipoToken.palabraReservada;
                case -64:
                    return TipoToken.palabraReservada;
                case -65:
                    return TipoToken.palabraReservada;
                case -66:
                    return TipoToken.palabraReservada;
                case -67:
                    return TipoToken.palabraReservada;
                case -68:
                    return TipoToken.palabraReservada;
                case -69:
                    return TipoToken.palabraReservada;
                case -70:
                    return TipoToken.palabraReservada;
                case -71:
                    return TipoToken.palabraReservada;
                case -72:
                    return TipoToken.palabraReservada;
                case -73:
                    return TipoToken.palabraReservada;
                case -74:
                    return TipoToken.palabraReservada;
                case -75:
                    return TipoToken.palabraReservada;
                case -76:
                    return TipoToken.palabraReservada;
                case -77:
                    return TipoToken.palabraReservada;
                case -78:
                    return TipoToken.palabraReservada;
                case -79:
                    return TipoToken.palabraReservada;
                case -80:
                    return TipoToken.palabraReservada;
                case -81:
                    return TipoToken.palabraReservada;
                case -82:
                    return TipoToken.palabraReservada;
                case -83:
                    return TipoToken.palabraReservada;
                case -84:
                    return TipoToken.palabraReservada;
                case -85:
                    return TipoToken.palabraReservada;
                case -86:
                    return TipoToken.palabraReservada;
                case -87:
                    return TipoToken.palabraReservada;
                case -88:
                    return TipoToken.palabraReservada;
                case -89:
                    return TipoToken.palabraReservada;
                case -90:
                    return TipoToken.palabraReservada;
                case -91:
                    return TipoToken.palabraReservada;
                case -92:
                    return TipoToken.palabraReservada;
                case -93:
                    return TipoToken.palabraReservada;
                case -94:
                    return TipoToken.palabraReservada;
                default:
                    return TipoToken.desconocido;

            }
        }

        private Error manejoErrores(int estadoError)
        {
            string mensajeError;

            switch (estadoError)
            {
                case -500:
                    mensajeError = "Formato de identificador incorrecto";
                    break;
                case -501:
                    mensajeError = "Formato de operador relacional incorrecto";
                    break;
                case -502:
                    mensajeError = "Formato de numero entero incorrecto";
                    break;
                case -503:
                    mensajeError = "Formato de numero decimal incorrecto";
                    break;
                case -504:
                    mensajeError = "Formato de cadena no valida";
                    break;
                case -505:
                    mensajeError = "Formato de caracter no valida";
                    break;
                case -506:
                    mensajeError = "Formato de comentario incorrecto, falta cierre de comentario";
                    break;
                case -507:
                    mensajeError = "Palabra desconocida";
                    break;
                case -508:
                    mensajeError = "Error al cerrar la cadena, se encuentra vacia";
                    break;
                default:
                    mensajeError = "Error desconocido";
                    break;
            }

            return new Error() { Codigo = estadoError, MensajeError = mensajeError, TipoError = tipoError.Lexico, Linea = linea };
        }

        public List<Token> AnalizarLexico()
        {
            int estado = 0;
            int columna = 0;

            char caracterActual;
            char caracterSiguiente;
            string lexema = string.Empty;
            linea = 1;

            for (int i = 0; i < codigoFuente.ToCharArray().Length; i++)
            {
                caracterActual = siguienteCaracter(i);

                if (caracterActual.Equals('\n'))
                {
                    linea++;
                    
                }
                
                lexema += caracterActual;

                if (caracterActual.Equals('"'))//Mostrar cadenas sin comillas 
                {
                    lexema = lexema.Remove(lexema.Length - 1);
                    lexema = lexema + " ";
                }

                if (caracterActual.Equals('\''))//Mostrar caracteres sin comillas simples
                {
                    lexema = lexema.Remove(lexema.Length - 1);
                    lexema = lexema + " ";
                }


                columna = determinarColumna(caracterActual);
                estado = matrizTransicion[estado,columna];

                if (estado < 0 && estado > -499)
                {
                    
                    if (caracterActual.Equals('"') || caracterActual.Equals('\''))//Solucion a error de las cadenas validas
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + " ";
                        if (lexema.Equals(""))
                        {
                            lexema = string.Empty;
                        }
                        
                    }
                    /*
                    if (estado == -1)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                    }
                    */
                    else if (caracterActual.Equals('|'))//Solucion a error del operador relacional 'or'
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        lexema = lexema + "|";
                    }

                    else if (caracterActual.Equals('&'))//Solucion a error del operador relacional 'and'
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        lexema = lexema + "&";
                    }

                    else if (caracterActual.Equals('-'))//Solucion a operador de incremento '--'
                    {
                        //lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + "-";
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "-=";
                        }
                        
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "-";
                        }
                    }

                    else if (caracterActual.Equals('+'))//Solucion a operador de incremento '++'
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "+=";
                        }
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "+";
                        }
                        
                    }
                    else if (caracterActual.Equals('*'))//Solucion a operador de incremento '*='
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "*=";
                        }
                        /*
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            i--;
                        }
                        */
                    }
                    else if (caracterActual.Equals('/'))//Solucion a operador de incremento '/='
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "/=";
                        }
                        /*
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            i--;
                        }
                        */
                    }
                    else if (caracterActual.Equals('!'))//Solucion a operador relacional '!='
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "!=";
                        }
                        /*
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            i--;
                        }
                        */
                    }
                    else if (caracterActual.Equals('<'))//Solucion a operador relacional '<='
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + "<=";
                        }
                        /*
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            
                        }
                        */

                    }
                    else if (caracterActual.Equals('>'))//Solucion a operador relacional '>='
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('='))
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            lexema = lexema + ">=";
                        }
                        /*
                        else
                        {
                            lexema = lexema.Remove(lexema.Length - 1);
                            
                        }
                        */
                    }
                    else if (caracterActual.Equals('='))//Solucion a operador relacional '=='
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        
                        lexema = lexema + "=";
                    }
                    /*
                    else if (lexema.Length > 1)
                    {
                        
                        lexema = lexema.Remove(lexema.Length - 1);
                        i--;
                        
                        
                    }
                    */
                    if (estado == -1 || estado == -2 || estado == -3)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                        //linea--;

                    }
                    if (estado == -13 || estado == -14)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                    }
                    if (estado == -6 || estado == -7)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                    }
                    if (estado == -19)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                    }
                    if (estado == -36)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                    }
                    if (estado == -8 || estado == -9)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        i--;
                    }
                    /*
                    if (estado == 6)
                    {
                        caracterSiguiente = siguienteCaracter(i);
                        if (caracterSiguiente.Equals('\n'))
                        {
                            linea--;
                        }
                        //lexema = lexema.Remove(lexema.Length - 1);
                        //lexema = lexema + caracterActual;
                        //i--;
                    }
                    */



                    /*
                    if (lexema.Length > 1)
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        i--;
                    }
                    */
                    Token nuevoToken = new Token() { ValorToken = estado, Lexema = lexema, Linea = linea };

                    if (estado == -1)
                    {
                        nuevoToken.ValorToken = determinarPalabraReservada(nuevoToken.Lexema);
                    }

                    nuevoToken.TipoToken = determianrTipoToken(nuevoToken.ValorToken);
                    listaToken.Add(nuevoToken);

                    estado = 0;
                    columna = 0;
                    lexema = string.Empty;
                }
                else if (estado <= -499)
                {
                    listaError.Add(manejoErrores(estado));

                    estado = 0;
                    columna = 0;
                    lexema = string.Empty;
                }
                else if (estado == 0)
                {
                    columna = 0;
                    lexema = string.Empty;
                }
            }

            if(estado != 0)
            {
                if (estado == 6)
                {
                    listaError.Add(manejoErrores(-504));
                }
                else if (estado == 7)
                {
                    listaError.Add(manejoErrores(-505));
                }
                else if (estado == 8)
                {
                    listaError.Add(manejoErrores(-505));
                }
                else
                {
                    listaError.Add(manejoErrores(-506));
                }

            }

            /*
             
             */
           
            return listaToken;
        }

    }
}
