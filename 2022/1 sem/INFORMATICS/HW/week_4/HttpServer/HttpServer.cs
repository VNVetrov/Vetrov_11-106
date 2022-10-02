using System.Net;
using System.Text;

namespace NetConsoleApp
{
    public class HttpServer : IDisposable
    {
        private readonly string url;
        private readonly HttpListener listener;
        private HttpListenerContext? context;
        private bool isRunning = false;

        public HttpServer(string url)
        {
            this.url = url;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
        }

        public void Start()
        {
            if (isRunning)
            {
                Console.WriteLine("Сервер уже запущен");
                return;
            }
            Console.WriteLine("Запуск сервера...");
            listener.Start();
            Console.WriteLine("Сервер запущен");
            isRunning = true;
            Listening();
        }

        public void Restart()
        {
            Console.WriteLine("Перезапуск сервера...");
            Stop();
            Start();
        }

        public void Stop()
        {
            if (!isRunning)
            {
                Console.WriteLine("Сервер уже остановлен");
                return;
            }
            Console.WriteLine("Остановка сервера...");
            listener.Stop();
            Console.WriteLine("Сервер остановлен");
            isRunning = false;
        }
        private void Listening()
        {
            listener.BeginGetContext(new AsyncCallback(Listen), listener);
        }

        public void Listen(IAsyncResult result)
        {
            if (listener.IsListening)
            {
                context = listener.EndGetContext(result);
                HttpListenerRequest request = context.Request;
                // получаем объект ответа
                HttpListenerResponse response = context.Response;
                // создаем ответ в виде кода html
                var path = Directory.GetCurrentDirectory();
                byte[] buffer;
                if (Directory.Exists("C:\\Users\\Vladimir\\source\\repos\\HttpServer\\HttpServer\\web")){
                    response.Headers.Set("Content-Type", "text/html");
                    buffer = File.ReadAllBytes("C:\\Users\\Vladimir\\source\\repos\\HttpServer\\HttpServer\\web\\google.html");
                }
                else
                {
                    response.Headers.Set("Content-Type", "text/plain");
                    buffer = Encoding.UTF8.GetBytes("404 - Not Found");
                }
                // получаем поток ответа и пишем в него ответ
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // закрываем поток
                output.Close();
                Listening();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
