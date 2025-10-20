using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace NguyenTranHuuDuc_1150080049_BTtuan8
{
    public partial class Form1 : Form
    {
    
        string strCon = @"Data Source=(LocalDB)\MSSQLLocalDB;
        AttachDbFilename=C:\Users\Duc\Documents\Nam 4\HK1\PP PTPM HDT\NguyenTranHuuDuc_1150080049_BTtuan8\NguyenTranHuuDuc_1150080049_BTtuan8\QuanLyBanSach.mdf;
        Integrated Security=True";

        SqlConnection sqlCon = null;

        public Form1()
        {
            InitializeComponent();
        }

        // 🔹 Mở kết nối
        private void MoKetNoi()
        {
            if (sqlCon == null)
                sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
        }

        // 🔹 Đóng kết nối
        private void DongKetNoi()
        {
            if (sqlCon != null && sqlCon.State == ConnectionState.Open)
                sqlCon.Close();
        }

        // 🔹 Hiển thị danh sách NXB (gọi thủ tục "HienThiNXB")
        private void HienThiDanhSachXB()
        {
            MoKetNoi();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = "HienThiNXB";
            sqlCmd.Connection = sqlCon;

            SqlDataReader reader = sqlCmd.ExecuteReader();
            lsvDanhSach.Items.Clear();

            while (reader.Read())
            {
                string maXB = reader.GetString(0);
                string tenXB = reader.GetString(1);
                string diaChi = reader.GetString(2);

                ListViewItem lvi = new ListViewItem(maXB);
                lvi.SubItems.Add(tenXB);
                lvi.SubItems.Add(diaChi);

                lsvDanhSach.Items.Add(lvi);
            }

            reader.Close();
            DongKetNoi();
        }

        // 🔹 Khi form mở, tự động hiển thị danh sách
        private void Form1_Load(object sender, EventArgs e)
        {
            HienThiDanhSachXB();
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            MoKetNoi();
            SqlCommand sqlCmd = new SqlCommand("ThemDuLieu", sqlCon);
            sqlCmd.CommandType = CommandType.StoredProcedure;

            sqlCmd.Parameters.AddWithValue("@NXB", txtMaNXB.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@TenNXB", txtTenNXB.Text.Trim());
            sqlCmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text.Trim());

            int kq = sqlCmd.ExecuteNonQuery();
            if (kq > 0)
            {
                MessageBox.Show("Thêm dữ liệu thành công!");
                HienThiDanhSachXB();
                txtMaNXB.Clear(); txtTenNXB.Clear(); txtDiaChi.Clear();
            }
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (lsvDanhSach.SelectedItems.Count == 0)
            {
                MessageBox.Show("Chọn một dòng trong danh sách để sửa.");
                return;
            }

            try
            {
                MoKetNoi();
                using (var sqlCmd = new SqlCommand("dbo.SuaDuLieu", sqlCon))
                {
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.Add("@NXB", SqlDbType.Char, 10).Value = txtMaNXB.Text.Trim();
                    sqlCmd.Parameters.Add("@TenNXB", SqlDbType.NVarChar, 100).Value = txtTenNXB.Text.Trim();
                    sqlCmd.Parameters.Add("@DiaChi", SqlDbType.NVarChar, 500).Value = txtDiaChi.Text.Trim();
                    sqlCmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật thành công!");
                HienThiDanhSachXB();
                // chọn lại dòng vừa sửa (tùy chọn)
                foreach (ListViewItem it in lsvDanhSach.Items)
                    if (it.SubItems[0].Text.Equals(txtMaNXB.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    { it.Selected = true; it.EnsureVisible(); break; }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("SQL error: " + ex.Message);
            }
            finally
            {
                DongKetNoi();
            }
        }
    }
}
