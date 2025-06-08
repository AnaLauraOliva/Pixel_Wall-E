# Pixel Wall-E

## Ejecutables

Los ejecutables de Linux y Windows están en la carpeta ejecutables. Arquitecturas disponibles:

+ Linux
  + arm64
  + arm32
  + x86_64
  + x86_32
+ Windows
  + x86_64
  + x86_32
  + arm64

---

## Descripción

Pixel Wall-E es una aplicación diseñada para crear arte pixelado mediante un lenguaje de programación simple. El proyecto consiste en un editor de texto que interpreta comandos específicos para controlar un robot llamado Wall-E, permitiéndole dibujar en un canvas cuadriculado. La aplicación incluye funcionalidades como la carga y guardado de archivos .pw, ejecución de comandos y una interfaz gráfica. Además dentro de la propia aplicación hay una documentación que explica todo lo relativo al lenguaje.

---

## Sobre el código

Dividido en:

+ Compilador
  + Lexer
  + Parser
  + Semantic Analyzer
  + Interpreter
+ Canvas Logic
  + Canvas
  + Pixel
  + WallE
+ Visual
  + CanvasPanel
  + CodeEdit
  + Compiler
  + MainMenu
  + Manual
  + Music

---

### Compilador

El compilador es la parte de la lógica en la que se compila el código. El lexer se encarga en dividir en tokens el texto plano que el Handler del CodeEdit le pasa. Una vez tokenizado, el Handler le pasa la lista de tokens al parser, este lo divide en statements velando que cumplan con las reglas del lenguaje. Después, se le pasa al Semantic Analyzer, este se encarga de que las funciones declaradas  por el usuario devuelvan el tipo que se especifica, que las mismas estén declaradas en un lugar válido y que la cantidad de parámetros sea la correspondiente, que todas las variables y labels estén declarados, que los tipos sean los correctos, entre otras reglas sintácticas.

---

### Canvas Logic

Es la representación abstracta del canvas, un array bidimensional de pixels de n x n que contiene todos los métodos de pintar las figuras, llenado, spawneo de Wall-E y el resto de instrucciones y funciones nativas del lenguaje. Los pixeles guardan la posición inicial y final de la casilla en el canvas original(texture rect de godot), el color de la casilla y si contiene al robot. WallE es la clase que representa al robot, contiene la posición en el array bidimensional y se encarga de moverse a sí mismo.

---

### Visual

Contiene el código relacionado con Godot: resaltado de sintaxis, lógica de los botones, imprimir los problemas, la animación del canvas, los menús y la música de fondo.
