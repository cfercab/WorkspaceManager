﻿using WorkspaceManagerTool.Models.QuickAccess;
using Ookii.Dialogs.WinForms;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System;
using WorkspaceManagerTool.Controllers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Ookii.Dialogs.WinForms;
using FolderQuickAccess = WorkspaceManagerTool.Models.QuickAccess.QuickAccess;

namespace WorkspaceManagerTool.Views.QuickAccess {


    public partial class QuickAccess_CreationPanel : UserControl, INotifyPropertyChanged {

        private string name;
        private string path;
        private string description;
        private string group;
        private Color color;
        private Group selectedGroup;
        private ObservableCollection<Group> groups;


        public string NameText {
            get => name;
            set => SetProperty(ref name, value);
        }
        public string PathText {
            get => path;
            set => SetProperty(ref path, value);
        }
        public string DescriptionText {
            get => description;
            set => SetProperty(ref description, value);
        }
        public string GroupText {
            get => group;
            set => SetProperty(ref group, value);
        }
        public Color ColorBrush {
            get => color;
            set => SetProperty(ref color, value);
        }
        public Group SelectedGroupOption {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }
        public ObservableCollection<Group> ComboBoxGroupOptions {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        public static string DefaultName => "Nuevo acceso directo";
        public static string DefaultPath => "Ruta sin definir";
        public static string DefaultDescription => "";
        public static string DefaultGroup => "Nuevo";
        public static Color DefaultColor => Color.FromRgb(17, 166, 143);

        public QuickAccess_CreationPanel(ObservableCollection<Group> groups) {
            DataContext = this;
            InitializeComponent();

            PathText = DefaultPath;
            NameText = DefaultName;
            DescriptionText = DefaultDescription;
            GroupText = DefaultGroup;
            ColorBrush = DefaultColor;
            ComboBoxGroupOptions = new ObservableCollection<Group>(groups);
        }

        public QuickAccess_CreationPanel(FolderQuickAccess qa, ObservableCollection<Group> groups) {
            DataContext = this;
            InitializeComponent();

            if (qa == null) {
                return;
            }

            PathText = qa.Path;
            NameText = qa.Name;
            DescriptionText = qa.Description;
            GroupText = qa.Group.Name;
            ColorBrush = Color.FromRgb(qa.Group.Color.Color.R, qa.Group.Color.Color.G, qa.Group.Color.Color.B);

            ComboBoxGroupOptions = groups;
            SelectedGroupOption = qa.Group;
        }

        #region Events handlers

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null) {
            if (!EqualityComparer<T>.Default.Equals(field, value)) {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
        private void OnPropertyChanged([CallerMemberName]string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        /// <summary>
        /// Select all text when textbox gets focus
        /// </summary>
        private void OnPalabraGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (sender is TextBox textBox) {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Add focus when the user clicks on the textbox
        /// </summary>
        private void OnPalabraPreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (sender is TextBox textBox) {
                if (!textBox.IsKeyboardFocusWithin) {
                    e.Handled = true;
                    textBox.Focus();
                }
            }
        }


        private void ComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectedGroupOption = (sender as ComboBox).SelectedItem as Group;
        }


        private void Browse_Click(object sender, EventArgs e) {
            VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog();
            fbd.Description = "Seleccionar carpeta";
            fbd.UseDescriptionForTitle = true;
            fbd.ShowNewFolderButton = true;
            var result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
                PathText = fbd.SelectedPath;
            }

        }

        private void NewGroup_Click(object sender, EventArgs e) {

            Group_CreationDialog dialog = new Group_CreationDialog();

            if (dialog.ShowDialog() == true) {
                SelectedGroupOption = new Group(dialog.NameText, dialog.Color);
                ComboBoxGroupOptions.Add(SelectedGroupOption);
            } 

        }

        public FolderQuickAccess GetQuickAccess() {
            // Set default values if empty
            if (string.IsNullOrWhiteSpace(NameText)) {
                NameText = DefaultName;
            }
            if (string.IsNullOrWhiteSpace(PathText)) {
                PathText = DefaultPath;
            }
            if (string.IsNullOrWhiteSpace(GroupText)) {
                GroupText = DefaultGroup;
                ColorBrush = DefaultColor;
            }
            if (string.IsNullOrWhiteSpace(DescriptionText)) {
                DescriptionText = DefaultDescription;
            }
            Group group = new Group(GroupText, new SolidColorBrush(ColorBrush));
            return new FolderQuickAccess(PathText, NameText, DescriptionText, group);
        }

    }
}
