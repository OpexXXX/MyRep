using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MoonPdf
{
    /// <summary>
    /// Класс для работы с базой SQLite
    /// </summary>
    public class DataBaseWorker
    {
        /// <summary>
        /// Коннектор к рабочей базе данных
        /// </summary>
        SQLiteConnection connector;
        /// <summary>
        /// Коннектор к базе для сохранения загрузки
        /// </summary>
        SQLiteConnection connectorOnSaveLoad;
        /// <summary>
        /// Коннектор к базе данных заявок
        /// </summary>
        SQLiteConnection connectorOplombirovki;

        public DataBaseWorker()
        {
            connector = new SQLiteConnection("Data Source=filename.db; Version=3;");
            try
            {
                connector.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            connectorOplombirovki = new SQLiteConnection("Data Source=oplombirovki.db; Version=3;");
            try
            {
                connectorOplombirovki.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Заполнение списка монтируеммых ПУ
        /// </summary>
        /// <param name="spisokPU">ссылка на список ПУ</param>
        public void PUListInit(SpisokPUObserv spisokPU)
        {
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `PriboriUcheta`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            while (r.Read())
            {
                spisokPU.Add(new PriborUcheta(r["SapNumberPU"].ToString(), r["NamePU"].ToString(), Int32.Parse(r["Poverka"].ToString()), r["Znachnost"].ToString()));
            }
            r.Close();
        }
        /// <summary>
        /// Заполнение списка Агентов
        /// </summary>
        /// <param name="agentList"></param>
        public void AgentListInit(AgentList agentList)
        {
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `AgentList`;";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            while (r.Read())
            {
                agentList.Add(new Agent(r["SapNumber"].ToString(), r["Post"].ToString(), r["Surname"].ToString(), r["SearchString"].ToString()));
            }
            r.Close();

        }
        /// <summary>
        /// Поиск абонента в базе по номеру лицевого счета
        /// </summary>
        /// <param name="numberLS">Номер лицевого счета</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        public List<Dictionary<String, String>> GetAbonentFromLS(string numberLS)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber,Kv, Ustanovka,PuKod "
  + " FROM SAPFL WHERE LsNumber LIKE '%" + numberLS + "%' ";
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
        /// Поиск абонента в базе по номеру ПУ
        /// </summary>
        /// <param name="numberPU">Номер ПУ</param>
        /// <returns>Лист словарей с данными если успешно, null если не найден</returns>
        public List<Dictionary<String, String>> GetAbonentFromDbByPU(string numberPU)
        {
            List<Dictionary<String, String>> result = new List<Dictionary<string, string>>();

            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT FIO, PuType, LsNumber,City,Street,House,Korpus,PuNumber, Kv, Ustanovka,PuKod "
  + " FROM SAPFL WHERE PuNumber LIKE '%" + numberPU + "%' ";
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
        /// Сохранение актов находящихся в работе
        /// </summary>
        /// <param name="ATP"></param>
        /// <param name="Path">Путь к файлу сохранения *.atp</param>
        /// <returns></returns>
        public bool SaveATP(ATPWorker ATP, string Path)
        {
            //Открываем соединение
            connectorOnSaveLoad = new SQLiteConnection("Data Source=" + Path + "; Version=3;");
            try
            {
                connectorOnSaveLoad.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            string result_command = "";
            ///   Создаем таблицу для ATP work
            SQLiteCommand cmd = connectorOnSaveLoad.CreateCommand();
            string sql_command = "DROP TABLE IF EXISTS ATPWork;"
              + "CREATE TABLE ATPWork("
              + "id INTEGER PRIMARY KEY AUTOINCREMENT, "
              + "aktATPInWork TEXT,     "
              + "PathPdf TEXT );"
              + "INSERT INTO ATPWork(aktATPInWork, PathPdf) "
              + "VALUES ('" + ATP.AktATPInWork.ID + "', '" + ATP.pathOfFile + "');";
            result_command += sql_command;
            sql_command = "DROP TABLE IF EXISTS ATPList; "
              + "CREATE TABLE ATPList("
              + "'index' INTEGER PRIMARY KEY AUTOINCREMENT, "
              + "ID INTEGER,"
               + "PathOfPdfFile TEXT ,"
              + "Adress TEXT ,"
               + "Agent_1 TEXT ,"
               + "Agent_2 TEXT ,"
               + "DateWork TEXT ,"
               + "FIO TEXT ,"
               + "DopuskFlag INTEGER ,"
               + "Number TEXT ,"
               + "NumberLS TEXT ,"
               + "PuNewNumber TEXT ,"
               + "PuNewPokazanie TEXT ,"
               + "PuNewPoverkaEar TEXT ,"
               + "PuNewPoverKvartal TEXT ,"
               + "PuNewType TEXT ,"
               + "PuOldMPI INTEGER ,"
               + "PuOldNumber TEXT ,"
               + "PuOldPokazanie TEXT ,"
               + "PuOldType TEXT ,"
               + "Page1 INTEGER ,"
               + "Page2 INTEGER ,"
               + "TypeOfWork TEXT"
              + ");";
            result_command += sql_command;
            foreach (aktATP item in ATP.AllAtpInWorkList)
            {
                //добавляем строку с актом
                sql_command = "INSERT INTO ATPList(  'ID',`PathOfPdfFile`, 'Adress', Agent_1, Agent_2, DateWork, FIO, DopuskFlag, Number,NumberLS,PuNewNumber,PuNewPokazanie,PuNewPoverkaEar,PuNewPoverKvartal,PuNewType,PuOldMPI,PuOldNumber,PuOldPokazanie,PuOldType,TypeOfWork, Page1, Page2)"
              + "VALUES ('"
               + item.ID.ToString() + "', '"
                 + item.PathOfPdfFile + "', '"
                + item.Adress + "', '"
                + (item.Agent_1 != null ? item.Agent_1.SapNumber : "") + "', '"
                + (item.Agent_2 != null ? item.Agent_2.SapNumber : "") + "', '"
                + item.DateWork + "', '"
                + item.FIO + "', '"
                + (item.DopuskFlag ? "1" : "0") + "', '"
                + item.Number + "', '"
                + item.NumberLS + "', '"
                + item.PuNewNumber + "', '"
                + item.PuNewPokazanie + "', '"
                + item.PuNewPoverkaEar + "', '"
                + item.PuNewPoverKvartal + "', '"
                + (item.PuNewType != null ? item.PuNewType.SapNumberPU : "") + "', '"
                + (item.PuOldMPI ? "1" : "0") + "', '"
                + item.PuOldNumber + "', '"
                + item.PuOldPokazanie + "', '"
                + item.PuOldType + "','"
                + item.TypeOfWork + "','"
                + item.NumberOfPagesInSoursePdf[0] + "','"
                + item.NumberOfPagesInSoursePdf[1] + "');";
                result_command += sql_command;
                // Создаем Таблицу для пломб к текущему акту
                if (item.plomb.Count > 0)
                {
                    string plombTableName = item.ID.ToString() + item.Number + item.NumberLS + "Plobm";
                    //Имя таблицы пломб
                    sql_command = "DROP TABLE IF EXISTS '" + plombTableName + "';"
                      + "CREATE TABLE '" + plombTableName + "' ("
                      + "'id' INTEGER PRIMARY KEY AUTOINCREMENT, "
                      + "'Type' TEXT,     "
                      + "'Number' TEXT,     "
                      + "'Remove' INTEGER,     "
                      + "'Place' TEXT );";
                    result_command += sql_command;
                    foreach (plomba Plomba in item.plomb)
                    {
                        //добавляем строку с пломбой
                        sql_command = "INSERT INTO `" + plombTableName + "` (Type, Number, Remove, Place) "
                      + "VALUES ('"
                       + Plomba.Type + "', '"
                        + Plomba.Number + "', '"
                        + ((Plomba.Remove) ? "1'" : "0'") + ", '"
                        + Plomba.Place + "');";
                        result_command += sql_command;
                    }
                }
            }
            result_command = "BEGIN;" + result_command + "COMMIT;";
            try
            {
                cmd.CommandText = result_command;
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                connectorOnSaveLoad.Close();
                return false;
            }
            connectorOnSaveLoad.Close();
            return true;

        }
        /// <summary>
        /// Загрузка актов для работы
        /// </summary>
        /// <param name="ATP"></param>
        /// <param name="Path">Путь к файлу сохранения *.atp</param>
        /// <returns></returns>
        public bool LoadATP(string Path, ATPWorker ATP)
        {
            //Открываем соединение
            connectorOnSaveLoad = new SQLiteConnection("Data Source=" + Path + "; Version=3;");
            try
            {
                connectorOnSaveLoad.Open();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            // Загрузка листа проверок
            List<aktATP> result = new List<aktATP>();
            SQLiteCommand CommandSQL = new SQLiteCommand(connectorOnSaveLoad);
            CommandSQL.CommandText = "SELECT * "
            + " FROM `ATPList`;";
            try
            {
                SQLiteDataReader r = CommandSQL.ExecuteReader();
                string line = String.Empty;
                int i = 0;
                while (r.Read())
                {
                    List<int> Page = new List<int>();
                    Page.Add(Int32.Parse(r["Page1"].ToString()));
                    Page.Add(Int32.Parse(r["Page2"].ToString()));
                    result.Add(new aktATP(Int32.Parse(r["ID"].ToString()), Page, r["PathOfPdfFile"].ToString()));
                    //Ищем агентов по номеру
                    string temp_agent = r["Agent_1"].ToString();
                    if (temp_agent != "")
                    {
                        foreach (Agent item in ATP.agents)
                        {
                            if (item.SapNumber == temp_agent) result[i].Agent_1 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString);
                        }
                    }
                    temp_agent = r["Agent_2"].ToString();
                    if (temp_agent != "")
                    {
                        foreach (Agent item in ATP.agents)
                        {
                            if (item.SapNumber == temp_agent) result[i].Agent_1 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString);
                        }
                    }
                    //Создаем новй ПУ
                    string temp_pu = r["PuNewType"].ToString();
                    if (temp_pu != "")
                    {
                        foreach (PriborUcheta item in ATP.SpisokPU)
                        {
                            if (item.SapNumberPU == temp_pu) result[i].PuNewType = new PriborUcheta(item.SapNumberPU, item.Nazvanie, item.Poverka, item.Znachnost);
                        }
                    }
                    result[i].PuOldMPI = Int32.Parse(r["PuOldMPI"].ToString()) == 0 ? false : true;
                    result[i].DopuskFlag = Int32.Parse(r["DopuskFlag"].ToString()) == 0 ? false : true;
                    result[i].Adress = r["Adress"].ToString();
                    result[i].DateWork = DateTime.Parse(r["DateWork"].ToString());
                    result[i].FIO = r["FIO"].ToString();
                    result[i].Number = r["Number"].ToString();
                    result[i].NumberLS = r["NumberLS"].ToString();
                    result[i].PuNewNumber = r["PuNewNumber"].ToString();
                    result[i].PuNewPokazanie = r["PuNewPokazanie"].ToString();
                    result[i].PuNewPoverkaEar = r["PuNewPoverkaEar"].ToString();
                    result[i].PuNewPoverKvartal = r["PuNewPoverKvartal"].ToString();
                    result[i].PuOldNumber = r["PuOldNumber"].ToString();
                    result[i].PuOldPokazanie = r["PuOldPokazanie"].ToString();
                    result[i].PuOldType = r["PuOldType"].ToString();
                    result[i].TypeOfWork = r["TypeOfWork"].ToString();
                    SQLiteCommand CommandPlomb = new SQLiteCommand(connectorOnSaveLoad);
                    string plombTableName = result[i].ID.ToString() + result[i].Number + result[i].NumberLS + "Plobm";
                    CommandPlomb.CommandText = "SELECT name FROM sqlite_master WHERE TYPE = 'table' AND NAME = '" + plombTableName + "'";
                    try
                    {
                        SQLiteDataReader plomb_reader = CommandPlomb.ExecuteReader();
                        if (plomb_reader.Read())
                        {
                            plomb_reader.Close();
                            CommandPlomb.CommandText = "SELECT * "
                            + " FROM `" + plombTableName + "`;";
                            plomb_reader = CommandPlomb.ExecuteReader();
                            while (plomb_reader.Read())
                            {
                                string plomb_Type, plomb_Number, plomb_Place;
                                bool plomb_Remove;
                                plomb_Type = plomb_reader["Type"].ToString();
                                plomb_Number = plomb_reader["Number"].ToString();
                                plomb_Remove = Int32.Parse(plomb_reader["Remove"].ToString()) == 0 ? false : true; ;
                                plomb_Place = plomb_reader["Place"].ToString();
                                result[i].plomb.Add(new plomba(plomb_Type, plomb_Number, plomb_Place, plomb_Remove));
                            }
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    i++;
                }
                r.Close();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
            ATP.AllAtpInWorkList.Clear();
            foreach (aktATP item in result)
            {
                ATP.AllAtpInWorkList.Add(item);
            }
            ATP.AktATPInWork = ATP.AllAtpInWorkList[0];
            return true;
        }
        /// <summary>
        /// Очистка таблицы заполненных актов
        /// </summary>
        internal void DromCompliteTable()
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
        /// <summary>
        /// Инициализация листа актов из базы при загрузке приложения
        /// </summary>
        /// <param name="ATP"></param>
        public void LoadCompleteATP(ATPWorker ATP)
        {
            //Открываем соединение
            // Загрузка листа проверок
            List<aktATP> result = new List<aktATP>();
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
                result.Add(new aktATP(Int32.Parse(r["ID"].ToString()), Page, r["PathOfPdfFile"].ToString()));
                //Ищем агентов по номеру
                string temp_agent = r["Agent_1"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in ATP.agents)
                    {
                        if (item.SapNumber == temp_agent) result[i].Agent_1 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString);
                    }
                }
                temp_agent = r["Agent_2"].ToString();
                if (temp_agent != "")
                {
                    foreach (Agent item in ATP.agents)
                    {
                        if (item.SapNumber == temp_agent) result[i].Agent_2 = new Agent(item.SapNumber, item.Post, item.Surname, item.SearchString);
                    }
                }
                //Создаем новй ПУ
                string temp_pu = r["PuNewType"].ToString();
                if (temp_pu != "")
                {
                    foreach (PriborUcheta item in ATP.SpisokPU)
                    {
                        if (item.SapNumberPU == temp_pu) result[i].PuNewType = new PriborUcheta(item.SapNumberPU, item.Nazvanie, item.Poverka, item.Znachnost);
                    }
                }
                result[i].PuOldMPI = Int32.Parse(r["PuOldMPI"].ToString()) == 0 ? false : true;
                result[i].DopuskFlag = Int32.Parse(r["DopuskFlag"].ToString()) == 0 ? false : true;
                result[i].Adress = r["Adress"].ToString();
                DateTime date1 = DateTime.Parse(r["DateWork"].ToString());
                result[i].DateWork = date1;
                result[i].FIO = r["FIO"].ToString();
                result[i].Number = r["Number"].ToString();
                result[i].NumberLS = r["NumberLS"].ToString();
                result[i].PuNewNumber = r["PuNewNumber"].ToString();
                result[i].PuNewPokazanie = r["PuNewPokazanie"].ToString();
                result[i].PuNewPoverkaEar = r["PuNewPoverkaEar"].ToString();
                result[i].PuNewPoverKvartal = r["PuNewPoverKvartal"].ToString();
                result[i].PuOldNumber = r["PuOldNumber"].ToString();
                result[i].PuOldPokazanie = r["PuOldPokazanie"].ToString();
                result[i].PuOldType = r["PuOldType"].ToString();
                result[i].TypeOfWork = r["TypeOfWork"].ToString();
                result[i].NumberMail = Int32.Parse(r["NumberMail"].ToString());
                result[i].DateMail = r["DateMail"].ToString();
                result[i].Ustanovka = r["Ustanovka"].ToString();
                result[i].SapNumberAkt = r["SapNumberAkt"].ToString();
                result[i].EdOborudovania = r["EdOborudovania"].ToString();
                SQLiteCommand CommandPlomb = new SQLiteCommand(connector);
                string plombTableName = result[i].ID.ToString() + result[i].Number + result[i].NumberLS + "Plobm";
                CommandPlomb.CommandText = "SELECT name FROM sqlite_master WHERE TYPE = 'table' AND NAME = '" + plombTableName + "'";
                try
                {
                    SQLiteDataReader plomb_reader = CommandPlomb.ExecuteReader();
                    if (plomb_reader.Read())
                    {
                        plomb_reader.Close();
                        CommandPlomb.CommandText = "SELECT * "
                        + " FROM `" + plombTableName + "`;";
                        plomb_reader = CommandPlomb.ExecuteReader();
                        while (plomb_reader.Read())
                        {
                            string plomb_Type, plomb_Number, plomb_Place;
                            bool plomb_Remove;
                            plomb_Type = plomb_reader["Type"].ToString();
                            plomb_Number = plomb_reader["Number"].ToString();
                            plomb_Remove = Int32.Parse(plomb_reader["Remove"].ToString()) == 0 ? false : true; ;
                            plomb_Place = plomb_reader["Place"].ToString();
                            result[i].plomb.Add(new plomba(plomb_Type, plomb_Number, plomb_Place, plomb_Remove));
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                i++;
            }
            r.Close();
        


            ATP.CompleteAtpWorkList.Clear();
            foreach (aktATP item in result)
            {
                ATP.CompleteAtpWorkList.Add(item);
            }
       }
        /// <summary>
        /// Проверка на наличие акта в базе данных
        /// </summary>
        /// <param name="akt"></param>
        /// <returns></returns>
        public bool chekForContainsCompleteAktATP(aktATP akt)
        {
            SQLiteCommand CommandSQL = new SQLiteCommand(connector);
            CommandSQL.CommandText = "SELECT DateWork, Number, NumberLS "
            + " FROM `CompleteATPList` WHERE `DateWork` = '" + akt.DateWork + "' AND " + "`Number`='" + akt.Number + "' AND `NumberLS`='" + akt.NumberLS + "';";
            SQLiteDataReader r = CommandSQL.ExecuteReader();
            bool result = r.Read();
            r.Close();
            return result;
        }
        /// <summary>
        /// Запись в базу данных завершенных актов
        /// </summary>
        /// <param name="akti">Лист с актами тех. проверок</param>
        public void InsertCompleteAktAPT(AllATPObserv akti)
        {
            string resultCommand = "BEGIN; ";
            foreach (aktATP akt in akti)
            {
                if (!chekForContainsCompleteAktATP(akt))
                {
                    string sql_command = "INSERT INTO CompleteATPList(  'ID',`PathOfPdfFile`, 'Adress', Agent_1, Agent_2, DateWork, FIO, DopuskFlag, Number,NumberLS,PuNewNumber,PuNewPokazanie,PuNewPoverkaEar,PuNewPoverKvartal,PuNewType,PuOldMPI,PuOldNumber,PuOldPokazanie,PuOldType,TypeOfWork, Page1, Page2, NumberMail, DateMail,`Ustanovka`,`SapNumberAkt`,`EdOborudovania`)"
                  + "VALUES ('"
                   + akt.ID.ToString() + "', '"
                     + akt.PathOfPdfFile + "', '"
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
                    + akt.TypeOfWork + "','"
                    + akt.NumberOfPagesInSoursePdf[0] + "','"
                     + akt.NumberOfPagesInSoursePdf[1] + "','"
                      + akt.NumberMail.ToString() + "','"
                       + (akt.DateMail != null ? akt.DateMail : "") + "','"
                       + akt.Ustanovka + "','"
                       + akt.SapNumberAkt + "','"
                    + akt.EdOborudovania + "');";

                    resultCommand += sql_command;
                    // Создаем Таблицу для пломб к текущему акту
                    if (akt.plomb.Count > 0)
                    {
                        string plombTableName = akt.ID.ToString() + akt.Number + akt.NumberLS + "Plobm"; //Имя таблицы пломб
                        sql_command = "DROP TABLE IF EXISTS '" + plombTableName + "';"
                          + "CREATE TABLE '" + plombTableName + "' ("
                          + "'id' INTEGER PRIMARY KEY AUTOINCREMENT, "
                          + "'Type' TEXT,     "
                          + "'Number' TEXT,     "
                          + "'Remove' INTEGER,     "
                          + "'Place' TEXT );";
                        resultCommand += sql_command;
                        foreach (plomba Plomba in akt.plomb)
                        {
                            //добавляем строку с пломбой
                            sql_command = "INSERT INTO `" + plombTableName + "` (Type, Number, Remove, Place) "
                          + "VALUES ('"
                           + Plomba.Type + "', '"
                            + Plomba.Number + "', '"
                            + ((Plomba.Remove) ? "1'" : "0'") + ", '"
                            + Plomba.Place + "');";
                            resultCommand += sql_command;
                        }
                    }
                }
            }
            resultCommand += " COMMIT;";
            var cmd = connector.CreateCommand();
            cmd.CommandText = resultCommand;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// Запись в базу данных завершенного акта
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        public void InsertCompleteAktAPT(aktATP akt)
        {
            if (!chekForContainsCompleteAktATP(akt))
            {
                var cmd = connector.CreateCommand();
                string sql_command = "INSERT INTO CompleteATPList(  'ID',`PathOfPdfFile`, 'Adress', Agent_1, Agent_2, DateWork, FIO, DopuskFlag, Number,NumberLS,PuNewNumber,PuNewPokazanie,PuNewPoverkaEar,PuNewPoverKvartal,PuNewType,PuOldMPI,PuOldNumber,PuOldPokazanie,PuOldType,TypeOfWork, Page1, Page2, NumberMail, DateMail,`Ustanovka`,`SapNumberAkt`,`EdOborudovania`)"
              + "VALUES ('"
               + akt.ID.ToString() + "', '"
                 + akt.PathOfPdfFile + "', '"
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
                + akt.TypeOfWork + "','"
                + akt.NumberOfPagesInSoursePdf[0] + "','"
                 + akt.NumberOfPagesInSoursePdf[1] + "','"
                  + akt.NumberMail.ToString() + "','"
                   + (akt.DateMail != null ? akt.DateMail : "") + "','"
                   + akt.Ustanovka + "','"
                   + akt.SapNumberAkt + "','"
                + akt.EdOborudovania + "');";

                cmd.CommandText = sql_command;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                // Создаем Таблицу для пломб к текущему акту
                if (akt.plomb.Count > 0)
                {
                    cmd = connector.CreateCommand();
                    string plombTableName = akt.ID.ToString() + akt.Number + akt.NumberLS + "Plobm"; //Имя таблицы пломб
                    sql_command = "DROP TABLE IF EXISTS '" + plombTableName + "';"
                      + "CREATE TABLE '" + plombTableName + "' ("
                      + "'id' INTEGER PRIMARY KEY AUTOINCREMENT, "
                      + "'Type' TEXT,     "
                      + "'Number' TEXT,     "
                      + "'Remove' INTEGER,     "
                      + "'Place' TEXT );";
                    cmd.CommandText = sql_command;
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show(ex.Message);
                        connector.Close();
                    }
                    foreach (plomba Plomba in akt.plomb)
                    {
                        //добавляем строку с пломбой
                        cmd = connector.CreateCommand();
                        sql_command = "INSERT INTO `" + plombTableName + "` (Type, Number, Remove, Place) "
                      + "VALUES ('"
                       + Plomba.Type + "', '"
                        + Plomba.Number + "', '"
                        + ((Plomba.Remove) ? "1'" : "0'") + ", '"
                        + Plomba.Place + "');";
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
                }
            }
        }
        /// <summary>
        /// Обновление базы данных физ.  лиц
        /// </summary>
        /// <param name="dataSetSAPFL"></param>
        public void RefreshSAPFL(DataSet dataSetSAPFL)
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
                            dt.Dispose();
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
    }
}
