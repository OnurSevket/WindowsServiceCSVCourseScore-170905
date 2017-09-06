using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;
using System.Linq.Expressions;
using System.Collections.Specialized;

namespace NoteScoreWinService
{
    public partial class LutegProjectService : ServiceBase
    {
        //SqlHelper Nesnesinde bir kere örnek alıp bütün methodlarda kullanmamızı sağlar.
        SqlCommand _command;
        public LutegProjectService()
        {
            _command = SqlHelper.createSqlCommand();
            InitializeComponent();
        }

        //Desktop veya istediğiniz yolu yourdesktop field'a eklemeniz gerekmektedir.
        private static string yourDesktop = @"C:\Users\Onur\Desktop";

        private string path = yourDesktop + @"\DragCSVFile";
  
        //Windows Service çalıştırıldığı zaman çalışacak olan methodumuzdur.
        protected override void OnStart(string[] args)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(path + @"\errors.txt"))
                {
                    using (StreamWriter streamWriter = File.CreateText(path + @"\errors.txt"))
                    {
                        streamWriter.WriteLine("****(Windows Service Çalışıyor!!!)*******HATA SAYFASI!!!***************************");
                        streamWriter.WriteLine("---------------------------------------------------------------------------------------");
                        streamWriter.Close();
                    }
                }
                var taskTest = Task.Run(() =>
                {
                    while (true)
                    {
                        FileCreated();
                        Thread.Sleep(5000);
                    }
                });
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("CSV OnStart Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
        }

        //Dosyamız Oluştuğu an bütün işlemleri yapan ana Method.
        public void FileCreated()
        {
            try
            {
                foreach (var file in Directory.GetFiles(path))
                {
                    if (Path.GetExtension(file) != ".csv") continue;

                    DataTable dataTable = CsvFileDataTable(file, ",");
                    List<StudentCourse> stuCourseList = new List<StudentCourse>();

                    foreach (DataRow item in dataTable.Rows)
                    {
                        StudentCourse stCourse = new StudentCourse();
                        stCourse.Course = item[0].ToString();
                        stCourse.StudentNumber = item[1].ToString();
                        stCourse.Midterm1 = item[2].ToString();
                        stCourse.Midterm2 = item[3].ToString();
                        stCourse.Midterm3 = item[4].ToString();
                        stCourse.Final = item[5].ToString();
                        stuCourseList.Add(stCourse);
                    }
                    
                    CompareWithMultipleScores(stuCourseList);

                    System.IO.File.Delete(file);
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(@"\errors.txt", true))
                {
                    writer.WriteLine("CSV FileWatch Hata Mesajı {0}", ex.Message);
                    writer.Close();

                }
            }
        }

        //Veri Tabanı ve CSV'den gelen verileri karşılaştıran ve çoklananlardan yeni 
        //gelen datayı güncelleyip diğerlerini DB'ye ekleyen method.
        private void CompareWithMultipleScores(List<StudentCourse> csvScoreList)
        {
           
            try
            {
                List<StudentCourse> allStudentsScores = GetAllStudentCourseData();

                //Burada yeni gelen CSV dosyamız ve Database'den gelen Datalar karşılaştırılarak 
                //Dersismi ve OkulNumarası aynı ise yeni gelen data yı Update yapmasını sağladım.
                //Update yaptıktan sonra Csv Listesinden Sildim.
                foreach (StudentCourse csvScoreItem in csvScoreList.ToList())
                {
                    foreach (StudentCourse dataScoreItem in allStudentsScores)
                    {
                        if (csvScoreItem.Course == dataScoreItem.Course && csvScoreItem.StudentNumber == dataScoreItem.StudentNumber)
                        {
                            StudentCourse getStudentScore = GetOneStudentCourseData((int)dataScoreItem.ID);
                            getStudentScore.ID = dataScoreItem.ID;
                            getStudentScore.Course = csvScoreItem.Course;
                            getStudentScore.StudentNumber = csvScoreItem.StudentNumber;
                            getStudentScore.Midterm1 = csvScoreItem.Midterm1;
                            getStudentScore.Midterm2 = csvScoreItem.Midterm2;
                            getStudentScore.Midterm3 = csvScoreItem.Midterm3;
                            getStudentScore.Final = csvScoreItem.Final;
                            UpdateStudentCourseData(getStudentScore);
                            csvScoreList.Remove(csvScoreItem);
                        }                     
                    }
                }
                //Update Yapılanlar silindikten sonra temiz olan veriyi teker teker DB'ye ekledim.
                foreach (var singleCsvItem in csvScoreList.ToList())
                {                  
                        InsertStudentCourseData(singleCsvItem);
                    
                }

            }
            catch (Exception ex)
            {

                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("CompareWith Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
        }

        //(GETALLDATA)Veri Tabanından bütün Nesnelerimizi getirmemizi sağlayan method
        public List<StudentCourse> GetAllStudentCourseData()
        {
            List<StudentCourse> studentList = new List<StudentCourse>();
            _command.CommandText = "sp_GetAllStudentScore";
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Clear();
            try
            {
                if (_command.Connection.State == System.Data.ConnectionState.Closed)
                {
                    _command.Connection.Open();
                }
                SqlDataReader reader = _command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        StudentCourse studentCou = new StudentCourse();

                        studentCou.ID = (int)reader[0];
                        studentCou.Course = Convert.ToString(reader[1]);
                        studentCou.StudentNumber = Convert.ToString(reader[2]);
                        studentCou.Midterm1 = Convert.ToString(reader[3]);
                        studentCou.Midterm2 = Convert.ToString(reader[4]);
                        studentCou.Midterm3 = Convert.ToString(reader[5]);
                        studentCou.Final = Convert.ToString(reader[6]);

                        studentList.Add(studentCou);
                    }
                }
                else
                {
                    return studentList;
                }
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("DB Ado GetAll Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
            finally
            {
                _command.Connection.Close();
            }
            return studentList;
        }

        //(GETONEDATA)Veri Tabanından id'ye göre bir tane Nesne getirmemizi sağlayan method
        public StudentCourse GetOneStudentCourseData(int id)
        {
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = "sp_GetStudentScore";
            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@id", id);
            StudentCourse courseScore = new StudentCourse();
            try
            {
               
                if (_command.Connection.State == ConnectionState.Closed)
                    _command.Connection.Open();
                SqlDataReader reader = _command.ExecuteReader();
                if (reader.HasRows)
                    while (reader.Read())
                    {                      
                        courseScore.ID = (int)reader[0];
                        courseScore.Course = reader[1].ToString();
                        courseScore.StudentNumber = reader[2].ToString();
                        courseScore.Midterm1 = reader[3].ToString();
                        courseScore.Midterm2 = reader[4].ToString();
                        courseScore.Midterm3 = reader[5].ToString();
                        courseScore.Final = reader[6].ToString();
                    }           
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("DB Ado Get Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
            finally
            {
                _command.Connection.Close();
            }
            return courseScore;
        }

        //(UPDATE) Veri Tabanına veri güncellememizisağlayan method
        public void UpdateStudentCourseData(StudentCourse item)
        {

            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = "sp_UpdateStudentScore";
            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@course", item.Course);
            _command.Parameters.AddWithValue("@studentNumber", item.StudentNumber);
            _command.Parameters.AddWithValue("@midterm1", item.Midterm1);
            _command.Parameters.AddWithValue("@midterm2", item.Midterm2);
            _command.Parameters.AddWithValue("@midterm3", item.Midterm3);
            _command.Parameters.AddWithValue("@final", item.Final);
            _command.Parameters.AddWithValue("@id", item.ID);

            try
            {
                if (_command.Connection.State == System.Data.ConnectionState.Closed)
                    _command.Connection.Open();
                _command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("DB Ado Update Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
            finally
            {
                _command.Connection.Close();
            }
        }

        //(INSERT) Veri Tabanına veri eklemimizisağlayan method
        public void InsertStudentCourseData(StudentCourse scoreList)
        {     
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = "sp_InsertStudentScore";
            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@course", scoreList.Course);
            _command.Parameters.AddWithValue("@studentNumber", scoreList.StudentNumber);
            _command.Parameters.AddWithValue("@midterm1", scoreList.Midterm1);
            _command.Parameters.AddWithValue("@midterm2", scoreList.Midterm2);
            _command.Parameters.AddWithValue("@midterm3", scoreList.Midterm3);
            _command.Parameters.AddWithValue("@final", scoreList.Final);

            try
            {
                if (_command.Connection.State == ConnectionState.Closed)
                    _command.Connection.Open();
                _command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("DB Ado Insert Hata Mesajı {0}", ex.Message);
                    writer.Close();
                }
            }
            finally
            {
                _command.Connection.Close();
            }
        }

        //CSV dosyamızı DataTable nesnesine doldurup çoklanan Satırlardan en son satırı 
        //alıp bize bir tablo döndüren method.
        DataTable CsvFileDataTable(string filePath, string seperator)
        {
            DataTable dataTable = new DataTable();
            string path = filePath;
            string[] streamReaderValues;
            bool isFirstRow = true;
            string[] coloumnNames = null;

            try
            {
                using (StreamReader streamReader = new StreamReader(filePath))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamReaderRow = streamReader.ReadLine().Trim();

                        if (streamReaderRow.Length > 0)
                        {
                            streamReaderValues = streamReaderRow.Split(Convert.ToChar(seperator));
                            if (isFirstRow)
                            {
                                isFirstRow = false;
                                coloumnNames = streamReaderRow.Split(Convert.ToChar(seperator));
                                foreach (var item in coloumnNames)
                                {

                                    DataColumn dataColumn = new DataColumn(item, typeof(string));
                                    dataTable.Columns.Add(dataColumn);
                                }
                            }
                            else
                            {
                                DataRow dataRow = dataTable.NewRow();
                                for (int i = 0; i < coloumnNames.Length; i++)
                                {
                                    dataRow[coloumnNames[i]] = streamReaderValues[i] == null ? string.Empty : streamReaderValues[i].ToString();
                                }
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("CSV Datatable Hata Mesajı {0}", ex.Message);
                    writer.Close();

                }
            }
            //CSV dosyamızdan gelen Multiple Dataların Son ekleneni alarak tabloya gönderiyoruz.
            var singleRowData = dataTable.AsEnumerable()
                       .GroupBy(r => new { course = r.Field<string>("ders"), studentNumber = r.Field<string>("ogrenci_no") })
                       .Select(g => g.Last())
                       .CopyToDataTable();

            return singleRowData;
        }

        //Windows Service durdurulduğu zaman çalışacak olan methodumuzdur.
        protected override void OnStop()
        {
             using (StreamWriter writer = new StreamWriter(path + @"\errors.txt", true))
                {
                    writer.WriteLine("Windows Service Kapatıldı!!!");
                    writer.Close();
                }
        }

        //Debug Yapılması sağlayan method
        public void OnStartDebug()
        {
            this.OnStart(null);
        }
    }
}
