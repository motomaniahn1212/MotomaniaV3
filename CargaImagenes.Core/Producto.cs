using System.ComponentModel;
using System.Drawing;

namespace CargaImagenes.Core
{
    public class Producto : INotifyPropertyChanged
    {
        private bool _seleccionado;
        private decimal _precioFiltrado;

        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string DescripcionAmpliada { get; set; } = string.Empty;
        public string Subdescripcion1 { get; set; } = string.Empty;

        public decimal Precio { get; set; }
        public decimal PrecioFiltrado
        {
            get => _precioFiltrado;
            set
            {
                if (_precioFiltrado != value)
                {
                    _precioFiltrado = value;
                    OnPropertyChanged(nameof(PrecioFiltrado));
                }
            }
        }
        public decimal PrecioA { get; set; }
        public decimal PrecioB { get; set; }
        public decimal PrecioC { get; set; }
        public int Cantidad { get; set; }
        public int OrderQuantity { get; set; } = 0;

        public int? ProveedorId { get; set; }
        public int? CategoriaId { get; set; }
        public int? DepartamentoId { get; set; }

        public string? ProveedorNombre { get; set; }
        public string? CategoriaNombre { get; set; }
        public string? DepartamentoNombre { get; set; }

        public bool TieneImagen { get; set; }
        public Image? Imagen { get; set; }
        public Image? ImagenMiniatura { get; set; }
        public string? RutaImagen { get; set; }

        public bool Seleccionado
        {
            get => _seleccionado;
            set
            {
                if (_seleccionado != value)
                {
                    _seleccionado = value;
                    OnPropertyChanged(nameof(Seleccionado));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}