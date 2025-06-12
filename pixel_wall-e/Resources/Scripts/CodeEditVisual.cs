using Godot;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
public partial class CodeEditVisual : CodeEdit
{
    CodeHighlighter highlighter = new CodeHighlighter();
    private PanelContainer tooltip;
    private RichTextLabel richLabel;
    private ScrollContainer scroll;
    private MarginContainer margin;
    private List<Stmt> statements = new List<Stmt>();

    #region AutoComplete Dictionaries
    private Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)> _colorsData = new()
    {
        ["Red"] = (new Color("#AE81FF"), "[b]\"Red\"[/b]\nColor rojo predefinido", "\"Red\"", CodeCompletionKind.Constant),
        ["Blue"] = (new Color("#AE81FF"), "[b]\"Blue\"[/b]\nColor azul predefinido", "\"Blue\"", CodeCompletionKind.Constant),
        ["Green"] = (new Color("#AE81FF"), "[b]\"Green\"[/b]\nColor verde predefinido", "\"Green\"", CodeCompletionKind.Constant),
        ["Yellow"] = (new Color("#AE81FF"), "[b]\"Yellow\"[/b]\nColor amarillo predefinido", "\"Yellow\"", CodeCompletionKind.Constant),
        ["Orange"] = (new Color("#AE81FF"), "[b]\"Orange\"[/b]\nColor naranja predefinido", "\"Orange\"", CodeCompletionKind.Constant),
        ["Purple"] = (new Color("#AE81FF"), "[b]\"Purple\"[/b]\nColor púrpura predefinido", "\"Purple\"", CodeCompletionKind.Constant),
        ["Black"] = (new Color("#AE81FF"), "[b]\"Black\"[/b]\nColor negro predefinido", "\"Black\"", CodeCompletionKind.Constant),
        ["White"] = (new Color("#AE81FF"), "[b]\"White\"[/b]\nColor blanco predefinido", "\"White\"", CodeCompletionKind.Constant),
        ["Transparent"] = (new Color("#AE81FF"), "[b]\"Transparent\"[/b]\nColor transparente predefinido", "\"Transparent\"", CodeCompletionKind.Constant)
    };
    private Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)> _varData = new Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)>();
    private Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)> _lblData = new Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)>();
    private Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)> _completionData;
    #endregion
    public override void _Ready()
    {
        ConfigureSyntaxHighlighting();
        ConfigureEditorSettings();
        CaretChanged += OnRequestCodeCompletion;
        TextChanged += Compile;
    }
    public void AddLabelData(string token)
    {
        _lblData[token] = (new Color("#AE81FF"), "Etiqueta local", token, CodeCompletionKind.Variable);
    }
    public void AddVariableData(string token)
    {
        _varData[token] = (new Color("#AE81FF"), "Variable local", token, CodeCompletionKind.Variable);

    }
    public void AddFunctionData(string token, List<Token> arguments)
    {
        string args = "(";
        for (int i = 0; i < arguments.Count; i++)
        {
            if (i == arguments.Count - 1)
            {
                args += arguments[i].Lexeme;
            }
            else
            {
                args += $"{arguments[i].Lexeme},";
            }
        }
        args += ")";
        if (!_completionData.ContainsKey(token))
        {
            _completionData[token] = (new Color("#66D9EF"), $"[b]{token}[/b]{args} \n Función local", token + args, CodeCompletionKind.Function);
            return;
        }
    }
    private void DefineDictionary()
    {
        _completionData = new Dictionary<string, (Color color, string tooltip, string autocomplete, CodeCompletionKind kind)>()
        {
            //Completion data
            ["Spawn"] = (new Color("#A6E22E"), "[b]Spawn[/b]([color=#F92672]int[/color] x, [color=#F92672]int[/color] y)\n" +
        "Inicializa a Wall-E sobre el canvas. Las entradas x, y representan las coordenadas iniciales.\n\n" +
        "[color=#75715E]Ejemplo: Spawn(0,0)[/color]", "Spawn(x, y)", CodeCompletionKind.Function),
            ["Color"] = (new Color("#A6E22E"), "[b]Color[/b]([color=#66D9EF]string[/color] color)\n" +
        "Cambia el color del pincel.\n\n" +
        "[color=#FD971F]Opciones: \"Red\", \"Blue\", \"Green\", \"Yellow\", \"Orange\", \"Purple\", " +
         "\"Black\", \"White\", \"Transparent\", código HEX[/color]\n" +
        "[color=#75715E]Ejemplo: Color(\"Red\")[/color]", "Color(color)", CodeCompletionKind.Function),
            ["Size"] = (new Color("#A6E22E"), "[b]Size[/b]([color=#F92672]int[/color] k)\n" +
        "Modifica el tamaño del pincel (números impares).\n\n" +
        "[color=#75715E]Ejemplo: Size(3)[/color]", "Size(k)", CodeCompletionKind.Function),
            ["DrawLine"] = (new Color("#A6E22E"), "[b]DrawLine[/b]([color=#F92672]int[/color] dirX, [color=#F92672]int[/color] dirY" +
        "[color=#F92672]int[/color] distance)\n Dibuja una línea en el canvas.\n\n" +
        "[color=#AE81FF]Direcciones:(-1,-1) Diagonal arriba izquierda, (-1, 0) Izquierda, " +
        "(-1, 1) Diagonal abajo izquierda, (0, 1) Abajo, (1, 1) Diagonal abajo derecha, " +
        "(1, 0) Derecha, (1, -1) Diagonal arriba derecha, (0, -1)Arriba.[/color]", "DrawLine(dirX, dirY, distance)", CodeCompletionKind.Function),
            ["DrawCircle"] = (new Color("#A6E22E"), "[b]DrawCircle[/b]([color=#F92672]int[/color] dirX, [color=#F92672]int[/color] dirY, [color=#F92672]int[/color] radius)\n" +
        "Dibuja un círculo en el canvas de radio [i]radius[/i] en la dirección establecida.\n\n" +
        "[color=#AE81FF]Direcciones:(-1,-1) Diagonal arriba izquierda, (-1, 0) Izquierda, (-1, 1) Diagonal abajo izquierda, (0, 1) Abajo, (1, 1) Diagonal abajo derecha, (1, 0) Derecha, (1, -1) Diagonal arriba derecha, (0, -1)Arriba.[/color]", "DrawCircle(dirX, dirY, radius)", CodeCompletionKind.Function),
            ["DrawRectangle"] = (new Color("#A6E22E"), "[b]DrawRectangle[/b]([color=#F92672]int[/color] dirX, [color=#F92672]int[/color] dirY, [color=#F92672]int[/color] distance, [color=#F92672]int[/color] width, [color=#F92672]int[/color] height)\n" +
        "Dibuja un rectángulo de ancho [i]width[/i] y largo [i]height[/i] en la dirección establecida a una distancia [i]distance[/i].\n\n" +
        "[color=#AE81FF]Direcciones:(-1,-1) Diagonal arriba izquierda, (-1, 0) Izquierda, (-1, 1) Diagonal abajo izquierda, (0, 1) Abajo, (1, 1) Diagonal abajo derecha, (1, 0) Derecha, (1, -1) Diagonal arriba derecha, (0, -1)Arriba.[/color]", "DrawRectangle(dirX, dirY, distance, width, height)", CodeCompletionKind.Function),
            ["Fill"] = (new Color("#A6E22E"), "[b]Fill[/b]()\n Pinta con el color de brocha actual todos los píxeles del color de la posición actual que son alcanzables sin tener que caminar sobre algún otro color.", "Fill()", CodeCompletionKind.Function),
            ["GetActualX"] = (new Color("#66D9EF"), "[b]GetActualX[/b]()\n Retorna el valor X de la posición actual de Wall-E.", "GetActualX()", CodeCompletionKind.Function),
            ["GetActualY"] = (new Color("#66D9EF"), "[b]GetActualY[/b]()\n Retorna el valor Y de la posición actual de Wall-E.", "GetActualY()", CodeCompletionKind.Function),
            ["GetCanvasSize"] = (new Color("#66D9EF"), "[b]GetCanvasSize[/b]()\n Retorna tamaño largo y ancho del canvas. Para un canvas de n x n se retorna n.", "GetCanvasSize()", CodeCompletionKind.Function),
            ["GetColorCount"] = (new Color("#66D9EF"), "[b]GetColorCount[/b]([color=#66D9EF]string[/color] color, [color=#F92672]int[/color] x1, [color=#F92672]int[/color] y1, [color=#F92672]int[/color]x2, [color=#F92672]int[/color]y2)" +
        "Retorna la cantidad de casillas con color [i]color[/i] que hay en el rectángulo formado por las posiciones [i]x1[/i], [i]y1[/i] y [i]x2[/i], [i]y2[/i]. Si cualquiera de las esquinas cae fuera de las dimensiones del canvas, retorna 0.", "GetColorCount(color, x1, x2, y1, y2)", CodeCompletionKind.Function),
            ["IsBrushColor"] = (new Color("#66D9EF"), "[b]IsBrushColor[/b]([color=#66D9EF]string[/color] color)\n Retorna 1 si el color de la brocha actual es [i]color[/i], 0 en caso contrario.", "IsBrushColor(color)", CodeCompletionKind.Function),
            ["IsBrushSize"] = (new Color("#66D9EF"), "[b]IsBrushSize[/b]([color=#F92672]int[/color] size)\n" +
        "Retorna 1 si el tamaño de la brocha actual es [i]size[/i], 0 en caso contrario", "IsBrushSize(size)", CodeCompletionKind.Function),
            ["IsCanvasColor"] = (new Color("#66D9EF"), "[b]IsBrushColor[/b]([color=#66D9EF]string[/color] color, [color=#F92672]int[/color] vertical, [color=#F92672]int[/color] horizontal)\n" +
        "Retorna 1 si la casilla señalada está pintada del parámetro [i]color[/i], 0 en caso contrario. La casilla en cuestión se determina por la posición actual de Wall-E (X,Y) y se calcula como: (X + horizontal, Y + vertical). Si cae fuera del canvas retorna 0.",
        "IsCanvasColor(color, vertical, horizontal)", CodeCompletionKind.Function),
            ["ConvertToHex"] = (new Color("#66D9EF"), "[b]ConvertToHex[/b]([color=#F92672]int[/color] R, [color=#F92672]int[/color] G, [color=#F92672]int[/color] B)\n" +
        "Convierte de RGB a código Hex y devuelve el string correspondiente. Las entradas R, G y B son enteros entre 0 y 255, si se introduce un número menor que 0 la función lo convierte a 0, y si es mayor que 255 lo transforma a 255",
        "ConvertToHex(R, G, B)", CodeCompletionKind.Function),
            ["if"] = (new Color("#F92672"), "[b]if[/b]([color=#AE81FF]condition[/color])\nEjecuta el bloque si es verdadero\n\n" +
        "[color=#75715E]Ejemplo: if(x<10)[/color]", "if(condition)\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["else if"] = (new Color("#F92672"), "[b]else if[/b]([color=#AE81FF]condition[/color])\nCondicional secundario", "else if(condition)\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["else"] = (new Color("#F92672"), "[b]else[/b]\nBloque ejecutado si no se cumple if/elif", "else\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["while"] = (new Color("#F92672"), "[b]while[/b]([color=#AE81FF]condition[/color])\nBucle mientras sea verdadero", "while(true)\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["for"] = (new Color("#F92672"), "[b]for[/b]([color=#AE81FF]inicio[/color], [color=#AE81FF]condition[/color], [color=#AE81FF]incremento[/color])\nBucle con contador", "for(i<-0,i<Length,i<-i+1)\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["forr"] = (new Color("#F92672"), "[b]for[/b]([color=#AE81FF]inicio[/color], [color=#AE81FF]condition[/color], [color=#AE81FF]incremento[/color])\nBucle con contador", "for(i<-Length,i>=0,i<-i-1)\n{\n//Put your code here\n}", CodeCompletionKind.PlainText),
            ["GoTo"] = (new Color("#F92672"), "[b]GoTo[/b][[color=#AE81FF]Label[/color]]([color=#AE81FF]condition[/color])\nSi la condición es verdadera, el código continúa su ejecución en la línea de la etiqueta correspondiente", "GoTo[Label](x<10)", CodeCompletionKind.PlainText),
            ["true"] = (new Color("#AE81FF"), "[b]true[/b]\nValor booleano verdadero", "true", CodeCompletionKind.Constant),
            ["false"] = (new Color("#AE81FF"), "[b]false[/b]\nValor booleano falso", "false", CodeCompletionKind.Constant),
            ["NUMBER"] = (new Color("#AE81FF"), "[b]NUMBER[/b]\nEsta variable o función es un entero", "NUMBER", CodeCompletionKind.Constant),
            ["VOID"] = (new Color("#AE81FF"), "[b]false[/b]\nEl retorno de esta función es vacío", "VOID", CodeCompletionKind.Constant),
            ["BOOL"] = (new Color("#AE81FF"), "[b]false[/b]\nEsta variable o función es booleana", "BOOL", CodeCompletionKind.Constant)
        };
        _varData.Clear();
        _lblData.Clear();
        foreach (Stmt item in statements)
        {
            if (item is FunctionStmt function)
            {
                AddFunctionData(function.Name.Lexeme, function.Parameters);
            }
            else if (item is VariableStmt variable) AddVariableData(variable.Name.Lexeme);
            else if (item is LabelStmt lbl) AddLabelData(lbl.Name.Lexeme);
        }
    }
    private void OnRequestCodeCompletion()
    {
        CancelCodeCompletion();
        DefineDictionary();
        string text = Text;
        int cursorLine = GetCaretLine();
        int cursorColumn = GetCaretColumn();
        string currentWord = GetCurrentWord(text, cursorLine, cursorColumn);
        List<string> metadata = new List<string>();

        foreach (var entry in _colorsData)
        {
            if (currentWord.Length > 0 && entry.Key.StartsWith(currentWord.Substring(1, Math.Clamp(currentWord.Length - 1, 0, currentWord.Length - 1))) && currentWord[0] == '"')
            {
                metadata.Add(entry.Key);
                AddCodeCompletionOption(entry.Value.kind, entry.Key, entry.Value.autocomplete, entry.Value.color);
            }
        }
        if (metadata.Count == 0 || currentWord == "")
        {

            foreach (var entry in _completionData)
            {
                if (entry.Key.StartsWith(currentWord))
                {
                    metadata.Add(entry.Key);
                    AddCodeCompletionOption(entry.Value.kind, entry.Key, entry.Value.autocomplete, entry.Value.color);
                }
            }
            foreach (var entry in _varData)
            {
                if (entry.Key.StartsWith(currentWord))
                {
                    metadata.Add(entry.Key);
                    AddCodeCompletionOption(entry.Value.kind, entry.Key, entry.Value.autocomplete, entry.Value.color);
                }
            }
            foreach (var entry in _lblData)
            {
                if (entry.Key.StartsWith(currentWord))
                {
                    metadata.Add(entry.Key);
                    AddCodeCompletionOption(entry.Value.kind, entry.Key, entry.Value.autocomplete, entry.Value.color);
                }
            }
        }
        UpdateCodeCompletionOptions(false);
        if (metadata.Contains(currentWord))
        {
            _MakeCustomTooltip(metadata[metadata.IndexOf(currentWord)]);
            TooltipText = metadata[metadata.IndexOf(currentWord)].ToString();
        }
        else if (GetCodeCompletionSelectedIndex() < metadata.Count && GetCodeCompletionSelectedIndex() >= 0)
        {
            //MakeCustomTooltip makes the tooltip format
            _MakeCustomTooltip(metadata[GetCodeCompletionSelectedIndex()]);
            TooltipText = metadata[GetCodeCompletionSelectedIndex()].ToString();
        }
        else
        {
            _MakeCustomTooltip(" ");
            TooltipText = " ";
        }
    }
    private string GetCurrentWord(string fullText, int line, int column)
    {
        string[] lines = fullText.Split('\n');
        if (line >= lines.Length) return "";
        string lineText = lines[line];
        if (column > lineText.Length) column = lineText.Length;

        int start = column - 1;
        while (start >= 0 && Regex.IsMatch(lineText[start].ToString(), @"^[a-zA-Z0-9""]"))
        {
            start--;
        }
        start++;
        int end = column;
        while (end < lineText.Length && Regex.IsMatch(lineText[end].ToString(), @"^[a-zA-Z0-9]"))
        {
            end++;
        }
        return lineText.Substring(start, end - start);
    }
    private void ConfigureSyntaxHighlighting()
    {
        SyntaxHighlighter = highlighter;
        string[] control = ["GoTo", "while", "for", "if", "else", "func", "return", "NUMBER", "BOOL", "VOID"];
        highlighter.AddKeywordColor("true", new Color("#F92672"));
        highlighter.AddKeywordColor("false", new Color("#F92672"));
        foreach (var item in control)
        {
            highlighter.AddKeywordColor(item, new Color("#F92672"));
        }
        highlighter.SymbolColor = new Color("#F92672");
        highlighter.FunctionColor = new Color("#A6E22E");
        highlighter.MemberVariableColor = new Color("#F8F8F2");
        highlighter.NumberColor = new Color("#AE81FF");
        highlighter.AddColorRegion("//", "", new Color("#75715E"));
        highlighter.AddColorRegion("/*", "*/", new Color("#75715E"));
        highlighter.AddColorRegion("\"", "\"", new Color("#E6DB74"));
    }
    private void ConfigureEditorSettings()
    {
        AutoBraceCompletionEnabled = true;
        HighlightCurrentLine = true;
        DrawTabs = true;
        DrawSpaces = true;
        CodeCompletionEnabled = true;
        CodeCompletionPrefixes = [" ", "("];
    }
    public override GodotObject _MakeCustomTooltip(string forText)
    {
        if (tooltip == null || !IsInstanceValid(tooltip)) InitializeTooltipComponents();
        if (IsInstanceValid(richLabel))
        {
            richLabel.Text = GetTooltipText(forText);
            Callable.From(() => UpdateTooltipSize()).CallDeferred();
        }
        return tooltip;
    }
    private void InitializeTooltipComponents()
    {
        tooltip = new PanelContainer();
        tooltip.CustomMinimumSize = new Vector2(500, 250);

        var style = new StyleBoxFlat
        {
            BgColor = new Color(0.1f, 0.1f, 0.2f),
            ContentMarginBottom = 8,
            ContentMarginLeft = 12,
            ContentMarginRight = 12,
            ContentMarginTop = 8
        };
        tooltip.AddThemeStyleboxOverride("panel", style);

        scroll = new ScrollContainer();
        scroll.SizeFlagsVertical = SizeFlags.ExpandFill;
        scroll.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        tooltip.AddChild(scroll);/*

        margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left",4);
        margin.AddThemeConstantOverride("margin_right",4);
        margin.AddThemeConstantOverride("margin_top",4);
        margin.AddThemeConstantOverride("margin_bottom",4);
        scroll.AddChild(margin);*/

        richLabel = new RichTextLabel
        {
            BbcodeEnabled = true,
            AutowrapMode = TextServer.AutowrapMode.Arbitrary,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
        };
        //margin.AddChild(richLabel);
        scroll.AddChild(richLabel);
    }
    private void UpdateTooltipSize()
    {
        if (IsInstanceValid(richLabel) && IsInstanceValid(tooltip))
        {
            float width = 500;
            float height = richLabel.GetContentHeight() + 30;
            tooltip.CustomMinimumSize = new Vector2(width, height);
        }
    }
    private string GetTooltipText(string key)
    {
        string complete = _completionData is null ? $"[i] No hay información para '{key}'[/i]" : "";
        if (_varData is not null && _varData.ContainsKey(key)) complete = _varData[key].tooltip + "\n";
        if (_lblData is not null && _lblData.ContainsKey(key)) complete += _lblData[key].tooltip + "\n";
        if (_completionData is null) return complete;
        return _completionData.TryGetValue(key, out (Color color, string tooltip, string autocomplete, CodeCompletionKind kind) value) ? complete += value.tooltip : _colorsData.TryGetValue(key, out value) ? complete + value.tooltip : $"[i] No hay información para '{key}'[/i]";
    }
    public void Compile()
    {
        Handler handler = new Handler(this.Text);
        handler.Handle();
        List<CompilerException> exception = handler.GetExceptions();
        HBoxContainer container = GetParent<HBoxContainer>();
        CompilerVisual visual = container.GetParent<CompilerVisual>();
        visual.PrintExceptions(exception);
        statements = handler.GetStmts();
        visual.FillStatements(statements);

    }
}
