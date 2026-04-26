using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
namespace QuanLyCuaHangBanQuaTet
{
    public static class DatabaseHelper
    {
        // Sử dụng tên đầy đủ để tránh xung đột
        // Đổi thành chuỗi kết nối cố định để tránh lỗi App.config
        private static string connectionString = @"Server=localhost\SQLEXPRESS;Database=DB_QuanLyQuaTet;Integrated Security=True;";
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
        public static DataTable GetData(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
                }
            }
            return dt;
        }
        public static void ExecuteQuery(string query)
        {
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi thực thi dữ liệu: " + ex.Message);
                }
            }
        }
        public static bool CheckLogin(string username, string password, out string maNhanVien, out string quyenHan)
        {
            maNhanVien = "";
            quyenHan = "";
            bool isValid = false;
            string query = "SELECT MaNhanVien, QuyenHan FROM tblTaiKhoan WHERE TenDangNhap = @user AND MatKhau = @pass";

            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            maNhanVien = reader["MaNhanVien"].ToString();
                            quyenHan = reader["QuyenHan"].ToString();
                            isValid = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đăng nhập: " + ex.Message);
                }
            }
            return isValid;
        }
        public static void FillCombo(string sql, ComboBox cbo, string valueField, string displayField)
        {
            DataTable dt = GetData(sql);
            cbo.DataSource = dt;
            cbo.ValueMember = valueField;
            cbo.DisplayMember = displayField;
        }
        public static string GetFieldValues(string sql)
        {
            string value = "";
            using (SqlConnection con = GetConnection())
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sql, con);
                    object result = cmd.ExecuteScalar();
                    if (result != null) value = result.ToString();
                }
                catch { }
            }
            return value;
        }
    }
}