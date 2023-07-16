using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopUI
{
    public partial class ListVM : ObservableObject
    {
        public Action CloseAction { get; internal set; }

        
        [ObservableProperty]
        public static ObservableCollection<Student> students;

        [ObservableProperty]
        public Student selectedPerson = null;

        public ListVM()
        {
            students = new ObservableCollection<Student>(GetStudents());
        }

        public static List<Student> GetStudents()
        {
            using (var db = new StudentContext())
            {
                var studentList = db.Students.OrderBy(s => s.RegNo).ToList();

                foreach (var student in studentList)
                {
                    if (student.ImageBytes != null && student.ImageBytes.Length > 0)
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        using (MemoryStream memoryStream = new MemoryStream(student.ImageBytes))
                        {
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = memoryStream;
                            bitmapImage.EndInit();
                        }

                        student.Image = bitmapImage;
                    }
                }

                return studentList;
            }
        }



        public void RefreshStudents()
        {
            students.Clear();
            foreach (var student in GetStudents())
            {
                students.Add(student);
            }
        }


        [RelayCommand]
        public void messageWindow()
        {

            MessageBox.Show($"{selectedPerson.FullName} GPA value must be in between 0 and 4.", "Error");
        }


        [RelayCommand]
        public void AddStudent()
        {
            var vm = new AddStudentVM();
            vm.title = "ADD NEW STUDENT";
            AddStudentWindow window = new AddStudentWindow(vm);

            var listWindows = Application.Current.Windows.OfType<ListWindow>().Where(w => w.IsActive);
            foreach (var listWindow in listWindows)
            {
                listWindow.Hide();
            }

            bool? dialogResult = window.ShowDialog();

            if (dialogResult == true && vm.Person.RegNo != null)
            {
                students.Add(vm.Person);
            }
            window.Closed += (s, e) =>
            {
                Application.Current.MainWindow.ShowDialog();
            };

        }


        [RelayCommand]
        public void EditStudent()
        {
            if (selectedPerson != null)
            {

                BitmapImage displayImage = selectedPerson.Image;

                if (displayImage == null)
                {
                    using (var db = new StudentContext())
                    {

                        if (selectedPerson.ImageBytes != null && selectedPerson.ImageBytes.Length > 0)
                        {
                            displayImage = new BitmapImage();
                            using (MemoryStream memoryStream = new MemoryStream(selectedPerson.ImageBytes))
                            {
                                displayImage.BeginInit();
                                displayImage.CacheOption = BitmapCacheOption.OnLoad;
                                displayImage.StreamSource = memoryStream;
                                displayImage.EndInit();
                            }

                            selectedPerson.Image = displayImage;
                        }
                        
                    }
                }
                var vm = new AddStudentVM(selectedPerson, displayImage);
                vm.title = "EDIT STUDENT";


                var window = new AddStudentWindow(vm);

                var listWindows = Application.Current.Windows.OfType<ListWindow>().Where(w => w.IsActive);
                foreach (var listWindow in listWindows)
                {
                    listWindow.Hide();
                }

                window.ShowDialog();

                if (vm.Person != null && students.Contains(selectedPerson))
                {
                    int index = students.IndexOf(selectedPerson);
                    students.RemoveAt(index);
                    students.Insert(index, vm.Person);
                }
            }
            else
            {
                MessageBox.Show("Please Select the student", "Error");
            }
        }

        [RelayCommand]
        public void Delete()
        {
            if (selectedPerson != null)
            {

                using (var db = new StudentContext())
                {
                    var studentToDelete = db.Students.Find(selectedPerson.RegNo);
                    if (studentToDelete != null)
                    {
                        db.Students.Remove(studentToDelete);
                        db.SaveChanges();
                    }
                    else
                    {
                        MessageBox.Show($"Cannot find student with ID {selectedPerson.RegNo}.", "Error");
                    }

                    students.Remove(selectedPerson);

                    MessageBox.Show("Student is deleted successfully.", "DELETED");

                }
            }
            else
            {
                MessageBox.Show("Please Select the Student before Deleting.", "Error");
            }
        }


        [RelayCommand]

        public void Back()
        {
            var mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            var listWindows = Application.Current.Windows.OfType<ListWindow>().Where(w => w.IsActive);
            foreach (var listWindow in listWindows)
            {
                listWindow.Hide();
            }

            if (mainWindow != null)
            {
                mainWindow.Show();
            }
            else
            {

                mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
        }
    }
}
