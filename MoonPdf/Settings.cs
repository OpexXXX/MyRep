﻿using MyApp.Model;
using System.Configuration;

namespace ATPWork.Properties
{


    // Этот класс позволяет обрабатывать определенные события в классе параметров:
    //  Событие SettingChanging возникает перед изменением значения параметра.
    //  Событие PropertyChanged возникает после изменения значения параметра.
    //  Событие SettingsLoaded возникает после загрузки значений параметров.
    //  Событие SettingsSaving возникает перед сохранением значений параметров.
    public sealed partial class Settings
    {

        public Settings()
        {
            // // Для добавления обработчиков событий для сохранения и изменения параметров раскомментируйте приведенные ниже строки:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            this.SettingsSaving += this.SettingsSavingEventHandler;
            this.SettingsLoaded += SettingsLoadEventHandler;
            //
        }

        private void SettingsLoadEventHandler(object sender, SettingsLoadedEventArgs e)
        {
            MainAtpModel.LoadSettings(this);
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            // Добавьте здесь код для обработки события SettingChangingEvent.
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {

            MainAtpModel.LoadSettings(this);
        }
    }
}
