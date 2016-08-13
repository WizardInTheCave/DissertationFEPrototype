using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisertationFEPrototype
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine("YOU WHAT MATE!!");

            string lisaString = @"D:\Documents\DissertationWork\basicCube.liml";
            var control = new Control(lisaString);


            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
