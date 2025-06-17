using System.ComponentModel;

namespace CargaImagenes.UI
{
    partial class FormConexion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblServidor = new Label();
            txtServidor = new TextBox();
            lblBaseDatos = new Label();
            txtBaseDatos = new TextBox();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblContrasena = new Label();
            txtContrasena = new TextBox();
            chkGuardarConfig = new CheckBox();
            btnConectar = new Button();
            btnCancelar = new Button();
            SuspendLayout();

            // lblServidor
            lblServidor.AutoSize = true;
            lblServidor.Location = new Point(20, 20);
            lblServidor.Name = "lblServidor";
            lblServidor.Size = new Size(67, 20);
            lblServidor.Text = "Servidor:";

            // txtServidor
            txtServidor.Location = new Point(140, 17);
            txtServidor.Name = "txtServidor";
            txtServidor.Size = new Size(250, 27);
            txtServidor.TabIndex = 0;

            // lblBaseDatos
            lblBaseDatos.AutoSize = true;
            lblBaseDatos.Location = new Point(20, 60);
            lblBaseDatos.Name = "lblBaseDatos";
            lblBaseDatos.Size = new Size(101, 20);
            lblBaseDatos.Text = "Base de datos:";

            // txtBaseDatos
            txtBaseDatos.Location = new Point(140, 57);
            txtBaseDatos.Name = "txtBaseDatos";
            txtBaseDatos.Size = new Size(250, 27);
            txtBaseDatos.TabIndex = 1;

            // lblUsuario
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(20, 100);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(62, 20);
            lblUsuario.Text = "Usuario:";

            // txtUsuario
            txtUsuario.Location = new Point(140, 97);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(250, 27);
            txtUsuario.TabIndex = 2;

            // lblContrasena
            lblContrasena.AutoSize = true;
            lblContrasena.Location = new Point(20, 140);
            lblContrasena.Name = "lblContrasena";
            lblContrasena.Size = new Size(86, 20);
            lblContrasena.Text = "Contraseña:";

            // txtContrasena
            txtContrasena.Location = new Point(140, 137);
            txtContrasena.Name = "txtContrasena";
            txtContrasena.PasswordChar = '*';
            txtContrasena.Size = new Size(250, 27);
            txtContrasena.TabIndex = 3;

            // chkGuardarConfig
            chkGuardarConfig.AutoSize = true;
            chkGuardarConfig.Location = new Point(140, 180);
            chkGuardarConfig.Name = "chkGuardarConfig";
            chkGuardarConfig.Size = new Size(177, 24);
            chkGuardarConfig.TabIndex = 4;
            chkGuardarConfig.Text = "Guardar configuración";

            // btnConectar
            btnConectar.Location = new Point(100, 220);
            btnConectar.Name = "btnConectar";
            btnConectar.Size = new Size(120, 35);
            btnConectar.TabIndex = 5;
            btnConectar.Text = "Conectar";
            btnConectar.UseVisualStyleBackColor = true;
            btnConectar.Click += btnConectar_Click;

            // btnCancelar
            btnCancelar.Location = new Point(240, 220);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(120, 35);
            btnCancelar.TabIndex = 6;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;

            // FormConexion
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(420, 280);
            Controls.Add(lblServidor);
            Controls.Add(txtServidor);
            Controls.Add(lblBaseDatos);
            Controls.Add(txtBaseDatos);
            Controls.Add(lblUsuario);
            Controls.Add(txtUsuario);
            Controls.Add(lblContrasena);
            Controls.Add(txtContrasena);
            Controls.Add(chkGuardarConfig);
            Controls.Add(btnConectar);
            Controls.Add(btnCancelar);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            Name = "FormConexion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configuración de Conexión";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblServidor;
        private TextBox txtServidor;
        private Label lblBaseDatos;
        private TextBox txtBaseDatos;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblContrasena;
        private TextBox txtContrasena;
        private CheckBox chkGuardarConfig;
        private Button btnConectar;
        private Button btnCancelar;
    }
}