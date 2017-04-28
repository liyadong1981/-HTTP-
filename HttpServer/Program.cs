using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    /// <summary>
	/// 自定义HTTP处理类
	/// </summary>
	public class HttpProcessor
    {

        /// <summary>
        /// 传入的TCP客户端连接对象
        /// </summary>
        public TcpClient socket;
        /// <summary>
        /// 传入的HTTP服务器对象
        /// </summary>
        public HttpServer srv;

        /// <summary>
        /// 传入的TCP客户端连接输入流
        /// </summary>
        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <remarks>初始化类所需要的TCP客户端连接及HTTP服务器对象</remarks>
        /// <param name="s">传入的TCP客户端</param>
        /// <param name="srv">传入的HTTP服务器对象</param>
        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        /// <summary>
        /// 从TCP客户端连接的输入流中读取一行数据
        /// </summary>
        /// <returns>返回读取的一行数据</returns>
        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                //从流中读取一个字节
                next_char = inputStream.ReadByte();
                if (next_char == '\n')
                {//遇到换行符
                    break;
                }
                if (next_char == '\r')
                {//遇到回车符
                    continue;
                }
                if (next_char == -1)
                {//读取到流的末尾
                    Thread.Sleep(1);   //延时1毫秒
                    continue;               //继续循环
                };
                data += Convert.ToChar(next_char);   //将读取的数据放入数据区最后
            }
            //返回读取的数据
            return data;
        }
        /// <summary>
        /// 主要的处理函数，用于被多线程调用
        /// </summary>
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            //取得TCP客户端连接对象的发送和接收数据的流对象 ，
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            //从TCP客户端连接的输入流中读取一行数据
            String request = streamReadLine(inputStream);
            //以空格为分隔符分割字符串
            string[] tokens = request.Split(' ');
            Console.WriteLine("-------------------------------------------------------------");
            foreach (var s in tokens)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("-------------------------------------------------------------");
            if (tokens.Length != 3)
            {//如果字符串数组长度不为3的话，产生异常，正常的http请求字符串为：GET / HTTP/1.1
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        /// <summary>
        /// 缓存区
        /// </summary>
        /// <remarks>默认缓存为10M</remarks>
        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeSuccess()
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: text/html");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        public void writeFailure()
        {
            outputStream.WriteLine("HTTP/1.0 404 File not found");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }
    }
    /// <summary>
    /// HTTP服务器
    /// </summary>
    public abstract class HttpServer
    {

        /// <summary>
        /// 服务器监听端口号
        /// </summary>
        /// <remarks></remarks>
        protected int port;

        /// <summary>
        /// 侦听TCP客户端连接
        /// </summary>
        TcpListener listener;

        /// <summary>
        /// 服务器是否处于活动状态
        /// </summary>
        /// <remarks>默认状态为活动</remarks>
        bool is_active = true;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="port">传入进行监听的端口号</param>
        public HttpServer(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void listen()
        {
            //建立TCP服务器监听指定的端口 
            listener = new TcpListener(port);
            //开始侦听传入的连接请求
            listener.Start();
            while (is_active)
            {//当服务器处于活动状态时
             //取得可用于发送和接收数据的 TcpClient
                TcpClient s = listener.AcceptTcpClient();
                //生成自定义的HTTP处理对象，并初始化
                HttpProcessor processor = new HttpProcessor(s, this);
                //生成一个独立线程，并传入线程所要调用的方法
                Thread thread = new Thread(new ThreadStart(processor.process));
                //执行线程
                thread.Start();
                Thread.Sleep(1);
            }
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
    }

    public class MyHttpServer : HttpServer
    {
        public MyHttpServer(int port) : base(port)
        {
        }
        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("request: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);

            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
            p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
            p.outputStream.WriteLine("</form>");
        }

        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("POST request: {0}", p.http_url);
            string data = inputData.ReadToEnd();

            p.outputStream.WriteLine("<html><body><h1>test server</h1>");
            p.outputStream.WriteLine("<a href=/test>return</a><p>");
            p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer httpServer;
            if (args.GetLength(0) > 0)
            {//根据命令行参数确定服务器端口号
                httpServer = new MyHttpServer(Convert.ToInt16(args[0]));
            }
            else
            {//默认服务器端口号为8080
                httpServer = new MyHttpServer(8080);
            }
            //生成一个线程，并设置委托函数
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            //启动线程
            thread.Start();
            return;
        }
    }
}
