/*! MoonPdf - A WPF-based PDF Viewer application that uses the MoonPdfLib library
Copyright (C) 2013  (see AUTHORS file)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
!*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace MyApp
{

	public partial class App : Application
	{
        private static ATPWork.Properties.Settings _globalSett = new ATPWork.Properties.Settings();
        public static ATPWork.Properties.Settings GlobalSett
        {
            get { return _globalSett; }
            set { _globalSett = value; }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            WalkDictionary(this.Resources);

            base.OnStartup(e);
        }


        private static void WalkDictionary(ResourceDictionary resources)
        {
            foreach (DictionaryEntry entry in resources)
            {
            }

            foreach (ResourceDictionary rd in resources.MergedDictionaries)
                WalkDictionary(rd);
        }
    }
}
