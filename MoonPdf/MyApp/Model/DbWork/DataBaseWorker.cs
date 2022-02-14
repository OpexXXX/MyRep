using ATPWork.MyApp.Model.VnePlan;
using ATPWork.Properties;
using ExcelDataReader;
using MyApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MyApp.Model
{
    static public class DataBaseWorker
    {
        public static bool ConnectorBusy()
        {
            return connector.State != ConnectionState.Executing;
        }
        /// <summary>
        /// Коннектор к рабочей базе данных
        /// </summary>
        static SQLiteConnection connector = new SQLiteConnection("Data Source=filename.db; Version=3;PRAGMA synchronous = OFF");
        /// <summary>
        /// Коннектор к базе данных заявок
        /// </summary>
        static SQLiteConnection connectorOplombirovki = new SQLiteConnection("Data Source=oplombirovki.db; Version=3;");
        /// <summary>
        /// Возвращает список монтируеммых ПУ
        /// </summary>
        /// <param name="spisokPU">ссылка на список ПУ</param>
        public static void Initial()
        {
            
            connector.Open();
            SQLiteCommand cmd = connector.CreateCommand();
            cmd.CommandText = "PRAGMA synchronous = OFF";
            cmd.ExecuteNonQuery();
            connectorOplombirovki.Open();
        }
        public static void ClosedApp()
        {
            connector.Close();
            connectorOplombirovki.Close();
        }
        internal static void DropFromVnePlanZayvki()
        {
            SQLiteCommand cmd = connector.CreateCommand();
            string sql_command = "DELETE  FROM Oplombirovki;";
            cmd.CommandText = sql_command;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static List<PriborUcheta> PUListInit()
        {
            List<PriborUcheta> spisokPU = new List<PriborUcheta>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `PriboriUcheta`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            while (r.Read())
            {
                spisokPU.Add(new PriborUcheta(r["SapNumberPU"].ToString(), r["NamePU"].ToString(), Int32.Parse(r["Poverka"].ToString()), r["Znachnost"].ToString()));
            }
            r.Close();
            return spisokPU;
        }
        /// <summary>
        /// Возвращает список типов пломб
        /// </summary>
        /// <returns></returns>
        public static List<String> TypePlombListInit()
        {
            List<String> TypePlombList = new List<String>();
            TypePlombList.Add("2400_4");
            TypePlombList.Add("2400_5");
            TypePlombList.Add("2400_6");
            /* 
             
             SQLiteCommand CommandSQL = new SQLiteCommand(connector);
             CommandSQL.CommandText = "SELECT * "
             + " FROM `AgentList`;";
             SQLiteDataReader r = CommandSQL.ExecuteReader();
             while (r.Read())
             {
                 TypePlombList.Add("");
             }
             r.Close();
            */
            return TypePlombList;
        }
        /// <summary>
        /// Возвращает список мест установки пломб
        /// </summary>
        /// <returns></returns>
        public static List<String> PlacePlombListInit()
        {
            List<String> PlacePlomb = new List<String>();
            PlacePlomb.Add("Клеммная крышка ПУ");
            PlacePlomb.Add("Вводной коммутационный аппарат");
            PlacePlomb.Add("Корпус прибора учета");
            PlacePlomb.Add("Щит учета");
            /*
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `AgentList`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            while (r.Read())
            {
                PlacePlomb.Add("");
            }
            r.Close();
           */
            return PlacePlomb;
        }
        /// <summary>
        /// Заполнение списка Агентов
        /// </summary>
        /// <param name="agentList"></param>
        public static List<Agent> AgentListInit()
        {
            List<Agent> agentList = new List<Agent>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `AgentList`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            while (r.Read())
            {
                agentList.Add(new Agent(r["SapNumber"].ToString(), r["Post"].ToString(), r["Surname"].ToString(), r["SearchString"].ToString(), r["DateB"].ToString(), r["PlaceB"].ToString()));
            }
            r.Close();
            return agentList;
        }
        /// <summary>
        /// Выгрузка личта пломб по еденице оборудования
        /// </summary>
        /// <param name="edenicaOborudovania"Еденица оборудования></param>
        /// <returns></returns>
        static public List<Dictionary<String, String>> GetPlombsFromEdOb(string edenicaOborudovania)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            if (edenicaOborudovania == "") return result;
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT Type, Number, Place, InstallDate, Status  "
    + " FROM SAPPlomb WHERE EdenicaOborud LIKE '%" + edenicaOborudovania + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(new Dictionary<string, string>());
                    result[i].Add("Type", r["Type"].ToString());
                    result[i].Add("Number", r["Number"].ToString());
                    result[i].Add("Place", r["Place"].ToString());
                    result[i].Add("InstallDate", r["InstallDate"].ToString());
                    result[i].Add("Status", r["Status"].ToString());
                    i++;
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
/// <summary>
/// Получение листа проверок, где [0] = Номер акта, [1] = дата
/// </summary>
/// <param name="numberLS"></param>
/// <returns></returns>
        internal static List<string[]> GetPrevusAktFenix(string numberLS)
        {
            List<string[]> result = new List<string[]>(2);
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT NumberAkt, Date "
    + " FROM ProverkiFenix WHERE NumbeLS LIKE '%" + numberLS + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(new string[] { r["NumberAkt"].ToString(), r["Date"].ToString()});
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
        /// <summary>
        /// Поиск абонента в базе по Улице
        /// </summary>
        /// <param name="numberLS">Улица</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        static public List<Dictionary<String, String>> GetAbonentFromStreet(string numberLS)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber,Kv, Ustanovka,PuKod, Podkluchenie "
    + " FROM SAPFL WHERE Street LIKE '%" + numberLS + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(new Dictionary<string, string>());

                    result[i].Add("FIO", r["FIO"].ToString());
                    result[i].Add("PuType", r["PuType"].ToString());
                    result[i].Add("LsNumber", r["LsNumber"].ToString());
                    result[i].Add("City", r["City"].ToString());
                    result[i].Add("Street", r["Street"].ToString());
                    result[i].Add("House", r["House"].ToString());
                    result[i].Add("Korpus", r["Korpus"].ToString());
                    result[i].Add("Kv", r["Kv"].ToString());
                    result[i].Add("PuNumber", r["PuNumber"].ToString());
                    result[i].Add("Ustanovka", r["Ustanovka"].ToString());
                    result[i].Add("EdOborudovania", r["PuKod"].ToString());
                    result[i].Add("Podkluchenie", r["Podkluchenie"].ToString());

                    i++;
                }
                r.Close();

                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Поиск абонента в базе по номеру лицевого счета
        /// </summary>
        /// <param name="numberLS">Номер лицевого счета</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        static public List<Dictionary<String, String>> GetAbonentFromLS(string numberLS)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber,Kv, Ustanovka,PuKod, Podkluchenie "
    + " FROM SAPFL WHERE NumberTU LIKE '%" + numberLS + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(new Dictionary<string, string>());

                    result[i].Add("FIO", r["FIO"].ToString());
                    result[i].Add("PuType", r["PuType"].ToString());
                    result[i].Add("LsNumber", r["LsNumber"].ToString());
                    result[i].Add("City", r["City"].ToString());
                    result[i].Add("Street", r["Street"].ToString());
                    result[i].Add("House", r["House"].ToString());
                    result[i].Add("Korpus", r["Korpus"].ToString());
                    result[i].Add("Kv", r["Kv"].ToString());
                    result[i].Add("PuNumber", r["PuNumber"].ToString());
                    result[i].Add("Ustanovka", r["Ustanovka"].ToString());
                    result[i].Add("EdOborudovania", r["PuKod"].ToString());
                    result[i].Add("Podkluchenie", r["Podkluchenie"].ToString());
                    i++;
                }
                r.Close();

                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        /// <summary>
        /// Поиск абонента в базе по фио
        /// </summary>
        /// <param name="name">Номер ПУ</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        static public List<Dictionary<String, String>> GetAbonentFromDbByName(string name)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber, Kv, Ustanovka,PuKod , Podkluchenie"
    + " FROM SAPFL WHERE FIO LIKE '%" + name + "%' ";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            string line = String.Empty;
            int i = 0;
            while (r.Read())
            {
                
                result.Add(new Dictionary<string, string>());
                result[i].Add("FIO", r["FIO"].ToString());
                result[i].Add("PuType", r["PuType"].ToString());
                result[i].Add("LsNumber", r["LsNumber"].ToString());
                result[i].Add("City", r["City"].ToString());
                result[i].Add("Street", r["Street"].ToString());
                result[i].Add("House", r["House"].ToString());
                result[i].Add("Korpus", r["Korpus"].ToString());
                result[i].Add("Kv", r["Kv"].ToString());
                result[i].Add("PuNumber", r["PuNumber"].ToString());
                result[i].Add("Ustanovka", r["Ustanovka"].ToString());
                result[i].Add("EdOborudovania", r["PuKod"].ToString());
                result[i].Add("Podkluchenie", r["Podkluchenie"].ToString());
                i++;
            }
            r.Close();
            return result;
        }
        /// <summary>
        /// Поиск абонента в базе по номеру ПУ
        /// </summary>
        /// <param name="numberPU">Номер ПУ</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        static public List<Dictionary<String, String>> GetAbonentFromDbByPU(string numberPU)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber, Kv, Ustanovka,PuKod , Podkluchenie "
    + " FROM SAPFL WHERE PuNumber LIKE '%" + numberPU + "%' ";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            string line = String.Empty;
            int i = 0;
            while (r.Read())
            {
                result.Add(new Dictionary<string, string>());
                result[i].Add("FIO", r["FIO"].ToString());
                result[i].Add("PuType", r["PuType"].ToString());
                result[i].Add("LsNumber", r["LsNumber"].ToString());
                result[i].Add("City", r["City"].ToString());
                result[i].Add("Street", r["Street"].ToString());
                result[i].Add("House", r["House"].ToString());
                result[i].Add("Korpus", r["Korpus"].ToString());
                result[i].Add("Kv", r["Kv"].ToString());
                result[i].Add("PuNumber", r["PuNumber"].ToString());
                result[i].Add("Ustanovka", r["Ustanovka"].ToString());
                result[i].Add("EdOborudovania", r["PuKod"].ToString());
                result[i].Add("Podkluchenie", r["Podkluchenie"].ToString());
                i++;
            }
            r.Close();
            return result;
        }
        /// <summary>
        /// Инициализация ласта не отработанных актов при загрузке приложения
        /// </summary>
        /// <returns></returns>
        public static List<VnePlanZayavka> LoadZayavki()
        {
            //Открываем соединение
            // Загрузка листа проверок
            List<VnePlanZayavka> result = new List<VnePlanZayavka>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `Oplombirovki`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            string line = String.Empty;
            int i = 0;
            while (r.Read())
            {
                result.Add(new VnePlanZayavka());
                result[i].City = r["City"].ToString();
                if (r["RegNumber"].ToString() != "") result[i].RegNumber = int.Parse(r["RegNumber"].ToString());
                if (r["DateRegister"].ToString() != "") result[i].DateReg =  DateTime.Parse(r["DateRegister"].ToString());
                result[i].NumberLS = r["LsNumber"].ToString();
                result[i].FIO = r["FIO"].ToString();
                result[i].City = r["City"].ToString();
                result[i].Street = r["Street"].ToString();
                result[i].House = r["House"].ToString() != "" ? int.Parse(r["House"].ToString()) : 0;
                if (r["Korpus"].ToString() != "") result[i].Korpus = r["Korpus"].ToString();
                if (r["Kv"].ToString() != "") result[i].Kvartira = int.Parse(r["Kv"].ToString());
                result[i].Prichina = r["Prichina"].ToString();
                result[i].NumberAktTehProverki = r["NumberAkt"].ToString();
                result[i].PhoneNumbers=r["PhoneNumber"].ToString();
                result[i].Primechanie = r["Primechanie"].ToString();
                int res = 0;
                result[i].ProvFlag = Int32.TryParse(r["Proverka"].ToString(),out res) ? res == 1 : false;
                result[i].DopuskFlag = Int32.TryParse(r["Dopusk"].ToString(), out res) ? res == 1 : false;
                result[i].DemontageFlag = Int32.TryParse(r["Demontage"].ToString(), out res) ? res == 1 : false;
                i++;
            }
            r.Close();
            return result;
        }
        /// <summary>
        /// Запись в базу данных завершенных актов
        /// </summary>
        /// <param name="akti">Лист с актами тех. проверок</param>
        public static void InsertZayavki(List<VnePlanZayavka> akti)
        {
            using (var cmdd = new SQLiteCommand(connector))
            {
                using (var transaction = connector.BeginTransaction())
                {
                    foreach (VnePlanZayavka akt in akti)
                    {
                        string sql_command = "INSERT INTO Oplombirovki( 'City', 'Street', 'House', 'Korpus', 'Kv', DateRegister, FIO, RegNumber, LsNumber, Prichina,NumberAkt,PhoneNumber,Primechanie,Dopusk,Proverka,Demontage)"
                      + "VALUES ('"
                       + akt.City + "', '"
                       + akt.Street + "', '"
                       + akt.House + "', '"
                       + akt.Korpus + "', '"
                       + akt.Kvartira + "', '"
                        + akt.DateReg.ToString("d") + "', '"
                        + akt.FIO + "', '"
                        + akt.RegNumber + "', '"
                        + akt.NumberLS + "', '"
                        + akt.Prichina + "', '"
                        + akt.NumberAktTehProverki + "', '"
                        + akt.PhoneNumbers + "', '"
                        + akt.Primechanie + "', '"
                        + (akt.DopuskFlag ? "1" : "0") + "', '"
                        + (akt.ProvFlag ? "1" : "0") + "', '"
                        + (akt.DemontageFlag ? "1" : "0")  + "');";
                        cmdd.CommandText = sql_command;
                        cmdd.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
        }
            }
        /// <summary>
        /// Инициализация ласта не отработанных актов при загрузке приложения
        /// </summary>
        /// <returns></returns>
        public static List<AktTehProverki> LoadATPInWork()
        {
            //Открываем соединение
            // Загрузка листа проверок
            List<AktTehProverki> result = new List<AktTehProverki>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `ATPInWorkList`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            string line = String.Empty;
            int i = 0;
            while (r.Read())
            {
                List<int> Page = new List<int>();
                Page.Add(Int32.Parse(r["Page1"].ToString()));
                Page.Add(Int32.Parse(r["Page2"].ToString()));
                result.Add(new AktTehProverki(Int32.Parse(r["ID"].ToString()), Page, r["PathOfPdfFile"].ToString(), long.Parse(r["SizePDF"].ToString())));
                //Ищем агентов по номеру
                string temp_agent = r["Agent_1"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in MainAtpModel.AgentList)
                    {
                        if (item.SapNumber == temp_agent) result[i].Agent_1 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString, item.DateB, item.PlaceB);
                    }
                }
                temp_agent = r["Agent_2"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in MainAtpModel.AgentList)
                    {
                        if (item.SapNumber == temp_agent) result[i].Agent_2 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString, item.DateB, item.PlaceB);
                    }
                }
                //Создаем новй ПУ
                string temp_pu = r["PuNewType"].ToString();
                if (temp_pu != "")
                {
                    foreach (PriborUcheta item in MainAtpModel.NewPuList)
                    {
                        if (item.SapNumberPU == temp_pu) result[i].PuNewType = new PriborUcheta(item.SapNumberPU, item.Nazvanie, item.Poverka, item.Znachnost);
                    }
                }
                result[i].PuOldMPI = Int32.Parse(r["PuOldMPI"].ToString()) == 0 ? false : true;
                result[i].DopuskFlag = Int32.Parse(r["DopuskFlag"].ToString()) == 0 ? false : true;

              
                result[i].City = r["City"].ToString();
                result[i].Street = r["Street"].ToString();
                result[i].House = r["House"].ToString() != "" ? int.Parse(r["House"].ToString()) : 0;
                if (r["Korpus"].ToString() != "") result[i].Korpus = r["Korpus"].ToString();
                if (r["Kvartira"].ToString() != "") result[i].Kvartira = int.Parse(r["Kvartira"].ToString());
                string dt = r["DateWork"].ToString();
                if (dt == "") result[i].DateWork = null;
                else result[i].DateWork = DateTime.Parse(dt);
                result[i].FIO = r["FIO"].ToString();
                result[i].Number = Int32.Parse(r["Number"].ToString());
                result[i].NumberLS = r["NumberLS"].ToString();
                result[i].PuNewNumber = r["PuNewNumber"].ToString();
                result[i].PuNewPokazanie = r["PuNewPokazanie"].ToString();
                result[i].PuNewPoverkaEar = r["PuNewPoverkaEar"].ToString();
                result[i].PuNewPoverKvartal = r["PuNewPoverKvartal"].ToString();
                result[i].PuOldNumber = r["PuOldNumber"].ToString();
                result[i].PuOldPokazanie = r["PuOldPokazanie"].ToString();
                result[i].PuOldType = r["PuOldType"].ToString();
                result[i].NumberMail = Int32.Parse(r["NumberMail"].ToString());

                string dtm = r["DateMail"].ToString();
                if (dtm == "") result[i].DateMail = null;
                else result[i].DateMail = DateTime.Parse(dtm);

                result[i].Ustanovka = r["Ustanovka"].ToString();
                result[i].SapNumberAkt = r["SapNumberAkt"].ToString();
                result[i].EdOborudovania = r["EdOborudovania"].ToString();
                loadPlombs(result[i]);
                i++;
            }
            r.Close();
            return result;
        }
        /// <summary>
        /// Инициализация листа актов из базы при загрузке приложения
        /// </summary>
        /// <param name="ATP"></param>
        public static List<AktTehProverki> LoadCompleteATP()
        {
            // Загрузка листа проверок
            List<AktTehProverki> result = new List<AktTehProverki>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `CompleteATPList`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            string line = String.Empty;
            int i = 0;
            while (r.Read())
            {
                List<int> Page = new List<int>();
                Page.Add(Int32.Parse(r["Page1"].ToString()));
                Page.Add(Int32.Parse(r["Page2"].ToString()));
                result.Add(new AktTehProverki(Int32.Parse(r["ID"].ToString()), Page, r["PathOfPdfFile"].ToString(), long.Parse(r["SizePDF"].ToString())));
                //Ищем агентов по номеру
                string temp_agent = r["Agent_1"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in MainAtpModel.AgentList)
                    {
                        if (item.SapNumber == temp_agent) result[i].Agent_1 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString, item.DateB, item.PlaceB);
                    }
                }
                temp_agent = r["Agent_2"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in MainAtpModel.AgentList)
                    {
                        if (item.SapNumber == temp_agent)
                        {
                            result[i].Agent_2 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString, item.DateB, item.PlaceB);
                            break;
                        }
                    }
                }
                //Создаем новй ПУ
                string temp_pu = r["PuNewType"].ToString();
                if (temp_pu != "")
                {
                    foreach (PriborUcheta item in MainAtpModel.NewPuList)
                    {
                        if (item.SapNumberPU == temp_pu)
                        {
                            result[i].PuNewType = new PriborUcheta(item.SapNumberPU, item.Nazvanie, item.Poverka, item.Znachnost);
                            break;
                        }
                    }
                }
                result[i].PuOldMPI = Int32.Parse(r["PuOldMPI"].ToString()) == 0 ? false : true;
                result[i].DopuskFlag = Int32.Parse(r["DopuskFlag"].ToString()) == 0 ? false : true;
                result[i].City = r["City"].ToString();
                result[i].Street = r["Street"].ToString();
                result[i].House = r["House"].ToString()!=""?int.Parse(r["House"].ToString()):0;
                if ( r["Korpus"].ToString() != "") result[i].Korpus = r["Korpus"].ToString();
                if ( r["Kvartira"].ToString() != "") result[i].Kvartira = int.Parse(r["Kvartira"].ToString());
                DateTime date1 = DateTime.Parse(r["DateWork"].ToString());
                result[i].DateWork = date1;
                result[i].FIO = r["FIO"].ToString();
                result[i].Number = Int32.Parse(r["Number"].ToString());
                result[i].NumberLS = r["NumberLS"].ToString();
                result[i].PuNewNumber = r["PuNewNumber"].ToString();
                result[i].PuNewPokazanie = r["PuNewPokazanie"].ToString();
                result[i].PuNewPoverkaEar = r["PuNewPoverkaEar"].ToString();
                result[i].PuNewPoverKvartal = r["PuNewPoverKvartal"].ToString();
                result[i].PuOldNumber = r["PuOldNumber"].ToString();
                result[i].PuOldPokazanie = r["PuOldPokazanie"].ToString();
                result[i].PuOldType = r["PuOldType"].ToString();
                result[i].NumberMail = Int32.Parse(r["NumberMail"].ToString());
                string dtm = r["DateMail"].ToString();
                if (dtm == "") result[i].DateMail = null;
                else result[i].DateMail = DateTime.Parse(dtm);
                result[i].Ustanovka = r["Ustanovka"].ToString();
                result[i].SapNumberAkt = r["SapNumberAkt"].ToString();
                result[i].EdOborudovania = r["EdOborudovania"].ToString();
                loadPlombs(result[i]);
                i++;
            }
            r.Close();
            return result;
        }
        public static void DromCompliteTable()
        {
            SQLiteCommand cmd = connector.CreateCommand();
            string sql_command = "DELETE  FROM CompleteATPList;";
            cmd.CommandText = sql_command;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void DromInWorkTable()
        {
            SQLiteCommand cmd = connector.CreateCommand();
            string sql_command = "DELETE  FROM ATPInWorkList;";
            cmd.CommandText = sql_command;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private static void loadPlombs(AktTehProverki akt)
        {
            SQLiteCommand CommandPlomb = new SQLiteCommand(connector);
            string aktNameForPlomb = akt.ID.ToString() + akt.Number + akt.NumberLS;
            CommandPlomb.CommandText = "SELECT * FROM `InstallingPlombs` WHERE Akt = '" + aktNameForPlomb + "'";
            SQLiteDataReader plomb_reader = CommandPlomb.ExecuteReader();
            while (plomb_reader.Read())
            {
                string plomb_Type, plomb_Number, plomb_Place, plomb_Status, plomb_DateInstall;
                bool plomb_Remove;
                bool plomb_Old;
                plomb_Type = plomb_reader["Type"].ToString();
                plomb_Number = plomb_reader["Number"].ToString();
                plomb_Old = Int32.Parse(plomb_reader["OldPlomb"].ToString()) == 0 ? false : true; ;
                plomb_Remove = Int32.Parse(plomb_reader["Remove"].ToString()) == 0 ? false : true; ;
                plomb_Place = plomb_reader["Place"].ToString();
                plomb_Status = plomb_reader["Status"].ToString();
                plomb_DateInstall = plomb_reader["InstallDate"].ToString();
                if (plomb_Old) akt.OldPlombs.Add(new Plomba(plomb_Type, plomb_Number, plomb_Place, plomb_Remove, true, plomb_Status, plomb_DateInstall));
                else akt.NewPlombs.Add(new Plomba(plomb_Type, plomb_Number, plomb_Place));
            }
        }
        private static void InsertPlombs(SQLiteCommand cmdd, IEnumerable<Plomba> plombs, string aktName)
        {
            foreach (Plomba Plomba in plombs)
            {
                string sql_command = "INSERT INTO `InstallingPlombs` (Akt, Type, Number, Remove, Place, OldPlomb, InstallDate, Status) "
           + "VALUES ('"
           + aktName + "', '"
           + Plomba.Type + "', '"
           + Plomba.Number + "', '"
           + ((Plomba.Demontage) ? "1'" : "0'") + ", '"
           + Plomba.Place + "', '"
           + ((Plomba.OldPlomb) ? "1'" : "0'") + ", '"
           + Plomba.InstallDate + "', '"
           + Plomba.Status + "');";
                cmdd.CommandText = sql_command;
                cmdd.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Запись в базу данных завершенных актов
        /// </summary>
        /// <param name="akti">Лист с актами тех. проверок</param>
        public static void InsertCompleteAktAPT(List<AktTehProverki> akti)
        {
            using (var cmdd = new SQLiteCommand(connector))
            {
                using (var transaction = connector.BeginTransaction())
                {
                    foreach (AktTehProverki akt in akti)
                    {
                        string sql_command = "INSERT INTO CompleteATPList( 'City', 'Street', 'House', 'Korpus', 'Kvartira', 'ID',`PathOfPdfFile`, 'Adress', Agent_1, Agent_2, DateWork, FIO, DopuskFlag, Number,NumberLS,PuNewNumber,PuNewPokazanie,PuNewPoverkaEar,PuNewPoverKvartal,PuNewType,PuOldMPI,PuOldNumber,PuOldPokazanie,PuOldType, Page1, Page2, NumberMail, DateMail,`Ustanovka`,`SapNumberAkt`,`SizePDF`,`EdOborudovania`)"
                      + "VALUES ('"
                       + akt.City + "', '"
                       + akt.Street + "', '"
                       + akt.House + "', '"
                       + akt.Korpus + "', '"
                       + akt.Kvartira + "', '"
                       + akt.ID.ToString() + "', '"
                         + akt.NamePdfFile + "', '"
                        + akt.Adress + "', '"
                        + (akt.Agent_1 != null ? akt.Agent_1.SapNumber : "") + "', '"
                        + (akt.Agent_2 != null ? akt.Agent_2.SapNumber : "") + "', '"
                        + akt.DateWork + "', '"
                        + akt.FIO + "', '"
                        + (akt.DopuskFlag ? "1" : "0") + "', '"
                        + akt.Number + "', '"
                        + akt.NumberLS + "', '"
                        + akt.PuNewNumber + "', '"
                        + akt.PuNewPokazanie + "', '"
                        + akt.PuNewPoverkaEar + "', '"
                        + akt.PuNewPoverKvartal + "', '"
                        + (akt.PuNewType != null ? akt.PuNewType.SapNumberPU : "") + "', '"
                        + (akt.PuOldMPI ? "1" : "0") + "', '"
                        + akt.PuOldNumber + "', '"
                        + akt.PuOldPokazanie + "', '"
                        + akt.PuOldType + "','"
                        + akt.NumberOfPagesInSoursePdf[0] + "','"
                         + akt.NumberOfPagesInSoursePdf[1] + "','"
                          + akt.NumberMail.ToString() + "','"
                           + (akt.DateMail != null ? akt.DateMail?.ToString("d") : "") + "','"
                           + akt.Ustanovka + "','"
                           + akt.SapNumberAkt + "','"
                             + akt.SizePDF + "','"
                        + akt.EdOborudovania + "');";
                        cmdd.CommandText = sql_command;
                        cmdd.ExecuteNonQuery();
                        string aktName = akt.ID.ToString() + akt.Number + akt.NumberLS;
                        sql_command = "DELETE FROM `InstallingPlombs` WHERE Akt = \"" + aktName + "\";";
                        cmdd.CommandText = sql_command;
                        cmdd.ExecuteNonQuery();
                        if (akt.NewPlombs.Count > 0) InsertPlombs(cmdd, akt.NewPlombs, aktName);
                        if (akt.OldPlombs.Count > 0) InsertPlombs(cmdd, akt.OldPlombs, aktName);

                    }
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// Запись в базу данных заполняемых актов
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        public static void InsertAPTInWork(List<AktTehProverki> akti)
        {
            
            using (var cmdd = new SQLiteCommand(connector))
            {
                using (var transaction = connector.BeginTransaction())
                {
                    foreach (AktTehProverki akt in akti)
                    {
                        string sql_command = "INSERT INTO ATPInWorkList( 'City', 'Street', 'House', 'Korpus', 'Kvartira', 'ID',`PathOfPdfFile`, 'Adress', Agent_1, Agent_2, DateWork, FIO, DopuskFlag, Number,NumberLS,PuNewNumber,PuNewPokazanie,PuNewPoverkaEar,PuNewPoverKvartal,PuNewType,PuOldMPI,PuOldNumber,PuOldPokazanie,PuOldType, Page1, Page2, NumberMail, DateMail,`Ustanovka`,`SapNumberAkt`,`SizePDF`,`EdOborudovania`)"
                        + "VALUES ('"
                       + akt.City + "', '"
                       + akt.Street + "', '"
                       + akt.House + "', '"
                       + akt.Korpus + "', '"
                       + akt.Kvartira + "', '"
                        + akt.ID.ToString() + "', '"
                        + akt.NamePdfFile + "', '"
                        + akt.Adress + "', '"
                        + (akt.Agent_1 != null ? akt.Agent_1.SapNumber : "") + "', '"
                        + (akt.Agent_2 != null ? akt.Agent_2.SapNumber : "") + "', '"
                        + akt.DateWork + "', '"
                        + akt.FIO + "', '"
                        + (akt.DopuskFlag ? "1" : "0") + "', '"
                        + akt.Number + "', '"
                        + akt.NumberLS + "', '"
                        + akt.PuNewNumber + "', '"
                        + akt.PuNewPokazanie + "', '"
                        + akt.PuNewPoverkaEar + "', '"
                        + akt.PuNewPoverKvartal + "', '"
                        + (akt.PuNewType != null ? akt.PuNewType.SapNumberPU : "") + "', '"
                        + (akt.PuOldMPI ? "1" : "0") + "', '"
                        + akt.PuOldNumber + "', '"
                        + akt.PuOldPokazanie + "', '"
                        + akt.PuOldType + "','"
                        + akt.NumberOfPagesInSoursePdf[0] + "','"
                        + akt.NumberOfPagesInSoursePdf[1] + "','"
                        + akt.NumberMail.ToString() + "','"
                        + (akt.DateMail != null ? akt.DateMail?.ToString("d") : "") + "','"
                        + akt.Ustanovka + "','"
                        + akt.SapNumberAkt + "','"
                        + akt.SizePDF + "','"
                        + akt.EdOborudovania + "');";
                        cmdd.CommandText = sql_command;
                        cmdd.ExecuteNonQuery();
                        string aktName = akt.ID.ToString() + akt.Number + akt.NumberLS;
                        sql_command = "DELETE FROM `InstallingPlombs` WHERE Akt = \"" + aktName + "\";";
                        cmdd.CommandText = sql_command;
                        cmdd.ExecuteNonQuery();
                        if (akt.NewPlombs.Count > 0) InsertPlombs(cmdd, akt.NewPlombs, aktName);
                        if (akt.OldPlombs.Count > 0) InsertPlombs(cmdd, akt.OldPlombs, aktName);
                    }
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// Обновление базы данных физ.  лиц
        /// </summary>
        /// <param name="dataSetSAPFL"></param>
        /// 
        public static void RefreshSAPFL(DataSet dataSetSAPFL)
        {
            if (dataSetSAPFL != null)
            {
                try
                {
                    string sSQLTable = @"SAPFL"; //Таблица с сервера
                    DataTable dt = dataSetSAPFL.Tables[0];
                    string sclearsql = "delete from " + sSQLTable;
                    SQLiteCommand cmd = connector.CreateCommand();
                    cmd.CommandText = sclearsql;
                    cmd.ExecuteNonQuery();
                    string sql_command;
                    using (var cmdd = new SQLiteCommand(connector))
                    {
                        using (var transaction = connector.BeginTransaction())
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    sql_command = "INSERT INTO `" + sSQLTable + "` ( `NumberTU` , `Ustanovka` , `City` , `Street` , `House` , `Korpus` , `Kv` , `PuNumber` , `LsNumber` , `Parthner` , `FIO` , `PuType` , `Raspologenie` , `TehMesto` , `Podkluchenie` , `TmTopologii` , `PhoneNumber` , `PuKod` , `Kladr` ) "
                                    + "VALUES (\""
                                       + row["Номер точки учета"] + "\", \""
                                       + row["Установка"] + "\", \""
                                        + row["Город"] + "\", \""
                                       + row["Улица"] + "\", \""
                                        + row["Номер дома"] + "\", \""
                                       + row["Корпус"] + "\", \""
                                        + row["Номер квартиры"] + "\", \""
                                       + row["Заводской номер"] + "\", \""
                                        + row["Лицевой счет"] + "\", \""
                                       + row["Деловой партнер"] + "\", \""
                                        + row["Наименование абонента"] + "\", \""
                                       + row["Тип прибора"] + "\", \""
                                        + row["Расположение ТУ"] + "\", \""
                                       + row["ТМ прибора"] + "\", '"
                                        + row["Подключение"] + "', \""
                                       + row["ТМ топологии сети"] + "\", \""
                                       + row["Номер телефона абонента"] + "\", \""
                                       + row["Код прибора"] + "\", \""
                                       + row["КЛАДР"] + "\");";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    return;
                                }
                                cmdd.CommandText = sql_command;
                                cmdd.ExecuteNonQuery();
                            }
                            transaction.Commit();
                        }
                    }
                    MessageBox.Show("База ФЛ успешно обновлена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// Обновление базы установленных пломб из xlsx
        /// </summary>
        /// <param name="dataSetSAPFL"></param>
        public static void RefreshSAPPlomb(DataSet dataSetSAPFL)
        {
            if (dataSetSAPFL != null)
            {
                try
                {
                    string sSQLTable = @"SAPPlomb"; //Таблица с сервера
                    DataTable dt = dataSetSAPFL.Tables[0];
                    string sclearsql = "delete from " + sSQLTable;
                    SQLiteCommand cmd = connector.CreateCommand();
                    cmd.CommandText = sclearsql;
                    cmd.ExecuteNonQuery();
                    string sql_command;
                    using (var cmdd = new SQLiteCommand(connector))
                    {
                        using (var transaction = connector.BeginTransaction())
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    sql_command = "INSERT INTO `" + sSQLTable + "` ( `Type` , `Number` , `Place` , `InstallDate` , `EdenicaOborud` , `Status` ) "
                                    + "VALUES (\""
                                       + row["Тип пломбы - группа пломб одного вида"] + "\", \""
                                       + row["Код пломбы - разные пломбы в рамках типа"] + "\", \""
                                        + row["ПоложенПломбы"] + "\", \""
                                       + row["Дата установки пломбы"] + "\", \""
                                        + row["ЕдОборуд"] + "\", \""
                                       + row["Статус пломбы - статус ЖизнЦикла пломбы"] + "\");";
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                    return;
                                }
                                cmdd.CommandText = sql_command;
                                cmdd.ExecuteNonQuery();
                            }
                            dt.Dispose();
                            transaction.Commit();
                        }
                    }
                    MessageBox.Show("База пломб успешно обновлена");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                return;
            }
        }
        /// <summary>
        /// Возвращает список абонентов в плане на дату
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<string> GetAbonentPO(string ear, string numberLS)
        {
            List<string> result = new List<string>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT  Value  "
    + " FROM PoSbit WHERE NumberTu = '" + numberLS + "' AND Ear = '" + ear + "'  ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(r["Value"].ToString());
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
        /// <summary>
        /// Возвращает список абонентов в плане на дату
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<string> FindAbonentPlan(DateTime date)
        {
            List<string> result = new List<string>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT NumberLS  "
    + " FROM Plan WHERE DateWork LIKE '%" + date.ToString("d") + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result.Add(r["NumberLS"].ToString());
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
        /// <summary>
        /// Возвращает даты включения в план для абонента
        /// </summary>
        /// <param name="numberLS"></param>
        /// <returns></returns>
        public static List<string> FindAbonentPlan(string numberLS)
        {
            List<string> result = new List<string>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT DateWork  "
    + " FROM Plan WHERE NumberLS LIKE '%" + numberLS + "%' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    string res = r["DateWork"].ToString();
                    result.Add(res);
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
        /// <summary>
        /// Возвращает колво комнат, прописанных, наличие электроотопления
        /// </summary>
        /// <param name="numberLs"></param>
        /// <returns></returns>
        public static Dictionary<String, String> GetInfoForNormativ(string numberLs)
        {
            Dictionary<String, String> result = new Dictionary<String, String>();

            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT People, Rooms, Kategorya  "
    + " FROM SbitNormotiv WHERE NumberLs ='" + numberLs + "' ";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                if (r.Read())
                {
                    result.Add("People", r["People"].ToString());
                    result.Add("Rooms", r["Rooms"].ToString());
                    result.Add("Kategorya", r["Kategorya"].ToString());
                }
                r.Close();
                return result;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }

        }
        /// <summary>
        /// Возвращает норматив 
        /// </summary>
        /// <param name="people"></param>
        /// <param name="rooms"></param>
        /// <param name="kategory"></param>
        /// <returns></returns>
        public static int GetNormativ(int people, int rooms, int kategory)
        {

            int result = 0;
            string peopleReq, roomReq;
            if (people == 0)
                people = 1;
            if (people >= 5) peopleReq = "More5People";
            else peopleReq = "People" + people.ToString();

            if (rooms == 0) rooms = 1;
            if (rooms >= 4) roomReq = "More4";
            else roomReq = rooms.ToString();



            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT " + peopleReq
            + " FROM Normativ WHERE Kategorya ='" + kategory + "' AND RoomCount =   '" + roomReq + "'";

            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    result = int.Parse(r[peopleReq].ToString());
                }

                r.Close();
                return result * people;
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return result;
            }
        }
    }
}
