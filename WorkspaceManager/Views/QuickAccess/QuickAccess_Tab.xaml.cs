﻿using WorkspaceManagerTool.Models.QuickAccess;
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

    /// <summary>
    /// Interaction logic for QuickAccessTabWindow.xaml
    /// </summary>
    public partial class QuickAccess_Tab : UserControl, INotifyPropertyChanged {

        #region Properties and Constructor method

        private ObservableCollection<Group> groups;
        private ObservableCollection<FolderQuickAccess> quickAccess;
        private ObservableCollection<FolderQuickAccess> auxiliar;
        private Group selectedGroup;
        private FolderQuickAccess selectedQuickAccess;

        public ObservableCollection<Group> GroupItems {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        public ObservableCollection<FolderQuickAccess> QuickAccessItems {
            get => quickAccess;
            set => SetProperty(ref quickAccess, value);
        }
        public ObservableCollection<FolderQuickAccess> AuxiliarItems {
            get => auxiliar;
            set => SetProperty(ref auxiliar, value);
        }
        public Group SelectedGroup {
            get => selectedGroup;
            set {
                SetProperty(ref selectedGroup, value);
            }
        }
        public Group FilterApplied { get; private set; }

        public FolderQuickAccess SelectedQuickAccessItem {
            get => selectedQuickAccess;
            set {
                SetProperty(ref selectedQuickAccess, value);
            }
        }
        public FolderQuickAccess SelectedQAToEdit { get; private set; }

        public bool EditingMode { get; private set; }
        public bool FilterMode { get; private set; }

        private QuickAccessController QuickAccessController { get; set; }

        public QuickAccess_CreationPanel QuickAccessPanel { get; set; }



        public QuickAccess_Tab() {
            DataContext = this;
            InitializeComponent();

            QuickAccessController = new QuickAccessController();
            QuickAccessController.Init();

            QuickAccessItems = QuickAccessController.QAItems;
            GroupItems = QuickAccessController.GroupItems;
            _FiltersListBox.UnselectAll();
        }
        #endregion


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

        private void CreateQuickAccess_Click(object sender, EventArgs e) {
            // new item, so empty values
            QuickAccessPanel = new QuickAccess_CreationPanel(GroupItems);
            OpenCreationPanel();
        }
        private void EditQuickAccess_MenuClick(object sender, RoutedEventArgs e) {
            // set current values to edit
            QuickAccessPanel = new QuickAccess_CreationPanel(SelectedQuickAccessItem, GroupItems);
            OpenCreationPanel();
            EditingMode = true;
        }
        private void RemoveQuickAccess_MenuClick(object sender, RoutedEventArgs e) {
            MessageBoxResult result = MessageBox.Show("¿Desea eliminar el acceso directo de forma permanente?", "Eliminar acceso directo", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) {
                var removedItem = SelectedQuickAccessItem;
                RemoveAndUpdate(removedItem);
            }
        }
        private void CopyToClipboard_MenuClick(object sender, RoutedEventArgs e) {
            Clipboard.SetText(SelectedQuickAccessItem.Path);
        }

        private void OpenQuickAccess_MenuClick(object sender, RoutedEventArgs e) {
            OpenLink();
        }

        private void OpenQuickAccess_DoubleClick(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count == 0 || listBox.SelectedValue == null) {
                return;
            }

            OpenLink();
        }

        private void OpenLink() {
            try {
                QuickAccessController.OpenQuickAccess(SelectedQuickAccessItem);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter_Click(object sender, MouseButtonEventArgs e) {
            if (!this.IsInitialized) {
                return;
            }

            ListBox listBox = sender as ListBox;
            if (listBox.Items.Count <= 1 || listBox.SelectedValue == null) {
                return;
            }

            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Visible;


            FilterApplied = (Group)listBox.SelectedValue;
            ApplyFilter(FilterApplied);
        }



        private void AcceptButton_Click(object sender, EventArgs e) {

            FolderQuickAccess new_qa = QuickAccessPanel.GetQuickAccess();

            var list = GetCurrentList();

            if (list.Contains(new_qa)) {
                MessageBox.Show("El acceso directo ya existe.", "Acceso directo duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // comprueba si el grupo ha cambiado para actualizar el color si el usuario quiere
            if (EditingMode && !SelectedQAToEdit.Group.Equals(new_qa.Group) && list.Select(qa => qa.Group).Any(gr => gr.Name == new_qa.Group.Name)) {
                MessageBoxResult result = MessageBox.Show("¿Desea actualizar el color del grupo \"" + new_qa.Group.Name + "\" en el resto de accesos directos?", "Grupo modificado", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes) {
                    foreach (var it in list) {
                        if (it.Group.Name == new_qa.Group.Name) {
                            it.Group.Color = new_qa.Group.Color;
                        }
                    }
                }
            }
            AddItem(new_qa);
            CloseCreationPanel();
            
        }

        private void CancelButton_Click(object sender, EventArgs e) {
            CloseCreationPanel();
            EditingMode = false;
            SelectedQAToEdit = null;
        }


        private void RemoveFilter_Click(object sender, EventArgs e) {
            QuickAccessItems = AuxiliarItems;
            AuxiliarItems = null;

            _CreationQuickAccess_Button.Visibility = Visibility.Visible;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            _FiltersListBox.UnselectAll();

            FilterMode = false;
        }

        #endregion

        #region GUI methods
        private void OpenCreationPanel() {
            _CreationQuickAcess_Container.Children.Add(QuickAccessPanel);
            _CreationPanel_Buttons.Visibility = Visibility.Visible;
            _CreationQuickAccess_Button.Visibility = Visibility.Collapsed;
            _RemoveFilter_Button.Visibility = Visibility.Collapsed;
            // Disable list interactions
            _FiltersListBox.IsHitTestVisible = false;
            _QuickAcessListBox.IsHitTestVisible = false;

        }

        private void CloseCreationPanel() {
            _CreationQuickAcess_Container.Children.RemoveAt(_CreationQuickAcess_Container.Children.Count - 1);
            _CreationPanel_Buttons.Visibility = Visibility.Collapsed;
            // Enable list interactions
            _FiltersListBox.IsHitTestVisible = true;
            _QuickAcessListBox.IsHitTestVisible = true;

        }



        #endregion


        #region Auxiliar methods

        private void RemoveGroupIfNotExists(Group item) {
            if (!QuickAccessItems.Any(qa => qa.Group.Equals(item))) {
                GroupItems.Remove(item);
            }
        }

        private void AddItem(FolderQuickAccess newItem) {

            if (EditingMode) {
                var list = GetCurrentList().Remove(SelectedQAToEdit);
                //AuxiliarItems.Select(qa => qa)
            }

            QuickAccessItems.Add(newItem);

            if (FilterMode) {
                AuxiliarItems.Add(newItem);
                UpdateGroupList();
                ApplyFilter(FilterApplied);
            } else {
                UpdateGroupList();
                QuickAccessItems = new ObservableCollection<FolderQuickAccess>(GetCurrentList());
            }

            //UpdateGroupList();
            SaveChanges();
        }

        private void RemoveAndUpdate(FolderQuickAccess item) {
            Remove(item);
            UpdateGroupList();
            SaveChanges();
        }

        private void Remove(FolderQuickAccess item) {
            if (FilterMode) {
                AuxiliarItems.Remove(item);
            }
            QuickAccessItems.Remove(item);
        }

        private void UpdateGroupList() {
            var qa_list = GetCurrentList();

            if (SelectedGroup == null || !SelectedGroup.Equals(FilterApplied))
                GroupItems = new ObservableCollection<Group>(qa_list.Select(qa => qa.Group).Distinct().OrderBy(gr => gr.Name));
        }

        private void ApplyFilter(Group filter) {
            if (!FilterMode) {
                AuxiliarItems = QuickAccessItems;
                FilterMode = true;
            }
            _FiltersListBox.SelectedItem = FilterApplied;
            QuickAccessItems = new ObservableCollection<FolderQuickAccess>(AuxiliarItems.Where(qa => qa.Group.Equals(filter)));
        }


        private void SaveChanges() {
            var list = GetCurrentList();
            QuickAccessController.SaveChanges(list);
        }



        private ObservableCollection<FolderQuickAccess> GetCurrentList() {
            return FilterMode ? AuxiliarItems : QuickAccessItems;
        }

        #endregion
    }
}
