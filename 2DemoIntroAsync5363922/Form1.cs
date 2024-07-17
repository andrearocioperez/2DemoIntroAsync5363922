using System.Diagnostics;

namespace _2DemoIntroAsync5363922
{
    public partial class Form1 : Form
    {
        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;


            var directorioActual = AppDomain.CurrentDomain.BaseDirectory;
            var destinoBaseSecuencial = Path.Combine(directorioActual, @"Imagenes\resultado-secuencial");
            var destinoBaseParalelo = Path.Combine(directorioActual, @"Imagenes\resultado-paralelo");
            PrepararEjecucion(destinoBaseParalelo, destinoBaseSecuencial);

            Console.WriteLine("Inicio");
            List<Imagen> imagenes = ObtenerImagenes();

          var sw = new Stopwatch();

            sw.Start();

            foreach (var imagen in imagenes)
            {
                await ProcesarImagen(destinoBaseSecuencial, imagen);
            }

            Console.WriteLine("Secuencial - duración en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Reset();

            sw.Start();

            var tareasEnumerable = imagenes.Select(async imagen =>
            {
                await ProcesarImagen(destinoBaseParalelo, imagen);
            });

            await Task.WhenAll(tareasEnumerable);

            Console.WriteLine("Paralelos - duración en segundos: {0}",
                sw.ElapsedMilliseconds / 1000.0);

            sw.Stop();


            pictureBox1.Visible = false;
        }

        private async Task  ProcesarImagen(string directorio, Imagen imagen) 
        {
            var respuesta = await httpClient.GetAsync(imagen.URL);
            var contenido = await respuesta.Content.ReadAsByteArrayAsync();

            Bitmap bitmap;
            using (var ms  = new MemoryStream(contenido))
            {
                bitmap = new Bitmap(ms);
            }
            bitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            var destino = Path.Combine(directorio, imagen.Nombre);
            bitmap.Save(destino);
        }

        private static List<Imagen> ObtenerImagenes()
        {
            var imagenes = new List<Imagen>();

            for (int i = 0; i < 7; i++)
            {
                imagenes.Add(
                    new Imagen()
                    {
                        Nombre = $"Bandera {i}.jpg",
                        URL = "https://th.bing.com/th/id/R.63332a5c8b9e27962b2918e2b7aa47f6?rik=nMQzCgEE6gJPvw&riu=http%3a%2f%2fwww.clickgratis.com.br%2f_upload%2fwallpapers%2f2012%2f11%2f01%2flinda-paisagem-5091e25e13b73.jpg&ehk=xfSckpMImQ1ji0FZjk6lBrwUsMO3P8QU2ivWFJyJF3Q%3d&risl=&pid=ImgRaw&r=0"
                    });

                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Tazumal {i}.jpg",
                    URL = "https://i.pinimg.com/736x/bf/55/17/bf551796a8605452e36845e1a968e567.jpg"
                });

                imagenes.Add(
                new Imagen()
                {
                    Nombre = $"Escudo {i}.jpeg",
                    URL = "https://th.bing.com/th/id/OIP.Z9RnQzp2fusRYSkip-FXmQHaE8?rs=1&pid=ImgDetMain"
                });
        }

            return imagenes;
        }

        private void BorrarArchivos(string directorio)
        {
            var archivos = Directory.EnumerateFiles(directorio);
            foreach (var archivo in archivos)
            {
                File.Delete(archivo);
            }
        }

        private void PrepararEjecucion(string destinoBaseParalelo, string destinoBaseSecuencial)
        {
            if (!Directory.Exists(destinoBaseParalelo))
            {
                Directory.CreateDirectory(destinoBaseParalelo);
            }

            if (!Directory.Exists(destinoBaseSecuencial))
            {
                Directory.CreateDirectory (destinoBaseSecuencial);
            }

            BorrarArchivos(destinoBaseSecuencial);
            BorrarArchivos(destinoBaseParalelo);
        }

        private async Task<string> ProcesamientoLargo()
        {
            await Task.Delay(3000);
            return "Felipe";
        }

        private async Task ProcesamientoLargoA()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso A Finalizado");
        }

        private async Task ProcesamientoLargoB()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso B Finalizado");
        }

        private async Task ProcesamientoLargoC()
        {
            await Task.Delay(1000);
            Console.WriteLine("Proceso C Finalizado");
        }
    }
}
