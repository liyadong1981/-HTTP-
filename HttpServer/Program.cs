﻿using System;
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

        /// <summary>
        /// HTTP请求的类型
        /// </summary>
        /// <remarks>GET or POST</remarks>
        public String http_method;
        /// <summary>
        /// http请求的资源
        /// </summary>
        public String http_url;
        /// <summary>
        /// HTTP请求的版本
        /// </summary>
        public String http_protocol_versionstring;
        /// <summary>
        /// HTTP头部信息
        /// </summary>
        /// <remarks>名称：值</remarks>
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
        /// <remarks>
        /// HTTP请求的格式如下所示：
        /// request-line
        /// headers
        /// blank line
        /// request-body
        /// 在HTTP请求中，第一行必须是一个请求行（request line），用来说明请求类型、要访问的资源以及使用的HTTP版本。紧接着是一个首部（header）小节，用来说明服务器要使用的附加信息。在首部之后是一个空行，再此之后可以添加任意的其他数据[称之为主体（body）]。
        /// GET / HTTP/1.1
        /// Host: www.baidu.com
        /// User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.7.6)
        /// Gecko/20050225 Firefox/1.0.1
        /// Connection: Keep-Alive
        /// </remarks>
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
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine( DateTime.Now.ToString());
                //处理HTTP请求行 --第一行
                parseRequest();
                //处理HTTP请求头部信息，将头部信息的名称及值放入数组httpHeaders中
                readHeaders();
                if (http_method.Equals("GET"))
                {//HTTP请求采用GET方式
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {//HTTP请求采用POST方式
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("产生异常: " + e.ToString());
                //输出失败信息
                writeFailure();
            }
            //清理当前编写器的所有缓冲区，并使所有缓冲数据写入基础流
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null;
            outputStream = null; // bs = null;            
			
			//关闭TCP客户端连接 
			socket.Close();
		}

        /// <summary>
        /// 处理HTTP请求行
        /// </summary>
        public void parseRequest()
        {
            //从TCP客户端连接的输入流中读取一行数据    判断HTTP请求行
            String request = streamReadLine(inputStream);
            //以空格为分隔符分割字符串
            string[] tokens = request.Split(' ');

            /*用于接收的  参数输出 即HTTP请求
            Console.WriteLine("-------------------------------------------------------------");
            foreach (var s in tokens)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("-------------------------------------------------------------");
            */
            if (tokens.Length != 3)
            {//如果字符串数组长度不为3的话，产生异常，正常的http请求字符串为：GET / HTTP/1.1
                throw new Exception("无效的HTTP请求行");
            }
            //取得http请求方法  GET
            http_method = tokens[0].ToUpper();
            //http请求的资源
            http_url = tokens[1];
            //http请求的版本号
            http_protocol_versionstring = tokens[2];
            //输出HTTP请求行
            Console.WriteLine("http请求开始 " + request);
        }

        /// <summary>
        /// 处理HTTP请求头部信息
        /// </summary>
        public void readHeaders()
        {
            Console.WriteLine("读取HTTP头部信息:");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {//不断从TCP客户端流中读取数据行
                if (line.Equals(""))
                {//读到空行，说明已经获取完HTTP头部信息
                    Console.WriteLine("已经获取HTTP头部信息");
                    return;
                }
                //取得分隔符
                int separator = line.IndexOf(':');
                if (separator == -1)
                {//如果在头部信息的每一行中没有：符号，说明HTTP有误
                    throw new Exception("无效的HTTP头部信息: " + line);
                }
                //取得HTTP头部信息每一行的名称
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {//去除：符号后边的空格
                    pos++; // strip any spaces
                }
                //取得HTTP头部信息每一行的值
                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("头部信息: {0}:{1}", name, value);
                //将头部信息的名称及值放入表中
                httpHeaders[name] = value;
            }
        }

        /// <summary>
        /// 返回HTTP GET信息
        /// </summary>
        /// <remarks>直接调用传入的HTTP对象的函数</remarks>
        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        /// <summary>
        /// 缓存区
        /// </summary>
        /// <remarks>默认缓存为10M</remarks>
        private const int BUF_SIZE = 4096;
        /// <summary>
        /// 返回HTTP POST信息
        /// </summary>
        /// <remarks>先调用类内部的函数处理POST发送过来的数据，在调用传入的 HTTP对象中的函数</remarks>
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("开始取得POST数据");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();                          
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {//如果HTTP头部信息中包含长度信息
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);    //得出数据长度
                if (content_len > MAX_POST_SIZE)
                {//如果数据长度大于  
                    throw new Exception(String.Format("发送的数据长度为({0}) 超出服务器接收能力", content_len));
                }
                byte[] buf = new byte[BUF_SIZE];                     //开辟数据缓冲区
                int to_read = content_len; 
                while (to_read > 0)
                {
                    Console.WriteLine("开始读取POST数据, 需要读取数据长度={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("读取数据完毕, 已经读取数据长度={0}", numread);
                    if (numread == 0)
                    {//数据读取完毕   --经过测试此段代码无执行的可能性
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;                          //重新计算需要读取的数据长度
                    ms.Write(buf, 0, numread);                 //将读取的数据写入内存
                }
                ms.Seek(0, SeekOrigin.Begin);                //将内存流定位到开始位置
				{//将接收的数据保存文件
					FileStream fs = new FileStream(@"c:\a.txt", FileMode.OpenOrCreate);
					byte[] buff = ms.ToArray();
					fs.Write(buff, 0, buff.Length);
					fs.Flush();
					fs.Close();
				}

			}
            Console.WriteLine("获取POST数据完毕");
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        /// <summary>
        /// 返回HTTP请求成功信息
        /// </summary>
        public void writeSuccess()
        {
            outputStream.WriteLine("HTTP/1.0 200 OK");
            outputStream.WriteLine("Content-Type: application/xml");
            outputStream.WriteLine("Connection: close");
            outputStream.WriteLine("");
        }

        /// <summary>
        /// 返回HTTP请求失败信息
        /// </summary>
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
        /// <summary>
        /// 返回HTTP GET请求的信息
        /// </summary>
        public override void handleGETRequest(HttpProcessor p)
        {
            Console.WriteLine("返回GET请求信息: {0}", p.http_url);
            p.writeSuccess();
            p.outputStream.WriteLine("<html><body><h1>返回GET请求</h1>");
            p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
            p.outputStream.WriteLine("url : {0}", p.http_url);

            p.outputStream.WriteLine("<form method=post action=/form>");
            p.outputStream.WriteLine("<input type=text name=foo value=''>");
            p.outputStream.WriteLine("<input type=submit name=bar value=点击进行POST测试>");
            p.outputStream.WriteLine("</form>");
        }

        /// <summary>
        /// 返回HTTP POST请求的信息
        /// </summary>
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            Console.WriteLine("返回POST请求信息: {0}", p.http_url);
			p.writeSuccess();

			// string data = inputData.ReadToEnd();

			//  p.outputStream.WriteLine("<html><body><h1>test server</h1>");
			//  p.outputStream.WriteLine("<a href=/test>return</a><p>");
			// p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);
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
                httpServer = new MyHttpServer(8989);
            }
            //生成一个线程，并设置委托函数
            Thread thread = new Thread(new ThreadStart(httpServer.listen));
            //启动线程
            thread.Start();
            return;
        }
    }
}
