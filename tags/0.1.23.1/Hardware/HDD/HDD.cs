﻿/*
  
  Version: MPL 1.1/GPL 2.0/LGPL 2.1

  The contents of this file are subject to the Mozilla Public License Version
  1.1 (the "License"); you may not use this file except in compliance with
  the License. You may obtain a copy of the License at
 
  http://www.mozilla.org/MPL/

  Software distributed under the License is distributed on an "AS IS" basis,
  WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
  for the specific language governing rights and limitations under the License.

  The Original Code is the Open Hardware Monitor code.

  The Initial Developer of the Original Code is 
  Michael Möller <m.moeller@gmx.ch>.
  Portions created by the Initial Developer are Copyright (C) 2009-2010
  the Initial Developer. All Rights Reserved.

  Contributor(s):

  Alternatively, the contents of this file may be used under the terms of
  either the GNU General Public License Version 2 or later (the "GPL"), or
  the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
  in which case the provisions of the GPL or the LGPL are applicable instead
  of those above. If you wish to allow use of your version of this file only
  under the terms of either the GPL or the LGPL, and not to allow others to
  use your version of this file under the terms of the MPL, indicate your
  decision by deleting the provisions above and replace them with the notice
  and other provisions required by the GPL or the LGPL. If you do not delete
  the provisions above, a recipient may use your version of this file under
  the terms of any one of the MPL, the GPL or the LGPL.
 
*/

using System;
using System.Collections.Generic;
using System.Drawing;

namespace OpenHardwareMonitor.Hardware.HDD {
  public class HDD : IHardware {

    private const int UPDATE_DIVIDER = 30; // update only every 30s

    private string name;
    private IntPtr handle;
    private int drive;
    private int attribute;    
    private Image icon;
    private Sensor temperature;
    private int count;
    

    public HDD(string name, IntPtr handle, int drive, int attribute) {
      this.name = name;
      this.handle = handle;
      this.drive = drive;
      this.attribute = attribute;
      this.count = 0;
      this.icon = Utilities.EmbeddedResources.GetImage("hdd.png");
      this.temperature = new Sensor("HDD", 0, SensorType.Temperature, this);
      Update();
    }


    public string Name {
      get { return name; }
    }

    public string Identifier {
      get { return "/hdd/" + drive; }
    }

    public Image Icon {
      get { return icon; }
    }

    public ISensor[] Sensors {
      get {
        return new ISensor[] { temperature };
      }
    }

    public string GetReport() {
      return null;
    }

    public void Update() {
      if (count == 0) {
        SMART.DriveAttribute[] attributes = SMART.ReadSmart(handle, drive);
        temperature.Value = attributes[attribute].RawValue[0];
      } else {
        temperature.Value = temperature.Value;
      }

      count++; count %= UPDATE_DIVIDER; 
    }

    public void Close() {
      SMART.CloseHandle(handle);
    }

    #pragma warning disable 67
    public event SensorEventHandler SensorAdded;
    public event SensorEventHandler SensorRemoved;
    #pragma warning restore 67
  }
}
