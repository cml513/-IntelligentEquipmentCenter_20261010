using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI编写
{
   public  class HttpServerAPI
    {
			private int port = 8000;

			private HttpListener listener;

			private Encoding encoding = Encoding.UTF8;

			private Func<HttpListenerRequest, HttpListenerResponse, string, string> handleRequestFunc;

			/// <summary>
			/// 当前服务器的编码信息
			/// </summary>
			public Encoding ServerEncoding
			{
				get
				{
					return encoding;
				}
				set
				{
					encoding = value;
				}
			}

			/// <summary>
			/// 获取或设置是否支持跨域操作。
			/// </summary>
			public bool IsCrossDomain
			{
				get;
				set;
			}

			/// <summary>
			/// 当前的自定义的处理信息
			/// </summary>
			public Func<HttpListenerRequest, HttpListenerResponse, string, string> HandleRequestFunc
			{
				get
				{
					return handleRequestFunc;
				}
				set
				{
					handleRequestFunc = value;
				}
			}

			/// <summary>
			/// 启动服务器
			/// </summary>
			/// <param name="port">端口号信息</param>
			/// <exception cref="T:System.Net.HttpListenerException"></exception>
			/// <exception cref="T:System.ObjectDisposedException"></exception>
			public void Start(int port)
			{
				this.port = port;
				listener = new HttpListener();
				listener.Prefixes.Add($"http://192.168.20.90:{port}/");
				listener.Start();
				listener.BeginGetContext(GetConnectCallBack, listener);
			}

			/// <summary>
			/// 关闭服务器
			/// </summary>
			public void Close()
			{
				listener?.Close();
			}

			private void GetConnectCallBack(IAsyncResult ar)
			{
				HttpListener httpListener = ar.AsyncState as HttpListener;
				if (httpListener == null)
				{
					return;
				}
				HttpListenerContext httpListenerContext = null;
				try
				{
					httpListenerContext = httpListener.EndGetContext(ar);
				}
				catch (Exception ex)
				{

				}
				int num = 0;
				while (true)
				{
					try
					{
						httpListener.BeginGetContext(GetConnectCallBack, httpListener);
					}
					catch (Exception ex2)
					{
						Thread.Sleep(1000);
						continue;
					}
					break;
				}
				if (httpListenerContext != null)
				{
					HttpListenerRequest request = httpListenerContext.Request;
					HttpListenerResponse response = httpListenerContext.Response;
					if (IsCrossDomain)
					{
						httpListenerContext.Response.AppendHeader("Access-Control-Allow-Origin", request.Headers["Origin"]);
						httpListenerContext.Response.AppendHeader("Access-Control-Allow-Headers", "*");
						httpListenerContext.Response.AppendHeader("Access-Control-Allow-Method", "POST,GET,PUT,OPTIONS,DELETE");
						httpListenerContext.Response.AppendHeader("Access-Control-Allow-Credentials", "true");
						httpListenerContext.Response.AppendHeader("Access-Control-Max-Age", "3600");
					}
					httpListenerContext.Response.AddHeader("Content-type", "Content-Type: application/json; charset=utf-8");
					string dataFromRequest = GetDataFromRequest(request);
					response.StatusCode = 200;
					try
					{
						string text = HandleRequest(request, response, dataFromRequest);
						using (Stream stream = response.OutputStream)
						{
							if (string.IsNullOrEmpty(text))
							{
								stream.Write(new byte[0], 0, 0);
							}
							else
							{
								byte[] bytes = encoding.GetBytes(text);
								stream.Write(bytes, 0, bytes.Length);
							}
						}
					}
					catch (Exception ex3)
					{

					}
				}
			}

			private string GetDataFromRequest(HttpListenerRequest request)
			{
				try
				{
					List<byte> list = new List<byte>();
					byte[] array = new byte[2048];
					int num = 0;
					int num2 = 0;
					do
					{
						num = request.InputStream.Read(array, 0, array.Length);
						num2 += num;
						list.AddRange(array);
					}
					while (num != 0);
					return encoding.GetString(list.ToArray(), 0, num2);
				}
				catch
				{
					return string.Empty;
				}
			}

			/// <summary>
			/// 支持重写的请求
			/// </summary>
			/// <param name="request">请求</param>
			/// <param name="response">回应</param>
			/// <param name="data">Body数据</param>
			/// <returns>返回的内容</returns>
			protected virtual string HandleRequest(HttpListenerRequest request, HttpListenerResponse response, string data)
			{
				if (HandleRequestFunc != null)
				{
					return HandleRequestFunc(request, response, data);
				}
				return "Thank you for use!";
			}

			/// <summary>
			/// 返回表示当前对象的字符串
			/// </summary>
			/// <returns>字符串</returns>
			public override string ToString()
			{
				return $"HttpServer[{port}]";
			}



    }
}
