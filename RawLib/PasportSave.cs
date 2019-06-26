using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;


namespace RawLib
{
    [Obsolete]
    class PasportSave : IPasportSaver
    {
       
        

     

       

        public void Save(ProcessDTO dto)
        {
            try
            {

                
                string ReporName = dto.ReportPath.Remove(0, dto.ReportPath.LastIndexOf(@"\") + 1);
                
                if (!String.IsNullOrWhiteSpace(ReporName))
                {
                    

                    if (!String.IsNullOrWhiteSpace(dto.Json) & !String.IsNullOrWhiteSpace(ReporName))
                    {
                        //DbOracle db = new DbOracle();
                        try
                        {
                            //if (db.Open("", "", ""))
                            //{
                               
                                    using (StreamWriter writer = new StreamWriter("C:\\call.txt", true))
                                    {
                                        writer.WriteLine(String.Format("Вызов процедур базы данных {0} {1} ( {2}, {3} )",
                                            DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), "proc", dto.DateTimeStart.ToString(), ReporName));
                                        writer.Flush();
                                    }
                                    
                            //        db.Close();
                               
                            //}
                        }
                        catch (Exception err)
                        {
                           
                            //db.Close();

                            throw new Exception(String.Format("Ошибка PasportSave метод Save Вызов процедур базы данных делаем откат {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), err.ToString()));
                        }
                    }
                }

               

                
            }
            catch (Exception ex)
            {
               
               
                    throw new Exception(String.Format("Ошибка PasportSave метод Save {0} {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), ex.ToString()));
               
               
            }

          
        }

        
    }
}
