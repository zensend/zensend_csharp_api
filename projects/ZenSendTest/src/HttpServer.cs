using System.Net;
using System;
using System.Text;
using System.IO;

namespace ZenSendTest {
	
	public class HttpServer : IDisposable {
		
		private HttpListener listener;
		private HttpListenerRequest lastRequest;
		private byte[] lastBody;
		
		
		private string contentType;
		private int statusCode;
		private string body;
		
		private const string URL = "http://127.0.0.1:10008";
		
		public HttpServer() {
			this.listener = new HttpListener();
			listener.Prefixes.Add(URL + "/");
			listener.Start();
			listener.BeginGetContext(new AsyncCallback(ListenerCallback), null);			
		}
		
		public string Url {
			get {
				return URL;
			}
		}
		
		public string LastBodyAsString {
			get {
				return Encoding.UTF8.GetString(lastBody);
			}
		}
		
		public HttpListenerRequest LastRequest {
			get {
				return lastRequest;
			}
		}
		
		public void SetResponse(string contentType, int statusCode, string body) {
			this.contentType = contentType;
			this.statusCode = statusCode;
			this.body = body;
		}
		private void ListenerCallback(IAsyncResult result) {
			var context = this.listener.EndGetContext(result);
			
			
			
			lastRequest = context.Request;
			
			if (context.Request.HasEntityBody) {
				lastBody = ReadFully(context.Request.InputStream);
			} else {
				lastBody = null;
			}
			

			var response = context.Response;
			response.StatusCode = this.statusCode;
			response.AddHeader("content-type", this.contentType);
			var buffer = Encoding.UTF8.GetBytes(this.body);
			response.OutputStream.Write(buffer, 0, buffer.Length);
			response.OutputStream.Close();
		}
		
		public void Dispose() {
			this.listener.Stop();
		}
		
		private byte[] ReadFully(Stream stream) {
			MemoryStream mem = new MemoryStream();
			stream.CopyTo(mem);
			var buffer = mem.ToArray();
			mem.Dispose();
			return buffer;
		}

	}
}