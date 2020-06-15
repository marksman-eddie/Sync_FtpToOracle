using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Microsoft.EntityFrameworkCore;





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
            foreach(xml x in xmlList)
            {
                
                if (x.eis_number==order.OOS_DOC_NUMBER)
                {
                    if (order.OOSKEY == null)
                    {
                        order.OOSKEY = x.ooskey;
                        order.pathToXml = x.path;
                    }
                    int a, b;
                    a = Convert.ToInt32(order.OOSKEY);
                    b = Convert.ToInt32(x.ooskey);
                    if (a<b)
                    {
                        order.OOSKEY = x.ooskey;
                        order.pathToXml = x.path;
                    }
                }
                
            }
        }
        static void Main(string[] args)
        {
            /*if (args.Length == 0)
            {
                Console.WriteLine("Введите параметром к запуску аргумент - путь до папки");
            }
            */
            //FileWork fw = new FileWork(args[0]);
            
            string x = "d:/ccc/1/1.txt";
            string y = "d:/ccc";
            FileWork fw = new FileWork(x,y);
            List<OrderdocApp> listImport = new List<OrderdocApp>();
            listImport = fw.FileIn();
            Console.WriteLine($"сформировано {listImport.Count} обьектов");
           /* List<xml> xmls  = new List<xml>();
            xmls = fw.InDir();
            foreach (xml a in xmls )
            {

                Console.WriteLine($"путь {a.path }--номер {a.eis_number}-- ооскей {a.ooskey} имя {a.name}");
            }

            foreach(Orderdoc order in listImport)
            {
                Comparison(order, xmls);
                Console.WriteLine($"имя {order.azk_id}-- ооскей {order.ooskey}-- номер {order.eis_number}-- путь {order.pathToXml}");
            }*/
            
            var options = new DbContextOptionsBuilder<OrdersDBContext>()
                .UseOracle(Database.ConnectionString)
                .Options;


            using (OrdersDBContext dbc = new OrdersDBContext(options))
            {
                var orderdocs = dbc.ORDERDOC.//FirstOrDefault();
                   Where(x => x.ID == 165709).ToList();
               foreach (var order in orderdocs)
                    Console.WriteLine($"{order.OOS_DOC_NUMBER}");
                //Console.WriteLine(orderdocs.ToListAsync().ToString()) ;

            // zalupa
            }
            






        }
    }
}
