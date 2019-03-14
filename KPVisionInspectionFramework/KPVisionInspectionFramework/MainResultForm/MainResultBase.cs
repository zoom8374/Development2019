﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

using ParameterManager;
using LogMessageManager;

namespace KPVisionInspectionFramework
{
    public partial class MainResultBase : Form
    {
        private ucMainResultNone        MainResultNoneWnd;
        private ucMainResultSorter      MainResultSorterWnd;
        private ucMainResultTrimForm    MainResultTrimFormWnd;

        private eProjectType ProjectType;
        private string[] LastRecipeName;

        private bool ResizingFlag = false;
        private bool IsResizing = false;
        private Point LastPosition = new Point(0, 0);

        public delegate void ReadLOTNumHandler(string LOTNum);
        public event ReadLOTNumHandler ReadLOTNumEvent;

        #region Initialize & DeInitialize
        public MainResultBase(string[] _LastRecipeName)
        {
            LastRecipeName = new string[_LastRecipeName.Count()];
            for (int iLoopCount = 0; iLoopCount < _LastRecipeName.Count(); iLoopCount++)
            {
                LastRecipeName[iLoopCount] = _LastRecipeName[iLoopCount];
            }
            InitializeComponent();
        }

        public void Initialize(Object _OwnerForm, int _ProjectType)
        {
            this.Owner = (Form)_OwnerForm;
            ProjectType = (eProjectType)_ProjectType;

            if (ProjectType == eProjectType.NONE)
            {
                MainResultNoneWnd = new ucMainResultNone(LastRecipeName);
                MainResultNoneWnd.ScreenshotEvent += new ucMainResultNone.ScreenshotHandler(ScreenShot);
                panelMain.Controls.Add(MainResultNoneWnd);
            }

            else if (ProjectType == eProjectType.TRIM_FORM)
            {
                MainResultTrimFormWnd = new ucMainResultTrimForm(LastRecipeName);
                MainResultTrimFormWnd.ScreenshotEvent += new ucMainResultTrimForm.ScreenshotHandler(ScreenShot);
                panelMain.Controls.Add(MainResultTrimFormWnd);
            }

            else if (ProjectType == eProjectType.SORTER)
            {
                MainResultSorterWnd = new ucMainResultSorter(LastRecipeName);
                panelMain.Controls.Add(MainResultSorterWnd);
            }

            SetWindowLocation(1482, 148);
        }

        public void DeInitialize()
        {
            if (ProjectType == eProjectType.NONE)
            {
                MainResultNoneWnd.ScreenshotEvent -= new ucMainResultNone.ScreenshotHandler(ScreenShot);
            }

            else if (ProjectType == eProjectType.SORTER)
            {

            }

            else if (ProjectType == eProjectType.TRIM_FORM)
            {
                MainResultTrimFormWnd.ScreenshotEvent -= new ucMainResultTrimForm.ScreenshotHandler(ScreenShot);
            }

            panelMain.Controls.Clear();
        }

        public void SetWindowLocation(int _StartX, int _StartY)
        {
            this.Location = new Point(_StartX, _StartY);
        }

        public void SetWindowSize(int _Width, int _Height)
        {
            this.Size = new Size(_Width, _Height);
        }
        #endregion Initialize & DeInitialize

        #region Control Default Event
        private void MainResultBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt && e.KeyCode == Keys.F4) e.Handled = true;
        }

        private void MainResultBase_MouseDown(object sender, MouseEventArgs e)
        {
            this.IsResizing = true;
            this.LastPosition = e.Location;
        }

        private void MainResultBase_MouseUp(object sender, MouseEventArgs e)
        {
            this.IsResizing = false;
        }

        private void MainResultBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (false == ResizingFlag) { this.Cursor = Cursors.Default; return; }

            if (!IsResizing) // handle cursor type
            {
                bool resize_x = e.X > (this.Width - 8);
                bool resize_y = e.Y > (this.Height - 8);

                if (resize_x && resize_y) this.Cursor = Cursors.SizeNWSE;
                else if (resize_x) this.Cursor = Cursors.SizeWE;
                else if (resize_y) this.Cursor = Cursors.SizeNS;
                else this.Cursor = Cursors.Default;
            }
            else // handle resize
            {
                if (e.Button != System.Windows.Forms.MouseButtons.Right) return;

                int w = this.Size.Width;
                int h = this.Size.Height;

                if (this.Cursor.Equals(Cursors.SizeNWSE)) this.Size = new Size(w + (e.Location.X - this.LastPosition.X), h + (e.Location.Y - this.LastPosition.Y));
                else if (this.Cursor.Equals(Cursors.SizeWE)) this.Size = new Size(w + (e.Location.X - this.LastPosition.X), h);
                else if (this.Cursor.Equals(Cursors.SizeNS)) this.Size = new Size(w, h + (e.Location.Y - this.LastPosition.Y));

                this.LastPosition = e.Location;
            }
        }

        private void MainResultBase_Resize(object sender, EventArgs e)
        {
            Size _TitleSize = new Size(this.Size.Width, labelTitle.Size.Height);
            Point _Location = panelMain.Location;

            labelTitle.Size = new Size(_TitleSize.Width - 6, _TitleSize.Height);
            labelTitle.Location = new Point(3, 2);

            panelMain.Size = new Size(_TitleSize.Width - 6, this.Size.Height - _TitleSize.Height - 6);
            panelMain.Location = new Point(3, _Location.Y);

            panelMain.Invalidate();
            labelTitle.Invalidate();
        }

        private void labelTitle_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.labelTitle.ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void labelTitle_DoubleClick(object sender, EventArgs e)
        {
            ResizingFlag = !ResizingFlag;
            if (true == ResizingFlag) labelTitle.ForeColor = Color.Chocolate;
            else labelTitle.ForeColor = Color.WhiteSmoke;
        }

        private void labelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (false == ResizingFlag) { this.Cursor = Cursors.Default; return; }

            var s = sender as Label;
            if (e.Button != System.Windows.Forms.MouseButtons.Right) return;

            s.Parent.Left = this.Left + (e.X - ((Point)s.Tag).X);
            s.Parent.Top = this.Top + (e.Y - ((Point)s.Tag).Y);

            this.Cursor = Cursors.Default;
        }

        private void labelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            var s = sender as Label;
            s.Tag = new Point(e.X, e.Y);
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, this.panelMain.ClientRectangle, Color.White, ButtonBorderStyle.Solid);
        }

        private void panelMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (false == ResizingFlag) { this.Cursor = Cursors.Default; return; }

            this.Cursor = Cursors.Default;
        }
        #endregion Control Default Event

        public void ClearResultData(string _LOTNum = "", string _InDataPath = "", string _OutDataPath = "")
        {
            if (ProjectType == eProjectType.NONE)           MainResultNoneWnd.ClearResult();
            else if (ProjectType == eProjectType.SORTER)    MainResultSorterWnd.ClearResult();
            else if (ProjectType == eProjectType.TRIM_FORM) MainResultTrimFormWnd.ClearResult();
        }

        public void SetResultData(SendResultParameter _ResultParam)
        {
            if (_ResultParam.ProjectItem == eProjectItem.NONE)                  MainResultNoneWnd.SetNoneResultData(_ResultParam);
            else if (_ResultParam.ProjectItem == eProjectItem.SURFACE)          MainResultSorterWnd.SetSurfaceResultData(_ResultParam);
            else if (_ResultParam.ProjectItem == eProjectItem.LEAD_TRIM_INSP)   MainResultTrimFormWnd.SetTrimResultData(_ResultParam);
            else if (_ResultParam.ProjectItem == eProjectItem.LEAD_FORM_ALIGN)  MainResultTrimFormWnd.SetFormResultData(_ResultParam);
        }

        /// <summary>
        /// 프로젝트별 AutoMode 관리
        /// </summary>
        /// <param name="_AutoModeFlag"></param>
        /// <returns></returns>
        public bool SetAutoMode(bool _AutoModeFlag)
        {
            bool _Result = false;

            if (ProjectType == eProjectType.NONE)           MainResultNoneWnd.SetAutoMode(_AutoModeFlag);
            else if (ProjectType == eProjectType.TRIM_FORM) MainResultTrimFormWnd.SetAutoMode(_AutoModeFlag);

            return _Result;
        }

        public void SetLastRecipeName(eProjectType _ProjectType, string[] _LastRecipeName)
        {
            if (_ProjectType == eProjectType.NONE)              MainResultNoneWnd.SetLastRecipeName(_LastRecipeName);
            else if (_ProjectType == eProjectType.TRIM_FORM)    MainResultTrimFormWnd.SetLastRecipeName(_LastRecipeName);
        }
        
        private void ScreenShot(string ImageSaveFile)
        {
            try
            {
                Size Wondbounds = new Size(1280, 1024 - 150);
                Bitmap printScreen = new Bitmap(1280, 1024 - 150);
                Graphics graphics = Graphics.FromImage(printScreen as Image);
                graphics.CopyFromScreen(new Point(8, 150), Point.Empty, Wondbounds);
                printScreen.Save(ImageSaveFile, ImageFormat.Jpeg);
                printScreen.Dispose();
            }
            catch (System.Exception ex)
            {
                CLogManager.AddSystemLog(CLogManager.LOG_TYPE.ERR, "Screenshot Exception : " + ex.ToString(), CLogManager.LOG_LEVEL.LOW);
            }
        }
    }
}
