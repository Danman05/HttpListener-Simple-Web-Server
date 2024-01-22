using System.Net;
using System.Text;

namespace Webserver
{
    public class Server
    {
        /// <summary>
        /// Setup of server
        /// </summary>
        public void Start()
        {

            // Check for support issues
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();


            while (true)
            {
                Console.WriteLine("Listening...");
                // GetContext method blocks while waiting for a request
                HttpListenerContext context = listener.GetContext();
                Console.WriteLine("Client has connected");
                HandleClient(context);
            }

        }

        private void HandleClient(HttpListenerContext context)
        {
            // Request
            HttpListenerRequest request = context.Request;


            string requestedUrl = request.Url.LocalPath;
            string filePath = "{path}" + requestedUrl;


            // Response

            HttpListenerResponse response = context.Response;

            if (requestedUrl.Length > 1) //  Look for specific file if localpath has value
            {
                try
                {
                    if (File.Exists(filePath)) // Response by content
                    {
                        // Read content from file
                        string fileContent = File.ReadAllText(filePath);
                        SendResponse(response, fileContent, HttpStatusCode.OK);
                    }
                    else // Not Found Respond
                    {
                        SendResponse(response, SimpleHtmlResponse("<h1>404 - Not Found</h1>"), HttpStatusCode.NotFound);
                    }
                } 
                catch (Exception) // Bad Request on catch
                {
                    SendResponse(response, SimpleHtmlResponse("<h1>400 - Bad Request></h1>"), HttpStatusCode.BadRequest);
                    throw;
                }
            }
            else // Default html response
            {

                // Simple HTML page with client information
                string htmlContent = $@"
            <html>
                <body>
                    <h1>Hello, World!</h1>
                    <p>Client IP: {request.RemoteEndPoint.Address}</p>
                    <p>User-Agent (Browser): {request.UserAgent}</p>
                    <p>Protocol Version: {request.ProtocolVersion}</p>
                </body>
            </html>";

                SendResponse(response, htmlContent, HttpStatusCode.OK);
            }
        }

        /// <summary>
        /// Send response to client
        /// </summary>
        /// <param name="response"></param>
        /// <param name="content"></param>
        /// <param name="statusCode"></param>
        public void SendResponse(HttpListenerResponse response, string content, HttpStatusCode statusCode)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.StatusCode = (int)statusCode;
            response.ContentLength64 = buffer.Length;
            response.ContentType = "text/html";

            // Get the output stream and write the response
            using (Stream outputStream = response.OutputStream)
            {
                outputStream.Write(buffer, 0, buffer.Length);
            }
            response.Close();
        }


        /// <summary>
        /// Simple HTML template for messages
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string SimpleHtmlResponse(string bodyContent)
        {
            return $@"
<html>
 <body>
{bodyContent}
</body>
</html";
        }
    }
}