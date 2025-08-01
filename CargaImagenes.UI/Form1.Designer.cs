using System.ComponentModel;

namespace CargaImagenes.UI
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private IContainer components = null;
        private DataGridView dgvProductos;
        private TextBox txtBuscador;
        private ComboBox cboProveedores;
        private ComboBox cboCategorias;
        private ComboBox cboDepDetalle;
        private CheckBox chkConImagen;
        private CheckBox chkSinImagen;
        private CheckBox chkConExistencia;
        private CheckBox chkSinExistencia;
        private Button BtnLimpiarFiltros;
        private Label lblTotalProductos;


        // Paneles para agrupar secciones
        private Panel pnlFiltros;
        private Panel pnlResultados;
        private Panel pnlControles;
        private Panel pnlDetalleProducto;

        // Controles para el panel de detalle del producto
        private PictureBox pbProducto;
        private Button BtnCargarImagen;
        private Button BtnConfiguracion;
        private TextBox txtCodigo;
        private TextBox txtDescripcion;
        private TextBox txtPrecio;
        private TextBox txtPrecioA;
        private TextBox txtPrecioB;
        private TextBox txtPrecioC;
        private TextBox txtCantidad;
        private TextBox txtDescripcionAmpliada;
        private TextBox txtSubdescripcion1;
        // Controles para la barra de acción y selección
        private Button BtnGenerarPDF;
        private Button BtnLimpiarSeleccion;
        private CheckBox chkSoloSeleccionados;
        private ComboBox cboPrecios;
        private Button BtnSeleccionarTodo;


        /// <summary>
        /// Limpia los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            dgvProductos = new DataGridView();
            txtBuscador = new TextBox();
            cboProveedores = new ComboBox();
            cboCategorias = new ComboBox();
            cboDepDetalle = new ComboBox();
            chkConImagen = new CheckBox();
            chkSinImagen = new CheckBox();
            chkConExistencia = new CheckBox();
            chkSinExistencia = new CheckBox();
            BtnLimpiarFiltros = new Button();
            lblTotalProductos = new Label();
            BtnGenerarPDF = new Button();
            BtnLimpiarSeleccion = new Button();
            chkSoloSeleccionados = new CheckBox();
            cboPrecios = new ComboBox();
            BtnSeleccionarTodo = new Button();
            BtnConfiguracion = new Button();
            pnlFiltros = new Panel();
            lblProductosSeleccionados = new Label();
            pnlResultados = new Panel();
            BtnCargarImagen = new Button();
            pnlControles = new Panel();
            BtnGenerarCotizacion = new Button();
            pnlDetalleProducto = new Panel();
            pbProducto = new PictureBox();
            txtCodigo = new TextBox();
            txtDescripcion = new TextBox();
            txtPrecio = new TextBox();
            txtPrecioA = new TextBox();
            txtPrecioB = new TextBox();
            txtPrecioC = new TextBox();
            txtCantidad = new TextBox();
            txtSubdescripcion1 = new TextBox();
            txtDescripcionAmpliada = new TextBox();
            ((ISupportInitialize)dgvProductos).BeginInit();
            pnlFiltros.SuspendLayout();
            pnlResultados.SuspendLayout();
            pnlControles.SuspendLayout();
            pnlDetalleProducto.SuspendLayout();
            ((ISupportInitialize)pbProducto).BeginInit();
            SuspendLayout();
            // 
            // dgvProductos
            // 
            dgvProductos.AllowUserToAddRows = false;
            dgvProductos.AllowUserToDeleteRows = false;
            dgvProductos.BackgroundColor = SystemColors.ButtonFace;
            dgvProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvProductos.Location = new Point(4, 4);
            dgvProductos.Margin = new Padding(3, 2, 3, 2);
            dgvProductos.Name = "dgvProductos";
            dgvProductos.ReadOnly = true;
            dgvProductos.RowHeadersWidth = 51;
            dgvProductos.Size = new Size(492, 415);
            dgvProductos.TabIndex = 11;
            // 
            // txtBuscador
            // 
            txtBuscador.BackColor = Color.White;
            txtBuscador.BorderStyle = BorderStyle.FixedSingle;
            txtBuscador.Location = new Point(8, 62);
            txtBuscador.Margin = new Padding(3, 2, 3, 2);
            txtBuscador.Name = "txtBuscador";
            txtBuscador.PlaceholderText = "Separe múltiples términos con comas";
            txtBuscador.Size = new Size(386, 23);
            txtBuscador.TabIndex = 4;
            // 
            // cboProveedores
            // 
            cboProveedores.BackColor = Color.WhiteSmoke;
            cboProveedores.DropDownStyle = ComboBoxStyle.DropDownList;
            cboProveedores.Location = new Point(203, 7);
            cboProveedores.Margin = new Padding(3, 2, 3, 2);
            cboProveedores.Name = "cboProveedores";
            cboProveedores.Size = new Size(192, 23);
            cboProveedores.TabIndex = 1;
            // 
            // cboCategorias
            // 
            cboCategorias.BackColor = Color.WhiteSmoke;
            cboCategorias.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategorias.Location = new Point(8, 35);
            cboCategorias.Margin = new Padding(3, 2, 3, 2);
            cboCategorias.Name = "cboCategorias";
            cboCategorias.Size = new Size(189, 23);
            cboCategorias.TabIndex = 2;
            // 
            // cboDepDetalle
            // 
            cboDepDetalle.BackColor = Color.WhiteSmoke;
            cboDepDetalle.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDepDetalle.Location = new Point(8, 7);
            cboDepDetalle.Margin = new Padding(3, 2, 3, 2);
            cboDepDetalle.Name = "cboDepDetalle";
            cboDepDetalle.Size = new Size(189, 23);
            cboDepDetalle.TabIndex = 3;
            // 
            // chkConImagen
            // 
            chkConImagen.Location = new Point(403, 46);
            chkConImagen.Margin = new Padding(3, 2, 3, 2);
            chkConImagen.Name = "chkConImagen";
            chkConImagen.Size = new Size(105, 18);
            chkConImagen.TabIndex = 7;
            chkConImagen.Text = "Con Imagen";
            chkConImagen.UseVisualStyleBackColor = true;
            // 
            // chkSinImagen
            // 
            chkSinImagen.Location = new Point(403, 65);
            chkSinImagen.Margin = new Padding(3, 2, 3, 2);
            chkSinImagen.Name = "chkSinImagen";
            chkSinImagen.Size = new Size(97, 18);
            chkSinImagen.TabIndex = 6;
            chkSinImagen.Text = "Sin Imagen";
            chkSinImagen.UseVisualStyleBackColor = true;
            // 
            // chkConExistencia
            // 
            chkConExistencia.Location = new Point(403, 9);
            chkConExistencia.Margin = new Padding(3, 2, 3, 2);
            chkConExistencia.Name = "chkConExistencia";
            chkConExistencia.Size = new Size(118, 18);
            chkConExistencia.TabIndex = 8;
            chkConExistencia.Text = "Con Existencia";
            chkConExistencia.UseVisualStyleBackColor = true;
            // 
            // chkSinExistencia
            // 
            chkSinExistencia.Location = new Point(403, 27);
            chkSinExistencia.Margin = new Padding(3, 2, 3, 2);
            chkSinExistencia.Name = "chkSinExistencia";
            chkSinExistencia.Size = new Size(122, 18);
            chkSinExistencia.TabIndex = 9;
            chkSinExistencia.Text = "Sin Existencia";
            chkSinExistencia.UseVisualStyleBackColor = true;
            // 
            // BtnLimpiarFiltros
            // 
            BtnLimpiarFiltros.Location = new Point(284, 35);
            BtnLimpiarFiltros.Margin = new Padding(3, 2, 3, 2);
            BtnLimpiarFiltros.Name = "BtnLimpiarFiltros";
            BtnLimpiarFiltros.Size = new Size(111, 23);
            BtnLimpiarFiltros.TabIndex = 5;
            BtnLimpiarFiltros.Text = "Limpiar Filtros";
            BtnLimpiarFiltros.TextAlign = ContentAlignment.MiddleLeft;
            BtnLimpiarFiltros.UseVisualStyleBackColor = true;
            // 
            // lblTotalProductos
            // 
            lblTotalProductos.Location = new Point(0, 0);
            lblTotalProductos.Name = "lblTotalProductos";
            lblTotalProductos.Size = new Size(100, 23);
            lblTotalProductos.TabIndex = 0;
            // 
            // BtnGenerarPDF
            // 
            BtnGenerarPDF.BackColor = Color.PaleGoldenrod;
            BtnGenerarPDF.Location = new Point(335, 7);
            BtnGenerarPDF.Margin = new Padding(3, 2, 3, 2);
            BtnGenerarPDF.Name = "BtnGenerarPDF";
            BtnGenerarPDF.Size = new Size(50, 23);
            BtnGenerarPDF.TabIndex = 17;
            BtnGenerarPDF.Text = "PDF";
            BtnGenerarPDF.UseVisualStyleBackColor = false;
            // 
            // BtnLimpiarSeleccion
            // 
            BtnLimpiarSeleccion.Location = new Point(174, 7);
            BtnLimpiarSeleccion.Margin = new Padding(3, 2, 3, 2);
            BtnLimpiarSeleccion.Name = "BtnLimpiarSeleccion";
            BtnLimpiarSeleccion.Size = new Size(85, 23);
            BtnLimpiarSeleccion.TabIndex = 14;
            BtnLimpiarSeleccion.Text = "Limpiar Selección";
            BtnLimpiarSeleccion.UseVisualStyleBackColor = true;
            // 
            // chkSoloSeleccionados
            // 
            chkSoloSeleccionados.Location = new Point(3, 10);
            chkSoloSeleccionados.Margin = new Padding(3, 2, 3, 2);
            chkSoloSeleccionados.Name = "chkSoloSeleccionados";
            chkSoloSeleccionados.Size = new Size(80, 18);
            chkSoloSeleccionados.TabIndex = 15;
            chkSoloSeleccionados.Text = "Mostrar ✓";
            chkSoloSeleccionados.UseVisualStyleBackColor = true;
            // 
            // cboPrecios
            // 
            cboPrecios.BackColor = Color.WhiteSmoke;
            cboPrecios.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPrecios.Location = new Point(203, 35);
            cboPrecios.Margin = new Padding(3, 2, 3, 2);
            cboPrecios.Name = "cboPrecios";
            cboPrecios.Size = new Size(75, 23);
            cboPrecios.TabIndex = 16;
            // 
            // BtnSeleccionarTodo
            // 
            BtnSeleccionarTodo.Location = new Point(86, 7);
            BtnSeleccionarTodo.Margin = new Padding(3, 2, 3, 2);
            BtnSeleccionarTodo.Name = "BtnSeleccionarTodo";
            BtnSeleccionarTodo.Size = new Size(85, 23);
            BtnSeleccionarTodo.TabIndex = 13;
            BtnSeleccionarTodo.Text = "Seleccionar Todo";
            BtnSeleccionarTodo.UseVisualStyleBackColor = true;
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.WhiteSmoke;
            pnlFiltros.BorderStyle = BorderStyle.FixedSingle;
            pnlFiltros.Controls.Add(lblProductosSeleccionados);
            pnlFiltros.Controls.Add(cboProveedores);
            pnlFiltros.Controls.Add(txtBuscador);
            pnlFiltros.Controls.Add(cboCategorias);
            pnlFiltros.Controls.Add(cboDepDetalle);
            pnlFiltros.Controls.Add(cboPrecios);
            pnlFiltros.Controls.Add(BtnLimpiarFiltros);
            pnlFiltros.Controls.Add(chkSinImagen);
            pnlFiltros.Controls.Add(chkConImagen);
            pnlFiltros.Controls.Add(chkConExistencia);
            pnlFiltros.Controls.Add(chkSinExistencia);
            pnlFiltros.Location = new Point(10, 3);
            pnlFiltros.Margin = new Padding(3, 2, 3, 2);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(503, 91);
            pnlFiltros.TabIndex = 0;
            // 
            // lblProductosSeleccionados
            // 
            lblProductosSeleccionados.AutoSize = true;
            lblProductosSeleccionados.BackColor = Color.White;
            lblProductosSeleccionados.Location = new Point(203, 66);
            lblProductosSeleccionados.Name = "lblProductosSeleccionados";
            lblProductosSeleccionados.Size = new Size(94, 15);
            lblProductosSeleccionados.TabIndex = 27;
            lblProductosSeleccionados.Text = "Seleccionados: 0";
            // 
            // pnlResultados
            // 
            pnlResultados.BackColor = Color.WhiteSmoke;
            pnlResultados.BorderStyle = BorderStyle.FixedSingle;
            pnlResultados.Controls.Add(dgvProductos);
            pnlResultados.Location = new Point(10, 98);
            pnlResultados.Margin = new Padding(3, 2, 3, 2);
            pnlResultados.Name = "pnlResultados";
            pnlResultados.Size = new Size(503, 426);
            pnlResultados.TabIndex = 10;
            // 
            // BtnCargarImagen
            // 
            BtnCargarImagen.Location = new Point(3, 522);
            BtnCargarImagen.Margin = new Padding(3, 2, 3, 2);
            BtnCargarImagen.Name = "BtnCargarImagen";
            BtnCargarImagen.Size = new Size(234, 23);
            BtnCargarImagen.TabIndex = 21;
            BtnCargarImagen.Text = "Cargar Imagen";
            BtnCargarImagen.UseVisualStyleBackColor = true;
            // 
            // pnlControles
            // 
            pnlControles.BackColor = Color.WhiteSmoke;
            pnlControles.BorderStyle = BorderStyle.FixedSingle;
            pnlControles.Controls.Add(BtnConfiguracion);
            pnlControles.Controls.Add(BtnGenerarPDF);
            pnlControles.Controls.Add(BtnGenerarCotizacion);
            pnlControles.Controls.Add(BtnLimpiarSeleccion);
            pnlControles.Controls.Add(BtnSeleccionarTodo);
            pnlControles.Controls.Add(chkSoloSeleccionados);
            pnlControles.Location = new Point(10, 528);
            pnlControles.Margin = new Padding(3, 2, 3, 2);
            pnlControles.Name = "pnlControles";
            pnlControles.Size = new Size(503, 38);
            pnlControles.TabIndex = 12;
            // 
            // BtnGenerarCotizacion
            // 
            BtnGenerarCotizacion.BackColor = Color.AntiqueWhite;
            BtnGenerarCotizacion.Location = new Point(262, 7);
            BtnGenerarCotizacion.Margin = new Padding(1);
            BtnGenerarCotizacion.Name = "BtnGenerarCotizacion";
            BtnGenerarCotizacion.Size = new Size(70, 23);
            BtnGenerarCotizacion.TabIndex = 26;
            BtnGenerarCotizacion.Text = "Cotizacion";
            BtnGenerarCotizacion.UseVisualStyleBackColor = false;
            //
            // BtnConfiguracion
            //
            BtnConfiguracion.Location = new Point(388, 7);
            BtnConfiguracion.Margin = new Padding(3, 2, 3, 2);
            BtnConfiguracion.Name = "BtnConfiguracion";
            BtnConfiguracion.Size = new Size(110, 23);
            BtnConfiguracion.TabIndex = 27;
            BtnConfiguracion.Text = "Configuración";
            BtnConfiguracion.UseVisualStyleBackColor = true;
            BtnConfiguracion.Click += BtnConfiguracion_Click;
            //
            // pnlDetalleProducto
            //
            pnlDetalleProducto.BackColor = Color.WhiteSmoke;
            pnlDetalleProducto.BorderStyle = BorderStyle.FixedSingle;
            pnlDetalleProducto.Controls.Add(BtnCargarImagen);
            pnlDetalleProducto.Controls.Add(pbProducto);
            pnlDetalleProducto.Controls.Add(txtCodigo);
            pnlDetalleProducto.Controls.Add(txtDescripcion);
            pnlDetalleProducto.Controls.Add(txtPrecio);
            pnlDetalleProducto.Controls.Add(txtPrecioA);
            pnlDetalleProducto.Controls.Add(txtPrecioB);
            pnlDetalleProducto.Controls.Add(txtPrecioC);
            pnlDetalleProducto.Controls.Add(txtCantidad);
            pnlDetalleProducto.Controls.Add(txtSubdescripcion1);
            pnlDetalleProducto.Controls.Add(txtDescripcionAmpliada);
            pnlDetalleProducto.Location = new Point(519, 3);
            pnlDetalleProducto.Margin = new Padding(3, 2, 3, 2);
            pnlDetalleProducto.Name = "pnlDetalleProducto";
            pnlDetalleProducto.Size = new Size(243, 563);
            pnlDetalleProducto.TabIndex = 18;
            // 
            // pbProducto
            // 
            pbProducto.BackColor = Color.WhiteSmoke;
            pbProducto.Location = new Point(3, 4);
            pbProducto.Margin = new Padding(3, 2, 3, 2);
            pbProducto.Name = "pbProducto";
            pbProducto.Size = new Size(234, 234);
            pbProducto.SizeMode = PictureBoxSizeMode.Zoom;
            pbProducto.TabIndex = 0;
            pbProducto.TabStop = false;
            pbProducto.Click += PbProducto_Click;
            // 
            // txtCodigo
            // 
            txtCodigo.BackColor = SystemColors.ButtonFace;
            txtCodigo.BorderStyle = BorderStyle.FixedSingle;
            txtCodigo.Location = new Point(3, 277);
            txtCodigo.Margin = new Padding(3, 2, 3, 2);
            txtCodigo.Name = "txtCodigo";
            txtCodigo.ReadOnly = true;
            txtCodigo.Size = new Size(234, 23);
            txtCodigo.TabIndex = 2;
            // 
            // txtDescripcion
            // 
            txtDescripcion.BackColor = SystemColors.ButtonFace;
            txtDescripcion.BorderStyle = BorderStyle.FixedSingle;
            txtDescripcion.Location = new Point(3, 246);
            txtDescripcion.Margin = new Padding(3, 2, 3, 2);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.ReadOnly = true;
            txtDescripcion.Size = new Size(234, 23);
            txtDescripcion.TabIndex = 4;
            // 
            // txtPrecio
            // 
            txtPrecio.BackColor = SystemColors.ButtonFace;
            txtPrecio.BorderStyle = BorderStyle.FixedSingle;
            txtPrecio.Location = new Point(3, 309);
            txtPrecio.Margin = new Padding(3, 2, 3, 2);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.ReadOnly = true;
            txtPrecio.Size = new Size(234, 23);
            txtPrecio.TabIndex = 6;
            // 
            // txtPrecioA
            // 
            txtPrecioA.BackColor = SystemColors.ButtonFace;
            txtPrecioA.BorderStyle = BorderStyle.FixedSingle;
            txtPrecioA.Location = new Point(3, 339);
            txtPrecioA.Margin = new Padding(3, 2, 3, 2);
            txtPrecioA.Name = "txtPrecioA";
            txtPrecioA.ReadOnly = true;
            txtPrecioA.Size = new Size(234, 23);
            txtPrecioA.TabIndex = 8;
            // 
            // txtPrecioB
            // 
            txtPrecioB.BackColor = SystemColors.ButtonFace;
            txtPrecioB.BorderStyle = BorderStyle.FixedSingle;
            txtPrecioB.Location = new Point(3, 368);
            txtPrecioB.Margin = new Padding(3, 2, 3, 2);
            txtPrecioB.Name = "txtPrecioB";
            txtPrecioB.ReadOnly = true;
            txtPrecioB.Size = new Size(234, 23);
            txtPrecioB.TabIndex = 10;
            // 
            // txtPrecioC
            // 
            txtPrecioC.BackColor = SystemColors.ButtonFace;
            txtPrecioC.BorderStyle = BorderStyle.FixedSingle;
            txtPrecioC.Location = new Point(3, 397);
            txtPrecioC.Margin = new Padding(3, 2, 3, 2);
            txtPrecioC.Name = "txtPrecioC";
            txtPrecioC.ReadOnly = true;
            txtPrecioC.Size = new Size(234, 23);
            txtPrecioC.TabIndex = 12;
            // 
            // txtCantidad
            // 
            txtCantidad.BackColor = SystemColors.ButtonFace;
            txtCantidad.BorderStyle = BorderStyle.FixedSingle;
            txtCantidad.Location = new Point(3, 428);
            txtCantidad.Margin = new Padding(3, 2, 3, 2);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.ReadOnly = true;
            txtCantidad.Size = new Size(234, 23);
            txtCantidad.TabIndex = 16;
            // 
            // txtSubdescripcion1
            // 
            txtSubdescripcion1.BackColor = SystemColors.ButtonFace;
            txtSubdescripcion1.BorderStyle = BorderStyle.FixedSingle;
            txtSubdescripcion1.Location = new Point(3, 486);
            txtSubdescripcion1.Margin = new Padding(3, 2, 3, 2);
            txtSubdescripcion1.Multiline = true;
            txtSubdescripcion1.Name = "txtSubdescripcion1";
            txtSubdescripcion1.ReadOnly = true;
            txtSubdescripcion1.Size = new Size(234, 21);
            txtSubdescripcion1.TabIndex = 18;
            // 
            // txtDescripcionAmpliada
            // 
            txtDescripcionAmpliada.BackColor = SystemColors.ButtonFace;
            txtDescripcionAmpliada.BorderStyle = BorderStyle.FixedSingle;
            txtDescripcionAmpliada.Location = new Point(3, 457);
            txtDescripcionAmpliada.Margin = new Padding(3, 2, 3, 2);
            txtDescripcionAmpliada.Multiline = true;
            txtDescripcionAmpliada.Name = "txtDescripcionAmpliada";
            txtDescripcionAmpliada.ReadOnly = true;
            txtDescripcionAmpliada.Size = new Size(234, 21);
            txtDescripcionAmpliada.TabIndex = 20;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(770, 573);
            Controls.Add(pnlFiltros);
            Controls.Add(pnlResultados);
            Controls.Add(pnlControles);
            Controls.Add(pnlDetalleProducto);
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            Margin = new Padding(3, 2, 3, 2);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Utilidades Motomania";
            ((ISupportInitialize)dgvProductos).EndInit();
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            pnlResultados.ResumeLayout(false);
            pnlControles.ResumeLayout(false);
            pnlDetalleProducto.ResumeLayout(false);
            pnlDetalleProducto.PerformLayout();
            ((ISupportInitialize)pbProducto).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button BtnGenerarCotizacion;
        private Label lblProductosSeleccionados;
    }
}
