using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;
using System.Management;
using System.Threading;





namespace Sync_FtpToOracle
{
    
    // класс работы с файлом в котором список документов
    class FileWork
    {
        public string path { get; set; }
        public string dir { get; set; }

        public FileWork (string path,string dir)
        {
            this.path = path;
            this.dir = dir;
        }

        //метод считывает данные из файла в массив и возвращает лист документов с заполненными данными
        public List<OrderdocApp> FileIn()
        {
            List<OrderdocApp> ordersImport = new List<OrderdocApp>();
            string [] fileData = File.ReadAllLines(path,Encoding.Default);
            foreach (string line in fileData)
            {
                OrderdocApp order = new OrderdocApp();
                string[] pars = line.Split(new char[] {' '});
                order.ID = Convert.ToInt32(pars[0]);
                order.OOS_DOC_NUMBER = pars[1];
                ordersImport.Add(order);
            }

            return ordersImport;
        }
        public List<xml> InDir()
        {
            List<xml> listXml = new List<xml>();
            string[] files = Directory.GetFiles(dir, "*.xml");
            foreach (string xmlName in files)
            {
               xml xml = new xml();
                xml.path = xmlName;
               string[] pars = xmlName.Split(new char[] {'_','.' });
               string [] parsName = pars[0].Split(new char[] {'\\'});
                xml.name = parsName[1];
                xml.eis_number = pars[1];
                xml.ooskey = pars[2];
                listXml.Add(xml);
            }
            return listXml;
                       
        }

    }
   

    class xml
    {
        public string name { get; set; }
        public string ooskey { get; set; }
        public string eis_number { get; set; }
        public string path { get; set; }
    }

    public class OrderdocApp
    {
        public string OOSKEY { get; set; }
        public string OOS_DOC_NUMBER { get; set; }
        
        public int ID { get; set; }
        public string pathToXml { get; set; }
    }

    class Program
    {


        static void Comparison(OrderdocApp order, List<xml> xmlList)
        {
            foreach (xml x in xmlList)
            {

                if (x.eis_number == order.OOS_DOC_NUMBER)
                {
                    if (order.OOSKEY == null)
                    {
                        order.OOSKEY = x.ooskey;
                        order.pathToXml = x.path;
                    }
                    int a, b;
                    a = Convert.ToInt32(order.OOSKEY);
                    b = Convert.ToInt32(x.ooskey);
                    if (a < b)
                    {
                        order.OOSKEY = x.ooskey;
                        order.pathToXml = x.path;
                    }
                }

            }
        }
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Введите параметром к запуску аргумент - путь до папки");
            }

        //FileWork fw = new FileWork(args[0]);
        //D:\ccc
            string x = $"D:\\ccc/1/1.txt";
            string y = $"D:\\ccc";
            // контекст подключения к БД
            var options = new DbContextOptionsBuilder<OrdersDBContext>()
                .UseOracle(Database.ConnectionStringTest)
                .Options;

            //создаем обьект класса работы с файлами и директориями
            FileWork fw = new FileWork(x, y);
            //Создаем коллекции обьектов 
            List<OrderdocApp> listImport = new List<OrderdocApp>();
            List<Orderdoc> OrderToDBList = new List<Orderdoc>();
            List<xml> xmls = new List<xml>();

            listImport = fw.FileIn();
            Console.WriteLine($"сформировано {listImport.Count} обьектов");

            xmls = fw.InDir();
            foreach (xml a in xmls)
            {

                Console.WriteLine($"путь {a.path }-- номер {a.eis_number}-- ооскей {a.ooskey}-- имя {a.name}");
            }

            foreach (OrderdocApp order in listImport)
            {
                Comparison(order, xmls);
                Console.WriteLine($"id {order.ID}-- ооскей {order.OOSKEY}-- номер {order.OOS_DOC_NUMBER}-- путь {order.pathToXml}");
                Orderdoc orderdocDB = new Orderdoc();
                orderdocDB.ID = order.ID;
                orderdocDB.OOSKEY = order.OOSKEY;
                orderdocDB.OOS_DOC_NUMBER = order.OOS_DOC_NUMBER;
                OrderToDBList.Add(orderdocDB);

            }
            
            foreach (Orderdoc order in OrderToDBList)
            {
                using (OrdersDBContext dbc = new OrdersDBContext(options))
                {
                    var orderdocs = dbc.ORDERDOC.Where(x => x.ID == order.ID).ToList();
                    try
                    {
                       if( orderdocs[0]==null)
                        {

                        }
                    }
                    orderdocs[0].OOS_DOC_NUMBER = order.OOS_DOC_NUMBER;
                    orderdocs[0].OOSKEY = order.OOSKEY;
                    orderdocs[0].DISPSTATUS_ID = 51;
                    dbc.SaveChanges();
                    
                }
            }
            


            /* static void MapDriveECS()
             {
                 String myUser = "smbuser";
                 String myPass = "63257u6bc";
                 String cmdString = "net use \\\\10.12.128.6\\44FZ /user:" + myUser + " " + myPass;
                 Console.WriteLine(cmdString);
                 ManagementClass processClass = new ManagementClass("Win32_Process");
                 object[] methodArgs = { cmdString, null, null, 0 };
                 object result = processClass.InvokeMethod("Create", methodArgs);

                 Console.WriteLine("Creation of process returned: " + result);
                 Console.WriteLine("Process ID: {0}", methodArgs[3]);
             }
             MapDriveECS();

             Thread.Sleep(60000);



             foreach (OrderdocApp order in listImport)
             {

                 string pathDst = "\\\\10.12.128.6\\44FZ";
                 string name = Path.GetFileName(order.pathToXml);
                 string newPath = Path.Combine(pathDst, name);
                 File.Move(order.pathToXml, newPath);

             }
             */




        } 
            



            






        }
    }

