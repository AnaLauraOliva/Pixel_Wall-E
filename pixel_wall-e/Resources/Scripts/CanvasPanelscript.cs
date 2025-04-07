using Godot;
using System;

public partial class CanvasPanelscript : Panel
{
    private LineEdit _widthInput;
    private LineEdit _heightInput;
    private Button _applyBtn;
    private TextureRect _gridTextureRect;
    private int _gridWidth = 32;
    private int _gridHeight = 32;
    private readonly Color _gridColor = new Color("#cccccc");
    private readonly int _maxCanvasSize = 512;
    private ImageTexture _gridTexture;
    private Control _xAxisContainer;
    private Control _yAxisContainer;
    public override void _Ready()
    {
        //Obtener controles
        _widthInput = GetNode<LineEdit>("Width");
        _heightInput = GetNode<LineEdit>("Height");
        _applyBtn = GetNode<Button>("ApplyBtn");
        _gridTextureRect = GetNode<TextureRect>("Canvas");
        _xAxisContainer = GetNode<Control>("xAxisContainer");
        _yAxisContainer = GetNode<Control>("yAxisContainer");

        //Añadir los eventos
        _applyBtn.Pressed += OnApplyPressed;
        _widthInput.TextChanged += OnWidthTextChange;
        _heightInput.TextChanged += OnHeightTextChange;

        //Establecer tamaño mínimo del canvas
        _gridTextureRect.CustomMinimumSize = new Vector2(_maxCanvasSize, _maxCanvasSize);

        //Generar el canvas inicial
        GenerateGridTexture();
        
        //Generar los ejes
        UpdateAxisLabels();
    }
    private void OnApplyPressed()
    {
        //verificar que en los LineEdit halla un número y que sea mayor que 0
        if (int.TryParse(_widthInput.Text, out int newWidth) &&
        int.TryParse(_heightInput.Text, out int newHeight) &&
        newWidth > 0 && newHeight > 0)
        {
            _gridWidth = Math.Clamp(newWidth, 1, 256);
            _gridHeight = Math.Clamp(newHeight, 1, 256);
            if (newHeight > 256 || newWidth > 256) return;
            GenerateGridTexture();
            UpdateAxisLabels();
        }
    }
    //Se filtra el texto de los TextLine para que solo se puedan introducir números
    private void OnWidthTextChange(string newText)
    {
        string filtered = Filtered(newText);
        if (filtered != newText) _widthInput.Text = filtered;
    }
    private void OnHeightTextChange(string newText)
    {
        string filtered = Filtered(newText);
        if (filtered != newText) _heightInput.Text = filtered;
    }
    private string Filtered(string newText)
    {
        string filtered = string.Empty;
        foreach (char c in newText)
        {
            if (char.IsDigit(c)) filtered += c;
        }
        return filtered;
    }
    private void GenerateGridTexture()
    {
        //se crea una nueva imagen
        var image = Image.CreateEmpty(_maxCanvasSize, _maxCanvasSize, false, Image.Format.Rgba8);
        //Se pone el fondo blanco
        image.Fill(Colors.White);
        //Se obtiene el tamaño de las cuadrículas
        float spacingWidth = _maxCanvasSize / (float)_gridWidth;
        float spacingHeight = _maxCanvasSize / (float)_gridHeight;
        //lineas horizontales
        for (int i = 0; i < _gridWidth; i++)
        {
            int pos = (int)(i * spacingWidth);
            for (int x = 0; x < _maxCanvasSize; x++)
            {
                image.SetPixel(x, pos, _gridColor);
            }
        }
        //líneas verticales
        for (int i = 0; i < _gridHeight; i++)
        {
            int pos = (int)(i * spacingHeight);
            for (int y = 0; y < _maxCanvasSize; y++)
            {
                image.SetPixel(pos, y, _gridColor);
            }
        }
        //Liberar recursos nativos
        _gridTexture?.Dispose();
        //crear la textura
        _gridTexture = ImageTexture.CreateFromImage(image);
        //aplicar la textura
        _gridTextureRect.Texture = _gridTexture;
    }
    private void UpdateAxisLabels()
    {
        //Limpiamos los ejes
        foreach (Node child in _xAxisContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Node child in _yAxisContainer.GetChildren())
        {
            child.QueueFree();
        }
        //obtenemos tamaño de las celdas
        float cellWidth = _maxCanvasSize / (float)_gridWidth;
        float cellHeight = _maxCanvasSize / (float)_gridHeight;
        //Se crean los ejes coordenados
        if (cellHeight > 7)
        {
            for (int x = 0; x < _gridHeight; x++)
            {
                Label label = new Label();
                label.Text = x.ToString();
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.Position = new Vector2(x * cellHeight, 0);
                if ((int)cellHeight < 11)
                    label.AddThemeFontSizeOverride("font_size", (int)cellHeight);
                else
                    label.AddThemeFontSizeOverride("font_size", 11);
                label.AddThemeColorOverride("font_color", new Color(Colors.Black));
                _xAxisContainer.AddChild(label);
            }
        }

        if (cellWidth > 7)
        {
            for (int y = 0; y < _gridWidth; y++)
            {
                Label label = new Label();
                label.Text = y.ToString();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.Position = new Vector2(0, y * cellWidth);
                label.AddThemeColorOverride("font_color", new Color(Colors.Black));
                if ((int)cellWidth < 11)
                    label.AddThemeFontSizeOverride("font_size", (int)cellWidth);
                else
                    label.AddThemeFontSizeOverride("font_size", 11);
                _yAxisContainer.AddChild(label);
            }
        }
        //Se comprueba si el tamaño de la celda es mayor que 7 ya que si es menor la letra sería muy pequeña y a penas se vería
        //Por tanto sería ilógico colocar los ejes. Con esta configuración se muestran los números de los ejes con los tamaños
        //del 1 al 73
    }
}
