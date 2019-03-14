﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ParameterManager;

namespace KPVisionInspectionFramework
{
    public partial class RecipeNewNameWindow : Form
    {
        public delegate void RecipeCopyHandler(string _NewRecipeName);
        public event RecipeCopyHandler RecipeCopyEvent;

        private string[] RecipeList;

        public string NewRecipeName;

        public RecipeNewNameWindow(eProjectType _ProjectType)
        {
            InitializeComponent();
        }

        public void SetCurrentRecipe(string _CurrentRecipe, string[] _RecipeList)
        {
            textBoxCurrentRecipe.Text = _CurrentRecipe;

            RecipeList = new string[_RecipeList.Count()];
            RecipeList = _RecipeList;
        }

        private void btnRecipeConfirm_Click(object sender, EventArgs e)
        {
            if (textBoxNewRecipe.Text == null || textBoxNewRecipe.Text == "") { MessageBox.Show("Enter a name for the new recipe."); return; } 

            string RecipeName = textBoxNewRecipe.Text + "_" + textBoxNewRecipeSub.Text;

            for (int iLoopCount = 0; iLoopCount < RecipeList.Count(); iLoopCount++)
            {
                if (RecipeName == RecipeList[iLoopCount]) { MessageBox.Show("The recipe name is already in use."); return; }
            }

            var _RecipeCopyEvent = RecipeCopyEvent;
            _RecipeCopyEvent?.Invoke(RecipeName);

            NewRecipeName = RecipeName;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btnRecipeCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void labelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            var s = sender as Label;
            if (e.Button != System.Windows.Forms.MouseButtons.Left) return;

            s.Parent.Left = this.Left + (e.X - ((Point)s.Tag).X);
            s.Parent.Top = this.Top + (e.Y - ((Point)s.Tag).Y);

            this.Cursor = Cursors.Default;
        }

        private void labelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            var s = sender as Label;
            s.Tag = new Point(e.X, e.Y);
        }

        private void textBoxNewRecipe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar)) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                e.Handled = true;
            }
        }
    }
}
