using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Rahhal_System1.DAL;

namespace Rahhal_System1.Forms
{
    public partial class NewWord : Form
    {
        // متغير لتخزين رقم العبارة في حالة التعديل (null تعني إضافة جديدة)
        private int? editingPhraseID = null;

        // المُنشئ الافتراضي (يستدعي المُنشئ الآخر مع قيمة null)
        public NewWord() : this(null) { }

        // مُنشئ يُستخدم عند التعديل على عبارة موجودة (يستقبل ID للعبارة)
        public NewWord(int? phraseID)
        {
            InitializeComponent();
            editingPhraseID = phraseID;
        }

        // الحدث الذي يتم تنفيذه عند تحميل الفورم
        private void NewWord_Load(object sender, EventArgs e)
        {
            LoadTrips();   // تحميل قائمة الرحلات
            LoadCities();  // تحميل قائمة المدن

            // إذا كنا في وضع التعديل، نحمل بيانات العبارة
            if (editingPhraseID != null)
            {
                LoadPhraseData((int)editingPhraseID);
            }
        }

        // تحميل بيانات الرحلات الخاصة بالمستخدم الحالي
        private void LoadTrips()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT TripID, TripName FROM Trip WHERE IsDeleted = 0 AND UserID = @UserID", con);
                cmd.Parameters.AddWithValue("@UserID", ActivityLogger.CurrentUser.UserID);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                // ربط البيانات بـ ComboBox الخاص بالرحلات
                cbTrip.DataSource = dt;
                cbTrip.DisplayMember = "TripName"; // الاسم الظاهر
                cbTrip.ValueMember = "TripID";     // القيمة المرتبطة
            }
        }

        // تحميل المدن المرتبطة بالزيارات (VisitID هو المفتاح الأجنبي)
        private void LoadCities()
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(@"
            SELECT v.VisitID, c.CityName 
            FROM CityVisit v
            INNER JOIN City c ON v.CityID = c.CityID
            INNER JOIN Trip t ON v.TripID = t.TripID -- ربط الزيارات بالرحلات
            WHERE v.IsDeleted = 0 AND t.UserID = @UserID", con);

                cmd.Parameters.AddWithValue("@UserID", ActivityLogger.CurrentUser.UserID);

                DataTable dt = new DataTable();
                new SqlDataAdapter(cmd).Fill(dt);

                // ربط البيانات بـ ComboBox الخاص بالمدن
                cbCity.DataSource = dt;
                cbCity.DisplayMember = "CityName";
                cbCity.ValueMember = "VisitID"; // المفتاح المستخدم لاحقًا في Phrase
            }
        }


        // التحقق من إدخال جميع الحقول المطلوبة
        private bool ValidateFields()
        {
            bool isValid = true;
            errorProvider1.Clear(); // إزالة الأخطاء السابقة

            if (cbTrip.SelectedItem == null)
            {
                errorProvider1.SetError(cbTrip, "Please select a trip");
                isValid = false;
            }

            if (cbCity.SelectedItem == null)
            {
                errorProvider1.SetError(cbCity, "Please select a city");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtOriginal.Text))
            {
                errorProvider1.SetError(txtOriginal, "Please enter the original phrase");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTranslation.Text))
            {
                errorProvider1.SetError(txtTranslation, "Please enter the translation");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLanguage.Text))
            {
                errorProvider1.SetError(txtLanguage, "Please specify the language");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            return isValid;
        }

        // عند الضغط على زر "حفظ"
        private void btnSaveWord_Click(object sender, EventArgs e)
        {
            // إذا لم تمر عملية التحقق، لا تكمل التنفيذ
            if (!ValidateFields()) return;

            // استخراج البيانات من الواجهة
            int visitId = (int)cbCity.SelectedValue;
            string original = txtOriginal.Text.Trim();
            string translation = txtTranslation.Text.Trim();
            string language = txtLanguage.Text.Trim();
            string notes = txtNotes.Text.Trim();

            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();

                SqlCommand cmd;

                if (editingPhraseID == null)
                {
                    // في حالة الإضافة الجديدة
                    cmd = new SqlCommand(
                        @"INSERT INTO Phrase (VisitID, OriginalText, Translation, Language, Notes)
                          VALUES (@VisitID, @OriginalText, @Translation, @Language, @Notes)", con);
                }
                else
                {
                    // في حالة التعديل
                    cmd = new SqlCommand(
                        @"UPDATE Phrase
                          SET VisitID = @VisitID,
                              OriginalText = @OriginalText,
                              Translation = @Translation,
                              Language = @Language,
                              Notes = @Notes,
                              UpdatedAt = GETDATE()
                          WHERE PhraseID = @PhraseID", con);
                    cmd.Parameters.AddWithValue("@PhraseID", editingPhraseID.Value);
                }

                // إضافة القيم المشتركة
                cmd.Parameters.AddWithValue("@VisitID", visitId);
                cmd.Parameters.AddWithValue("@OriginalText", original);
                cmd.Parameters.AddWithValue("@Translation", translation);
                cmd.Parameters.AddWithValue("@Language", language);
                cmd.Parameters.AddWithValue("@Notes", notes);

                // تنفيذ الأمر
                cmd.ExecuteNonQuery();

                // تسجيل النشاط (إضافة أو تعديل)
                string action = editingPhraseID == null ? "Add Phrase" : "Update Phrase";
                ActivityLogger.Log(con, action, $"Phrase: {original} ({language})");

                // إعلام المستخدم بالنجاح
                MessageBox.Show("✅ Phrase saved successfully.");

                // إغلاق الفورم بعد النجاح
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // تحميل بيانات عبارة موجودة لغرض التعديل
        private void LoadPhraseData(int phraseID)
        {
            using (SqlConnection con = DbHelper.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT VisitID, OriginalText, Translation, Language, Notes
                      FROM Phrase WHERE PhraseID = @PhraseID", con);
                cmd.Parameters.AddWithValue("@PhraseID", phraseID);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // تعبئة الحقول بالبيانات المحفوظة مسبقًا
                        cbCity.SelectedValue = reader["VisitID"];
                        txtOriginal.Text = reader["OriginalText"].ToString();
                        txtTranslation.Text = reader["Translation"].ToString();
                        txtLanguage.Text = reader["Language"].ToString();
                        txtNotes.Text = reader["Notes"].ToString();
                    }
                }
            }
        }
    }
}
