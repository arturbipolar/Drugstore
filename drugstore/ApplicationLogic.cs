using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace Drugstore
{
    public static class ApplicationLogic
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand sqlCommand = null;
        private const string connectionString = @"Server=(LocalDB)\MSSQLLocalDB;Integrated Security=true;AttachDbFileName=C:\Users\369\source\repos\Drugstore\Drugstore\Database1.mdf";
        private static SqlDataAdapter adapter;
        private static DataTable dataset = null;

        private static string query = null;

        public enum Tables
        {
            Documents = 1,
            Drugs = 2,
            Logs = 3,
            Manufacturers = 4,
            Measures = 5,
            Warehouse = 6
        }
        public static void LoadTable(Tables tables, DataGridView dgv)
        {
            dataset = new DataTable();
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                switch (tables)
                {
                    case Tables.Documents:
                        query = "select * from Documents";
                        break;
                    case Tables.Drugs:
                        query = "select * from Drugs";
                        break;
                    case Tables.Logs:
                        query = "select * from Logs";
                        break;
                    case Tables.Manufacturers:
                        query = "select * from Manufacturers";
                        break;
                    case Tables.Measures:
                        query = "select * from Measures";
                        break;
                    case Tables.Warehouse:
                        query = "select * from Warehouse";
                        break;
                }
                adapter = new SqlDataAdapter(query, sqlConnection);
                adapter.Fill(dataset);
                dgv.DataSource = dataset;
                sqlConnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void Insert(Tables tables, IEntity entity)
        {
            try
            {
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    switch (tables)
                    {
                        case Tables.Drugs:
                            try
                            {
                                query = "insert into Drugs(Title,Indications,MeasureID,Price,Quantity,Manufacturer,ExpTerm,Purpose)" +
                             "values(@Tilte,@Indications,@MeasureID,@Price,@Quantity,@Manufacturer,@ExpTerm,@Purpose) select SCOPE_IDENTITY()";
                                var obj = (DrugsEntity)entity;
                                sqlCommand.Parameters.Add(new SqlParameter("@Title", obj.Title));
                                sqlCommand.Parameters.Add(new SqlParameter("@Indications", obj.Indications));
                                sqlCommand.Parameters.Add(new SqlParameter("@MeasureID", obj.MeasureID));
                                sqlCommand.Parameters.Add(new SqlParameter("@Price", obj.Price));
                                sqlCommand.Parameters.Add(new SqlParameter("@Quantity", obj.Quantity));
                                sqlCommand.Parameters.Add(new SqlParameter("@Manufacturer", obj.ManufacturerID));
                                sqlCommand.Parameters.Add(new SqlParameter("@ExpTerm", obj.ExpTerm));
                                sqlCommand.Parameters.Add(new SqlParameter("@Purpose", obj.Purpose));
                                int DrugID = sqlCommand.ExecuteNonQuery();

                                //добавление записи о поступлении в таблицу документы
                                query = "insert into Documents(ManufacturerID,DrugID,Quantity,Price,ProvisionDate)" +
                            "values(@ManufacturerID,@DrugID,@Quantity,@Price,@ProvisionDate)";
                                SqlCommand sqlDoc = new SqlCommand(query, sqlConnection);
                                sqlDoc.Parameters.Add(new SqlParameter("@ManufacturerID", obj.ManufacturerID));
                                sqlDoc.Parameters.Add(new SqlParameter("@DrugID", DrugID));
                                sqlDoc.Parameters.Add(new SqlParameter("@Quantity", obj.Quantity));
                                sqlDoc.Parameters.Add(new SqlParameter("@Price", obj.Quantity * obj.Price));
                                sqlDoc.Parameters.Add(new SqlParameter("@ProvisionDate", DateTime.Now));
                                sqlDoc.ExecuteNonQuery();
                                MessageBox.Show("Запись успешно добавлена");
                            }
                            catch
                            {
                                MessageBox.Show("Неверно заполненные поля");
                            }
                            break;
                        case Tables.Manufacturers:
                            try
                            {
                                query = "insert into Manufacturers(Title,Address,Phone,CheckingAccount)" +
                                      "values(@Title,@Address,@Phone,@CheckingAccount)";
                                var obj1 = (ManufacturersEntity)entity;
                                sqlCommand.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Title", Value = obj1.Title });
                                sqlCommand.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Phone", Value = obj1.Phone });
                                sqlCommand.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Address", Value = obj1.Address });
                                sqlCommand.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@CheckingAccount", Value = obj1.CheckingAccount });
                                sqlCommand.ExecuteNonQuery();
                                MessageBox.Show("Запись успешно добавлена");
                            }
                            catch
                            {
                                MessageBox.Show("Неверно заполненные поля");
                            }
                            break;

                        case Tables.Measures:
                            try
                            {
                                query = "insert into Measures(Title) values(@Title)";
                                var obj2 = (MeasuresEntity)entity;
                                sqlCommand.Parameters.Add(new SqlParameter("@Title", obj2.Title));
                                sqlCommand.ExecuteNonQuery();
                                MessageBox.Show("Запись успешно добавлена");
                            }
                            catch
                            {

                                MessageBox.Show("Неверно заполненные поля");
                            }
                            break;
                        case Tables.Warehouse:
                            try
                            {
                                query = "insert into Warehouse(DrugID,Quantity) values(@DrugID,@Quantity)";
                                var obj3 = (WarehouseItemsEntity)entity;
                                sqlCommand.Parameters.Add(new SqlParameter("@DrugID", SqlDbType.NVarChar, obj3.DrugID));
                                sqlCommand.Parameters.Add(new SqlParameter("@Quantity", obj3.Quantity));
                                sqlCommand.ExecuteNonQuery();
                                MessageBox.Show("Запись успешно добавлена");
                            }
                            catch
                            {
                                MessageBox.Show("Неверно заполненные поля");
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void Update(Tables tables, IEntity entity)
        {
            try
            {
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    switch (tables)
                    {
                        case Tables.Drugs:
                            query = "Update Drugs Set(Title=@Title,Indications=@Indications,MeasureID=@MeasureID" +
                                ",Price=@Price,Quantity=@Quantity,Manufacturer=@Manufacturer,ExpTerm=@ExpTerm,Purpose=@Purpose)" +
                            "where ID=@ID";
                            var obj = (DrugsEntity)entity;
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", obj.ID));
                            sqlCommand.Parameters.Add(new SqlParameter("@Title", obj.Title));
                            sqlCommand.Parameters.Add(new SqlParameter("@Indications", obj.Indications));
                            sqlCommand.Parameters.Add(new SqlParameter("@MeasureID", obj.MeasureID));
                            sqlCommand.Parameters.Add(new SqlParameter("@Price", obj.Price));
                            sqlCommand.Parameters.Add(new SqlParameter("@Quantity", obj.Quantity));
                            sqlCommand.Parameters.Add(new SqlParameter("@Manufacturer", obj.ManufacturerID));
                            sqlCommand.Parameters.Add(new SqlParameter("@ExpTerm", obj.ExpTerm));
                            sqlCommand.Parameters.Add(new SqlParameter("@Purpose", obj.Purpose));
                            sqlCommand.ExecuteNonQuery();
                            break;
                        case Tables.Manufacturers:
                            query = "Update Manufacturers Set(Title=@Title,Address=@Address,Phone=@Phone,CheckingAccount=@CheckingAccount) where ID=@ID";
                            var obj1 = (ManufacturersEntity)entity;
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", obj1.ID));
                            sqlCommand.Parameters.Add(new SqlParameter("@Title", obj1.Title));
                            sqlCommand.Parameters.Add(new SqlParameter("@Phone", obj1.Phone));
                            sqlCommand.Parameters.Add(new SqlParameter("@Address", obj1.Address));
                            sqlCommand.Parameters.Add(new SqlParameter("@CheckingAccount", obj1.CheckingAccount));
                            sqlCommand.ExecuteNonQuery();
                            break;

                        case Tables.Measures:
                            query = "Update Measures Set(Title=@Title) where ID=@ID";
                            var obj2 = (MeasuresEntity)entity;
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", obj2.ID));
                            sqlCommand.Parameters.Add(new SqlParameter("@Title", obj2.Title));
                            sqlCommand.ExecuteNonQuery();
                            break;
                        case Tables.Warehouse:
                            query = "Update Warehouse Set(DrugID=@DrugID,Quantity=@Quantity) where ID=@ID";
                            var obj3 = (WarehouseItemsEntity)entity;
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", obj3.ID));
                            sqlCommand.Parameters.Add(new SqlParameter("@DrugID", obj3.DrugID));
                            sqlCommand.Parameters.Add(new SqlParameter("@Quantity", obj3.Quantity));
                            sqlCommand.ExecuteNonQuery();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static async void Delete(Tables tables, int ID)
        {
            try
            {
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    switch (tables)
                    {
                        case Tables.Drugs:
                            query = "Delete from Drugs where Id=@ID";
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", ID));
                            await sqlCommand.ExecuteNonQueryAsync();
                            break;
                        case Tables.Manufacturers:
                            query = "Delete from Manufacturers where Id=@ID";
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", ID));
                            await sqlCommand.ExecuteNonQueryAsync();
                            break;

                        case Tables.Measures:
                            query = "Delete from Measures where Id=@ID";
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", ID));
                            await sqlCommand.ExecuteNonQueryAsync();
                            break;
                        case Tables.Warehouse:
                            query = "Delete from Warehouse where Id=@ID";
                            sqlCommand.Parameters.Add(new SqlParameter("@ID", ID));
                            await sqlCommand.ExecuteNonQueryAsync();
                            break;
                    }
                }
                MessageBox.Show("Запись успешно удалена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void ShowComboBoxItems(Tables tables, ComboBox lb)
        {
            try
            {
                List<IEntity> list = null;
                using (sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    sqlCommand = new SqlCommand(query, sqlConnection);
                    list = new List<IEntity>();
                    SqlDataReader reader;
                    switch (tables)
                    {
                        case Tables.Drugs:
                            query = "select * from Drugs";
                            reader = sqlCommand.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    list.Add(new DrugsEntity { ID = reader.GetInt32(0), Title = reader.GetString(1) });
                                }
                            }
                            break;
                        case Tables.Manufacturers:
                            query = "select * from Manufacturers";
                            reader = sqlCommand.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    list.Add(new ManufacturersEntity { ID = reader.GetInt32(0), Title = reader.GetString(1) });
                                }
                            }
                            break;
                        case Tables.Measures:
                            query = "select * from Measures";
                            reader = sqlCommand.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    list.Add(new MeasuresEntity { ID = reader.GetInt32(0), Title = reader.GetString(1) });
                                }
                            }
                            break;
                        case Tables.Warehouse:
                            query = "select * from Warehouse";
                            reader = sqlCommand.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    list.Add(new WarehouseItemsEntity { ID = reader.GetInt32(0) });
                                }
                            }
                            break;
                    }
                    lb.DataSource = list;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Фильтрация 
        public enum FilterDrugsCategory
        {
            ByManufacturer = 1,
            ByDateRange = 2,
            DrugQuantity = 3,
            ByDay = 4,
            BySum = 5
        }
        public static void FilterDrugs(FilterDrugsCategory category, DataGridView dgv, int intParameter = 0)
        {
            try
            {
                dataset = new DataTable();
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                switch (category)
                {
                    case FilterDrugsCategory.ByManufacturer:
                        query = "select * from Drugs where Manufacturer = '" + intParameter + "'";

                        break;
                    case FilterDrugsCategory.BySum:
                        query = "select * from Drugs where Price > '" + intParameter + "'";
                        break;
                }
                adapter = new SqlDataAdapter(query, sqlConnection);
                adapter.Fill(dataset);
                dgv.DataSource = dataset;
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }
        public static void FilterDrugs(FilterDrugsCategory category, int intParameter = 0)
        {
            try
            {
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                switch (category)
                {
                    case FilterDrugsCategory.DrugQuantity:
                        query = "select Quantity from Warehouse where DrugID = '" + intParameter + "'";
                        SqlCommand command = new SqlCommand(query, sqlConnection);
                        string message = command.ExecuteScalar().ToString();
                        MessageBox.Show(message, "Количество");
                        break;
                }
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        public static void FilterDrugs(FilterDrugsCategory category, DataGridView dgv, DateTime date)
        {
            try
            {
                dataset = new DataTable();
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                var comm = sqlConnection.CreateCommand();
                comm.CommandText = "select * from Documents where ProvisionDate = @datee";
                comm.Parameters.Add(new SqlParameter { ParameterName = "datee", SqlDbType = SqlDbType.Date, Value = dateStart});            
                adapter = new SqlDataAdapter(comm);
                adapter.Fill(dataset);
                dgv.DataSource = dataset;
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }
        public static void FilterDrugs(FilterDrugsCategory category, DataGridView dgv, DateTime dateStart, DateTime dateEnd)
        {
            try
            {
                dataset = new DataTable();
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                var comm = sqlConnection.CreateCommand();
                comm.CommandText = "select * from Documents where ProvisionDate between @dateStart and @dateEnd";
                comm.Parameters.Add(new SqlParameter { ParameterName = "dateStart", SqlDbType = SqlDbType.Date, Value = dateStart });
                comm.Parameters.Add(new SqlParameter { ParameterName = "dateEnd", SqlDbType = SqlDbType.Date, Value = dateEnd });

                adapter = new SqlDataAdapter(comm);
                adapter.Fill(dataset);
                dgv.DataSource = dataset;
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }

        #endregion

        #region Вывод в excel


        public enum ExcelOperation
        {
            DrugsRowsAmount = 1,
            Union = 2,
            GroupBy = 3
        }
        public static void ExcelOutput(ExcelOperation operation)
        {
            try
            {
                Excel.Application excel = new Excel.Application();

                excel.Visible = true;
                excel.SheetsInNewWorkbook = 1;
                Excel.Workbook workbook = excel.Workbooks.Add();
                excel.DisplayAlerts = false;

                Excel.Worksheet sheet = (Excel.Worksheet)excel.Worksheets.get_Item(1);
                sheet.Name = $"Отчет за {DateTime.Now.Date}";
                excel.Worksheets.Add(sheet);

                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                switch (operation)
                {
                    case ExcelOperation.DrugsRowsAmount:
                        query = "select Count(*) from Drugs";
                        sqlCommand = new SqlCommand(query, sqlConnection);
                        object count = sqlCommand.ExecuteScalar();
                        sheet.Cells[1, 1] = $"Количество записей : {count}";
                        break;
                    case ExcelOperation.Union:
                        query = "select Drugs.Title,Price,Quantity as 'Кол-во в упаковке'," +
                            "Manufacturers.Title as 'Производитель'," +
                            "Measures.Title as 'Мера измерения'" +
                            "from Drugs" +
                            "inner join Manufacturers on Drugs.Manufacturer = Manufacturers.ID" +
                            "inner join Measures on Drugs.MeasureID = Measures.ID";
                        dataset = new DataTable();
                        adapter = new SqlDataAdapter(query, sqlConnection);
                        adapter.Fill(dataset);
                        for (int i = 0; i < dataset.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataset.Columns.Count; j++)
                            {
                                sheet.Cells[i + 1, j + 1] = String.Format("{0} {1}", i, j);
                            }
                        }
                        break;
                    case ExcelOperation.GroupBy:
                        query = "select Drugs.Title, Warehouse.Quantity from Warehouse" +
                            "left join Drugs on Warehouse.DrugID = Drugs.ID";
                        dataset = new DataTable();
                        adapter = new SqlDataAdapter(query, sqlConnection);
                        adapter.Fill(dataset);
                        for (int i = 0; i < dataset.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataset.Columns.Count; j++)
                            {
                                sheet.Cells[i + 1, j + 1] = String.Format("{0} {1}", i, j);
                            }
                        }
                        break;

                }
                Excel.Range range = sheet.get_Range(sheet.Cells[1, 1], sheet.Cells[256, 256]);
                range.Cells.Font.Name = "Tahoma";
                range.Cells.Font.Size = 15;
                range.Cells.Font.Color = ColorTranslator.ToOle(Color.Red);
                excel.Application.ActiveWorkbook.SaveAs2(Path.Combine(Application.StartupPath, "doc.xlsx"));
                excel.Workbooks.Open(Path.Combine(Application.StartupPath, "doc.xlsx"));

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }



        #endregion

        // общие свойства
        public static DateTime dateStart { get; set; }
        public static DateTime dateEnd { get; set; }

        public static int intParameter { get; set; }
    }

         #region Сущности
    public interface IEntity { }

    public class DrugsEntity : IEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Indications { get; set; }
        public int MeasureID { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int ManufacturerID { get; set; }
        public string ExpTerm { get; set; }
        public string Purpose { get; set; }
        public override string ToString()
        {
            return "ID " + this.ID + " " + this.Title;
        }
    }
    public class ManufacturersEntity : IEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string CheckingAccount { get; set; }

        public override string ToString()
        {
            return "ID " + this.ID + " " + this.Title;
        }
    }
    public class MeasuresEntity : IEntity
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public override string ToString()
        {
            return "ID " + this.ID + " " + this.Title;
        }
    }
    public class WarehouseItemsEntity : IEntity
    {
        public int ID { get; set; }
        public int DrugID { get; set; }
        public int Quantity { get; set; }
        public override string ToString()
        {
            return "ID " + this.ID;
        }
    }
    #endregion
}
