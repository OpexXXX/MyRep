using SAPFEWSELib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoonPdf
{
    /// <summary>
    /// Класс для работы с SAPGui
    /// </summary>
    public class SAPActive
    {
        /// <summary>
        /// Приложение SAP
        /// </summary>
        private GuiApplication SapGuiApp { get; set; }
        /// <summary>
        /// SAP
        /// </summary>
        private GuiConnection SapConnection { get; set; }
        /// <summary>
        /// Сессия для работы
        /// </summary>
        private GuiSession SapSession { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="env">SID сервер SAP</param>
        public SAPActive(string env = "ER2")
        {
            SapGuiApp = new GuiApplication();

            string connectString = null;
            if (env.ToUpper().Equals("DEFAULT"))
            {
                connectString = "11.0 Test ERP (DEFAULT)";
            }
            else
            {
                connectString = env;
            }
            SapConnection = SapGuiApp.OpenConnection(connectString, Sync: true); //creates connection
            SapSession = (GuiSession)SapConnection.Sessions.Item(0); //creates the Gui session off the connection you made

        }
        /// <summary>
        /// Авторизация в SAP
        /// </summary>
        /// <param name="myclient">200</param>
        /// <param name="mylogin">Логин</param>
        /// <param name="mypass">Пароль</param>
        /// <param name="mylang"></param>
        /// <returns>Возвращает true при удачной авторизации</returns>
        public bool login(string myclient = "200", string mylogin = "MS24_3_RU", string mypass = "Jht[jdcrbq", string mylang = "")
        {
            try
            {
                GuiTextField client = (GuiTextField)SapSession.ActiveWindow.FindByName("RSYST-MANDT", "GuiTextField");
                GuiTextField login = (GuiTextField)SapSession.ActiveWindow.FindByName("RSYST-BNAME", "GuiTextField");
                GuiTextField pass = (GuiTextField)SapSession.ActiveWindow.FindByName("RSYST-BCODE", "GuiPasswordField");
                GuiTextField language = (GuiTextField)SapSession.ActiveWindow.FindByName("RSYST-LANGU", "GuiTextField");
                //            client.SetFocus();
                client.Text = myclient;
                //            login.SetFocus();
                login.Text = mylogin;
                pass.SetFocus();
                pass.Text = mypass;
                //            language.SetFocus();
                language.Text = mylang;
                //Press the green checkmark button which is about the same as the enter key 
                GuiButton btn = (GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[0]/btn[0]");
                //            btn.SetFocus();
                btn.Press();
            }
            catch (Exception)
            {

                return false;
            }
            try
            {
                GuiModalWindow wnd = (GuiModalWindow)SapSession.ActiveWindow;
                GuiRadioButton radioButtonS = (GuiRadioButton)SapSession.ActiveWindow.FindByName("MULTI_LOGON_OPT2", "GuiRadioButton");
                radioButtonS.Select();
                GuiButton btnOk = (GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[1]/tbar[0]/btn[0]");
                btnOk.Press();
            }
            catch (Exception)
            {
            }
            return true;
        }
        /// <summary>
        /// Закупка и выдача пломбы, , запускаеся самостоятельной транзакцией
        /// </summary>
        /// <param name="plomb">Пломба</param>
        /// <param name="dateOfPlacement">Дата закупки, выдачи</param>
        public void shopPlomb(plomba plomb, string dateOfPlacement)
        {
            SapSession.StartTransaction("/SAPCE/IUSEALS");
            //Жмем кнопку закупка
            GuiButton btn = (GuiButton)SapSession.FindById("wnd[0]/tbar[1]/btn[25]");
            btn.Press();
            GuiTextField typePlombS = (GuiTextField)SapSession.ActiveWindow.FindByName("N_SCAT", "GuiCTextField");
            GuiTextField numberPlombS = (GuiTextField)SapSession.ActiveWindow.FindByName("N_SCODE-LOW", "GuiTextField");
            GuiTextField colorCodPlombS = (GuiTextField)SapSession.ActiveWindow.FindByName("N_COLOR", "GuiCTextField");
            GuiTextField idMasterS = (GuiTextField)SapSession.ActiveWindow.FindByName("N_UTMAS", "GuiCTextField");
            GuiTextField dateShopS = (GuiTextField)SapSession.ActiveWindow.FindByName("N_DPURCH", "GuiCTextField");
            typePlombS.Text = plomb.Type;
            numberPlombS.Text = plomb.Number;
            colorCodPlombS.Text = "1";
            idMasterS.Text = "6";
            dateShopS.Text = dateOfPlacement;
            btn = (GuiButton)SapSession.FindById("wnd[1]/tbar[0]/btn[8]");
            btn.Press(); //Жмем закупить

            GuiStatusbar sbarS = (GuiStatusbar)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/sbar");
            if (!sbarS.Text.Contains("в базе данных уже существует"))
            {


                SapSession.StartTransaction("/SAPCE/IUSEALS");
                //Выдача
                //Поиск закупленных пломб
                GuiTextField typePlombSearch = (GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtSCAT-LOW");
                GuiTextField numberPlombSearch = (GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/txtSCODE-LOW");
                typePlombSearch.Text = plomb.Type;
                numberPlombSearch.Text = plomb.Number;
                GuiCheckBox PURCH = (GuiCheckBox)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/chkPURCH");
                GuiCheckBox ISSUE = (GuiCheckBox)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/chkISSUE");
                GuiCheckBox INSTA = (GuiCheckBox)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/chkINSTA");
                PURCH.Selected = true;
                ISSUE.Selected = false;
                INSTA.Selected = false;
                GuiButton btnSearch = (GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[8]");
                btnSearch.Press(); //жмем поиск

                //Окно с результатами поиска
                GuiGridView plombArea = (GuiGridView)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlALV_SEALS/shellcont/shell");
                if (plombArea.RowCount > 0)
                {
                    ((GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[25]")).Press(); //Редактировать
                    plombArea.SelectAll();
                    ((GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[1]/btn[31]")).Press(); //Выдать
                    ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[1]/usr/ctxt/SAPCE/IURU_SEALS_CHANGED-REPER")).Text = "6"; //Ответственное лицо "Красноярскэнерго"
                    ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[1]/usr/ctxt/SAPCE/IURU_SEALS_CHANGED-DISSUE")).Text = dateOfPlacement; //Дата выдачи пломбы
                    ((GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[1]/tbar[0]/btn[5]")).Press(); //Скопировать во все
                    ((GuiButton)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/tbar[0]/btn[11]")).Press(); //Сохранить
                }
            }

        }
        /// <summary>
        /// Занесение акта тех. проверки в SAP
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        /// <param name="pdfDirectory">Дериктория с PDF файлами проверок</param>
        public void enterAktTehProverki(aktATP akt, string pdfDirectory)
        {
            string dataProvodkiAkta = akt.DateWork.ToString("d");

            string pokazanieProverki, primechanieKAkty;
            //Закупаем пломбы 
            foreach (plomba item in akt.plomb)
            {
                if (item.Number.Length > 0) shopPlomb(item, akt.DateWork.ToString("d"));
            }

            if (akt.DopuskFlag) //Если тип акта допуск
            {
                pokazanieProverki = akt.PuNewPokazanie; //Показания для акта проверки
                primechanieKAkty = "Допуск " + ((akt.Agent_2 != null) ? (akt.Agent_2.Surname) : "") + " "; //Примечание для акта
                string result = demontirovatPU(akt, akt.DateWork);//Демонтируем счетчик
                if (result == "") return;
                if (result != "ok") dataProvodkiAkta = result;
                string dateMontagPU = akt.DateWork.ToString("d");
                if (result != "ok") dateMontagPU = result;
                result = montirovatPU(akt, dateMontagPU);//Монтируем счетчик
                if (result == "") return;
            }

            else//Если тип акта проверка
            {
                pokazanieProverki = akt.PuOldPokazanie;
                primechanieKAkty = ((akt.Agent_2 != null) ? (akt.Agent_2.Surname) : "") + " "; //Примечание
            }

            SapSession.StartTransaction("CV01N");
            ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtDRAW-DOKNR")).Text = "*";
            ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtDRAW-DOKAR")).Text = "ATP";
            SapSession.SendCommand(""); //Enter

            /*******************Вкладка оснвная****************************************/
            GuiCTextField BEText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-BUKRS", "GuiCTextField");
            GuiCTextField UstanovkaText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-ANLAGE", "GuiCTextField");
            GuiComboBox TypeAktCombo = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-VID", "GuiComboBox");
            GuiComboBox TypeProverkaCombo = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-ISTABLART", "GuiComboBox");
            GuiTextField NumberProvText = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-NUM", "GuiTextField");
            GuiCTextField DateAktText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-DATE_ACT", "GuiCTextField");
            GuiCTextField AgentText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-PERNR", "GuiCTextField");
            GuiComboBox ResultProverkaCombo = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-RES", "GuiComboBox");
            GuiCTextField typePredpisan = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-VID_RES", "GuiCTextField");
            GuiCTextField SrokUstraneniyaText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-SROK", "GuiCTextField");
            GuiTextField PrimechanieText = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-PRIM", "GuiTextField");
            GuiTextField Pokazanie = ((GuiTextField)SapSession.FindById("wnd[0]/usr/tabsTAB_MAIN/tabpTSMAIN/ssubSCR_MAIN:SAPLCV110:0200/ssubSCRCUST:SAPLXCV110:0100/subMAIN_SCR:/MRSKS/ZWF_EXEC_ISU_ATP:0200/tabsTAB_MAIN/tabpTAB_MAIN_FC1/ssubMAIN_SCA:/MRSKS/ZWF_EXEC_ISU_ATP:0201/tbl/MRSKS/ZWF_EXEC_ISU_ATPEABL_VIEW/txtGT_OIG-POKAZ_TEC[5,0]"));
            GuiTab FilesTab = (GuiTab)SapSession.ActiveWindow.FindByName("TSFILES", "GuiTab");
            GuiTab MainTab = (GuiTab)SapSession.ActiveWindow.FindByName("TSMAIN", "GuiTab");
            GuiButton SaveAktBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[11]", "GuiButton"); //Сохранить акт
            GuiStatusbar statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");//sbar
            BEText.Text = "2400";
            UstanovkaText.Text = akt.Ustanovka; //Установка
            TypeAktCombo.Key = "1"; //Контрольная проверка
            TypeProverkaCombo.Key = "13";//13 плановая
            NumberProvText.Text = akt.Number.ToString(); //Номер проверки
            DateAktText.Text = dataProvodkiAkta;// Дата проверки

            AgentText.Text = akt.Agent_1.SapNumber;// 1ый Агент к акту
            ResultProverkaCombo.Key = akt.PuOldMPI ? "3" : "1";// 1-Соответсвует. 3 -Не соответствует
            typePredpisan.Text = akt.PuOldMPI ? "10" : ""; //10 - Истек МПИ
            SrokUstraneniyaText.Text = akt.PuOldMPI ? dataProvodkiAkta : "";//Дата устранения

            Pokazanie.Text = pokazanieProverki; //Показание ПУ
            SapSession.SendCommand(""); //Enter

            if (statusBar.Text.Contains("закрыт")) //Если период закрыт разносим вторым числом текущего месяца
            {
                var datet = akt.DateWork.AddMonths(1);
                string date = "02." + datet.Month.ToString() + "." + datet.Year.ToString();
                DateAktText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-DATE_ACT", "GuiCTextField");
                DateAktText.Text = date;
                SapSession.SendCommand("");
                primechanieKAkty += " " + akt.DateWork.ToString("d");
            }
            PrimechanieText = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-PRIM", "GuiTextField");
            PrimechanieText.Text = primechanieKAkty; //Примечание


            //Ведение Пломб*******************************/
            ustanovkaPlomb(akt);
            /*Добавление файлов*/
            addFIleToAkt(akt, pdfDirectory);
            //Сохранение
            SaveAktBtn.Press();
            GuiButton SaveAktOKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //Сохранить акт
            SaveAktOKBtn.Press();
            try
            {
                GuiModalWindow ErrorWindow = ((GuiModalWindow)SapSession.FindById("/app/con[0]/ses[0]/wnd[1]"));
                if (ErrorWindow.Text.Contains("СтрокиДокум: Просмотр сообщений")) //Если окно 0 из 1 показаний загружено
                {
                    GuiButton OKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //
                    OKBtn.Press();
                }
            }
            catch (Exception)
            {

            }
            try
            {
                GuiTextField ErrorText = ((GuiTextField)SapSession.FindById("/wnd[1]/usr/txtMESSTXT1"));
                if (ErrorText.Text.Contains("Ошибка при проверке на входе и сохранении")) //Если ошибка сохранения оригиналов
                {
                    GuiButton OKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //
                    OKBtn.Press();
                }
            }
            catch (Exception)
            {

            }
            if (statusBar.Text.Contains("создан"))
            {
                string numberSAPakt = "", sbarText = "";
                sbarText = statusBar.Text;
                numberSAPakt = sbarText.Split(' ')[2];
                akt.SapNumberAkt = numberSAPakt;
            }
            else
            {

            }
        }
        /// <summary>
        /// Добавление файлов в карточку, запускается  из окна занесения акта
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        /// <param name="pdfDirectory">Дериктория с PDF файлами проверок</param>
        private void addFIleToAkt(aktATP akt, string pdfDirectory)
        {
            GuiTab FilesTab = (GuiTab)SapSession.ActiveWindow.FindByName("TSFILES", "GuiTab");
            FilesTab.Select();
            /************************Вкладка файлы********************/
            GuiButton dobavitFileBtn = (GuiButton)SapSession.ActiveWindow.FindByName("PB_FILE_CREATE", "GuiButton"); //PB_FILE_CREATE Создать вложение
            dobavitFileBtn.Press();
            /*Окно добавление файла*/
            GuiCTextField typeFile = (GuiCTextField)SapSession.ActiveWindow.FindByName("DRAW-DAPPL", "GuiCTextField");  //DRAW-DAPPL "ПДФ"
            GuiTextField primechanieFile = (GuiTextField)SapSession.ActiveWindow.FindByName("MCDOK-FILE_DESCRIPTION", "GuiTextField"); //MCDOK-FILE_DESCRIPTION Описание файла
            GuiCTextField pathFile = (GuiCTextField)SapSession.ActiveWindow.FindByName("DRAW-FILEP1", "GuiCTextField"); //DRAW-FILEP1 Путь к файлу
            GuiButton okFileBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //btn[0] КнопкаОК
            typeFile.Text = "PDF";
            primechanieFile.Text = "";
            pathFile.Text = pdfDirectory + akt.PathOfPdfFile;
            okFileBtn.Press();
            try
            {
                GuiModalWindow ErrorWindow = ((GuiModalWindow)SapSession.FindById("/app/con[0]/ses[0]/wnd[2]"));

            }
            catch (Exception)
            {

            }
            /***********************************/
        }
        /// <summary>
        /// Добавление пломб к акту, запускается  из окна занесения акта
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        private void ustanovkaPlomb(aktATP akt)
        {
            if (akt.plomb.Count == 0) return;
            GuiButton VedeniePlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("BT_SEAL", "GuiButton");
            VedeniePlombBtn.Press();
            GuiGridView GridPlomb = ((GuiGridView)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlALV_SEALS/shellcont/shell"));
            GuiButton addPlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[30]", "GuiButton"); //GuiButton//btn[30] Добавить выданные
            GuiButton ustanovitPlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[32]", "GuiButton"); // Установть выделенные
            GuiStatusbar statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");//sbar

            foreach (plomba Plomba in akt.plomb)
            {
                if (Plomba.Number.Length > 0)
                {
                    addPlombBtn.Press();
                    /*Окно Добавить выданные*/
                    GuiCTextField typePlombAdd = (GuiCTextField)SapSession.ActiveWindow.FindByName("P_SCAT-LOW", "GuiCTextField"); //P_SCAT-LOW Тип пломбы
                    GuiTextField kodPlombAdd = (GuiTextField)SapSession.ActiveWindow.FindByName("P_SCODE-LOW", "GuiTextField");//P_SCODE-LOW
                    GuiButton addPlombOk = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton");//btn[8] Выполнить
                    typePlombAdd.Text = Plomba.Type; //Тип пломбы
                    kodPlombAdd.Text = Plomba.Number; //Номер Пломбы
                    addPlombOk.Press();
                    if (statusBar.Text.Contains("недавно добавлено"))
                    {
                        GridPlomb.SelectColumn("SCODE");
                        GridPlomb.PressToolbarButton("&MB_FILTER");
                        /**Окно фильтра***************************/
                        GuiCTextField NumberPlombFilter = (GuiCTextField)SapSession.ActiveWindow.FindByName("%%DYN001-LOW", "GuiCTextField");
                        GuiButton Okbtn = ((GuiButton)SapSession.FindById("wnd[1]/tbar[0]/btn[0]")); //Ок
                        NumberPlombFilter.Text = Plomba.Number;//Код пломбы для фильтра
                        Okbtn.Press();
                        /*************************/
                        if (GridPlomb.RowCount > 0)
                        {
                            GridPlomb.SelectAll(); //ВЫделяем все
                            ustanovitPlombBtn.Press(); //Установить
                                                       /*Окно установки пломбы*/
                            GuiButton findMaterial = (GuiButton)SapSession.ActiveWindow.FindByName("ISU_FINDER_DIALOG", "GuiButton"); //ISU_FINDER_DIALOG
                            GuiButton copyToAll = (GuiButton)SapSession.ActiveWindow.FindByName("btn[5]", "GuiButton"); //Скопировать во все
                            findMaterial.Press();

                            /*Окно поиска материала*/
                            GuiTab TabTwo = (GuiTab)SapSession.ActiveWindow.FindByName("TAB2", "GuiTab"); //TAB2
                            TabTwo.Select();
                            GuiCTextField ustanovkaTextFind = (GuiCTextField)SapSession.ActiveWindow.FindByName("EFINDD-I_ANLAGE", "GuiCTextField");//EFINDD-I_ANLAGE
                            ustanovkaTextFind.Text = akt.Ustanovka;
                            GuiButton FindOk = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton");//btn[8] Выполнить//btn[0]
                            FindOk.Press();
                            try
                            {
                                GuiLabel Label = ((GuiLabel)SapSession.FindById("wnd[2]/usr/lbl[1,4]")); //Ок
                                Label.SetFocus();
                                GuiModalWindow rWindow = ((GuiModalWindow)SapSession.FindById("/app/con[0]/ses[0]/wnd[2]"));
                                rWindow.SendVKey(2);
                                ///findById" @()
                                ///app/con[0]/ses[0]/wnd[2]/usr/lbl[1,4] Описать окно выбора счетчика при установке пломбы
                            }
                            catch (Exception)
                            {


                            }

                            /************************/
                            GuiTextField placePlomb = (GuiTextField)SapSession.ActiveWindow.FindByName("/SAPCE/IURU_SEALS_CHANGED-PLACE", "GuiTextField");////Место установки пломбы /SAPCE/IURU_SEALS_CHANGED-PLACE
                            GuiCTextField datePlacePlomb = (GuiCTextField)SapSession.ActiveWindow.FindByName("/SAPCE/IURU_SEALS_CHANGED-DINST", "GuiCTextField");//Дата установки пломбы /SAPCE/IURU_SEALS_CHANGED-DINST
                            placePlomb.Text = Plomba.Place;
                            datePlacePlomb.Text = akt.DateWork.ToString("d");
                            copyToAll.Press();
                            /*******************************/
                        }
                        GridPlomb.PressToolbarContextButton("&MB_FILTER");
                        GridPlomb.SelectContextMenuItem("&DELETE_FILTER");
                    }
                }
            }

            GuiButton backBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[3]", "GuiButton"); //назад
            GuiButton SaveAktOKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[11]", "GuiButton"); //Сохранить
            SaveAktOKBtn.Press();
            backBtn.Press();
        }
        /// <summary>
        /// Демонтирует старый ПУ, запускаеся самостоятельной транзакцией
        /// </summary>
        /// <param name="akt"> Акт тех. проверки</param>
        /// <param name="dataP"> Дата демонтажа</param>
        /// <returns>Возвращает "" при неудачной попытке демонтажа, "ok" при удачной попытке и "02.01.2017" строку с датой при разноске в закрытый период</returns>
        private string demontirovatPU(aktATP akt, DateTime dataP)
        {
            DateTime dataProvodki = new DateTime(dataP.Year, dataP.Month, dataP.Day);

            /***********Узнаем серийныи номер пу*******************/
            string serNumberOldPU;
            SapSession.StartTransaction("IE03");
            GuiCTextField edinitcaOb = (GuiCTextField)SapSession.ActiveWindow.FindByName("RM63E-EQUNR", "GuiCTextField");
            edinitcaOb.Text = akt.EdOborudovania;
            SapSession.SendCommand("");
            GuiTab tabISU = (GuiTab)SapSession.ActiveWindow.FindByName("T\\04", "GuiTab");
            tabISU.Select();
            GuiTextField serNumber = (GuiTextField)SapSession.ActiveWindow.FindByName("EDEVICED-GERAET", "GuiTextField");
            serNumberOldPU = serNumber.Text;
            /*******************************************************/

            SapSession.StartTransaction("EG32");
            GuiCTextField dateDemontag = (GuiCTextField)SapSession.ActiveWindow.FindByName("REG30-EADAT", "GuiCTextField");
            GuiCTextField serNumberDemontag = (GuiCTextField)SapSession.ActiveWindow.FindByName("REG30-GERAETALT", "GuiCTextField");
            dateDemontag.Text = dataProvodki.ToString("d");
            serNumberDemontag.Text = serNumberOldPU;
            SapSession.SendCommand("");//

            GuiStatusbar statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");//sbar
            if (statusBar.Text.Contains("в будущем уже есть документы"))
            {
                //Удаляем показания
                SapSession.StartTransaction("EL37");
                GuiCTextField serNumberPUdel = (GuiCTextField)SapSession.ActiveWindow.FindByName("GERAET", "GuiCTextField");
                GuiCTextField lowDateDel = (GuiCTextField)SapSession.ActiveWindow.FindByName("ADATSOLL-LOW", "GuiCTextField");
                GuiCTextField highDateDel = (GuiCTextField)SapSession.ActiveWindow.FindByName("ADATSOLL-HIGH", "GuiCTextField");
                GuiButton GoBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton"); //Сохранить акт
                string startDateDel, endDateDel;
                var startDate = new DateTime(akt.DateWork.Year, akt.DateWork.Month, 1);
                startDateDel = startDate.ToString("d");
                endDateDel = (akt.DateWork.AddMonths(1).AddDays(-1)).ToString("d");
                serNumberPUdel.Text = serNumberOldPU;
                lowDateDel.Text = startDateDel;
                highDateDel.Text = endDateDel;
                GoBtn.Press();
                GuiGridView gridArea = (GuiGridView)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlCONTAINER_EABLD_EL37_EL35/shellcont/shell");
                // gridArea.SelectedRows = "0";
                gridArea.PressToolbarButton("MARK");
                gridArea.PressToolbarButton("CANC");

                SapSession.StartTransaction("EG32");
                dateDemontag = (GuiCTextField)SapSession.ActiveWindow.FindByName("REG30-EADAT", "GuiCTextField");
                serNumberDemontag = (GuiCTextField)SapSession.ActiveWindow.FindByName("REG30-GERAETALT", "GuiCTextField");
                dateDemontag.Text = dataProvodki.ToString("d");
                serNumberDemontag.Text = serNumberOldPU;
                SapSession.SendCommand("");
            }
            statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");//sbar
            if (statusBar.Text.Contains("не соответствует введенным данным")) return "ok";
            if (statusBar.Text.Contains("рассчитана после"))
            {
                dataProvodki = dataProvodki.AddMonths(1);
                // dataProvodki.AddDays(1);
                DateTime datet = new DateTime(dataProvodki.Year, dataProvodki.Month, 2);
                string res = demontirovatPU(akt, datet);
                if (res == "ok")
                {
                    return datet.ToString("d");
                }
                else
                {
                    return "";
                }
            }
            GuiTextField Pokazanie = ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/tblSAPLE30DCONTROL_RE_REM/txtREG30-ZWSTANDCA[5,0]"));
            Pokazanie.Text = akt.PuOldPokazanie;
            GuiButton SaveAktBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[11]", "GuiButton"); //Сохранить акт
            SaveAktBtn.Press();
            return "ok";
        }
        /// <summary>
        /// Монтирует новый ПУ к установке, запускаеся самостоятельной транзакцией
        /// </summary>
        /// <param name="akt">Акт тех. проверки</param>
        /// <param name="date">Дата монтажа</param>
        /// <returns>Возвращает серийный номер смонтированного ПУ  при удачной попытке демонтажа, "" при неудачной попытке</returns>
        private string montirovatPU(aktATP akt, string date)
        {
            SapSession.StartTransaction("/MRSKS/ISU_CARD");
            GuiCTextField ustanovka = (GuiCTextField)SapSession.ActiveWindow.FindByName("P_ANLAGE", "GuiCTextField");
            ustanovka.Text = akt.Ustanovka;
            GuiButton GoBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton"); //
            GoBtn.Press();
            GuiToolbarControl toolbarSAP = ((GuiToolbarControl)SapSession.FindById("wnd[0]/usr/subCUST_SCREEN:/MRSKS/SAPLISU_ARM:1002/cntlGO_CONT_TOOLBAR_ANLAGE/shellcont/shell"));
            toolbarSAP.PressButton("IQ01");
            GuiRadioButton newPU = (GuiRadioButton)SapSession.ActiveWindow.FindByName("SPOPLI-SELFLAG", "GuiRadioButton"); //
            newPU.Selected = true;
            GuiButton OKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //
            OKBtn.Press();
            GuiTextField yearProdaction = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-BAUJJ", "GuiTextField"); //GS_LOGIKNR-BAUJJ Год выпуска GuiTextField 
            GuiTextField dateMontag = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-AB", "GuiTextField"); //GS_LOGIKNR-AB Действительно с.. GuiTextField 
            GuiCTextField typePU = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-MATNR", "GuiCTextField"); //GS_LOGIKNR-MATNR Тип прибора  GuiCTextField 
            GuiTextField poverkaYar = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-BGLJAHR", "GuiTextField"); //GS_LOGIKNR-BGLJAHR год поверки GuiTextField 
            GuiTextField periodPoverki = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-CUST_FIELD-PERIOD", "GuiTextField"); //GS_LOGIKNR-CUST_FIELD-PERIOD период поверки GuiTextField 
            GuiComboBox poverkaKvartal = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-CUST_FIELD-CHECK_QUART", "GuiComboBox");//GS_LOGIKNR-CUST_FIELD-CHECK_QUART Квартал поверки GuiComboBox 
            GuiTextField PuNumber = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-CUST_FIELD-COUNTERNUMBER", "GuiTextField");//GS_LOGIKNR-CUST_FIELD-COUNTERNUMBER Номер ПУ GuiTextField 
            yearProdaction.Text = akt.PuNewPoverkaEar;
            dateMontag.Text = date;
            typePU.Text = akt.PuNewType.SapNumberPU;
            poverkaYar.Text = akt.PuNewPoverkaEar;
            periodPoverki.Text = akt.PuNewType.Poverka.ToString();
            poverkaKvartal.Key = akt.PuNewPoverKvartal;
            PuNumber.Text = akt.PuNewNumber;

            SapSession.SendCommand("");
            GuiCTextField znachnostPU = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-ZWGRUPPE", "GuiCTextField"); //GS_LOGIKNR-ZWGRUPPE значность GuiCTextField 
            GuiTextField Pokazanie = (GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/subCUST_SCREEN:/MRSKS/ISU_ARM_LOGIKNR:1003/tbl/MRSKS/ISU_ARM_LOGIKNRGT_LOGIKZW_SCR/txtGS_LOGIKZW-ARESULT_TXT[5,0]");//Показание  GuiTextField 
            znachnostPU.Text = akt.PuNewType.Znachnost;
            Pokazanie.Text = akt.PuNewPokazanie;
            GuiButton SavePUBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[11]", "GuiButton");

            GuiStatusbar statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");
            SavePUBtn.Press();


            if (!statusBar.Text.Contains("смонтирован"))
            {
                return "";
            }

            GuiButton OkBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton");
            OkBtn.Press();
            OkBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton");
            OkBtn.Press();
            OkBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton");
            OkBtn.Press();
            GuiTextField SeriiniiNomer = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_LOGIKNR-SERNR", "GuiTextField");
            //GS_LOGIKNR-SERNR GuiTextField Серийный номер

            return SeriiniiNomer.Text;
        }
    }
}
