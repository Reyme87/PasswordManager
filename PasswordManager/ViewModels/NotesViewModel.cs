using PasswordManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PasswordManager.ViewModels.Base;
using System.Collections.ObjectModel;
using PasswordManager.Models;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace PasswordManager.ViewModels
{
    internal class NotesViewModel : ViewModel
    {
        private static NotesViewModel _instance;
        public static NotesViewModel Instance => _instance ??= new NotesViewModel();

        #region Элементы полей

        private string _nameField;
        private string _textField;

        public string NameField
        {
            get => _nameField;
            set
            {
                Set(ref _nameField, value);
            }
        }

        public string TextField
        {
            get => _textField;
            set
            {
                Set(ref _textField, value);
            }
        }

        private bool isChanging = false;

        #endregion

        #region Коллекции элементов

        ObservableCollection<NoteModel> _notes;
        public ObservableCollection<NoteModel> Notes
        {
            get => _notes;
            set
            {
                Set(ref _notes, value);
            }
        }

        private NoteModel _selectedNote;

        public NoteModel SelectedNote
        {
            get => _selectedNote;
            set
            {
                Set(ref _selectedNote, value);
            }
        }

        #endregion

        #region Команды

        #region AddNoteCommand

        public ICommand AddNoteCommand { get; }

        public void OnAddNoteComandExecuted(object p)
        {
            if (!isChanging)
            {
                NoteModel note = new NoteModel(NameField, TextField);
                Notes.Add(note);
                JsonController<NoteModel>.LoadInfoAsync(Notes, "note.json");
                NameField = TextField = "";
                isChanging = false;
            }
            else
            {
                SelectedNote.Name = NameField;
                SelectedNote.Text = TextField;
                JsonController<NoteModel>.LoadInfoAsync(Notes, "note.json");
                NameField = TextField = "";
                isChanging = false;
            }
        }

        public bool CanAddNoteCommandExecute(object p) => !Equals(NameField, null) && !Equals(TextField, null);

        #endregion

        #region CancelCommand

        public ICommand CancelCommand { get; }

        public void OnCancelCommandExecuted(object p)
        {
            NameField = TextField = "";
            isChanging = false;
        }

        public bool CanCancelCommandExecute(object p) => true;

        #endregion

        #region RemoveNoteCommand

        public ICommand RemoveNoteCommand { get; }

        public void OnRemoveNoteCommandExecuted(object p)
        {
            Notes.Remove(SelectedNote);
            JsonController<NoteModel>.LoadInfoAsync(Notes, "note.json");
        }

        public bool CanRemoveNoteCommandExecute(object p) => !Equals(SelectedNote, null);

        #endregion

        #region ChangeNoteCommand

        public ICommand ChangeNoteCommand { get; }

        public void OnChangeNoteCommandExecuted(object p)
        {
            NameField = SelectedNote.Name;
            TextField = SelectedNote.Text;
            isChanging = true;
        }

        public bool CanChangeNoteCommandExecute(object p) => !Equals(SelectedNote, null);

        #endregion

        #region ImportDataCommand

        public ICommand ImportDataCommand { get; }

        public void OnImportDataCommandExecuted(object p)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "Data file (*.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string FileName = dialog.FileName;

                    ObservableCollection<NoteModel> additionalCards = JsonController<NoteModel>.GetInfo(FileName);

                    var temp = Notes.Union(additionalCards);

                    Notes = temp.ToObservableCollection();
                    JsonController<NoteModel>.LoadInfoAsync(Notes, "note.json");
                }
                catch
                {
                    MessageBox.Show("Error occured during data import!");
                }
            }
        }

        public bool CanImportDataCommandExecute(object p) => true;

        #endregion

        #region ExportDataCommand

        public ICommand ExportDataCommand { get; }

        public void OnExportDataCommandExecuted(object p)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            dialog.Filter = "Data file (*.json)|*.json";

            if (dialog.ShowDialog() == true)
            {
                string FileName = dialog.FileName;
                JsonController<NoteModel>.LoadInfoAsync(Notes, FileName);
            }
        }

        public bool CanExportDataCommandExecute(object p) => !Equals(Notes, null) && !Equals(Notes.Count, 0);

        #endregion

        #endregion

        public NotesViewModel()
        {
            #region Команды

            AddNoteCommand = new RelayCommand(OnAddNoteComandExecuted, CanAddNoteCommandExecute);

            CancelCommand = new RelayCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

            RemoveNoteCommand = new RelayCommand(OnRemoveNoteCommandExecuted, CanRemoveNoteCommandExecute);

            ChangeNoteCommand = new RelayCommand(OnChangeNoteCommandExecuted, CanChangeNoteCommandExecute);

            ImportDataCommand = new RelayCommand(OnImportDataCommandExecuted, CanImportDataCommandExecute);

            ExportDataCommand = new RelayCommand(OnExportDataCommandExecuted, CanExportDataCommandExecute);

            #endregion

            Notes = JsonController<NoteModel>.GetInfo("note.json");
        }
    }
}
