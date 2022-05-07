using GalaSoft.MvvmLight.Command;
using HeroCoMVVM.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeroCoMVVM.ViewModels
{
    public class SuperHeroesViewModels : INotifyPropertyChanged
    {
        public ObservableCollection<SuperHeroe> ListaSuperHeroes { get; set; } = new ObservableCollection<SuperHeroe>();
        private SuperHeroe superheroe;
        public SuperHeroe SuperHeroe {
            get { return superheroe; }
            set
            {
                superheroe = value;
                PropertyChange("SuperHeroe");
            }
        }
        public string Mensaje { get; set; }
        public string Vista { get; set; }="ver";
        public ICommand CancelarCommand { get; set; }
        public ICommand CambiarVistaCommand { get; set; }
        public ICommand AgregarCommand { get; set; }
        public ICommand EditarCommand { get; set; } 
        public ICommand EliminarCommand { get; set; }

        public SuperHeroesViewModels()
        {
            Abrir();
            CancelarCommand = new RelayCommand(Cancelar);
            CambiarVistaCommand = new RelayCommand<string>(CambiarVista);
            AgregarCommand = new RelayCommand(Agregar);
            EditarCommand = new RelayCommand(Editar);
            EliminarCommand = new RelayCommand(Eliminar);
        }
        public void Agregar()
        {
            Mensaje="";
            if (SuperHeroe != null)
            {
                if (string.IsNullOrWhiteSpace(SuperHeroe.NombreVideojuego))
                {
                    Mensaje="Debe ingresar el Nombre del Videojuego";
                    PropertyChange("Mensaje");
                }
                if (string.IsNullOrWhiteSpace(SuperHeroe.SuperHeroePrincipal))
                {
                    Mensaje = "Debe ingresar el o los Superheroes principales";
                    PropertyChange("Mensaje");
                }
                if (SuperHeroe.AñoLanzamiento<1950)
                {
                    Mensaje = "Debe ingresar un Año de lanzamiento valido";
                    PropertyChange("Mensaje");
                }
                if (SuperHeroe.EdadMinima<12)
                {
                    Mensaje = "Debe ingresar una edad minima mayor de 12 años";
                    PropertyChange("Mensaje");
                }
                if (!Uri.TryCreate(SuperHeroe.Imagen, UriKind.Absolute, out var uri))
                {
                    Mensaje = "Escriba una URL de la imagen valida";
                    PropertyChange("Mensaje");

                }
                if (Mensaje == "")
                {
                    ListaSuperHeroes.Add(SuperHeroe);
                    Guardar();
                    CambiarVista("ver");
                }
            }
        }
        public void Cancelar()
        {
            SuperHeroe = null;
            Guardar();
            CambiarVista("ver");
        }
        public void Eliminar()
        {
            if (SuperHeroe != null)
            {
                ListaSuperHeroes.Remove(SuperHeroe);
                Guardar();
                PropertyChange();
                Cancelar();
            }
        }
        int posicionOriginal;

        public void Editar()
        {
            if (SuperHeroe != null)
            {
                ListaSuperHeroes[posicionOriginal] = SuperHeroe;
                Guardar();
                CambiarVista("ver");
                
            }
        }
        public void Guardar()
        {
            var json = JsonConvert.SerializeObject(ListaSuperHeroes);
            File.WriteAllText("superheroes.json", json);
        }
        public void Abrir()
        {
            if (File.Exists("superheroes.json"))
            {
                var json = File.ReadAllText("superheroes.json");
                var datos = JsonConvert
                    .DeserializeObject<ObservableCollection<SuperHeroe>?>(json);

                if (datos != null)
                {
                    ListaSuperHeroes = datos;
                }
                else
                {
                    ListaSuperHeroes = new ObservableCollection<SuperHeroe>();
                }
            }
        }
        public void CambiarVista(string vista)
        {
            Vista = vista;
            if (Vista == "agregar")
            {
                SuperHeroe = new SuperHeroe();
            }
            if (Vista == "editar")
            {
                posicionOriginal = ListaSuperHeroes.IndexOf(SuperHeroe);
                if (SuperHeroe != null)
                {
                    var clon = new SuperHeroe()
                    {
                        NombreVideojuego = SuperHeroe.NombreVideojuego,
                        SuperHeroePrincipal = SuperHeroe.SuperHeroePrincipal,
                        Descripcion = SuperHeroe.Descripcion,
                        EdadMinima = SuperHeroe.EdadMinima,
                        AñoLanzamiento = SuperHeroe.AñoLanzamiento,
                        Imagen = SuperHeroe.Imagen
                    };
                    SuperHeroe = clon;
                }
            }
            PropertyChange();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void PropertyChange(string? propiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));
        }
    }
}
