using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using CargaImagenes.Core;
using CargaImagenes.Data;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Color = System.Drawing.Color;
using IOFileStream = System.IO.FileStream;
using IOPath = System.IO.Path;
using SystemDrawingImage = System.Drawing.Image;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace CargaImagenes.UI
{
    public partial class Form1 : Form
    {
        #region Variables Privadas
        private IDatabaseService _databaseService;
        private readonly AppSettings _appSettings;
        private readonly BindingList<Producto> _productos = new();
        private readonly List<Producto> _productosTodos = new();
        private readonly string _tempImagePath;
        private readonly HashSet<int> _productosSeleccionados = new();
        private readonly List<int> _proveedoresFiltro = new();
        private readonly List<int> _categoriasFiltro = new();
        private readonly List<int> _departamentosFiltro = new();
        private bool _filtrarConImagen;
        private bool _filtrarSinImagen;
        private bool _filtrarSoloSeleccionados;
        private string _textoBusqueda = string.Empty;
        private int _busquedaVersion = 0;
        private bool _suspendCategoriaEvent;
        private DataTable? _dtProveedores;
        private DataTable? _dtCategorias;
        private DataTable? _dtDepartamentos;
        private Producto? _ultimoProductoSeleccionado;
        private readonly Dictionary<int, int> _orderQuantities = new();
        private Point _dragStartPoint;
        #endregion

        #region Constructor e Inicialización
        public Form1(IDatabaseService databaseService, AppSettings appSettings)
        {
            InitializeComponent();
            lblProductosSeleccionados.BringToFront();
            _databaseService = databaseService;
            _appSettings = appSettings;
            _tempImagePath = _appSettings.TempImagePath;
            pbProducto.PreviewKeyDown += (s, e) => e.IsInputKey = true;
            pbProducto.KeyDown += PbProducto_KeyDown;
            pbProducto.MouseDown += PbProducto_MouseDown;
            pbProducto.MouseMove += PbProducto_MouseMove;
            pbProducto.AllowDrop = true;
            pbProducto.DragEnter += PbProducto_DragEnter;
            pbProducto.DragDrop += PbProducto_DragDrop;
            pbProducto.TabStop = false; // Desactivar TabStop en PictureBox para evitar que robe foco
            Icon = new Icon("iconn.ico");
            ConfigurarControlesResponsivos();
            Directory.CreateDirectory(_tempImagePath);
            Directory.CreateDirectory(_appSettings.DefaultImagePath);
            ConfigurarControlesExistentes();
            ConfigurarDataGridView();
            CargarComboPrecios();
            Load += Form1_Load;
            FormClosing += Form1_FormClosing;
            KeyPreview = true; // Permitir que el formulario capture eventos de teclado globales
            KeyDown += Form1_KeyDown; // Manejar teclas a nivel de formulario
        }

        private async void Form1_Load(object? sender, EventArgs e)
        {
            ConfigurarColumnasDataGridView();
            pbProducto.SizeMode = PictureBoxSizeMode.Zoom;
            var cfg = WindowConfig.Load();
            if (cfg.Width > 0 && cfg.Height > 0)
            {
                Size = new Size(cfg.Width, cfg.Height);
            }
            await CargarDatosInicialesAsync();
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Establecer foco inicial en DataGridView
            UpdateCounters();    // Mostrar contador de productos seleccionados desde el inicio
        }
        #endregion

        #region Configuración de Controles y Eventos
        private void UpdateCounters()
        {
            lblTotalProductos.Text = $"Total de productos: {_productos.Count}";
            lblProductosSeleccionados.Text = $"Productos seleccionados: {_productosSeleccionados.Count}";
        }

        private void DgvProductos_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (dgvProductos.IsCurrentCellDirty)
                dgvProductos.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void ConfigurarControlesResponsivos()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            AutoSize = false;
            txtBuscador.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cboProveedores.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cboCategorias.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cboDepDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            cboPrecios.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chkConImagen.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chkSinImagen.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chkConExistencia.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chkSinExistencia.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            chkSoloSeleccionados.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            BtnLimpiarFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            dgvProductos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlResultados.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlControles.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlDetalleProducto.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            lblTotalProductos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConfigurarColumnasDataGridView();
        }

        private void CargarComboPrecios()
        {
            cboPrecios.Items.Clear();
            cboPrecios.Items.Add("Sin Precio");
            cboPrecios.Items.Add("Regular");
            cboPrecios.Items.Add("PrecioA");
            cboPrecios.Items.Add("PrecioB");
            cboPrecios.Items.Add("PrecioC");
            cboPrecios.Items.Add("Oferta");
            cboPrecios.SelectedIndex = 1;
        }

        private void ConfigurarColumnasDataGridView()
        {
            if (dgvProductos.Columns.Count >= 6)
            {
                dgvProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvProductos.Columns[0].FillWeight = 4;
                dgvProductos.Columns[1].FillWeight = 15;
                dgvProductos.Columns[2].FillWeight = 33;
                dgvProductos.Columns[3].FillWeight = 10;
                dgvProductos.Columns[4].FillWeight = 12;
                dgvProductos.Columns[5].FillWeight = 8;
                dgvProductos.Columns[6].FillWeight = 4;
                dgvProductos.Columns[0].MinimumWidth = 4;
                dgvProductos.Columns[1].MinimumWidth = 15;
                dgvProductos.Columns[2].MinimumWidth = 33;
                dgvProductos.Columns[3].MinimumWidth = 20;
                dgvProductos.Columns[4].MinimumWidth = 20;
                dgvProductos.Columns[5].MinimumWidth = 8;
                dgvProductos.Columns[6].MinimumWidth = 4;
                dgvProductos.BorderStyle = BorderStyle.Fixed3D;
                dgvProductos.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dgvProductos.GridColor = Color.LightGray;
                dgvProductos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dgvProductos.AllowUserToResizeRows = false;
                dgvProductos.RowTemplate.Height = 28;
            }
        }

        private string ObtenerCampoPrecioSeleccionado()
        {
            return cboPrecios.SelectedItem?.ToString() switch
            {
                "Sin Precio" => "0",
                "Regular" => "i.Price",
                "PrecioA" => "i.PriceA",
                "PrecioB" => "i.PriceB",
                "PrecioC" => "i.PriceC",
                "Oferta" => "i.SalePrice",
                _ => "i.Price",
            };
        }

        private Dictionary<int, bool> GuardarEstadoSeleccion()
        {
            var dic = new Dictionary<int, bool>();
            foreach (var p in _productos)
                dic[p.Id] = p.Seleccionado;
            foreach (var p in _productosTodos)
                if (!dic.ContainsKey(p.Id))
                    dic[p.Id] = p.Seleccionado;
            return dic;
        }

        private void RestaurarEstadoSeleccion(Dictionary<int, bool> seleccionados)
        {
            foreach (var producto in _productos)
                if (seleccionados.TryGetValue(producto.Id, out bool seleccionado))
                    producto.Seleccionado = seleccionado;
        }

        private async Task CargarDatosInicialesAsync()
        {
            await CargarProveedoresAsync();
            await CargarCategoriasAsync();
            await CargarDepartamentosAsync();
            await CargarProductosAsync();
        }

        private async void CboDepartamentos_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                _suspendCategoriaEvent = true;
                await ActualizarCategoriasPorDepartamentoAsync();
                _suspendCategoriaEvent = false;
                await FiltrarProductosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar categorías: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task ActualizarCategoriasPorDepartamentoAsync()
        {
            int? depId = cboDepDetalle.SelectedItem is DataRowView row
                ? (int?)row["ID"]
                : null;
            string sql = depId.HasValue
                ? @"SELECT DISTINCT c.ID, c.Name
            FROM Category c
            INNER JOIN Item i ON c.ID = i.CategoryID
            WHERE i.DepartmentID = @DeptID
            ORDER BY c.Name"
                : @"SELECT ID, Name FROM Category ORDER BY Name";
            Dictionary<string, object>? p = depId.HasValue
                ? new() { ["@DeptID"] = depId.Value }
                : null;
            DataTable dtCat = p != null
                ? await _databaseService.ExecuteQueryWithParametersAsync(sql, p)
                : await _databaseService.ExecuteQueryAsync(sql);
            CargarComboDesdeDataTable(cboCategorias, dtCat, "Name", "-Todas las categorías-");
        }

        private void ActualizarCamposDetalle(Producto producto)
        {
            txtCodigo.Text = producto.Codigo;
            txtDescripcion.Text = producto.Descripcion;
            txtPrecio.Text = $"L. {producto.Precio.ToString("F2", CultureInfo.InvariantCulture)}";
            txtPrecioA.Text = $"L. {producto.PrecioA.ToString("F2", CultureInfo.InvariantCulture)}";
            txtPrecioB.Text = $"L. {producto.PrecioB.ToString("F2", CultureInfo.InvariantCulture)}";
            txtPrecioC.Text = $"L. {producto.PrecioC.ToString("F2", CultureInfo.InvariantCulture)}";
            txtCantidad.Text = producto.Cantidad.ToString();
            txtDescripcionAmpliada.Text = producto.DescripcionAmpliada;
            txtSubdescripcion1.Text = producto.Subdescripcion1;
        }

        private void ConfigurarControlesExistentes()
        {
            ConfigurarEventosDataGridView();
            ConfigurarEventosBotones();
            ConfigurarCamposNumericosYTexto();
            ConfigurarCheckboxes();
            txtBuscador.TextChanged += TxtBuscador_TextChanged;
            cboProveedores.SelectedIndexChanged += async (s, e) => await FiltrarProductosAsync();
            cboCategorias.SelectedIndexChanged += async (s, e) => await FiltrarProductosAsync();
            cboPrecios.SelectedIndexChanged += async (s, e) => await FiltrarProductosAsync();
            cboDepDetalle.SelectedIndexChanged += async (s, e) => await FiltrarProductosAsync();
        }

        private async void CboCategorias_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_suspendCategoriaEvent) return;
            try
            {
                await FiltrarProductosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar departamentos: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ConfigurarEventosDataGridView()
        {
            dgvProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvProductos.SelectionChanged += DgvProductos_SelectionChanged;
            dgvProductos.ColumnHeaderMouseClick += DgvProductos_ColumnHeaderMouseClick;
            dgvProductos.CellFormatting += DgvProductos_CellFormatting;
            dgvProductos.CellContentClick += DgvProductos_CellContentClick;
            dgvProductos.KeyDown += DgvProductos_KeyDown; // Manejar teclas para las flechas
        }

        private void ConfigurarEventosBotones()
        {
            BtnLimpiarFiltros.Click += BtnLimpiarFiltros_Click;
            BtnCargarImagen.Click += BtnCargarImagen_Click;
            BtnGenerarPDF.Click += BtnGenerarPDF_Click;
            BtnLimpiarSeleccion.Click += BtnLimpiarSeleccion_Click;
            BtnSeleccionarTodo.Click += BtnSeleccionarTodo_Click;
            BtnGenerarCotizacion.Click += BtnGenerarCotizacion_Click;
        }

        private void ConfigurarCamposNumericosYTexto()
        {
            ConfigurarCampoNumerico(txtPrecio, TxtPrecio_Leave);
            ConfigurarCampoNumerico(txtPrecioA, TxtPrecioA_Leave);
            ConfigurarCampoNumerico(txtPrecioB, TxtPrecioB_Leave);
            ConfigurarCampoNumerico(txtPrecioC, TxtPrecioC_Leave);
            ConfigurarCampoNumerico(txtCantidad, TxtCantidad_Leave);
            ConfigurarCampoTexto(txtDescripcion, TxtDescripcion_Leave);
            ConfigurarCampoTexto(txtDescripcionAmpliada, TxtDescripcionAmpliada_Leave);
            ConfigurarCampoTexto(txtSubdescripcion1, TxtSubdescripcion1_Leave);
            ConfigurarCampoTexto(txtCodigo, TxtCodigo_Leave);
        }

        private void ConfigurarCampoNumerico(TextBox textBox, EventHandler leaveHandler)
        {
            textBox.Leave += leaveHandler;
            textBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    leaveHandler(s, e);
                    SeleccionarSiguienteControl(textBox);
                }
            };
        }

        private void ConfigurarCampoTexto(TextBox textBox, EventHandler leaveHandler)
        {
            textBox.Leave += leaveHandler;
            textBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    leaveHandler(s, e);
                    SeleccionarSiguienteControl(textBox);
                }
            };
        }

        private void SeleccionarSiguienteControl(Control control)
        {
            Control nextControl = GetNextControl(control, true);
            if (nextControl != null)
            {
                nextControl.Focus();
                if (nextControl is TextBox txtBox)
                    txtBox.SelectAll();
            }
        }

        private void ConfigurarCheckboxes()
        {
            chkConImagen.CheckedChanged += async (s, e) =>
            {
                _filtrarConImagen = chkConImagen.Checked;
                await FiltrarProductosAsync();
            };
            chkSinImagen.CheckedChanged += async (s, e) =>
            {
                _filtrarSinImagen = chkSinImagen.Checked;
                await FiltrarProductosAsync();
            };
            chkConExistencia.CheckedChanged += async (s, e) => await FiltrarProductosAsync();
            chkSinExistencia.CheckedChanged += async (s, e) => await FiltrarProductosAsync();
            chkSoloSeleccionados.CheckedChanged += async (s, e) =>
            {
                _filtrarSoloSeleccionados = chkSoloSeleccionados.Checked;
                await FiltrarProductosAsync();
            };
        }

        private async Task FiltrarProductosAsync()
        {
            SaveOrderQuantities();
            ActualizarFiltros();
            await CargarProductosAsync();
            dgvProductos.Refresh();
        }
        #endregion

        #region Manejadores de eventos para TextBox
        private void TxtPrecio_Leave(object? sender, EventArgs e)
        {
            ProcesarCampoPrecio(txtPrecio, "Precio", "precio");
        }

        private void TxtPrecioA_Leave(object? sender, EventArgs e)
        {
            ProcesarCampoPrecio(txtPrecioA, "PrecioA", "precio A");
        }

        private void TxtPrecioB_Leave(object? sender, EventArgs e)
        {
            ProcesarCampoPrecio(txtPrecioB, "PrecioB", "precio B");
        }

        private void TxtPrecioC_Leave(object? sender, EventArgs e)
        {
            ProcesarCampoPrecio(txtPrecioC, "PrecioC", "precio C");
        }

        private void ProcesarCampoPrecio(TextBox textBox, string propName, string nombreCampo)
        {
            var texto = textBox.Text.Trim();
            if (texto.StartsWith("L."))
                texto = texto[2..].Trim();

            if (decimal.TryParse(texto, out decimal precio))
            {
                textBox.Text = $"L. {precio.ToString("F2", CultureInfo.InvariantCulture)}";
                if (dgvProductos.SelectedRows.Count > 0 &&
                    dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
                {
                    typeof(Producto).GetProperty(propName)?.SetValue(producto, precio);
                }
            }
            else
            {
                MessageBox.Show($"Por favor, ingrese un {nombreCampo} válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (dgvProductos.SelectedRows.Count > 0 &&
                    dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
                {
                    decimal valorActual = (decimal)(typeof(Producto).GetProperty(propName)?.GetValue(producto, null) ?? 0m);
                    textBox.Text = $"L. {valorActual.ToString("F2", CultureInfo.InvariantCulture)}";
                }
            }
        }

        private void TxtCantidad_Leave(object? sender, EventArgs e)
        {
            if (int.TryParse(txtCantidad.Text, out int cantidad))
            {
                txtCantidad.Text = cantidad.ToString();
                if (dgvProductos.SelectedRows.Count > 0 &&
                    dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
                {
                    producto.Cantidad = cantidad;
                }
            }
            else
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (dgvProductos.SelectedRows.Count > 0 &&
                    dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
                    txtCantidad.Text = producto.Cantidad.ToString();
            }
        }

        private void ActualizarCampoTexto(TextBox textBox, string propName)
        {
            if (dgvProductos.SelectedRows.Count > 0 &&
                dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
            {
                typeof(Producto).GetProperty(propName)?.SetValue(producto, textBox.Text);
            }
        }

        private void TxtDescripcion_Leave(object? sender, EventArgs e)
        {
            ActualizarCampoTexto(txtDescripcion, "Descripcion");
        }

        private void TxtDescripcionAmpliada_Leave(object? sender, EventArgs e)
        {
            ActualizarCampoTexto(txtDescripcionAmpliada, "DescripcionAmpliada");
        }

        private void TxtSubdescripcion1_Leave(object? sender, EventArgs e)
        {
            ActualizarCampoTexto(txtSubdescripcion1, "Subdescripcion1");
        }

        private void TxtCodigo_Leave(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0 &&
                dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
            {
                string nuevoCodigo = txtCodigo.Text.Trim();
                producto.Codigo = nuevoCodigo;
            }
        }
        #endregion

        #region Eventos del DataGridView
        private void DgvProductos_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvProductos.Columns.Count)
                    return;
                string propertyName = dgvProductos.Columns[e.ColumnIndex].DataPropertyName;
                List<Producto> productosOrdenados = new(_productos);
                foreach (DataGridViewColumn col in dgvProductos.Columns)
                    col.HeaderCell.SortGlyphDirection = SortOrder.None;
                ListSortDirection direction;
                DataGridViewColumn clickedColumn = dgvProductos.Columns[e.ColumnIndex];
                if (clickedColumn.Tag == null || (SortOrder)clickedColumn.Tag == SortOrder.Descending)
                {
                    direction = ListSortDirection.Ascending;
                    clickedColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    clickedColumn.Tag = SortOrder.Ascending;
                }
                else
                {
                    direction = ListSortDirection.Descending;
                    clickedColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    clickedColumn.Tag = SortOrder.Descending;
                }
                _productos.RaiseListChangedEvents = false;
                SaveOrderQuantities();
                _productos.Clear();
                PropertyInfo? propInfo = typeof(Producto).GetProperty(propertyName);
                if (propInfo != null)
                {
                    IOrderedEnumerable<Producto> sortedList = (direction == ListSortDirection.Ascending) ?
                        productosOrdenados.OrderBy(p => propInfo.GetValue(p, null) ?? string.Empty) :
                        productosOrdenados.OrderByDescending(p => propInfo.GetValue(p, null) ?? string.Empty);
                    foreach (var item in sortedList)
                        _productos.Add(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ordenar productos: {ex.Message}");
            }
            finally
            {
                _productos.RaiseListChangedEvents = true;
                _productos.ResetBindings();
            }
        }

        private async void DgvProductos_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count > 0 &&
                dgvProductos.SelectedRows[0].DataBoundItem is Producto producto)
            {
                ActualizarCamposDetalle(producto);
                if (producto.TieneImagen && producto.Imagen == null)

                    await CargarImagenProductoAsync(producto);

                    await CargarImagenesProductos(new[] { producto });

                pbProducto.Image = producto.Imagen;
                if (_ultimoProductoSeleccionado != null && _ultimoProductoSeleccionado.Id == producto.Id)
                    return;
                _ultimoProductoSeleccionado = producto;
                // No establecer foco en pbProducto, mantenerlo en DataGridView
            }
            // Evitar que el DataGridView robe el foco mientras se escribe en el buscador
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener foco en DataGridView cuando corresponda
        }

        private void DgvProductos_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                dgvProductos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                if (dgvProductos.Rows[e.RowIndex].DataBoundItem is Producto producto)
                {
                    producto.Seleccionado = !producto.Seleccionado;
                    if (_productosSeleccionados.Contains(producto.Id))
                        _productosSeleccionados.Remove(producto.Id);
                    else
                        _productosSeleccionados.Add(producto.Id);
                    dgvProductos.InvalidateCell(e.ColumnIndex, e.RowIndex);
                    dgvProductos.RefreshEdit();
                    UpdateCounters();
                }
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener el foco en DataGridView después de hacer clic
        }

        private void DgvProductos_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                e.Handled = false; // Permitir navegación por defecto con las flechas
            }
        }
        #endregion

        private void ActualizarFiltros()
        {
            _proveedoresFiltro.Clear();
            _categoriasFiltro.Clear();
            _departamentosFiltro.Clear();
            if (cboProveedores.SelectedIndex > 0 && _dtProveedores != null)
            {
                string proveedorSeleccionado = cboProveedores.SelectedItem.ToString();
                var row = _dtProveedores.AsEnumerable()
                    .FirstOrDefault(r => r["SupplierName"].ToString() == proveedorSeleccionado);
                if (row != null)
                    _proveedoresFiltro.Add(Convert.ToInt32(row["ID"]));
            }
            if (cboCategorias.SelectedIndex > 0 && _dtCategorias != null)
            {
                string categoriaSeleccionada = cboCategorias.SelectedItem.ToString();
                var row = _dtCategorias.AsEnumerable()
                    .FirstOrDefault(r => r["Name"].ToString() == categoriaSeleccionada);
                if (row != null)
                    _categoriasFiltro.Add(Convert.ToInt32(row["ID"]));
            }
            if (cboDepDetalle.SelectedIndex > 0 && _dtDepartamentos != null)
            {
                string departamentoSeleccionado = cboDepDetalle.SelectedItem.ToString();
                var row = _dtDepartamentos.AsEnumerable()
                    .FirstOrDefault(r => r["Name"].ToString() == departamentoSeleccionado);
                if (row != null)
                    _departamentosFiltro.Add(Convert.ToInt32(row["ID"]));
            }
        }

        #region Eventos de Botones
        private void LimpiarSeleccionProductos()
        {
            try
            {
                _productos.RaiseListChangedEvents = false;
                foreach (var producto in _productos)
                    producto.Seleccionado = false;
                _productos.RaiseListChangedEvents = true;
                _productos.ResetBindings();
                dgvProductos.Refresh();
                MessageBox.Show("Se ha limpiado la selección de productos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar selección: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnLimpiarFiltros_Click(object? sender, EventArgs e)
        {
            SaveOrderQuantities();
            if (cboProveedores.Items.Count > 0)
                cboProveedores.SelectedIndex = 0;
            if (cboCategorias.Items.Count > 0)
                cboCategorias.SelectedIndex = 0;
            if (cboDepDetalle.Items.Count > 0)
                cboDepDetalle.SelectedIndex = 0;
            if (cboPrecios.Items.Count > 0)
                cboPrecios.SelectedIndex = 1;
            chkConImagen.Checked = false;
            chkSinImagen.Checked = false;
            _filtrarConImagen = false;
            _filtrarSinImagen = false;
            chkConExistencia.Checked = false;
            chkSinExistencia.Checked = false;
            txtBuscador.Text = string.Empty;
            _textoBusqueda = string.Empty;
            _proveedoresFiltro.Clear();
            _categoriasFiltro.Clear();
            _departamentosFiltro.Clear();
            chkSoloSeleccionados.Checked = false;
            _filtrarSoloSeleccionados = false;
            await CargarProductosAsync();
            dgvProductos.Refresh();
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener foco en DataGridView después de limpiar filtros
        }

        private async void BtnCargarImagen_Click(object? sender, EventArgs e)
        {
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un producto primero", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dgvProductos.SelectedRows[0].DataBoundItem is not Producto producto)
                return;
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.gif;*.bmp",
                Title = "Seleccionar imagen de producto"
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                byte[] imageData = await File.ReadAllBytesAsync(openFileDialog.FileName);
                using var displayStream = new MemoryStream(imageData);
                using var tempImg = SystemDrawingImage.FromStream(displayStream);
                producto.Imagen = new Bitmap(tempImg);
                producto.ImagenMiniatura = producto.Imagen.GetThumbnailImage(80, 80, null, IntPtr.Zero);
                producto.TieneImagen = true;
                var tempPath = IOPath.Combine(_tempImagePath, $"temp_{producto.Id}.jpg");
                producto.RutaImagen = tempPath;
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                using var fileStream = new IOFileStream(tempPath, FileMode.Create, FileAccess.Write);
                await Task.Run(() => producto.Imagen.Save(fileStream, ImageFormat.Jpeg));
                await GuardarImagenEnBaseDeDatosAsync(producto.Id, imageData);
                pbProducto.Image = new Bitmap(producto.Imagen);
                dgvProductos.Refresh();
                MessageBox.Show("Imagen actualizada correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Devolver foco al DataGridView después de cargar imagen
        }
        #endregion

        #region Eventos de Búsqueda
        private async void TxtBuscador_TextChanged(object? sender, EventArgs e)
        {
            _textoBusqueda = txtBuscador.Text.Trim();
            int version = ++_busquedaVersion;
            await Task.Delay(300);
            if (version != _busquedaVersion)
                return;
            await BuscarProductosAsync(version);
        }

        private async Task BuscarProductosAsync(int version)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(async () => await BuscarProductosAsync(version)));
                return;
            }
            try
            {
                // 1) Guardar si el buscador tenía el foco y la posición del cursor
                bool buscadorTeniaFoco = txtBuscador.Focused;
                int posicionCursor = txtBuscador.SelectionStart;

                SaveOrderQuantities();

                // 2) Si el texto de búsqueda está vacío, recargar todos los productos
                if (string.IsNullOrWhiteSpace(_textoBusqueda))
                {
                    await CargarProductosAsync();
                }
                else
                {
                    var seleccionados = GuardarEstadoSeleccion();
                    _productos.Clear();

                    string campoPrecio = ObtenerCampoPrecioSeleccionado();
                    var query = $@"
    SELECT i.ID, i.ItemLookupCode AS Codigo, i.Description AS Descripcion,
           i.Price AS Price, {campoPrecio} AS Precio,
           i.PriceA AS PrecioA, i.PriceB AS PrecioB, i.PriceC AS PrecioC,
           i.SalePrice, i.SaleStartDate, i.SaleEndDate,
           i.Quantity AS Cantidad,
           i.ExtendedDescription AS DescripcionAmpliada,
           i.SubDescription1 AS Subdescripcion1,
           i.SupplierID, i.CategoryID, i.DepartmentID,
           s.SupplierName, c.Name AS CategoryName, d.Name AS DepartmentName,
           CASE WHEN img.Imagen IS NOT NULL THEN 1 ELSE 0 END AS TieneImagen
      FROM Item i
 LEFT JOIN ItemImage img ON i.ID = img.ItemID
 LEFT JOIN Supplier s ON i.SupplierID = s.ID
 LEFT JOIN Category c ON i.CategoryID = c.ID
 LEFT JOIN Department d ON i.DepartmentID = d.ID
     WHERE 1=1 ";
                    var parameters = new Dictionary<string, object>();
                    var terminos = ObtenerTerminosBusqueda();
                    for (int i = 0; i < terminos.Count; i++)
                    {
                        string param = "@Busqueda" + i;
                        query += $@"AND (i.ItemLookupCode LIKE {param} OR
                                    i.Description LIKE {param} OR
                                    i.ExtendedDescription LIKE {param} OR
                                    i.SubDescription1 LIKE {param} OR
                                    s.SupplierName LIKE {param} OR
                                    c.Name LIKE {param} OR
                                    d.Name LIKE {param}) ";
                        parameters[param] = $"%{terminos[i]}%";
                    }

                    AplicarFiltrosAQuery(ref query, parameters);
                    if (cboPrecios.SelectedItem?.ToString() == "Oferta")
                        query += "ORDER BY i.SaleEndDate DESC";
                    else
                        query += "ORDER BY i.ItemLookupCode";

                    var dtProductos = await _databaseService.ExecuteQueryWithParametersAsync(query, parameters);
                    CargarProductosDesdeDataTable(dtProductos);

                    if (_filtrarSoloSeleccionados)
                    {
                        var productosSeleccionadosTemp = new List<Producto>();
                        foreach (var producto in _productos)
                            if (_productosSeleccionados.Contains(producto.Id))
                                productosSeleccionadosTemp.Add(producto);
                        _productos.Clear();
                        foreach (var producto in productosSeleccionadosTemp)
                            _productos.Add(producto);
                    }

                    RestaurarEstadoSeleccion(seleccionados);
                    dgvProductos.Refresh();
                }

                // 3) Restaurar el foco donde estaba
                if (buscadorTeniaFoco && version == _busquedaVersion && txtBuscador.Text == _textoBusqueda)
                {
                    txtBuscador.Focus();
                    txtBuscador.SelectionStart = posicionCursor;
                }
                else
                {
                    if (!txtBuscador.Focused)
                        dgvProductos.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar productos: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Configuración del DataGridView
        private void ConfigurarDataGridView()
        {
            dgvProductos.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgvProductos.Columns.Clear();
            dgvProductos.DataError += DgvProductos_DataError;
            dgvProductos.CellEndEdit += DgvProductos_CellEndEdit;
            DataGridViewColumnSortMode sortModeGlobal = DataGridViewColumnSortMode.Programmatic;
            var checkBoxColumn = new DataGridViewCheckBoxColumn
            {
                Name = "Seleccionar",
                HeaderText = "✔",
                Width = 20,
                ReadOnly = false,
                DataPropertyName = "Seleccionado",
                SortMode = sortModeGlobal
            };
            var codigoColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Codigo",
                HeaderText = "Código",
                SortMode = sortModeGlobal,
                ReadOnly = true
            };
            var descripcionColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Descripcion",
                HeaderText = "Descripción",
                SortMode = sortModeGlobal,
                ReadOnly = true
            };
            var precioColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "PrecioFiltrado",
                HeaderText = "Precio",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "F2",
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    FormatProvider = CultureInfo.InvariantCulture
                },
                SortMode = sortModeGlobal,
                ReadOnly = true
            };
            var cantidadColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Cantidad",
                HeaderText = "Existencia",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                },
                SortMode = sortModeGlobal,
                ReadOnly = true
            };
            var orderColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OrderQuantity",
                HeaderText = "Orden",
                Width = 40,
                SortMode = sortModeGlobal,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            };
            var tieneImagenColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TieneImagen",
                HeaderText = "📷",
                ReadOnly = true,
                SortMode = sortModeGlobal,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            };
            dgvProductos.Columns.AddRange(checkBoxColumn, codigoColumn, descripcionColumn, precioColumn, cantidadColumn, orderColumn, tieneImagenColumn);
            dgvProductos.AutoGenerateColumns = false;
            dgvProductos.DataSource = _productos;
            dgvProductos.RowTemplate.Height = 28;
            dgvProductos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgvProductos.ColumnHeadersDefaultCellStyle.SelectionBackColor = dgvProductos.ColumnHeadersDefaultCellStyle.BackColor;
            dgvProductos.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgvProductos.ColumnHeadersDefaultCellStyle.ForeColor;
            dgvProductos.RowHeadersVisible = false;
            dgvProductos.EnableHeadersVisualStyles = false;
            dgvProductos.BackgroundColor = SystemColors.Control;
            dgvProductos.DefaultCellStyle.BackColor = SystemColors.Control;
            dgvProductos.RowsDefaultCellStyle.BackColor = SystemColors.Control;
            dgvProductos.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Control;
            dgvProductos.ReadOnly = false;
            dgvProductos.VirtualMode = false;
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvProductos, new object[] { true });
            ConfigurarColumnasDataGridView();
            dgvProductos.StandardTab = false; // Permitir navegación con flechas
            dgvProductos.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2; // Evitar interferencia en la edición
        }

        private void DgvProductos_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvProductos.Columns[e.ColumnIndex].DataPropertyName != "OrderQuantity")
                return;
            var row = dgvProductos.Rows[e.RowIndex];
            if (!(row.DataBoundItem is Producto prod))
                return;
            string? texto = row.Cells[e.ColumnIndex].Value?.ToString();
            if (!int.TryParse(texto, out int qty))
            {
                row.Cells[e.ColumnIndex].Value = prod.OrderQuantity;
                return;
            }
            if (qty > prod.Cantidad)
            {
                row.Cells[e.ColumnIndex].Value = prod.Cantidad;
                return;
            }
            prod.OrderQuantity = qty;
            bool marcado = qty > 0;
            if (marcado)
                _productosSeleccionados.Add(prod.Id);
            else
                _productosSeleccionados.Remove(prod.Id);
            UpdateCounters();
            prod.Seleccionado = marcado;
            dgvProductos.InvalidateRow(e.RowIndex);
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener foco en DataGridView después de editar
        }

        private void DgvProductos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex >= 0 && dgvProductos.Columns[e.ColumnIndex].DataPropertyName == "TieneImagen" && e.Value != null)
            {
                bool tieneImagen = Convert.ToBoolean(e.Value);
                e.Value = tieneImagen ? "✓" : "✗";
                e.FormattingApplied = true;
            }
            if (e.ColumnIndex >= 0 && dgvProductos.Columns[e.ColumnIndex].Name == "Seleccionar" && e.RowIndex >= 0)
            {
                if (dgvProductos.Rows[e.RowIndex].DataBoundItem is Producto producto)
                {
                    e.Value = _productosSeleccionados.Contains(producto.Id);
                    e.FormattingApplied = true;
                }
            }
            if (e.ColumnIndex >= 0 && dgvProductos.Columns[e.ColumnIndex].DataPropertyName == "PrecioFiltrado" && e.Value != null)
            {
                if (decimal.TryParse(e.Value.ToString(), out var precio))
                {
                    e.Value = precio.ToString("F2", CultureInfo.InvariantCulture);
                    e.FormattingApplied = true;
                }
            }
        }
        #endregion

        #region Carga de Datos de Catálogos
        private async Task CargarProveedoresAsync()
        {
            try
            {
                _dtProveedores = await _databaseService.ExecuteQueryAsync("SELECT ID, SupplierName FROM Supplier ORDER BY SupplierName");
                CargarComboDesdeDataTable(cboProveedores, _dtProveedores, "SupplierName", "-Todos los proveedores-");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar proveedores: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarCategoriasAsync()
        {
            try
            {
                _dtCategorias = await _databaseService.ExecuteQueryAsync("SELECT ID, Name FROM Category ORDER BY Name");
                CargarComboDesdeDataTable(cboCategorias, _dtCategorias, "Name", "-Todas las categorías-");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar categorías: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task CargarDepartamentosAsync()
        {
            try
            {
                _dtDepartamentos = await _databaseService.ExecuteQueryAsync("SELECT ID, Name FROM Department ORDER BY Name");
                CargarComboDesdeDataTable(cboDepDetalle, _dtDepartamentos, "Name", "-Todos los departamentos-");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar departamentos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void CargarComboDesdeDataTable(ComboBox comboBox, DataTable dataTable, string displayMember, string primerElemento)
        {
            comboBox.Items.Clear();
            comboBox.Items.Add(primerElemento);
            foreach (DataRow row in dataTable.Rows)
                comboBox.Items.Add(row[displayMember].ToString());
            if (comboBox.Items.Count > 0)
                comboBox.SelectedIndex = 0;
        }
        #endregion

        #region Filtros y Carga de Productos
        private void AplicarFiltrosAQuery(ref string query, Dictionary<string, object> parameters)
        {
            if (_proveedoresFiltro.Count > 0)
            {
                var paramNames = new List<string>();
                for (int i = 0; i < _proveedoresFiltro.Count; i++)
                {
                    string name = "@Prov" + i;
                    paramNames.Add(name);
                    parameters[name] = _proveedoresFiltro[i];
                }
                query += $"AND i.SupplierID IN ({string.Join(",", paramNames)}) ";
            }
            if (_categoriasFiltro.Count > 0)
            {
                var paramNames = new List<string>();
                for (int i = 0; i < _categoriasFiltro.Count; i++)
                {
                    string name = "@Cat" + i;
                    paramNames.Add(name);
                    parameters[name] = _categoriasFiltro[i];
                }
                query += $"AND i.CategoryID IN ({string.Join(",", paramNames)}) ";
            }
            if (_departamentosFiltro.Count > 0)
            {
                var paramNames = new List<string>();
                for (int i = 0; i < _departamentosFiltro.Count; i++)
                {
                    string name = "@Dept" + i;
                    paramNames.Add(name);
                    parameters[name] = _departamentosFiltro[i];
                }
                query += $"AND i.DepartmentID IN ({string.Join(",", paramNames)}) ";
            }
            if (_filtrarConImagen && !_filtrarSinImagen)
                query += "AND img.Imagen IS NOT NULL ";
            else if (!_filtrarConImagen && _filtrarSinImagen)
                query += "AND img.Imagen IS NULL ";
            string opcionPrecio = cboPrecios.SelectedItem?.ToString();
            switch (opcionPrecio)
            {
                case "Regular":
                    query += "AND i.Price > 0 ";
                    break;
                case "PrecioA":
                    query += "AND i.PriceA > 0 ";
                    break;
                case "PrecioB":
                    query += "AND i.PriceB > 0 ";
                    break;
                case "PrecioC":
                    query += "AND i.PriceC > 0 ";
                    break;
                case "Oferta":
                    query += "AND i.SalePrice IS NOT NULL AND GETDATE() BETWEEN i.SaleStartDate AND i.SaleEndDate ";
                    break;
            }
        }

        private List<string> ObtenerTerminosBusqueda()
        {
            var terminos = _textoBusqueda
                .Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
            if (terminos.Count == 0 && !string.IsNullOrWhiteSpace(_textoBusqueda))
                terminos.Add(_textoBusqueda);
            return terminos;
        }

        private async Task CargarProductosAsync()
        {
            try
            {
                var seleccionados = GuardarEstadoSeleccion();
                SaveOrderQuantities();
                _productos.Clear();
                string campoPrecio = ObtenerCampoPrecioSeleccionado();
                var query = $@"SELECT i.ID, i.ItemLookupCode AS Codigo, i.Description AS Descripcion,
                    i.Price AS Price, {campoPrecio} AS Precio, i.PriceA AS PrecioA, i.PriceB AS PrecioB, i.PriceC AS PrecioC,
                    i.SalePrice, i.SaleStartDate, i.SaleEndDate,
                    i.Quantity AS Cantidad, i.ExtendedDescription AS DescripcionAmpliada,
                    i.SubDescription1 AS Subdescripcion1, i.SupplierID, i.CategoryID, i.DepartmentID,
                    s.SupplierName, c.Name AS CategoryName, d.Name AS DepartmentName,
                    CASE WHEN img.Imagen IS NOT NULL THEN 1 ELSE 0 END AS TieneImagen
                    FROM Item i
                    LEFT JOIN ItemImage img ON i.ID = img.ItemID
                    LEFT JOIN Supplier s ON i.SupplierID = s.ID
                    LEFT JOIN Category c ON i.CategoryID = c.ID
                    LEFT JOIN Department d ON i.DepartmentID = d.ID
                    WHERE 1=1 ";
                var parameters = new Dictionary<string, object>();
                AplicarFiltrosAQuery(ref query, parameters);
                if (!string.IsNullOrWhiteSpace(_textoBusqueda))
                {
                    var terminos = ObtenerTerminosBusqueda();
                    for (int i = 0; i < terminos.Count; i++)
                    {
                        string param = "@Busqueda" + i;
                        query += @"AND (i.ItemLookupCode LIKE " + param + @" OR
                     i.Description LIKE " + param + @" OR
                     i.ExtendedDescription LIKE " + param + @" OR
                     i.SubDescription1 LIKE " + param + @" OR
                     s.SupplierName LIKE " + param + @" OR
                     c.Name LIKE " + param + @" OR
                     d.Name LIKE " + param + @") ";
                        parameters.Add(param, $"%{terminos[i]}%");
                    }
                }
                if (chkConExistencia.Checked && !chkSinExistencia.Checked)
                    query += "AND i.Quantity > 0 ";
                else if (chkSinExistencia.Checked && !chkConExistencia.Checked)
                    query += "AND i.Quantity = 0 ";
                if (cboPrecios.SelectedItem?.ToString() == "Oferta")
                    query += "ORDER BY i.SaleEndDate DESC";
                else
                    query += "ORDER BY i.Description ASC";
                var dtProductos = await _databaseService.ExecuteQueryWithParametersAsync(query, parameters);
                CargarProductosDesdeDataTable(dtProductos);
                if (_filtrarSoloSeleccionados)
                {
                    var productosSeleccionadosTemp = new List<Producto>();
                    foreach (var producto in _productos)
                        if (_productosSeleccionados.Contains(producto.Id))
                            productosSeleccionadosTemp.Add(producto);
                    SaveOrderQuantities();
                    _productos.Clear();
                    foreach (var producto in productosSeleccionadosTemp)
                        _productos.Add(producto);
                }
                RestaurarEstadoSeleccion(seleccionados);
                SaveOrderQuantities();
                if (!txtBuscador.Focused)
                    dgvProductos.Focus(); // Mantener foco en DataGridView después de cargar productos
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarProductosDesdeDataTable(DataTable dtProductos)
        {
            try
            {
                SaveOrderQuantities();
                _productos.Clear();
                _productosTodos.Clear();
                foreach (DataRow row in dtProductos.Rows)
                {
                    var producto = new Producto
                    {
                        Id = Convert.ToInt32(row["ID"]),
                        Codigo = row["Codigo"]?.ToString() ?? string.Empty,
                        Descripcion = row["Descripcion"]?.ToString() ?? string.Empty,
                        Precio = row["Price"] != DBNull.Value ? Convert.ToDecimal(row["Price"]) : 0m,
                        PrecioFiltrado = decimal.TryParse(row["Precio"]?.ToString(), out var p) ? p : 0m,
                        PrecioA = row["PrecioA"] != DBNull.Value ? Convert.ToDecimal(row["PrecioA"]) : 0m,
                        PrecioB = row["PrecioB"] != DBNull.Value ? Convert.ToDecimal(row["PrecioB"]) : 0m,
                        PrecioC = row["PrecioC"] != DBNull.Value ? Convert.ToDecimal(row["PrecioC"]) : 0m,
                        PrecioOferta = row.Table.Columns.Contains("SalePrice") && row["SalePrice"] != DBNull.Value
                            ? Convert.ToDecimal(row["SalePrice"])
                            : (decimal?)null,
                        FechaInicioOferta = row.Table.Columns.Contains("SaleStartDate") && row["SaleStartDate"] != DBNull.Value
                            ? Convert.ToDateTime(row["SaleStartDate"])
                            : (DateTime?)null,
                        FechaFinOferta = row.Table.Columns.Contains("SaleEndDate") && row["SaleEndDate"] != DBNull.Value
                            ? Convert.ToDateTime(row["SaleEndDate"])
                            : (DateTime?)null,
                        Cantidad = row["Cantidad"] != DBNull.Value ? Convert.ToInt32(row["Cantidad"]) : 0,
                        DescripcionAmpliada = row["DescripcionAmpliada"]?.ToString() ?? string.Empty,
                        Subdescripcion1 = row["Subdescripcion1"]?.ToString() ?? string.Empty,
                        ProveedorId = row["SupplierID"] != DBNull.Value ? Convert.ToInt32(row["SupplierID"]) : null,
                        CategoriaId = row["CategoryID"] != DBNull.Value ? Convert.ToInt32(row["CategoryID"]) : null,
                        DepartamentoId = row["DepartmentID"] != DBNull.Value ? Convert.ToInt32(row["DepartmentID"]) : null,
                        ProveedorNombre = row.Table.Columns.Contains("SupplierName") ? row["SupplierName"].ToString() : null,
                        CategoriaNombre = row.Table.Columns.Contains("CategoryName") ? row["CategoryName"].ToString() : null,
                        DepartamentoNombre = row.Table.Columns.Contains("DepartmentName") ? row["DepartmentName"].ToString() : null,
                        TieneImagen = Convert.ToBoolean(row["TieneImagen"]),
                        Seleccionado = false
                    };
                    if (_orderQuantities.TryGetValue(producto.Id, out var qty))
                        producto.OrderQuantity = qty;
                    _productos.Add(producto);
                    _productosTodos.Add(producto);
                }
                lblTotalProductos.Text = $"Total de productos: {_productos.Count}";
                ConfigurarColumnasDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al procesar datos de productos: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        #endregion

        #region Manejo de Imágenes
        private async Task CargarImagenProductoAsync(Producto producto, bool mostrarErrores = true)
        {
            await CargarImagenesProductos(new[] { producto }, mostrarErrores);
        }

        private async Task CargarImagenesProductos(IEnumerable<Producto> productos, bool mostrarErrores = true)
        {
            var productosParaCargar = productos
                .Where(p => p.TieneImagen && p.Imagen == null)
                .ToList();
            if (productosParaCargar.Count == 0)
                return;

            try
            {
                var paramNames = productosParaCargar
                    .Select((p, i) => ($"@Id{i}", p.Id))
                    .ToList();
                var inClause = string.Join(",", paramNames.Select(t => t.Item1));
                var query = $"SELECT ItemID, Imagen FROM ItemImage WHERE ItemID IN ({inClause})";
                var parameters = paramNames.ToDictionary(t => t.Item1, t => (object)t.Item2);
                var dtImagenes = await _databaseService.ExecuteQueryWithParametersAsync(query, parameters);
                var imagenes = new Dictionary<int, byte[]>();
                foreach (DataRow row in dtImagenes.Rows)

                {
                    if (row["Imagen"] != DBNull.Value)
                        imagenes[(int)row["ItemID"]] = (byte[])row["Imagen"];
                }

                foreach (var producto in productosParaCargar)
                {
                    if (imagenes.TryGetValue(producto.Id, out var data))
                    {
                        using var memStream = new MemoryStream(data);
                        using var tempImg = SystemDrawingImage.FromStream(memStream);
                        producto.Imagen = new Bitmap(tempImg);
                        producto.ImagenMiniatura = producto.Imagen.GetThumbnailImage(80, 80, null, IntPtr.Zero);
                        var tempPath = IOPath.Combine(_tempImagePath, $"temp_{producto.Id}.jpg");
                        producto.RutaImagen = tempPath;
                        if (File.Exists(tempPath))
                            File.Delete(tempPath);
                        using var fileStream = new IOFileStream(tempPath, FileMode.Create, FileAccess.Write);
                        producto.Imagen.Save(fileStream, ImageFormat.Jpeg);
                    }
                    else
                    {
                        var rutaImagenDefecto = IOPath.Combine(_appSettings.DefaultImagePath, "no-image.png");
                        if (File.Exists(rutaImagenDefecto))
                        {
                            using var stream = new FileStream(rutaImagenDefecto, FileMode.Open, FileAccess.Read);
                            using var tempImg = SystemDrawingImage.FromStream(stream);
                            producto.Imagen = new Bitmap(tempImg);
                            producto.ImagenMiniatura = producto.Imagen.GetThumbnailImage(80, 80, null, IntPtr.Zero);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (mostrarErrores)
                    MessageBox.Show($"Error al cargar imágenes: {ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarImagenProducto(Producto producto, bool mostrarErrores = true)
            => CargarImagenProductoAsync(producto, mostrarErrores).GetAwaiter().GetResult();

        private void CopyImageToClipboard()
        {
            if (pbProducto.Image != null)
            {
                try
                {
                    using (Bitmap bmp = new Bitmap(pbProducto.Image))
                    {
                        Clipboard.Clear();
                        Clipboard.SetImage(bmp);
                    }
                    MessageBox.Show("Imagen copiada al portapapeles.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al copiar imagen: {ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No hay imagen para copiar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void PasteImageFromClipboard()
        {
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show("No hay imagen en el portapapeles.", "Atención",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (dgvProductos.SelectedRows.Count == 0 ||
                !(dgvProductos.SelectedRows[0].DataBoundItem is Producto prod))
            {
                MessageBox.Show("Selecciona un producto antes de pegar.", "Aviso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (prod.TieneImagen)
            {
                var result = MessageBox.Show(
                    "El producto ya tiene una imagen. ¿Desea sobreescribirla?",
                    "Confirmar Sobreescritura",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (result != DialogResult.Yes)
                {
                    return;
                }
            }

            try
            {
                var img = Clipboard.GetImage();
                if (img == null)
                {
                    MessageBox.Show("No se pudo obtener la imagen del portapapeles.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                pbProducto.Image = img;
                prod.Imagen = new Bitmap(img);
                prod.ImagenMiniatura = prod.Imagen.GetThumbnailImage(80, 80, null, IntPtr.Zero);
                prod.TieneImagen = true;

                using var ms = new MemoryStream();
                prod.Imagen.Save(ms, ImageFormat.Jpeg);
                var imageData = ms.ToArray();

                var tempPath = IOPath.Combine(_tempImagePath, $"temp_{prod.Id}.jpg");
                prod.RutaImagen = tempPath;
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                using var fileStream = new IOFileStream(tempPath, FileMode.Create, FileAccess.Write);
                prod.Imagen.Save(fileStream, ImageFormat.Jpeg);
                await GuardarImagenEnBaseDeDatosAsync(prod.Id, imageData);

                dgvProductos.Refresh();
                MessageBox.Show("Imagen pegada y guardada correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al pegar la imagen: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PbProducto_Click(object? sender, EventArgs e)
        {
            // No hacer nada con el foco, solo copiar la imagen si se hace clic
            CopyImageToClipboard();
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Devolver foco al DataGridView después de hacer clic en PictureBox
        }

        private void PbProducto_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyImageToClipboard();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteImageFromClipboard();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                if (!txtBuscador.Focused)
                    dgvProductos.Focus(); // Redirigir el foco al DataGridView al presionar flechas
                e.Handled = false; // Permitir que el DataGridView maneje las flechas
            }
        }

        private void PbProducto_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _dragStartPoint = e.Location;
            }
        }

        private void PbProducto_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && pbProducto.Image != null)
            {
                var dragRect = new Rectangle(
                    _dragStartPoint.X - SystemInformation.DragSize.Width / 2,
                    _dragStartPoint.Y - SystemInformation.DragSize.Height / 2,
                    SystemInformation.DragSize.Width,
                    SystemInformation.DragSize.Height);

                if (!dragRect.Contains(e.Location))
                {
                    try
                    {
                        var data = new DataObject();
                        data.SetData(DataFormats.Bitmap, pbProducto.Image);

                        var file = IOPath.Combine(_tempImagePath, $"drag_{Guid.NewGuid()}.jpg");
                        using (var fs = new IOFileStream(file, FileMode.Create, FileAccess.Write))
                        {
                            pbProducto.Image.Save(fs, ImageFormat.Jpeg);
                        }

                        var files = new StringCollection { file };
                        data.SetFileDropList(files);

                        pbProducto.DoDragDrop(data, DragDropEffects.Copy);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al arrastrar la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void PbProducto_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null && (e.Data.GetDataPresent(DataFormats.FileDrop) ||
                                   e.Data.GetDataPresent(DataFormats.Bitmap)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private async void PbProducto_DragDrop(object? sender, DragEventArgs e)
        {
            try
            {
                SystemDrawingImage? img = null;
                byte[]? imageData = null;

                if (e.Data?.GetDataPresent(DataFormats.FileDrop) == true)
                {
                    var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (files != null && files.Length > 0)
                    {
                        imageData = await File.ReadAllBytesAsync(files[0]);
                        using var ms = new MemoryStream(imageData);
                        using var tempImg = SystemDrawingImage.FromStream(ms);
                        img = new Bitmap(tempImg);
                    }
                }
                else if (e.Data?.GetDataPresent(DataFormats.Bitmap) == true)
                {
                    var bmp = e.Data.GetData(DataFormats.Bitmap) as SystemDrawingImage;
                    if (bmp != null)
                    {
                        img = new Bitmap(bmp);
                        using var ms = new MemoryStream();
                        img.Save(ms, ImageFormat.Jpeg);
                        imageData = ms.ToArray();
                    }
                }

                if (img == null || imageData == null)
                    return;

                if (dgvProductos.SelectedRows.Count == 0 ||
                    !(dgvProductos.SelectedRows[0].DataBoundItem is Producto prod))
                {
                    MessageBox.Show("Selecciona un producto antes de soltar la imagen.",
                                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (prod.TieneImagen)
                {
                    var result = MessageBox.Show(
                        "El producto ya tiene una imagen. ¿Desea sobreescribirla?",
                        "Confirmar Sobreescritura",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                    if (result != DialogResult.Yes)
                        return;
                }

                pbProducto.Image = img;
                prod.Imagen = new Bitmap(img);
                prod.ImagenMiniatura = prod.Imagen.GetThumbnailImage(80, 80, null, IntPtr.Zero);
                prod.TieneImagen = true;

                var tempPath = IOPath.Combine(_tempImagePath, $"temp_{prod.Id}.jpg");
                prod.RutaImagen = tempPath;
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                using var fileStream = new IOFileStream(tempPath, FileMode.Create, FileAccess.Write);
                prod.Imagen.Save(fileStream, ImageFormat.Jpeg);
                await GuardarImagenEnBaseDeDatosAsync(prod.Id, imageData);

                dgvProductos.Refresh();
                MessageBox.Show("Imagen cargada correctamente.", "Éxito",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar imagen: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task GuardarImagenEnBaseDeDatosAsync(int itemId, byte[] imageData)
        {
            try
            {
                const string checkQuery = "SELECT COUNT(*) FROM ItemImage WHERE ItemID = @ItemID";
                var pCheck = new Dictionary<string, object> { ["@ItemID"] = itemId };
                var result = await _databaseService.ExecuteScalarAsync(checkQuery, pCheck);
                int count = result != null ? Convert.ToInt32(result) : 0;

                string query = count > 0
                    ? "UPDATE ItemImage SET Imagen = @Imagen WHERE ItemID = @ItemID"
                    : "INSERT INTO ItemImage (ItemID, Imagen) VALUES (@ItemID, @Imagen)";

                var parameters = new Dictionary<string, object>
                {
                    ["@ItemID"] = itemId,
                    ["@Imagen"] = imageData
                };

                await _databaseService.ExecuteNonQueryAsync(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar imagen en la base de datos: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Exportación a PDF
        private async void BtnGenerarPDF_Click(object? sender, EventArgs e)
        {
            try
            {
                await CargarProductosAsync();
                var lista = _productos.Where(p => p.Seleccionado).ToList();
                if (lista.Count == 0)
                {
                    MessageBox.Show(
                        "Seleccione al menos un producto antes de generar el PDF.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }
                string opcionPrecio = cboPrecios.SelectedItem?.ToString() ?? "Desconocido";
                Console.WriteLine($"Filtro de precio seleccionado: {opcionPrecio}");
                foreach (var prod in lista)
                {
                    Console.WriteLine($"Producto ID: {prod.Id}, Codigo: {prod.Codigo}, " +
                                     $"PrecioFiltrado: {prod.PrecioFiltrado}, Precio: {prod.Precio}, " +
                                     $"PrecioA: {prod.PrecioA}, PrecioB: {prod.PrecioB}, PrecioC: {prod.PrecioC}");
                }
                foreach (var prod in lista)
                {
                    prod.Precio = prod.PrecioFiltrado;
                }
                using var sfd = new SaveFileDialog
                {
                    Filter = "Archivo PDF|*.pdf",
                    Title = "Guardar catálogo"
                };
                if (sfd.ShowDialog() != DialogResult.OK) return;
                BtnGenerarPDF.Enabled = false;
                Cursor.Current = Cursors.WaitCursor;
                MessageBox.Show($"Generando catálogo con filtro de precio: {opcionPrecio}",
                                "Depuración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    await Task.Run(async () =>
                    {
                        await CargarImagenesProductos(lista, false);
                        GeneradorCatalogoPdf.GenerarCatálogo(lista, sfd.FileName);
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al generar PDF:\n{ex.Message}",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                    BtnGenerarPDF.Enabled = true;
                }
                Process.Start(new ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error general al generar catálogo:\n{ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Devolver foco al DataGridView después de generar PDF
        }

        private void BtnLimpiarSeleccion_Click(object? sender, EventArgs e)
        {
            try
            {
                _productosSeleccionados.Clear();
                dgvProductos.Refresh();
                UpdateCounters();
                MessageBox.Show("Se ha limpiado la selección de productos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al limpiar selección: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener foco en DataGridView después de limpiar selección
        }
        #endregion

        private void BtnSeleccionarTodo_Click(object? sender, EventArgs e)
        {
            try
            {
                foreach (var producto in _productos)
                {
                    producto.Seleccionado = true;
                    if (!_productosSeleccionados.Contains(producto.Id))
                    {
                        _productosSeleccionados.Add(producto.Id);
                        UpdateCounters();
                    }
                }
                dgvProductos.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar todos los productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Mantener foco en DataGridView después de seleccionar todo
        }

        private async void BtnConfiguracion_Click(object? sender, EventArgs e)
        {
            using var form = new FormConexion();
            if (form.ShowDialog() == DialogResult.OK)
            {
                _databaseService = new DatabaseService(new ConnectionConfig
                {
                    ConnectionString = form.ConnectionString,
                    CommandTimeout = 30
                });
                await CargarDatosInicialesAsync();
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                if (!(ActiveControl is TextBoxBase))
                {
                    CopyImageToClipboard();
                    e.Handled = true;
                }
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                if (!(ActiveControl is TextBoxBase))
                {
                    PasteImageFromClipboard();
                    e.Handled = true;
                }
            }
            else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            {
                // Solo mover el foco al DataGridView si no está activo
                if (!dgvProductos.Focused && ActiveControl != txtBuscador)
                {
                    dgvProductos.Focus();
                }
                // No marcar como manejado para permitir la navegación nativa del DataGridView
                e.Handled = false;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C) && !(ActiveControl is TextBoxBase))
            {
                CopyImageToClipboard();
                return true;
            }
            if (keyData == (Keys.Control | Keys.V) && !(ActiveControl is TextBoxBase))
            {
                PasteImageFromClipboard();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private bool _isGeneratingCotizacion;

        private void BtnGenerarCotizacion_Click(object? sender, EventArgs e)
        {
            if (_isGeneratingCotizacion) return;
            try
            {
                _isGeneratingCotizacion = true;
                var lista = _productos.Where(p => p.OrderQuantity > 0).ToList();
                if (lista.Count == 0)
                {
                    MessageBox.Show(
                        "Ingresa una cantidad en la columna Orden para cotizar.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                    return;
                }
                string rutaArchivo = null;
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PDF|*.pdf";
                    sfd.Title = "Guardar Cotización";
                    if (sfd.ShowDialog() != DialogResult.OK)
                        return;
                    rutaArchivo = sfd.FileName;
                }
                if (string.IsNullOrEmpty(rutaArchivo))
                    return;
                GenerarCotizacionPdf(lista, rutaArchivo);
                AbrirArchivoPdf(rutaArchivo);
            }
            finally
            {
                _isGeneratingCotizacion = false;
            }
            if (!txtBuscador.Focused)
                dgvProductos.Focus(); // Devolver foco al DataGridView después de generar cotización
        }

        private static void GenerarCotizacionPdf(List<Producto> productos, string rutaArchivo)
        {
            string F(decimal v) => v.ToString("#,##0.00", CultureInfo.InvariantCulture);
            PdfWriter writer = null;
            PdfDocument pdf = null;
            Document doc = null;
            try
            {
                writer = new PdfWriter(rutaArchivo);
                pdf = new PdfDocument(writer);
                doc = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
                doc.SetMargins(20, 20, 20, 20);
                var fontB = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                doc.Add(new Paragraph("Cotización")
                           .SetFont(fontB).SetFontSize(16)
                           .SetTextAlignment(TextAlignment.CENTER));
                doc.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                           .SetFont(font).SetFontSize(10)
                           .SetMarginBottom(10));
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 6, 1, 2, 2 }))
                                .UseAllAvailableWidth();
                foreach (var h in new[] { "Código", "Descripción", "Cant.", "Precio U.", "Total" })
                    table.AddHeaderCell(new Cell()
                        .Add(new Paragraph(h).SetFont(fontB))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                decimal granTotal = 0m;
                foreach (var prod in productos)
                {
                    int qty = prod.OrderQuantity;
                    decimal price = prod.Precio;
                    decimal line = price * qty;
                    granTotal += line;
                    table.AddCell(new Cell().Add(new Paragraph(prod.Codigo).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(prod.Descripcion).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(qty.ToString()).SetFont(font)));
                    table.AddCell(new Cell()
                        .Add(new Paragraph($"L. {F(price)}").SetFont(font))
                        .SetTextAlignment(TextAlignment.RIGHT));
                    table.AddCell(new Cell()
                        .Add(new Paragraph($"L. {F(line)}").SetFont(font))
                        .SetTextAlignment(TextAlignment.RIGHT));
                }
                table.AddCell(new Cell(1, 4)
                    .Add(new Paragraph("Total").SetFont(fontB))
                    .SetTextAlignment(TextAlignment.RIGHT));
                table.AddCell(new Cell()
                    .Add(new Paragraph($"L. {F(granTotal)}").SetFont(fontB))
                    .SetTextAlignment(TextAlignment.RIGHT));
                doc.Add(table);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al generar PDF: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                doc?.Close();
                pdf?.Close();
                writer?.Close();
            }
        }

        private static void AbrirArchivoPdf(string rutaArchivo)
        {
            Thread.Sleep(500);
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = rutaArchivo,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"El PDF se ha generado correctamente pero no se pudo abrir: {ex.Message}",
                    "Información",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void DgvProductos_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            if (dgvProductos.Columns[e.ColumnIndex].DataPropertyName == "OrderQuantity")
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
        }

        private void SaveOrderQuantities()
        {
            foreach (var p in _productos)
            {
                if (p.OrderQuantity > 0)
                    _orderQuantities[p.Id] = p.OrderQuantity;
            }
            foreach (var p in _productosTodos)
            {
                if (p.OrderQuantity > 0)
                    _orderQuantities[p.Id] = p.OrderQuantity;
            }
        }

        private void RestaurarCantidadesOrden(List<Producto> productos)
        {
            foreach (var producto in productos)
            {
                if (_orderQuantities.TryGetValue(producto.Id, out int cantidad))
                    producto.OrderQuantity = cantidad;
            }
        }

        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            WindowConfig.Save(Width, Height);
        }
    }
}