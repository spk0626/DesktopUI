using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DesktopUI
{
    public partial class AddStudentVM : ObservableObject
    {
        [ObservableProperty]
        public string regNo;

        [ObservableProperty]
        public string fullName;

        [ObservableProperty]
        public string department;

        [ObservableProperty]
        public double gpa;

        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public BitmapImage selectedImage;

        private bool isSaved;

        private bool isImageUploaded;

        public BitmapImage DisplayedImage { get; set; }

        public Student Person { get; private set; }
        public Action CloseAction { get; internal set; }

        public AddStudentVM(Student s, BitmapImage displayImage)
        {
            Person = s;
            regNo = Person.RegNo;
            fullName = Person.FullName;
            gpa = Person.GPA;
            department = Person.Department;
            displayImage = Person.Image; 

            if (!isImageUploaded)
            {
                DisplayedImage = Person.Image;
            }
            else
            {
                DisplayedImage = selectedImage;
            }
        }


        public AddStudentVM()
        {

        }

        public BitmapImage ConvertBytesToImage(byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = BitmapCreateOptions.None;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
                return bitmapImage;
            }

            return null;
        }

        public byte[] ConvertImageToBytes(BitmapImage image)
        {
            if (image != null)
            {
                BitmapEncoder encoder = GetEncoderByFormat(GetImageFormat(image));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            return null;
        }


        private string GetImageFormat(BitmapImage image)
        {
            if (image != null && image.UriSource != null)
            {
                string? extension = Path.GetExtension(image.UriSource.LocalPath)?.ToLower();
                if (extension != null)
                {
                    switch (extension)
                    {
                        case ".png":
                            return "png";
                        case ".jpg":
                        case ".jpeg":
                            return "jpg";
                        case ".bmp":
                            return "bmp";
                        default:
                            MessageBox.Show("Unsupported image format: " + extension, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            throw new ArgumentException("Unsupported image format.");
                    }
                }
            }
            MessageBox.Show("Invalid Image Source");
            return string.Empty;
            throw new ArgumentException("Invalid image source.");
        }

        private BitmapEncoder GetEncoderByFormat(string format)
        {
            switch (format.ToLower())
            {
                case "png":
                    return new PngBitmapEncoder();
                case "jpg":
                    return new JpegBitmapEncoder();
                case "bmp":
                    return new BmpBitmapEncoder();
                default:
                    MessageBox.Show("Unsupported image format: " + format, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    throw new ArgumentException("Unsupported image format.");
            }
        }


        [RelayCommand]
        public void UploadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files | *.bmp; *.png; *.jpg";
            dialog.FilterIndex = 1;
            if (dialog.ShowDialog() == true)
            {
                selectedImage = new BitmapImage(new Uri(dialog.FileName));
                isImageUploaded = true;
                MessageBox.Show("Image successfully uploaded!", "successful");
            }
        }

        public void AddStudents(Student nStudent)
        {
            using (var db = new StudentContext())
            {
                db.Students.Add(nStudent);
                db.SaveChanges();
            }
        }

        public bool IsSaved
        {
            get { return isSaved; }
            set
            {
                isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
            }
        }

        [RelayCommand]
        public void SaveDetails()
        {

            if (gpa < 0 || gpa > 4)
            {
                MessageBox.Show("GPA value must be in between 0 and 4.", "Error");
                return;
            }

            if (string.IsNullOrEmpty(regNo) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(department))
            {
                MessageBox.Show("Please fill in all the fields", "Error");
            }
            else
            {

                if (Person == null)
                {

                    Person = new Student()
                    {
                        RegNo = regNo,
                        FullName = fullName,
                        Department = department,
                        Image = selectedImage,
                        GPA = gpa

                    };

                    if (selectedImage != null)
                    {
                        byte[] imageBytes = ConvertImageToBytes(selectedImage);
                        Person.ImageBytes = imageBytes;
                    }

                    AddStudents(Person);
                    MessageBox.Show("Student is Successfully Added", "Message");

                    regNo = "";
                    fullName = "";
                    department = "";
                    gpa = 0;

                    IsSaved = true;
                }
                else
                {
                    Person.RegNo = regNo;
                    Person.FullName = fullName;
                    Person.Department = department;
                    Person.Image = selectedImage;
                    Person.GPA = gpa;


                    using (var db = new StudentContext())
                    {
                        var recordToUpdate = db.Students.FirstOrDefault(r => r.RegNo == Person.RegNo);
                        if (recordToUpdate != null)
                        {
                            if (selectedImage != null)
                            {
                                byte[] imageBytes = ConvertImageToBytes(selectedImage);
                                recordToUpdate.ImageBytes = imageBytes;
                            }

                            recordToUpdate.RegNo = Person.RegNo;
                            recordToUpdate.FullName = Person.FullName;
                            recordToUpdate.Department = Person.Department;
                            recordToUpdate.GPA = Person.GPA;
                            db.SaveChanges();

                        }

                    }

                    var listWindow = Application.Current.Windows.OfType<ListWindow>().FirstOrDefault();

                    if (listWindow != null)
                    {
                        var listVM = listWindow.DataContext as ListVM;
                        listVM.RefreshStudents();
                    }

                    MessageBox.Show("Student is successfully updated", "Message");

                    IsSaved = true;

                }
            }

            
        }

        [RelayCommand]

        public void Back()
        {
            var listWindow = Application.Current.Windows.OfType<ListWindow>().FirstOrDefault();

            var addStudentWindows = Application.Current.Windows.OfType<AddStudentWindow>().Where(w => w.IsActive);
            foreach (var addStudentWindow in addStudentWindows)
            {
                addStudentWindow.Hide();
            }

            if (listWindow != null)
            {
                listWindow.Show();
                var listVM = listWindow.DataContext as ListVM;
                listVM.RefreshStudents();
            }
            else
            {

                listWindow = new ListWindow();
                listWindow.ShowDialog();
            }
        }


    }
}
