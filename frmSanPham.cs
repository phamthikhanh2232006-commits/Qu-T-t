using System;
using System.Data;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace QuanLyCuaHangBanQuaTet
{
    public partial class frmSanPham : Form
    {
        DataTable tblSP;

        public frmSanPham()
        {
            InitializeComponent();
        }

        private void frmSanPham_Load(object sender, EventArgs e)
        {
            LoadDataGridView();
            LoadComboBoxDanhMuc();
        }

        private void LoadDataGridView()
        {
            string sql = "SELECT * FROM tblSanPham";
            DataTable dataTable = DatabaseHelper.GetData(sql);
            tblSP = dataTable;

            // Tự động tính toán cột "Còn hạn" cho Grid
            if (!tblSP.Columns.Contains("ConHan")) tblSP.Columns.Add("ConHan", typeof(string));
            foreach (DataRow row in tblSP.Rows)
            {
                if (row["NgayHetHan"] != DBNull.Value)
                {
                    DateTime exp = Convert.ToDateTime(row["NgayHetHan"]);
                    if (exp < DateTime.Now) row["ConHan"] = "Hết hạn";
                    else if ((exp - DateTime.Now).TotalDays < 30) row["ConHan"] = "Sắp hết hạn";
                    else row["ConHan"] = "Còn hạn";
                }
                else row["ConHan"] = "N/A";
            }
            dgvSanPham.DataSource = tblSP;

            // Xóa rỗng các trường texbox
            ResetValues();
        }

        private void LoadComboBoxDanhMuc()
        {
            try
            {
                string sql = "SELECT MaDanhMuc, TenDanhMuc FROM tblDanhMuc";
                DatabaseHelper.FillCombo(sql, cboMaDanhMuc, "MaDanhMuc", "TenDanhMuc");
                cboMaDanhMuc.SelectedIndex = -1;
            }
            catch { }
        }

        private void ResetValues()
        {
            txtMaSanPham.Text = "";
            txtTenSanPham.Text = "";
            cboMaDanhMuc.Text = "";
            txtGiaBan.Text = "0";
            txtSoLuong.Text = "0";
            txtMoTa.Text = "";
            dtpNgayHetHan.Value = DateTime.Now;
        }

        private void dgvSanPham_Click(object sender, EventArgs e)
        {
            if (tblSP == null || tblSP.Rows.Count == 0 || dgvSanPham.CurrentRow == null) return;
            txtMaSanPham.Text = dgvSanPham.CurrentRow.Cells["MaSanPham"].Value.ToString();
            txtTenSanPham.Text = dgvSanPham.CurrentRow.Cells["TenSanPham"].Value.ToString();
            cboMaDanhMuc.SelectedValue = dgvSanPham.CurrentRow.Cells["MaDanhMuc"].Value.ToString();
            txtGiaBan.Text = dgvSanPham.CurrentRow.Cells["GiaBan"].Value.ToString();
            txtSoLuong.Text = dgvSanPham.CurrentRow.Cells["SoLuongTon"].Value.ToString();
            txtMoTa.Text = dgvSanPham.CurrentRow.Cells["MoTa"].Value.ToString();

            if (dgvSanPham.CurrentRow.Cells["NgayHetHan"].Value != DBNull.Value)
                dtpNgayHetHan.Value = Convert.ToDateTime(dgvSanPham.CurrentRow.Cells["NgayHetHan"].Value);
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            ResetValues();
            txtMaSanPham.Focus();
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (txtMaSanPham.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập mã sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtMaSanPham.Focus();
                return;
            }
            if (txtTenSanPham.Text.Trim().Length == 0)
            {
                MessageBox.Show("Bạn phải nhập tên sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtTenSanPham.Focus();
                return;
            }
            // Kiểm tra trùng mã
            string sqlCheck = $"SELECT MaSanPham FROM tblSanPham WHERE MaSanPham='{txtMaSanPham.Text.Trim()}'";
            if (DatabaseHelper.GetData(sqlCheck).Rows.Count > 0)
            {
                MessageBox.Show("Mã sản phẩm này đã có, bạn phải nhập mã khác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaSanPham.Focus();
                return;
            }
            string sql = $"INSERT INTO tblSanPham(MaSanPham, TenSanPham, MaDanhMuc, GiaBan, SoLuongTon, MoTa, NgayHetHan) " +
                         $"VALUES(N'{txtMaSanPham.Text.Trim()}', N'{txtTenSanPham.Text.Trim()}', N'{cboMaDanhMuc.SelectedValue}', " +
                         $"{txtGiaBan.Text.Trim()}, {txtSoLuong.Text.Trim()}, N'{txtMoTa.Text.Trim()}', '{dtpNgayHetHan.Value.ToString("yyyy-MM-dd HH:mm:ss")}')";
            DatabaseHelper.ExecuteQuery(sql);
            LoadDataGridView();
            ResetValues();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (tblSP == null || tblSP.Rows.Count == 0)
            {
                MessageBox.Show("Không còn dữ liệu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (string.IsNullOrEmpty(txtMaSanPham.Text))
            {
                MessageBox.Show("Bạn chưa chọn bản ghi nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string sql = $"UPDATE tblSanPham SET TenSanPham=N'{txtTenSanPham.Text.Trim()}', " +
                         $"MaDanhMuc=N'{cboMaDanhMuc.SelectedValue}', GiaBan={txtGiaBan.Text.Trim()}, SoLuongTon={txtSoLuong.Text.Trim()}, " +
                         $"MoTa=N'{txtMoTa.Text.Trim()}', NgayHetHan='{dtpNgayHetHan.Value.ToString("yyyy-MM-dd HH:mm:ss")}' " +
                         $"WHERE MaSanPham=N'{txtMaSanPham.Text}'";
            DatabaseHelper.ExecuteQuery(sql);
            LoadDataGridView();
            ResetValues();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (tblSP == null || tblSP.Rows.Count == 0) return;
            if (string.IsNullOrEmpty(txtMaSanPham.Text)) return;
            if (MessageBox.Show("Bạn có muốn xoá bản ghi này không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string sql = $"DELETE tblSanPham WHERE MaSanPham=N'{txtMaSanPham.Text}'";
                DatabaseHelper.ExecuteQuery(sql);
                LoadDataGridView();
                ResetValues();
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text.Trim();
            if (string.IsNullOrEmpty(keyword))
            {
                LoadDataGridView();
                return;
            }
            string sql = $"SELECT * FROM tblSanPham WHERE MaSanPham LIKE N'%{keyword}%' OR TenSanPham LIKE N'%{keyword}%'";
            tblSP = DatabaseHelper.GetData(sql);

            // Tái áp dụng logic tính "Còn hạn"
            if (!tblSP.Columns.Contains("ConHan")) tblSP.Columns.Add("ConHan", typeof(string));
            foreach (DataRow row in tblSP.Rows)
            {
                if (row["NgayHetHan"] != DBNull.Value)
                {
                    DateTime exp = Convert.ToDateTime(row["NgayHetHan"]);
                    if (exp < DateTime.Now) row["ConHan"] = "Hết hạn";
                    else if ((exp - DateTime.Now).TotalDays < 30) row["ConHan"] = "Sắp hết hạn";
                    else row["ConHan"] = "Còn hạn";
                }
                else row["ConHan"] = "N/A";
            }
            dgvSanPham.DataSource = tblSP;
        }

        private void btnInHetHan_Click(object sender, EventArgs e)
        {
            try
            {
                // Chỉ lấy những sản phẩm đã hết hạn (NgayHetHan < Ngày hiện tại)
                string sql = "SELECT MaSanPham, TenSanPham, GiaBan, SoLuongTon, DonViTinh, NgayHetHan FROM tblSanPham " +
                             "WHERE NgayHetHan < GETDATE()";
                DataTable dt = DatabaseHelper.GetData(sql);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có sản phẩm nào hết hạn để in!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Đã đổi sang rptHetHan (Cám ơn bạn đã tạo file!)
                rptHetHan rpt = new rptHetHan(); 
                rpt.SetDataSource(dt);

                frmInHoaDon viewer = new frmInHoaDon();
                viewer.crystalReportViewer1.ReportSource = rpt;
                viewer.Text = "BÁO CÁO HÀNG HẾT HẠN";
                viewer.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi in báo cáo hết hạn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
