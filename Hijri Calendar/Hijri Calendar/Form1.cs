using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Hijri_Calendar
{
    public partial class frmSetting : Form
    {
        bool toSave = true, showMonthName = true;
        bool fajirAlarm, zoharAlarm, aserAlarm, maghribAlarm, eshaAlarm, jummahAlarm, afterAlarm, beforAlarm;
        int HijriDiff, PlaceDiff;
        int AfterTime;
        int BeforTime;
        bool saving = false, Restore = false;
        private static bool ComAzan=false;
        public HijriLib.HijriConversion myConversion = new HijriLib.HijriConversion();
        public HijriLib.PrayerTime myPrayer = new HijriLib.PrayerTime();
        public HijriLib.HijriDateTime myDateTime = new HijriLib.HijriDateTime();
        private System.Collections.Specialized.NameValueCollection NVC = new System.Collections.Specialized.NameValueCollection();

        public frmSetting()
        {
            InitializeComponent();
            CatchPrayer myCatchPrayer = new CatchPrayer(myPrayer);
            CatchHijri myCatchHijri = new CatchHijri(myDateTime);
            ////////////////////////////////         
            LoadData();
            LoadEvents();
            ManipCalendar();
            myTimer.Start();
            myTray.Icon = PicureIcon.trayIcon1;
        }
        public static bool  shortAzan
        { get { return ComAzan; }}
        private void btnNamazSettingSave_Click(object sender, EventArgs e)
        {
            SetNAFReadOnly();
            int fAT = GetMinute(FAH.Text, FAM.Text);
            int zAT = GetMinute(ZAH.Text, ZAM.Text);
            int aAT = GetMinute(AAH.Text, AAM.Text);
            int mAT = GetMinute(MAH.Text, MAM.Text);
            int eAT = GetMinute(EAH.Text, EAM.Text);
            int jAT = GetMinute(JAH.Text, JAM.Text);

            int fNT = GetMinute(FNH.Text, FNM.Text);
            int zNT = GetMinute(ZNH.Text, ZNM.Text);
            int aNT = GetMinute(ANH.Text, ANM.Text);
            int mNT = GetMinute(MNH.Text, MNM.Text);
            int eNT = GetMinute(ENH.Text, ENM.Text);
            int jNT = GetMinute(JNH.Text, JNM.Text);

            myPrayer.SetAzanTimes(fAT, zAT, aAT, mAT, eAT, jAT);
            myPrayer.SetNamazTimes(fNT, zNT, aNT, mNT, eNT, jNT);
            ManipNamazAzanTimes();
            SetAlarmsBools();
        }
        public int GetMinute(string Hour, string Minutes)
        {
            int hour = 0, minutes = 0;
            if (Hour == "") { Hour = "0"; } else { hour = int.Parse(Hour); }
            if (Minutes == "") { Minutes = "0"; } else { minutes = int.Parse(Minutes); }
            return ((hour * 60) + minutes);
        }
        public void LoadData()
        {
            int tA, tB;
            RegistryKey NamazAndCale;
            NamazAndCale = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\NamazAndCale");

            if (NamazAndCale != null)
            {
                FAH.Text = (String)NamazAndCale.GetValue("FAH");
                FAM.Text = (String)NamazAndCale.GetValue("FAM");
                ZAH.Text = (String)NamazAndCale.GetValue("ZAH");
                ZAM.Text = (String)NamazAndCale.GetValue("ZAM");
                AAH.Text = (String)NamazAndCale.GetValue("AAH");
                AAM.Text = (String)NamazAndCale.GetValue("AAM");
                MAH.Text = (String)NamazAndCale.GetValue("MAH");
                MAM.Text = (String)NamazAndCale.GetValue("MAM");
                EAH.Text = (String)NamazAndCale.GetValue("EAH");
                EAM.Text = (String)NamazAndCale.GetValue("EAM");
                JAH.Text = (String)NamazAndCale.GetValue("JAH");
                JAM.Text = (String)NamazAndCale.GetValue("JAM");

                FNH.Text = (String)NamazAndCale.GetValue("FNH");
                FNM.Text = (String)NamazAndCale.GetValue("FNM");
                ZNH.Text = (String)NamazAndCale.GetValue("ZNH");
                ZNM.Text = (String)NamazAndCale.GetValue("ZNM");
                ANH.Text = (String)NamazAndCale.GetValue("ANH");
                ANM.Text = (String)NamazAndCale.GetValue("ANM");
                MNH.Text = (String)NamazAndCale.GetValue("MNH");
                MNM.Text = (String)NamazAndCale.GetValue("MNM");
                ENH.Text = (String)NamazAndCale.GetValue("ENH");
                ENM.Text = (String)NamazAndCale.GetValue("ENM");
                JNH.Text = (String)NamazAndCale.GetValue("JNH");
                JNM.Text = (String)NamazAndCale.GetValue("JNM");

                try
                {
                    tA = (int)NamazAndCale.GetValue("AfterTime");
                    tB = (int)NamazAndCale.GetValue("BeforTime");
                }
                catch
                {
                    tA = 0; tB = 0;
                }
                afterTime.Text = tA.ToString();
                beforTime.Text = tB.ToString();
                PlaceDiff = (int)NamazAndCale.GetValue("PlaceDiff", 8);
                HijriDiff = (int)NamazAndCale.GetValue("HijriDiff", 0);



                citiesCombo.SelectedIndex = (int)NamazAndCale.GetValue("city", 0);

                fajirAlarm = Convert.ToBoolean(NamazAndCale.GetValue("FAlarm", true));
                zoharAlarm = Convert.ToBoolean(NamazAndCale.GetValue("ZAlarm", true));
                aserAlarm = Convert.ToBoolean(NamazAndCale.GetValue("AAlarm", true));
                maghribAlarm = Convert.ToBoolean(NamazAndCale.GetValue("MAlarm", true));
                eshaAlarm = Convert.ToBoolean(NamazAndCale.GetValue("EAlarm", true));
                jummahAlarm = Convert.ToBoolean(NamazAndCale.GetValue("JAlarm", true));
                afterAlarm = Convert.ToBoolean(NamazAndCale.GetValue("AfterAlarm", true));
                beforAlarm = Convert.ToBoolean(NamazAndCale.GetValue("BeforAlarm", true));
                NamazAndCale.Close();
            }
            ////////////////////////////
            yearList.Value = myDateTime.GetYear;
            SetAfterBefor();
            btnNamazSettingSave_Click(this, null);
            LoadCalendarSetting();
            LoadAlarmBools();
            ManipNamazAzanTimes();
            ManipSunriseSunsetTimes();
        }
        private void LoadEvents()
        {
            if (InRegEvent())
            {
                RegistryKey EventKey;
                EventKey = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events");
                string[] dates = EventKey.GetSubKeyNames();
                //////////////////////////////////
                int length = 0;
                for (int i = 0; i < dates.Length; i++)
                {
                    EventKey = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events\\" + dates[i]);
                    string[] subEvents = EventKey.GetValueNames();
                    for (int j = 0; j < subEvents.Length; j++)
                    {
                        length += 1;
                    }
                }
                //////////////////////////////////////////
                string[,] Events = new string[2, length]; ;
                int x = 0;
                for (int i = 0; i < dates.Length; i++)
                {
                    EventKey = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events\\" + dates[i]);
                    string[] subEvents = EventKey.GetValueNames();
                    for (int j = 0; j < subEvents.Length; j++)
                    {
                        Events[0, x] = dates[i];
                        Events[1, x] = subEvents[j];
                        x++;
                    }
                }
                myDateTime.SetEvents(Events);
                SetEvents(myDateTime.GetEvents());
                ManipEventsToCombo();
            }
        }
        private void SaveEvents()
        {
            RegistryKey EReg;
            RegistryKey ESub;
            if (!InRegEvent())
            {
                EReg = Registry.LocalMachine.OpenSubKey("Software", true);
                EReg.CreateSubKey("Hijri\\Events");
                EReg.Close();
            }
            else
            {
                ESub = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events", true);
                for (int i = 0; i < NVC.Count; i++)
                {
                    ESub.CreateSubKey(NVC.Get(i));
                    string Str = "Software\\Hijri\\Events\\" + NVC.Get(i);
                    ESub = Registry.LocalMachine.OpenSubKey(Str, true);
                    ESub.SetValue(NVC.GetKey(i), "");
                    ESub = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events", true);
                }
            }
        }
        private void ClearEvents()
        {
            RegistryKey EventKey;
            EventKey = Registry.LocalMachine.OpenSubKey("Software\\Hijri", true);
            if (EventKey != null)
            {
                EventKey.DeleteSubKeyTree("Events");
                EventKey.CreateSubKey("Events");
            }
        }
        public void SaveData()
        {
            RegistryKey OReg;
            RegistryKey OSub;
            if (!InRegNamaz())
            {
                OReg = Registry.LocalMachine.OpenSubKey("Software", true);
                OReg.CreateSubKey("Hijri\\NamazAndCale");
                OReg.Close();
            }
            else
            {
                OSub = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\NamazAndCale", true);

                OSub.SetValue("FAH", FAH.Text);
                OSub.SetValue("FAM", FAM.Text);
                OSub.SetValue("ZAH", ZAH.Text);
                OSub.SetValue("ZAM", ZAM.Text);
                OSub.SetValue("AAH", AAH.Text);
                OSub.SetValue("AAM", AAM.Text);
                OSub.SetValue("MAH", MAH.Text);
                OSub.SetValue("MAM", MAM.Text);
                OSub.SetValue("EAH", EAH.Text);
                OSub.SetValue("EAM", EAM.Text);
                OSub.SetValue("JAH", JAH.Text);
                OSub.SetValue("JAM", JAM.Text);

                OSub.SetValue("FNH", FNH.Text);
                OSub.SetValue("FNM", FNM.Text);
                OSub.SetValue("ZNH", ZNH.Text);
                OSub.SetValue("ZNM", ZNM.Text);
                OSub.SetValue("ANH", ANH.Text);
                OSub.SetValue("ANM", ANM.Text);
                OSub.SetValue("MNH", MNH.Text);
                OSub.SetValue("MNM", MNM.Text);
                OSub.SetValue("ENH", ENH.Text);
                OSub.SetValue("ENM", ENM.Text);
                OSub.SetValue("JNH", JNH.Text);
                OSub.SetValue("JNM", JNM.Text);

                OSub.SetValue("HijriDiff", HijriDiff);
                OSub.SetValue("PlaceDiff", PlaceDiff);

                OSub.SetValue("FAlarm", fajirAlarm);
                OSub.SetValue("ZAlarm", zoharAlarm);
                OSub.SetValue("AAlarm", aserAlarm);
                OSub.SetValue("MAlarm", maghribAlarm);
                OSub.SetValue("EAlarm", eshaAlarm);
                OSub.SetValue("JAlarm", jummahAlarm);

                OSub.SetValue("AfterAlarm", afterAlarm);
                OSub.SetValue("BeforAlarm", beforAlarm);

                OSub.SetValue("AfterTime", AfterTime);
                OSub.SetValue("BeforTime", BeforTime);

                OSub.SetValue("city", citiesCombo.SelectedIndex);
                OSub.Close();
            }

        }
        public bool InRegNamaz()
        {
            RegistryKey NamazAndCale;
            NamazAndCale = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\NamazAndCale");
            if (NamazAndCale == null)
            {
                return false;
            }
            return true;
        }
        public bool InRegEvent()
        {
            RegistryKey EventKey;
            EventKey = Registry.LocalMachine.OpenSubKey("Software\\Hijri\\Events");
            if (EventKey == null)
            {
                return false;
            }
            return true;
        }
        public void ManipSunriseSunsetTimes()
        {
            Sadiq.Text = myPrayer.Sadiq;
            zoharStart.Text = myPrayer.ZoherStart;
            aserStart.Text = myPrayer.Aser;
            sunSet.Text = myPrayer.Maghrib;
            eshaStart.Text = myPrayer.Esha;
        }
        public void ManipNamazAzanTimes()
        {
            FAT.Text = myPrayer.Azan("Fajir");
            ZAT.Text = myPrayer.Azan("Zohar");
            AAT.Text = myPrayer.Azan("Aser");
            MAT.Text = myPrayer.Azan("Maghrib");
            EAT.Text = myPrayer.Azan("Esha");
            JAT.Text = myPrayer.Azan("Jummah");

            FNT.Text = myPrayer.Namaz("Fajir");
            ZNT.Text = myPrayer.Namaz("Zohar");
            ANT.Text = myPrayer.Namaz("Aser");
            MNT.Text = myPrayer.Namaz("Maghrib");
            ENT.Text = myPrayer.Namaz("Esha");
            JNT.Text = myPrayer.Namaz("Jummah");

        }
        public void LoadCalendarSetting()
        {
            HijriDiffNumaric.Value = HijriDiff;
            myPrayer.DateDifference = myConversion.DateDifference = myDateTime.DateDifference = (int)HijriDiffNumaric.Value;
        }

        private void ManipCalendar()
        {
            int hijriYear = (Int32)yearList.Value; //1428 I've to place the current hijri year here.
            MohDate.Text = myConversion.MonthDate(1, hijriYear, showMonthName);
            SafDate.Text = myConversion.MonthDate(02, hijriYear, showMonthName);
            RabiAwalDate.Text = myConversion.MonthDate(3, hijriYear, showMonthName);
            RabiThaniDate.Text = myConversion.MonthDate(4, hijriYear, showMonthName);
            JamadUlaDate.Text = myConversion.MonthDate(5, hijriYear, showMonthName);
            JamadThaniDate.Text = myConversion.MonthDate(6, hijriYear, showMonthName);
            RajDate.Text = myConversion.MonthDate(7, hijriYear, showMonthName);
            ShabDate.Text = myConversion.MonthDate(8, hijriYear, showMonthName);
            RamDate.Text = myConversion.MonthDate(9, hijriYear, showMonthName);
            ShawDate.Text = myConversion.MonthDate(10, hijriYear, showMonthName);
            QaidaDate.Text = myConversion.MonthDate(11, hijriYear, showMonthName);
            HijjaDate.Text = myConversion.MonthDate(12, hijriYear, showMonthName);
        }
        public void ManipEventsToCombo()
        {
            try
            {
                SetEvents(myDateTime.GetEvents());
                gregEvents.Items.Clear();
                gregEvents.Items.AddRange(NVC.AllKeys);
                gregEvents.Text = "";
                if (NVC.AllKeys.Length > 0)
                {
                    gregEvents.SelectedIndex = gregEvents.Items.Count - 1;
                }
            }
            catch
            {
                MessageBox.Show("An error occured in event data.\n Recovering....", "Event Save", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ManipEventFields()
        {
            if (NVC.Count > 0)
            {
                gregEvent.Text = NVC.GetKey(gregEvents.SelectedIndex);
                String date = NVC.Get(gregEvents.SelectedIndex);
                if (hijriDateCheck.Checked)
                {
                    eventGdate.Text = myConversion.GregToHijri(date, false);
                }
                else
                {
                    eventGdate.Text = date;
                }
            }
        }
        private void LoadAlarmBools()
        {
            FajirCheck.Checked = fajirAlarm;
            ZoharCheck.Checked = zoharAlarm;
            AserCheck.Checked = aserAlarm;
            MaghribCheck.Checked = maghribAlarm;
            EshaCheck.Checked = eshaAlarm;
            JummahCheck.Checked = jummahAlarm;
            checkAfter.Checked = afterAlarm;
            checkBefor.Checked = beforAlarm;
            SetAlarmsBools();
        }
        public void SetAlarmsBools()
        {
            myPrayer.SetAlarms(fajirAlarm, zoharAlarm, aserAlarm, maghribAlarm, eshaAlarm, jummahAlarm, afterAlarm, beforAlarm);
        }
        private void SetAfterBefor()
        {
            AfterTime = int.Parse(afterTime.Text);
            BeforTime = int.Parse(beforTime.Text);
            myPrayer.SetAfterBefor(AfterTime, BeforTime);
        }
        public void SetNAFReadOnly()
        {
            FAH.ReadOnly = true;
            FAH.BackColor = SystemColors.GradientActiveCaption;
            FAM.ReadOnly = true;
            FAM.BackColor = SystemColors.GradientActiveCaption;
            FNH.ReadOnly = true;
            FNH.BackColor = SystemColors.GradientActiveCaption;
            FNM.ReadOnly = true;
            FNM.BackColor = SystemColors.GradientActiveCaption;

            ZAH.ReadOnly = true;
            ZAH.BackColor = SystemColors.GradientActiveCaption;
            ZAM.ReadOnly = true;
            ZAM.BackColor = SystemColors.GradientActiveCaption;
            ZNH.ReadOnly = true;
            ZNH.BackColor = SystemColors.GradientActiveCaption;
            ZNM.ReadOnly = true;
            ZNM.BackColor = SystemColors.GradientActiveCaption;

            AAH.ReadOnly = true;
            AAH.BackColor = SystemColors.GradientActiveCaption;
            AAM.ReadOnly = true;
            AAM.BackColor = SystemColors.GradientActiveCaption;
            ANH.ReadOnly = true;
            ANH.BackColor = SystemColors.GradientActiveCaption;
            ANM.ReadOnly = true;
            ANM.BackColor = SystemColors.GradientActiveCaption;

            MAH.ReadOnly = true;
            MAH.BackColor = SystemColors.GradientActiveCaption;
            MAM.ReadOnly = true;
            MAM.BackColor = SystemColors.GradientActiveCaption;
            MNH.ReadOnly = true;
            MNH.BackColor = SystemColors.GradientActiveCaption;
            MNM.ReadOnly = true;
            MNM.BackColor = SystemColors.GradientActiveCaption;

            EAH.ReadOnly = true;
            EAH.BackColor = SystemColors.GradientActiveCaption;
            EAM.ReadOnly = true;
            EAM.BackColor = SystemColors.GradientActiveCaption;
            ENH.ReadOnly = true;
            ENH.BackColor = SystemColors.GradientActiveCaption;
            ENM.ReadOnly = true;
            ENM.BackColor = SystemColors.GradientActiveCaption;

            JAH.ReadOnly = true;
            JAH.BackColor = SystemColors.GradientActiveCaption;
            JAM.ReadOnly = true;
            JAM.BackColor = SystemColors.GradientActiveCaption;
            JNH.ReadOnly = true;
            JNH.BackColor = SystemColors.GradientActiveCaption;
            JNM.ReadOnly = true;
            JNM.BackColor = SystemColors.GradientActiveCaption;
        }
        private void RideosChackedChanged(object sender, EventArgs e)
        {
            toSave = false;
            SetNAFReadOnly();//use 'select' command below instead of 'if'
            if (sender.Equals(rdoFajir))
            {
                FAH.ReadOnly = false; FAH.BackColor = Color.White;
                FAM.ReadOnly = false; FAM.BackColor = Color.White;
                FNH.ReadOnly = false; FNH.BackColor = Color.White;
                FNM.ReadOnly = false; FNM.BackColor = Color.White;
            }
            if (sender.Equals(rdoZohar))
            {
                ZAH.ReadOnly = false; ZAH.BackColor = Color.White;
                ZAM.ReadOnly = false; ZAM.BackColor = Color.White;
                ZNH.ReadOnly = false; ZNH.BackColor = Color.White;
                ZNM.ReadOnly = false; ZNM.BackColor = Color.White;
            }
            if (sender.Equals(rdoAser))
            {
                AAH.ReadOnly = false; AAH.BackColor = Color.White;
                AAM.ReadOnly = false; AAM.BackColor = Color.White;
                ANH.ReadOnly = false; ANH.BackColor = Color.White;
                ANM.ReadOnly = false; ANM.BackColor = Color.White;
            }
            if (sender.Equals(rdoMaghrib))
            {
                MAH.ReadOnly = false; MAH.BackColor = Color.White;
                MAM.ReadOnly = false; MAM.BackColor = Color.White;
                MNH.ReadOnly = false; MNH.BackColor = Color.White;
                MNM.ReadOnly = false; MNM.BackColor = Color.White;
            }
            if (sender.Equals(rdoEsha))
            {
                EAH.ReadOnly = false; EAH.BackColor = Color.White;
                EAM.ReadOnly = false; EAM.BackColor = Color.White;
                ENH.ReadOnly = false; ENH.BackColor = Color.White;
                ENM.ReadOnly = false; ENM.BackColor = Color.White;
            }
            if (sender.Equals(rdoJummah))
            {
                JAH.ReadOnly = false; JAH.BackColor = Color.White;
                JAM.ReadOnly = false; JAM.BackColor = Color.White;
                JNH.ReadOnly = false; JNH.BackColor = Color.White;
                JNM.ReadOnly = false; JNM.BackColor = Color.White;
            }
        }

        private void CheckBoxes_CheckedChanged(object sender, EventArgs e)
        {
            toSave = false;
            if(sender.Equals(checkComAzan))
            {
                ComAzan=checkComAzan.Checked;
            }
            if (sender.Equals(FajirCheck))
            {
                fajirAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(ZoharCheck))
            {
                zoharAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(AserCheck))
            {
                aserAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(MaghribCheck))
            {
                maghribAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(EshaCheck))
            {
                eshaAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(JummahCheck))
            {
                jummahAlarm = FajirCheck.Checked;
            }
            if (sender.Equals(checkAfter))
            {
                afterAlarm = checkAfter.Checked;
            }
            if (sender.Equals(checkBefor))
            {
                beforAlarm = checkBefor.Checked;
            }
            SetAlarmsBools();
            if (sender.Equals(hijriDateCheck))
            {
                ManipEventFields();
            }
        }
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            dateTime.Text = myDateTime.GetCompleteDateTime;
            myDateTime.CheckEvent();
            myPrayer.CheckAlarm();
            myDateTime.Refresh();
            myTray.Text = myDateTime.GetCompleteDate;
        }
        private void btnEAddSave_Click(object sender, EventArgs e)
        {
            if (saving)
            {
                if (gregEvent.Text == "")
                {
                    if (MessageBox.Show("Are you shure to add an empty Event?", "Empty Event", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        if (hijriDateCheck.Checked)
                        {
                            myDateTime.AddEventHijri(myDateTime.GetLongDateTime, eventGdate.Text);
                            gregEvent.Text = "";
                        }
                        else
                        {
                            myDateTime.AddEvent(DateTime.Now.ToString(), eventGdate.Text);
                            gregEvent.Text = "";
                        }
                    }
                    gregEvent.Enabled = false;
                    eventGdate.Enabled = false;
                    btnEAddSave.Text = "&New";
                    saving = false;
                    btnEDelete.Enabled = true;
                    btnEEdit.Enabled = true;
                    ManipEventsToCombo();
                    hijriDateCheck.Enabled = true;
                }
                else
                {
                    gregEvent.Enabled = false;
                    eventGdate.Enabled = false;
                    btnEAddSave.Text = "&New";
                    saving = false;
                    btnEDelete.Enabled = true;
                    btnEEdit.Enabled = true;
                    if (hijriDateCheck.Checked)
                    {
                        myDateTime.AddEventHijri(gregEvent.Text, eventGdate.Text);
                    }
                    else
                    {
                        myDateTime.AddEvent(gregEvent.Text, eventGdate.Text);
                    }
                    gregEvent.Text = "";
                    ManipEventsToCombo();
                    hijriDateCheck.Enabled = true;
                }
                SaveEvents();

            }
            else
            {
                hijriDateCheck.Enabled = false;
                btnEAddSave.Text = "&Save";
                saving = true;
                gregEvent.Enabled = true;
                eventGdate.Enabled = true;
                btnEDelete.Enabled = false;
                btnEEdit.Enabled = false;
                gregEvent.Text = "";
                if (hijriDateCheck.Checked)
                {
                    eventGdate.Text = myConversion.NextHijriDate(myDateTime.GetLongDate);
                }
                else
                {
                    eventGdate.Text = myConversion.NextGregDate(DateTime.Now.ToShortDateString().ToString());
                }
            }
        }
        private void EventButtonControl(object sender, EventArgs e)
        {

            if (sender.Equals(btnECancel))
            {
                if (saving)
                {
                    btnEAddSave.Text = "&New"; saving = false;
                    gregEvent.Text = "";
                    eventGdate.Text = "";
                    gregEvent.Enabled = false;
                    eventGdate.Enabled = false;
                    btnEDelete.Enabled = true;
                    btnEEdit.Enabled = true;
                }
                eventGdate.Enabled = false;
                btnEEdit.Enabled = true;
            }
            if (sender.Equals(btnEEdit))
            {
                gregEvent.Enabled = true;
                eventGdate.Enabled = true;
                btnEAddSave.Text = "&Save";
                saving = true;
                btnEDelete.Enabled = false;
                btnEEdit.Enabled = false;

                NVC.Remove(gregEvent.Text);
                ClearEvents();
                SaveEvents();
                myDateTime.SetEvents(GetEvents());
                SetEvents(myDateTime.GetEvents());

            }
            if (sender.Equals(btnEDelete))
            {
                if (MessageBox.Show("The Event will be deleted.\nAre you sure?", "Delete Event", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    NVC.Remove(gregEvent.Text);
                    myDateTime.SetEvents(GetEvents());
                    SetEvents(myDateTime.GetEvents());
                    gregEvent.Text = "";
                    eventGdate.Text = "";
                    ManipEventsToCombo();
                    ClearEvents();
                    SaveEvents();
                }
            }
        }
        private void SetEvents(string[,] StrEvent)
        {
            try
            {
                int Loop = (StrEvent.Length) / 2;
                int i = 0;
                while (Loop > 0)
                {
                    NVC.Set(StrEvent[1, i], StrEvent[0, i]);
                    i++;
                    Loop--;
                }

            }
            catch
            {
                MessageBox.Show("Con't set events becouse of an error in given data or 2D array's elements or not in currect format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string[,] GetEvents()
        {
            string[,] StrEvent = new string[2, NVC.Count];
            int Loop = NVC.Count;
            int i = 0;
            while (Loop > 0)
            {
                StrEvent[0, i] = NVC[i];
                Loop--;
                i++;
            }
            Loop = NVC.Count;
            i = 0;
            while (Loop > 0)
            {
                StrEvent[1, i] = NVC.GetKey(i);
                Loop--;
                i++;
            }
            return StrEvent;
        }

        private void GregEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            ManipEventFields();
        }
        private void CitiesCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (citiesCombo.SelectedItem.ToString())
            {
                case "Thamewali":
                    placeDiffNumaric.Value = 8;
                    break;
                case "Rawalpindi":
                    placeDiffNumaric.Value = 6;
                    break;
                case "Meanwali":
                    placeDiffNumaric.Value = 9;
                    break;
                case "Sahiwal":
                    placeDiffNumaric.Value = 5;
                    break;
                case "Lahor":
                    placeDiffNumaric.Value = 0;
                    break;
                case "Karachi":
                    placeDiffNumaric.Value = 30;
                    break;
                case "Serghodha":
                    placeDiffNumaric.Value = 6;
                    break;
                case "D.I Khan":
                    placeDiffNumaric.Value = 15;
                    break;
                case "Pishawer":
                    placeDiffNumaric.Value = 12;
                    break;
                case "Multan":
                    placeDiffNumaric.Value = 2;
                    break;
                case "Quetta":
                    placeDiffNumaric.Value = 28;
                    break;
                case "Haderabad":
                    placeDiffNumaric.Value = 26;
                    break;
            }
            myPrayer.PlaceDiff = (int)placeDiffNumaric.Value;
            ManipSunriseSunsetTimes();
        }
        private void btnCDiffSave_Click(object sender, EventArgs e)
        {
            myPrayer.DateDifference = myConversion.DateDifference = myDateTime.DateDifference = HijriDiff = (int)HijriDiffNumaric.Value;
            ManipCalendar();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!toSave)
            {
                if (MessageBox.Show("Would you like to save data?", "Save", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SaveData();
                }
            }
            Restore = false;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }
        private void btnSettingSave_Click(object sender, EventArgs e)
        {
            toSave = true;
            SaveData();
            SaveEvents();
            MessageBox.Show("Data saved Successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void trayMenuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void trayMenuRestore_Click(object sender, EventArgs e)
        {
            if (Restore)
            {
                this.Hide();
                Restore = false;
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                Restore = true;
            }

        }
        private void myTray_DoubleClick(object sender, EventArgs e)
        {
            trayMenuRestore_Click(this, null);
        }
        private void btnNamazTimeSave_Click(object sender, EventArgs e)
        {
            myPrayer.SetAzanTimes(0, 0, 0, 0, 0, 0);
            myPrayer.SetNamazTimes(0, 0, 0, 0, 0, 0);
            ManipNamazAzanTimes();
            SetAlarmsBools();
            SaveData();
            LoadData();
            MessageBox.Show("Defulte Values loaded...", "Done!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void afterTime_TextChanged(object sender, EventArgs e)
        {
            toSave = false;
            SetAfterBefor();
            SetAlarmsBools();
        }
        private void beforTime_TextChanged(object sender, EventArgs e)
        {
            toSave = false;
            SetAfterBefor();
            SetAlarmsBools();
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (!rdoHijriToGreg.Checked & !rdoGregToHijri.Checked)
            {
                MessageBox.Show("Please Select an option.", "Conversion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            if (rdoGregToHijri.Checked)
            {
                try
                {
                    DateTime.Parse(gregDate.Text);
                    hijriDate.Text = myConversion.GregToHijri(gregDate.Text, true);

                }
                catch
                {
                    MessageBox.Show("Enter greg date in currect formate.\nmm/dd/yyyy", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    gregDate.Text = DateTime.Now.ToShortDateString();
                }
            }
            if (rdoHijriToGreg.Checked)
            {
                try
                {
                    DateTime.Parse(hijriDate.Text);
                    gregDate.Text = myConversion.HijriToGreg(hijriDate.Text, true);
                }
                catch
                {
                    MessageBox.Show("Enter greg date in currect formate.\nmm/dd/yyyy", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hijriDate.Text = myDateTime.GetLongDate;
                }
            }
        }

        private void compareButton_Click(object sender, EventArgs e)
        {
            if (hijriDate.Text != "" & gregDate.Text != "")
            {
                try
                {
                    DateTime.Parse(gregDate.Text);
                    DateTime.Parse(hijriDate.Text);
                    string Equal = "Hijri Date " + hijriDate.Text + " is 'Equal' to Gregorian Date " + gregDate.Text;
                    string Greater = "Hijri Date " + hijriDate.Text + " is 'Greater' then Gregorian Date " + gregDate.Text;
                    string Shorter = "Hijri Date " + hijriDate.Text + " is 'Shorter' then Gregorian Date " + gregDate.Text;
                    int strBool = myConversion.Compare(hijriDate.Text, gregDate.Text);
                    if (strBool == 0)
                        compareLabel.Text = Equal;
                    else
                    {
                        if (strBool == -1)
                            compareLabel.Text = Greater;
                        if (strBool == 1)
                            compareLabel.Text = Shorter;
                    }
                }
                catch
                {
                    MessageBox.Show("Enter Gregorian date in currect formate.\nmm/dd/yyyy", "Conversion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    hijriDate.Text = myDateTime.GetLongDate;
                }

            }
            else
            {
                MessageBox.Show("Enter Gregorian date and Hijri Date in formate.\nmm/dd/yyyy", "Comparing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                hijriDate.Text = myDateTime.GetLongDate;
                gregDate.Text = DateTime.Now.ToShortDateString();
            }
        }

        private void yearList_MouseUp(object sender, MouseEventArgs e)
        {
            ManipCalendar();
        }

        private void MonthNameCheck_CheckedChanged(object sender, EventArgs e)
        {
            showMonthName = !showMonthName;
            ManipCalendar();
        }

        private void monthCalCheck_CheckedChanged(object sender, EventArgs e)
        {

            MessageBox.Show(
                "Sorry, It is only available in registered version.\nContact at either on of these addresses.\nMobile: 03009725086\nE-mail: aahmadb1990@yahoo.com\nAddress: Ahmad Ali s/o M.Abdullah Thamewali Mianwali Punjab Pakistan.", "Sorry",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            monthCalCheck.Checked = false;
        }

        private void stopAzanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sounds mysounds = new sounds();
            mysounds.stopAzan();
        }
    }
    public class sounds
        {
        System.Media.SoundPlayer myPlayer = new System.Media.SoundPlayer();
        public  void startAzan(bool CompleteAzan)
        {
            if (CompleteAzan)
                myPlayer.Stream = PicureIcon.Azan;
            else
                myPlayer.Stream = PicureIcon.AzanShort;
            myPlayer.Play();
        }
        public void startRing()
        {
            myPlayer.Stop();
            myPlayer.Stream = PicureIcon.ringin;
            myPlayer.Play();
        }
        public void stopAzan()
        {
            myPlayer.Stop();
        }
    }
    public class CatchPrayer
    {
        public CatchPrayer(HijriLib.PrayerTime PrayerTime)
        {
            PrayerTime.ErrorFound += new HijriLib.PrayerTime.AnError(showErr);
            PrayerTime.NamazAlarm += new HijriLib.PrayerTime.Alarm(showMessage);
        }
        sounds mysounds = new sounds();
        void showMessage(object sender, HijriLib.PrayerErrorEventArgs EEA)
        {
            mysounds.startAzan(frmSetting.shortAzan);
            MessageBox.Show(EEA.AlarmMessage, "Alarm", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void showErr(object sender, HijriLib.PrayerErrorEventArgs EEA)
        {
            MessageBox.Show(EEA.ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    public class CatchHijri
    {
        public CatchHijri(HijriLib.HijriDateTime DateTime)
        {
            DateTime.SetError += new HijriLib.HijriDateTime.setErr(showErr);
            DateTime.ShowEvent += new HijriLib.HijriDateTime.showEvent(showEvent);
        }
        sounds mysounds = new sounds();
        void showEvent(object sender, HijriLib.HijriErrorEventArgs HEA)
        {
            mysounds.startRing();
            MessageBox.Show(HEA.EventMessage, "Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void showErr(object sender, HijriLib.HijriErrorEventArgs HEA)
        {
            MessageBox.Show(HEA.HijriError, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }
}