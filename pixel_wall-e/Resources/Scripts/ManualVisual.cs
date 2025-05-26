using Godot;
using System;
using System.Collections.Generic;

public partial class ManualVisual : Control
{
    Dictionary<string, (string text, string title)> _displayText = new Dictionary<string, (string, string)>
    {
        ["Welcome"] = (
@"[center][font_size=28][b]🛠️ ¡Bienvenido al Mundo de Pixel Wall-E! 🎨[/b][/font_size]

[img]res://Resources/Images/Designer.png[/img]

[i]Donde tu código se transforma en arte pixelado[/i][/center]

[left]¡Hola, futuro artista digital! 🌟 Con Pixel Wall-E, tienes el poder de crear:

■ 🖌️ [b]Obras maestras pixeladas[/b] con simples comandos
■ 🤖 [b]Control total[/b] sobre tu robot Wall-E
■ 🎭 [b]Animaciones y patrones[/b] complejos
■ 💡 [b]Lógica de programación[/b] visual y divertida
■ 🔍 [b]Ayudas inteligentes[/b] mientras programas

[color=#FF5733]”No solo programas… ¡Das vida a píxeles!”[/color][/left]

===== 🎮 CARACTERÍSTICAS DEL EDITOR =====

[font_size=28]✨ Autocompletado[/font_size]
[left]El editor sugiere comandos mientras escribes:
[bgcolor=#272822][code][table=1][cell]Spr[TAB] → [color=#A6E22E]DrawRectangle[/color]([color=#AE81FF]0[/color],[color=#AE81FF]0[/color])[/cell]
[cell]Dr[TAB] → [color=#A6E22E]DrawLine[/color]([color=#AE81FF]1[/color],[color=#AE81FF]1[/color],[color=#AE81FF]1[/color])[/cell][/table][/code][/bgcolor]

[b]Tooltips contextuales:[/b] Al posicionar el cursor sobre cualquier comando:
[bgcolor=#272822][code][table=1][cell]DrawLin|e ← Muestra descripción[/cell]
[cell]^ (solo en el nombre del comando)[/cell][/table][/code][/bgcolor][/left]

[font_size=28]📝 Sistema de Comentarios[/font_size]
[left][b]Una línea:[/b]
[bgcolor=#272822][code][table=1][cell][color=#75715E]// Esto es un comentario[/color][/cell]
[cell][color=#A6E22E]DrawRectangle[/color]([color=#AE81FF]0[/color],[color=#AE81FF]0[/color]) [color=#75715E]// Posición inicial[/color][/cell][/table][/code][/bgcolor]

[b]Multilínea:[/b]
[bgcolor=#272822][code][table=1][cell][color=#75715E]/*[/color][/cell]
[cell][color=#75715E]Programa de ejemplo[/color][/cell]
[cell][color=#75715E]Creado: 2024[/color][/cell]
[cell][color=#75715E]*/[/color][/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Red""[/color])[/cell][/table][/code][/bgcolor][/left]

[font_size=28]📥 Tipos de Entrada[/font_size]
[left]1.[b]Editor Integrado[/b] ✏️ con todas las ayudas
[bgcolor=#272822][code][table=1][cell][color=#75715E]// Mi primer programa[/color][/cell]
[cell][color=#A6E22E]DrawRectangle[/color]([color=#AE81FF]0[/color],[color=#AE81FF]0[/color])[/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Blue""[/color])[/cell][/table][/code][/bgcolor]

2.[b]Archivos .pw[/b] 💾
[bgcolor=#272822][code][table=1][cell]📂 [color=#E6DB74]arte.pw[/color][/cell]
[cell]└── Contiene tus comandos Wall-E[/cell][/table][/code][/bgcolor][/left]

[font_size=28]♾️🗺️ Sin limites teóricos para el tamaño del Canvas[/font_size]
Lo único que limita el tamaño del canvas es la memoria de tu dispositivo.
[b]Recomendación:[/b] Si la aplicación se vuelve lenta, disminuye las dimensiones.

[color=#2E86C1][b]Próximo paso:[/b] ¡Explora las instrucciones para empezar a crear![/color]",
"[center][b]Documentación: [i]Bienvenido[/i][/b][/center]"),

        ["Instructions"] = (
            @"===== 🛠 INSTRUCCIONES =====

[font_size=28]📍 Spawn(x, y) – Posicionamiento Inicial[/font_size]

[left][b]Función:[/b] Coloca a Wall-E en la posición inicial (obligatorio como primer comando)
[b]Parámetros:[/b]
  • x: Coordenada horizontal (0 = borde izquierdo)
  • y: Coordenada vertical (0 = borde superior)

[color=#3498DB][b]Ejemplos:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]0[/color], [color=#AE81FF]0[/color])    [color=#75715E]// Esquina superior izquierda[/color][/cell]
[cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]50[/color], [color=#AE81FF]50[/color])   [color=#75715E]// Centro en canvas de 100x100[/color][/cell]
[cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]255[/color], [color=#AE81FF]0[/color])   [color=#75715E]// Esquina derecha en canvas de 256px[/color][/cell][/table][/code][/bgcolor]
⚠️ Error si las coordenadas están fuera del canvas[/left]

[font_size=28]🎨 Color(""nombre"") – Control del Pincel[/font_size]

[left][b]Función:[/b] Cambia el color del pincel
[b]Colores disponibles:[/b]
  • ""Red"", ""Blue"", ""Green"", ""Yellow""
  • ""Orange"", ""Purple"", ""Black"", ""White""
  • ""Transparent"" (color por defecto), Cualquier código HEX

[color=#3498DB][b]Ejemplos:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Red""[/color])     [color=#75715E]// Pincel rojo[/color][/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""White""[/color])   [color=#75715E]// Activa modo ""borrador""[/color][/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Transparent""[/color]) [color=#75715E]// Desactiva pintado[/color][/cell][/table][/code][/bgcolor][/left]

[font_size=28]📏 Size(k) – Grosor del Pincel[/font_size]

[left][b]Función:[/b] Define el tamaño del pincel en píxeles
[b]Reglas:[/b]
  • k debe ser entero positivo
  • Si es par, usa el impar inferior (Size(4) → 3)
  • Valor por defecto: 1

[color=#3498DB][b]Ejemplos:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Size[/color]([color=#AE81FF]1[/color])   [color=#75715E]// Pincel fino (1px)[/color][/cell]
[cell][color=#A6E22E]Size[/color]([color=#AE81FF]5[/color])   [color=#75715E]// Pincel grueso (5px de diámetro)[/color][/cell]
[cell][color=#A6E22E]Size[/color]([color=#AE81FF]4[/color])   [color=#75715E]// Se convierte automáticamente a 3[/color][/cell][/table][/code][/bgcolor][/left]

[font_size=28]✏️ DrawLine(dirX, dirY, dist) – Líneas[/font_size]

[left][b]Función:[/b] Dibuja línea desde la posición actual
[b]Parámetros:[/b]
  • dirX, dirY: Dirección (-1, 0, 1)
  • dist: Longitud en píxeles

[color=#3498DB][b]Direcciones:[/b][/color]
[code](-1,-1) ↖   (0,-1) ↑   (1,-1) ↗
(-1,0)  ←               (1,0)  →
(-1,1)  ↙   (0,1)  ↓   (1,1)  ↘[/code]

[color=#3498DB][b]Ejemplo Práctico:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]10[/color], [color=#AE81FF]10[/color])[/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Blue""[/color])[/cell]
[cell][color=#A6E22E]Size[/color]([color=#AE81FF]3[/color])[/cell]
[cell][color=#A6E22E]DrawLine[/color]([color=#AE81FF]1[/color], [color=#AE81FF]0[/color], [color=#AE81FF]20[/color])  [color=#75715E]// Línea horizontal derecha[/color][/cell]
[cell][color=#A6E22E]DrawLine[/color]([color=#AE81FF]0[/color], [color=#AE81FF]1[/color], [color=#AE81FF]15[/color])  [color=#75715E]// Línea vertical abajo[/color][/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔵 DrawCircle(dirX, dirY, radio) – Círculos[/font_size]

[left][b]Función:[/b] Dibuja círculo desde la posición actual
[b]Comportamiento:[/b]
  • El centro se calcula moviéndose (dirX,dirY)*radio
  • Wall-E termina en el centro del círculo

[color=#3498DB][b]Ejemplo:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]30[/color], [color=#AE81FF]30[/color])[/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Green""[/color])[/cell]
[cell][color=#A6E22E]DrawCircle[/color]([color=#AE81FF]1[/color], [color=#AE81FF]1[/color], [color=#AE81FF]10[/color])  [color=#75715E]// Círculo hacia abajo-derecha[/color][/cell][/table][/code][/bgcolor][/left]

[font_size=28]🟦 DrawRectangle(dirX, dirY, dist, w, h) – Rectángulos[/font_size]

[left][b]Función:[/b] Dibuja rectángulo
[b]Parámetros:[/b]
  • Primero mueve Wall-E (dirX,dirY)*dist
  • w: ancho, h: alto del rectángulo

[color=#3498DB][b]Ejemplo:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]5[/color], [color=#AE81FF]5[/color])[/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Purple""[/color])[/cell]
[cell][color=#A6E22E]DrawRectangle[/color]([color=#AE81FF]1[/color], [color=#AE81FF]0[/color], [color=#AE81FF]10[/color], [color=#AE81FF]20[/color], [color=#AE81FF]30[/color])  [color=#75715E]// Rectángulo ancho[/color][/cell][/table][/code][/bgcolor][/left]

[font_size=28]🌊 Fill() – Relleno Mágico[/font_size]

[left][b]Función:[/b] Rellena área conectada con color actual
[b]Lógica:[/b]
  • Comienza en posición actual de Wall-E
  • Rellena todos los píxeles conectados del mismo color
  • No cruza bordes de otros colores

[color=#3498DB][b]Ejemplo Visual:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]Spawn[/color]([color=#AE81FF]50[/color], [color=#AE81FF]50[/color])[/cell]
[cell][color=#A6E22E]Color[/color]([color=#E6DB74]""Orange""[/color])[/cell]
[cell][color=#A6E22E]Fill[/color]()   [color=#75715E]// Rellena toda el área blanca conectada[/color][/cell][/table][/code][/bgcolor][/left]

[color=#E74C3C][b]Tip Pro:[/b] Combina estas instrucciones para crear arte complejo![/color]",
            "[center][b]Documentación: [i]Instrucciones[/i][/b][/center]"),
        ["Variables"] = (@"===== 📊 VARIABLES Y ASIGNACIONES =====

[font_size=28]📌 Declaración de Variables[/font_size]
[left][b]Sintaxis básica:[/b]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]nombre[/color] [color=#F92672]<-[/color] [color=#F8F8F2]valor[/color]        [/cell][/table][/code][/bgcolor]

[b]Reglas de nombres:[/b]
• Puede contener: letras (a-z, A-Z), números (0-9) y _
• No puede empezar con número o _
• Distingue mayúsculas/minúsculas
• Ejemplos válidos:
  [bgcolor=#272822][code][table=1][cell][color=#F8F8F2]contador[/color] [color=#F92672]<-[/color] [color=#AE81FF]0[/color][/cell]
  [cell][color=#F8F8F2]var_temp[/color] [color=#F92672]<-[/color] [color=#AE81FF]1[/color]        [/cell][/table][/code][/bgcolor]
• Ejemplos inválidos:
  [bgcolor=#272822][code][table=1][cell][color=#AE81FF]1[/color][color=#F8F8F2]valor[/color] [color=#F92672]<-[/color] [color=#AE81FF]10[/color]    [color=#75715E]// Error: empieza con número[/color][/cell]
  [cell][color=#F8F8F2]mi-valor[/color] [color=#F92672]<-[/color] [color=#AE81FF]5[/color]   [color=#75715E]// Error: guión no permitido[/color]        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔢 Tipos de Variables[/font_size]
[left][table=3,baseline,baseline,0]
[cell][b]Tipo[/b][/cell][cell][b]Ejemplo[/b][/cell][cell][b]Descripción[/b][/cell]
[cell]Numérico [/cell][cell]x <- 5 [/cell][cell]Enteros (positivos/negativos)[/cell]
[cell]Booleano [/cell][cell]activo <- 1 [/cell][cell]true o false[/cell]
[/table][/left]

[font_size=28]💡 Ejemplos Prácticos[/font_size]
[left][color=#3498DB][b]Asignaciones básicas:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]posX[/color] [color=#F92672]<-[/color] [color=#AE81FF]10[/color][/cell]
[cell][color=#F8F8F2]posY[/color] [color=#F92672]<-[/color] [color=#AE81FF]20[/color][/cell]
[cell][color=#F8F8F2]esta_activo[/color] [color=#F92672]<-[/color] ([color=#AE81FF]1[/color][color=#F92672]==[/color][color=#AE81FF]1[/color])        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Usando expresiones:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]area[/color] [color=#F92672]<-[/color] ([color=#F8F8F2]ancho[/color] [color=#F92672]*[/color] [color=#F8F8F2]alto[/color])[/cell]
[cell][color=#F8F8F2]Es_valido[/color] [color=#F92672]<-[/color] ([color=#F8F8F2]x[/color] [color=#F92672]>[/color] [color=#AE81FF]0[/color] [color=#F92672]&&[/color] [color=#F8F8F2]y[/color] [color=#F92672]>[/color] [color=#AE81FF]0[/color])        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Con funciones nativas:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]pos_actual_x[/color] [color=#F92672]<-[/color] [color=#A6E22E]GetActualX[/color]()[/cell]
[cell][color=#F8F8F2]Tamaño_canvas[/color] [color=#F92672]<-[/color] [color=#A6E22E]GetCanvasSize[/color]()[/cell]
[cell][color=#F8F8F2]Es_rojo[/color] [color=#F92672]<-[/color] [color=#A6E22E]IsBrushColor[/color]([color=#E6DB74]""Red""[/color])        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔄 Reasignación de Variables[/font_size]
[left]Las variables pueden cambiar de valor y tipo:
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]valor[/color] [color=#F92672]<-[/color] [color=#AE81FF]10[/color]        [color=#75715E]// Numérico[/color][/cell]
[cell][color=#F8F8F2]Valor[/color] [color=#F92672]<-[/color] ([color=#AE81FF]1[/color] [color=#F92672]==[/color] [color=#AE81FF]1[/color])   [color=#75715E]// Ahora booleano (1)[/color]        [/cell][/table][/code][/bgcolor]

[color=#E74C3C][b]Precaución:[/b] El tipo se determina por el valor actual[/color]

Ejemplo:
[bgcolor=#272822][code][table=1][cell][color=#A6E22E]DrawLine[/color]([color=#F8F8F2]x[/color], [color=#AE81FF]0[/color], [color=#AE81FF]10[/color])  [color=#75715E]// Funciona[/color]        [/cell][/table][/code][/bgcolor][/left]

[color=#2ECC71][b]Consejo profesional:[/b] Usa nombres descriptivos para tus variables![/color]",
"[center][b]Documentación: [i]Variables y Asignaciones[/i][/b][/center]"),
        ["Expr"] = (@"===== 🧮 EXPRESIONES DEL LENGUAJE =====

[font_size=28]➗ Expresiones Aritméticas[/font_size]
[left][b]Operadores disponibles:[/b]
[table=3,baseline,baseline,0]
[cell][b]Operador [/b][/cell][cell][b]Ejemplo [/b][/cell][cell][b]Descripción[/b][/cell]
[cell]+[/cell][cell]5 + 3[/cell][cell]Suma[/cell]
[cell]-[/cell][cell]x - 2[/cell][cell]Resta[/cell]
[cell]*[/cell][cell]ancho * alto[/cell][cell]Multiplicación[/cell]
[cell]/[/cell][cell]total / 4[/cell][cell]División entera[/cell]
[cell]**[/cell][cell]2 ** 8[/cell][cell]Potencia[/cell]
[cell]%[/cell][cell]contador % 3[/cell][cell]Módulo (resto)[/cell]
[cell]√[/cell][cell]2√4[/cell][cell]Raíz enésima[/cell]
[/table]

[color=#3498DB][b]Ejemplos complejos:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]area[/color] [color=#F92672]<-[/color] ([color=#F8F8F2]ancho[/color] [color=#F92672]+[/color] [color=#AE81FF]5[/color]) [color=#F92672]*[/color] ([color=#F8F8F2]alto[/color] [color=#F92672]-[/color] [color=#AE81FF]3[/color])[/cell]
[cell][color=#F8F8F2]Radio[/color] [color=#F92672]<-[/color] ([color=#A6E22E]GetCanvasSize[/color]() [color=#F92672]/[/color] [color=#AE81FF]2[/color]) [color=#F92672]**[/color] [color=#AE81FF]2[/color]        [/cell][/table][/code][/bgcolor]

[color=#E74C3C][b]Precedencia:[/b] Paréntesis > Potencia/Raíz > Multiplicación/División > Suma/Resta[/color][/left]

[font_size=28]🔘 Expresiones Booleanas[/font_size]
[left][b]Operadores de comparación:[/b]
[table=3,baseline,baseline,0]
[cell][b]Operador[/b][/cell][cell][b]Ejemplo[/b][/cell][cell][b]Descripción[/b][/cell]
[cell]==[/cell][cell]x == 5[/cell][cell]Igualdad[/cell]
[cell]!=[/cell][cell]color != 0[/cell][cell]Desigualdad[/cell]
[cell]>[/cell][cell]posX > limite[/cell][cell]Mayor que[/cell]
[cell]<[/cell][cell]contador < 10[/cell][cell]Menor que[/cell]
[cell]>=[/cell][cell]y >= GetActualY()[/cell][cell]Mayor o igual[/cell]
[cell]<=[/cell][cell]ancho <= 100[/cell][cell]Menor o igual[/cell]
[/table]

[b]Operadores lógicos:[/b]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]cond1[/color] [color=#F92672]&&[/color] [color=#F8F8F2]cond2[/color]   [color=#75715E]// AND (y)[/color][/cell]
[cell][color=#F8F8F2]Cond1[/color] [color=#F92672]||[/color] [color=#F8F8F2]cond2[/color]   [color=#75715E]// OR (o)[/color][/cell]
[cell][color=#F92672]![/color][color=#F8F8F2]cond[/color]            [color=#75715E]// NOT (no)[/color]        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Ejemplos prácticos:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]dentro_limites[/color] [color=#F92672]<-[/color] ([color=#F8F8F2]x[/color] [color=#F92672]>[/color] [color=#AE81FF]0[/color] [color=#F92672]&&[/color] [color=#F8F8F2]x[/color] [color=#F92672]<[/color] [color=#A6E22E]GetCanvasSize[/color]())[/cell]
[cell][color=#F8F8F2]Puede_dibujar[/color] [color=#F92672]<-[/color] ([color=#F8F8F2]color_actual[/color] [color=#F92672]==[/color] [color=#E6DB74]""Red""[/color] [color=#F92672]||[/color] [color=#F8F8F2]color_actual[/color] [color=#F92672]==[/color] [color=#E6DB74]""Blue""[/color])[/cell]
[cell][color=#F8F8F2]No_es_borrador[/color] [color=#F92672]<-[/color] [color=#F92672]![/color]([color=#F8F8F2]color_actual[/color] [color=#F92672]==[/color] [color=#E6DB74]""White""[/color])        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔄 Expresiones con Funciones[/font_size]
[left]Las funciones pueden usarse dentro de expresiones:
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]diagonal[/color] [color=#F92672]<-[/color] [color=#AE81FF]2[/color][color=#F92672]√[/color]([color=#A6E22E]GetActualX[/color]()[color=#F92672]**[/color][color=#AE81FF]2[/color] [color=#F92672]+[/color] [color=#A6E22E]GetActualY[/color]()[color=#F92672]**[/color][color=#AE81FF]2[/color])[/cell]
[cell][color=#F8F8F2]Es_centro[/color] [color=#F92672]<-[/color] ([color=#A6E22E]GetActualX[/color]() [color=#F92672]==[/color] [color=#A6E22E]GetCanvasSize[/color]()[color=#F92672]/[/color][color=#AE81FF]2[/color] [color=#F92672]&&[/color] [color=#A6E22E]GetActualY[/color]() [color=#F92672]==[/color] [color=#A6E22E]GetCanvasSize[/color]()[color=#F92672]/[/color][color=#AE81FF]2[/color])        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Combinando todo:[/b][/color]
[bgcolor=#272822][code][table=1][cell][color=#F8F8F2]area_valida[/color] [color=#F92672]<-[/color] ( ([color=#F8F8F2]ancho[/color] [color=#F92672]*[/color] [color=#F8F8F2]alto[/color]) [color=#F92672]>[/color] [color=#AE81FF]100[/color] [color=#F92672]&&[/color] ([color=#F8F8F2]ancho[/color] [color=#F92672]<[/color] [color=#A6E22E]GetCanvasSize[/color]()) ) [color=#F92672]||[/color] ([color=#F8F8F2]es_especial[/color] [color=#F92672]==[/color] [color=#AE81FF]1[/color])        [/cell][/table][/code][/bgcolor][/left]

[color=#2ECC71][b]Tip de depuración:[/b] Usa paréntesis para hacer claras las precedencias en expresiones complejas![/color]",
 "[center][b]Documentación: [i]Expresiones[/i][/b][/center]"),
        ["Func"] = (
    @"===== 🛠️ FUNCIONES DEL LENGUAJE =====

[font_size=28]🔧 Funciones Nativas[/font_size]
[left][table=3,baseline,baseline,0]
[cell][b]Función[/b][/cell][cell][b]Ejemplo[/b][/cell][cell][b]Descripción[/b][/cell]
[cell]GetActualX()[/cell][cell]pos <- GetActualX()[/cell][cell]Posición X actual de Wall-E[/cell]
[cell]GetActualY()[/cell][cell]y <- GetActualY()[/cell][cell]Posición Y actual de Wall-E[/cell]
[cell]GetCanvasSize()[/cell][cell]tam <- GetCanvasSize()[/cell][cell]Tamaño del canvas (n×n)[/cell]
[cell]IsBrushColor(c)[/cell][cell]esRojo <- IsBrushColor(“Red”)[/cell][cell]1 si el pincel es color c[/cell]
[cell]IsBrushSize(k)[/cell][cell]esGrueso <- IsBrushSize(5)[/cell][cell]1 si el tamaño es k[/cell]
[cell]GetColorCount(c,x1,y1,x2,y2)[/cell][cell]azules <- GetColorCount(“Blue”,0,0,10,10)[/cell][cell]Cuenta píxeles de color c en área[/cell]
[cell]IsCanvasColor(c,v,h)[/cell][cell]hayRojo <- IsCanvasColor(“Red”,1,0)[/cell][cell]Verifica color en posición relativa[/cell]
[/table][/left]

[font_size=28]📝 Declaración de Funciones[/font_size]
[left][b]Sintaxis:[/b]
[bgcolor=#272822][code][table=1][cell]func nombre(param1, param2)[/cell]
[cell]{[/cell]
[cell]    // Cuerpo[/cell]
[cell]    Return expresión[/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[b]Características:[/b]
• Pueden recibir parámetros numéricos y/o booleanos
• Deben retornar un valor (número o booleano)

[color=#3498DB][b]Ejemplo completo:[/b][/color]
[bgcolor=#272822][code][table=1][cell]func distancia(x1, y1, x2, y2)[/cell]
[cell]{[/cell]
[cell]    Dx <- x2 – x1[/cell]
[cell]    Dy <- y2 – y1[/cell]
[cell]    Return 2√(dx**2 + dy**2)[/cell]
[cell]}[/cell]
[cell]D <- distancia(0, 0, GetActualX(), GetActualY())        [/cell][/table][/code][/bgcolor][/left]

[color=#2ECC71][b]Consejo avanzado:[/b] Agrupa funciones relacionadas en tu código para mejor organización![/color]"
    , "[center][b]Documentación: [i]Funciones[/i][/b][/center]"),
    ["GoTo"] =(@"===== 🔀 SALTOS CONDICIONALES =====

[font_size=28]🏷️ Etiquetas[/font_size]
[left][b]Sintaxis:[/b]
[bgcolor=#272822][code][table=1][cell]nombre_etiqueta        [/cell][/table][/code][/bgcolor]

[b]Reglas:[/b]
• Debe comenzar con letra
• Puede contener letras, números y _
• Ejemplos válidos:
  [bgcolor=#272822][code][table=1][cell]inicio[/cell]
  [cell]Bucle_principal[/cell]
  [cell]Dibujar_cuadrado        [/cell][/table][/code][/bgcolor]
• Ejemplos inválidos:
  [bgcolor=#272822][code][table=1][cell]1etiqueta[/cell]
  [cell]-inicio        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Uso típico:[/b][/color]
[bgcolor=#272822][code][table=1][cell]inicio_programa[/cell]
[cell]  // Código aquí[/cell]
[cell]Fin_programa        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]↗️ GoTo Condicional[/font_size]
[left][b]Sintaxis completa:[/b]
[bgcolor=#272822][code][table=1][cell]GoTo [etiqueta] (condición)        [/cell][/table][/code][/bgcolor]

[b]Comportamiento:[/b]
• Si condición es true, salta a la etiqueta
• Si condición es false, continúa con la siguiente línea

[color=#3498DB][b]Ejemplo básico:[/b][/color]
[bgcolor=#272822][code][table=1][cell]contador <- 0[/cell]
[cell][/cell]
[cell]Bucle[/cell]
[cell]  [color=#A6E22E]DrawLine[/color](1, 0, 1)[/cell]
[cell]  Contador <- contador + 1[/cell]
[cell]  GoTo [bucle] (contador < 10)        [/cell][/table][/code][/bgcolor][/left]

[color=#E74C3C][b]Importante:[/b] La etiqueta debe existir en el código[/color]

[font_size=28]🔄 Patrones Comunes[/font_size]
[left][color=#3498DB][b]1. Bucles:[/b][/color]
[bgcolor=#272822][code][table=1][cell]i <- 0[/cell]
[cell]Inicio_bucle[/cell]
[cell]  // Código a repetir[/cell]
[cell]  I <- i + 1[/cell]
[cell]  GoTo [inicio_bucle] (i < 5)        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]2. Condicionales:[/b][/color]
[bgcolor=#272822][code][table=1][cell]  // If-Then[/cell]
[cell]  GoTo [si_es_rojo] (color == “Red”)[/cell]
[cell]  // Else implícito[/cell]
[cell]  GoTo [fin_if][/cell]
[cell]Si_es_rojo[/cell]
[cell]  // Bloque Then[/cell]
[cell]Fin_if        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]3. Menú de opciones:[/b][/color]
[bgcolor=#272822][code][table=1][cell]GoTo [opcion1] (selección == 1)[/cell]
[/cell]GoTo [opcion2] (selección == 2)[/cell]
[/cell]GoTo [opcion_default] (1)  // Default[/cell]
[cell][/cell]
[cell]Opcion1[/cell]
[cell]  // Código opción 1[/cell]
[cell]  GoTo [fin_menu][/cell]
[cell][/cell]
[cell]Opcion2[/cell]
[cell]  // Código opción 2[/cell]
[cell]  GoTo [fin_menu][/cell]
[cell][/cell]
[cell]Opcion_default[/cell]
[cell]  // Código default[/cell]
[cell]Fin_menu        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]⚠️ Consideraciones[/font_size]
[left][color=#E74C3C][b]1. Errores comunes:[/b][/color]
• Saltar a etiqueta inexistente: Salta un error en tiempo de ejecución
• Bucles infinitos (falta actualizar condición): Salta un error en tiempo de ejecución
• Condiciones mal formuladas

[color=#3498DB][b]2. Buenas prácticas:[/b][/color]
• Usa nombres descriptivos en etiquetas
• Comenta los saltos complejos
• Evita saltos excesivos (puede hacer código difícil de leer)[/left]

[color=#2ECC71][b]Tip avanzado:[/b] Combina con funciones para estructurar mejor tu código![/color]",
"[center][b]Documentación:[i]Saltos Condicionales[/i][/b][/center]"),
["Additionals"]=(@"===== 🏗️ ESTRUCTURAS DE CONTROL =====

[font_size=28]🔄 If-Then-Else[/font_size]
[left][b]Sintaxis básica:[/b]
[bgcolor=#272822][code][table=1][cell]if (condición)[/cell]
[cell]{[/cell]
[cell]    // Bloque if[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    // Bloque else[/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Ejemplo práctico:[/b][/color]
[bgcolor=#272822][code][table=1][cell]if (GetActualX() > 50)[/cell]
[cell]{[/cell]
[cell]    Color(“Red”)[/cell]
[cell]    [color=#A6E22E]DrawLine[/color](1, 0, 10)[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    Color(“Blue”)[/cell]
[cell]    [color=#A6E22E]DrawCircle[/color](1, 1, 5)   [/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[b]Con múltiples condiciones:[/b]
[bgcolor=#272822][code][table=1][cell]if (x > 100)[/cell]
[cell]{[/cell]
[cell]    // Bloque 1[/cell]
[cell]}[/cell]
[cell]else if (x > 50) [/cell]
[cell]{[/cell]
[cell]    // Bloque 2[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    // Bloque else[/cell]
[cell]}        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔄 While Loop[/font_size]
[left][b]Sintaxis:[/b]
[bgcolor=#272822][code][table=1][cell]while (condición)[/cell]
[cell]{[/cell]
[cell]    // Código a repetir[/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Ejemplo:[/b][/color]
[bgcolor=#272822][code][table=1][cell]contador <- 0
while (contador < 5)[/cell]
[cell]{[/cell]
[cell]    [color=#A6E22E]DrawLine[/color](1, 0, 10)[/cell]
[cell]    Contador <- contador + 1    [/cell]
[cell]}        [/cell][/table][/code][/bgcolor][/left]

[color=#E74C3C][b]Precaución:[/b] Asegurar que la condición pueda volverse falsa[/color]

[font_size=28]🔄 For Loop[/font_size]
[left][b]Sintaxis:[/b]
[bgcolor=#272822][code][table=1][cell]for(inicializador, condición, incremento)[/cell]
[cell]{[/cell]
[cell]    // Código a repetir        [/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Ejemplos:[/b][/color]
[bgcolor=#272822][code][table=1][cell]// Contar hacia adelante[/cell]
[cell]for (i <- 1, i < 10, i <- i + 1)[/cell]
[cell]{[/cell]
[cell]    Size(i)[/cell]
[cell]    [color=#A6E22E]DrawLine[/color](1, 0, 5)[/cell]
[cell]}[/cell]
[cell][/cell]
[cell]// Contar hacia atrás[/cell]
[cell]for (j <- 10, j >= 1, j <- j - 1)[/cell]
[cell]{[/cell]
[cell]    [color=#A6E22E]DrawCircle[/color](1, 1, j)[/cell]
[cell]}        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]🔄 Switch-Case[/font_size]
[left][b]Implementación con if-else if:[/b]
[bgcolor=#272822][code][table=1][cell]if (opcion == 1)[/cell]
[cell]{[/cell]
[cell]    // Caso 1[/cell]
[cell]}[/cell]
[cell]else if (opcion == 2)[/cell]
[cell]{[/cell]
[cell]    // Caso 2[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    // Caso default[/cell]
[cell]}        [/cell][/table][/code][/bgcolor]

[color=#3498DB][b]Ejemplo con colores:[/b][/color]
[bgcolor=#272822][code][table=1][cell]if (color_seleccionado == “Red”)[/cell]
[cell]{[/cell]
[cell]    Color(“Red”)[/cell]
[cell]}[/cell]
[cell]else if (color_seleccionado == “Blue”)[/cell]
[cell]{[/cell]
[cell]    Color(“Blue”)[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    Color(“Black”)[/cell]
[cell]}        [/cell][/table][/code][/bgcolor][/left]

[font_size=28]⚠️ Mejores Prácticas[/font_size]
[left]🔹 [b]Indentación:[/b] Mantener código bien alineado
🔹 [b]Condiciones claras:[/b] Usar paréntesis para complejas
🔹 [b]Evitar anidamiento profundo:[/b] Máximo 3 niveles
🔹 [b]Comentarios:[/b] Explicar lógica compleja

[color=#3498DB][b]Ejemplo bien estructurado:[/b][/color]
[bgcolor=#272822][code][table=1][cell]// Dibuja patrones según posición[/cell]
[cell]if (GetActualX() < 50)[/cell]
[cell]{[/cell]
[cell]    for (i <- 1, i < 5, 1 <- 1)[/cell]
[cell]    {[/cell]
[cell]        [color=#A6E22E]DrawLine[/color](1, 0, i*2)[/cell]
[cell]    }[/cell]
[cell]}[/cell]
[cell]else[/cell]
[cell]{[/cell]
[cell]    while (GetActualY() < 100)[/cell]
[cell]    {    [/cell]
[cell]        [color=#A6E22E]DrawCircle[/color](1, 1, 3)[/cell]
[cell]    }[/cell]
[cell]}        [/cell][/table][/code][/bgcolor][/left]

[color=#2ECC71][b]Consejo Pro:[/b] Usa estas estructuras para crear patrones complejos y algoritmos de dibujo avanzados![/color]",
"[center][b]Documentación:[i]Estructuras de Control[/i][/b][/center]")
    };
    RichTextLabel Title;
    RichTextLabel Document;
    public override void _Ready()
    {
        Title = GetNode<Panel>("Panel").GetNode<RichTextLabel>("Title");
        Document = GetNode<Panel>("Panel").GetNode<RichTextLabel>("Information");
        Title.Text = _displayText["Welcome"].title;
        Document.Text = _displayText["Welcome"].text;
    }
    public void _on_back_btn_pressed()
    {
        GetTree().ChangeSceneToPacked((PackedScene)ResourceLoader.Load("res://Resources/Scenes/MainMenu.tscn"));
    }
    private void _on_welcome_btn_pressed()
    {
        Title.Text = _displayText["Welcome"].title;
        Document.Text = _displayText["Welcome"].text;
    }
    private void _on_instructions_btn_pressed()
    {
        Title.Text = _displayText["Instructions"].title;
        Document.Text = _displayText["Instructions"].text;
        
    }
    private void _on_variables_btn_pressed()
    {
        Title.Text = _displayText["Variables"].title;
        Document.Text = _displayText["Variables"].text;
    }
    private void _on_expressions_btn_pressed()
    {
        Title.Text = _displayText["Expr"].title;
        Document.Text = _displayText["Expr"].text;
    }
    private void _on_function_btn_pressed()
    {
        Title.Text = _displayText["Func"].title;
        Document.Text = _displayText["Func"].text;
    }
    private void _on_conditional_jumps_btn_pressed()
    {
        Title.Text = _displayText["GoTo"].title;
        Document.Text = _displayText["GoTo"].text;
    }
    private void _on_control_statements_btn_pressed()
    {
        Title.Text = _displayText["Additionals"].title;
        Document.Text = _displayText["Additionals"].text;
    }
}
