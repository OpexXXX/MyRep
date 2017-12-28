﻿using SAPFEWSELib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MoonPdf
{
    public class SAPActive
    {
        private GuiApplication SapGuiApp { get; set; }
        private GuiConnection SapConnection { get; set; }
        private GuiSession SapSession { get; set; }

        public SAPActive(string env)
        {
            SapGuiApp = new GuiApplication();

            string connectString = null;
            if (env.ToUpper().Equals("DEFAULT"))
            {
                connectString = "1.0 Test ERP (DEFAULT)";
            }
            else
            {
                connectString = env;
            }
            SapConnection = SapGuiApp.OpenConnection(connectString, Sync: true); //creates connection
            SapSession = (GuiSession)SapConnection.Sessions.Item(0); //creates the Gui session off the connection you made

        }
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
            btn.Press(); //жмем поиск

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
        public void enterAktTehProverki(aktATP akt, string pdfDirectory)
        {
            //Закупаем пломбы 
            foreach (plomba item in akt.plomb)
            {
                shopPlomb(item, akt.DateWork.ToString("d"));
            }
            SapSession.StartTransaction("/CV01N");
            ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtDRAW-DOKNR")).Text = "*";
            ((GuiTextField)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/ctxtDRAW-DOKAR")).Text = "ATP";
            SapSession.SendCommand(""); //Enter
                                        /*******************Вкладка оснвная****************************************/
            GuiCTextField BEText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-BUKRS", "GuiCTextField");
            GuiCTextField UstanovkaText = (GuiCTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-ANLAGE", "GuiCTextField");
            GuiComboBox TypeAktCombo = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-VID", "GuiComboBox");
            GuiComboBox TypeProverkaCombo = (GuiComboBox)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-ISTABLART", "GuiComboBox");
            GuiTextField NumberProvText = (GuiTextField)SapSession.ActiveWindow.FindByName("GS_DATA_MAIN-ANLAGE", "GuiTextField");
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
            UstanovkaText.Text = akt.Ustanovka;
            TypeAktCombo.Key = "1"; //Контрольная проверка
            TypeProverkaCombo.Key = "13";//13 плановая
            NumberProvText.Text = akt.Number.ToString();
            DateAktText.Text = akt.DateWork.ToString("d");
            AgentText.Text = akt.Agent_1.SapNumber;
            ResultProverkaCombo.Key = "1";// 1-Соответсвует. 3 -Не соответствует
            typePredpisan.Text = ""; //10 - Истек МПИ
            SrokUstraneniyaText.Text = "";//Дата устранения
            PrimechanieText.Text = akt.Agent_2.Surname + ""; //Примечание
            Pokazanie.Text = akt.PuOldPokazanie; //Показание ПУ
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
                if(ErrorWindow.Text.Contains("СтрокиДокум: Просмотр сообщений")) //Если окно 0 из 1 показаний загружено
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
                statusBar.Text = sbarText;
                numberSAPakt = sbarText.Split(' ')[2];
            }
            else
            {

            }
        }
        /// <summary>
        /// Добавление файлов в карточку, запускается  из окна занесения акта
        /// </summary>
        private void addFIleToAkt(aktATP akt, string pdfDirectory)
        {
            GuiTab FilesTab = (GuiTab)SapSession.ActiveWindow.FindByName("TSFILES", "GuiTab");
            FilesTab.Select();
            /************************Вкладка файлы********************/
            GuiButton dobavitFileBtn = (GuiButton)SapSession.ActiveWindow.FindByName("PB_FILE_CREATE", "GuiButton"); //PB_FILE_CREATE Создать вложение
            dobavitFileBtn.Press();
            /*Окно добавление файла*/
            GuiCTextField typeFile = (GuiCTextField)SapSession.ActiveWindow.FindByName("DRAW-DAPPL", "GuiCTextField");  //DRAW-DAPPL "ПДФ"
            GuiCTextField primechanieFile = (GuiCTextField)SapSession.ActiveWindow.FindByName("MCDOK-FILE_DESCRIPTION", "GuiCTextField"); //MCDOK-FILE_DESCRIPTION Описание файла
            GuiCTextField pathFile = (GuiCTextField)SapSession.ActiveWindow.FindByName("DRAW-FILEP1", "GuiCTextField"); //DRAW-FILEP1 Путь к файлу
            GuiButton okFileBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //btn[0] КнопкаОК
            typeFile.Text = "PDF";
            primechanieFile.Text = "";
            pathFile.Text = pdfDirectory+akt.PathOfPdfFile;
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
        private void ustanovkaPlomb(aktATP akt)
        {
            if (akt.plomb.Count == 0) return;
            GuiButton VedeniePlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("BT_SEAL", "GuiButton");
            VedeniePlombBtn.Press();
            GuiGridView GridPlomb = ((GuiGridView)SapSession.FindById("/app/con[0]/ses[0]/wnd[0]/usr/cntlALV_SEALS/shellcont/shell"));
            GuiButton addPlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[30]", "GuiButton"); //GuiButton//btn[30] Добавить выданные
            GuiButton ustanovitPlombBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[32]", "GuiButton"); // Установть выделенные

            foreach (plomba Plomba in akt.plomb)
            {
                addPlombBtn.Press();
                /*Окно Добавить выданные*/
                GuiStatusbar statusBar = (GuiStatusbar)SapSession.ActiveWindow.FindByName("sbar", "GuiStatusbar");//sbar
                GuiCTextField typePlombAdd = (GuiCTextField)SapSession.ActiveWindow.FindByName("P_SCAT-LOW", "GuiCTextField"); //P_SCAT-LOW Тип пломбы
                GuiTextField kodPlombAdd = (GuiTextField)SapSession.ActiveWindow.FindByName("P_SCODE-LOW", "GuiTextField");//P_SCODE-LOW
                GuiButton addPlombOk = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton");//btn[8] Выполнить
                typePlombAdd.Text = Plomba.Type; //Тип пломбы
                kodPlombAdd.Text = Plomba.Number; //Номер Пломбы
                addPlombOk.Press();
                if (statusBar.Text.Contains("недавно добавлено"))
                {
                    GridPlomb.CurrentCellColumn = "SCODE";
                    GridPlomb.PressToolbarButton("&MB_FILTER");
                    /**Окно фильтра***************************/
                    GuiCTextField NumberPlombFilter = (GuiCTextField)SapSession.ActiveWindow.FindByName("/%%DYN001-LOW", "GuiCTextField");
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
                        GuiButton FindOk = (GuiButton)SapSession.ActiveWindow.FindByName("btn[8]", "GuiButton");//btn[8] Выполнить//btn[0]
                        FindOk.Press();
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

            GuiButton backBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[3]", "GuiButton"); //назад
            GuiButton SaveAktOKBtn = (GuiButton)SapSession.ActiveWindow.FindByName("btn[0]", "GuiButton"); //Сохранить
            SaveAktOKBtn.Press();
            backBtn.Press();
        }
    }
}
